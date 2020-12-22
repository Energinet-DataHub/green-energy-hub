/*
 * A Cosmos DB stored procedure that bulk peeks documents in the partition it is executed in.<br/>
 * It marks as peeked all existing documents (meters readings) for a given Market Actor (which is the partitionkey for our collection).</br>
 * Documents are set to PeekStatus = Old when there is a newer document (bigger MarketDocument_CreatedDateTime) for the same Meter ID </br>
 * that has values for the same ObservationTime. In this case the document will not be sent to the actor but will have a defined PeekedStatus field. </br>
 * Once the actor will acknowledge they have received the meters, then another SProc will be called to delete all docs with a defined peeked field </br>
 * therefore deleting all readings even when PeekStatus is defined.
 * Note: You may need to execute this sproc multiple times (depending whether the sproc is able to peek every document within the execution timeout limit).
 *
 * @function
 * @params {string} jobID - the Job / Request Correlation ID generated when the actor requested for new data
 * @returns {Object.<number, boolean>} Returns an object with the two properties:<br/>
 *   peeked - contains a count of documents that have been peeked<br/>
 *   continuation - a boolean whether you should execute the sproc again (true if there are more documents to peek; false otherwise).
*/

function bulkPeek(jobID) {
  var container = getContext().getCollection();
  var containerLink = container.getSelfLink();
  var response = getContext().getResponse();
  var responseBody = {
    peeked: 0,
    continuation: false
  };

  if (!jobID) throw new Error("bulkPeek: Job ID parameter was not set. Exiting.");

  // The following query determines latest received times for a given pair of observationTime and MarketEvaluationPoint_mRID.
  // Currently doing so using EventHubEnqueueTime.
  // For future implementations RequestDate should be used instead (currently not available).
  // Validated messages do not have the Reasons field defined. For future implementations we might want to consider having a message type field
  query = "select max(c.EventHubEnqueueTime) maxTime, c.ObservationTime, c.MarketEvaluationPoint_mRID from c where c.MessageType='ValidObservation' and (not is_defined(c.JobID) or c.JobID <>'"+ jobID +"') group by c.ObservationTime, c.MarketEvaluationPoint_mRID";

  tryQueryAndPeek();

  function tryQueryAndPeek(continuation) {
    var requestOptions = { continuation: continuation };
    var isAccepted = container.queryDocuments(
      containerLink,
      query,
      requestOptions,
      function(err, retrievedDocs, responseOptions) {
        if (err) throw err;

        if (retrievedDocs.length>0)
        {
          tryMarkDuplicates(retrievedDocs, null);

        } else if (responseOptions.continuation) {
          tryQueryAndPeek(responseOptions.continuation);
        } else {
          responseBody.continuation = false;
          response.setBody(responseBody);
        }
      }
    );
    if (!isAccepted) {
      responseBody.continuation = true;
      response.setBody(responseBody);
    }
  }

  function tryMarkDuplicates(docs, continuation)
  {
    var requestOptions = { continuation: continuation };
    if (docs.length>0)
    {
      var x = docs[0];

      var query = "select * from c where c.MessageType='ValidObservation' and c.MarketEvaluationPoint_mRID ='" + x.MarketEvaluationPoint_mRID + "' and c.ObservationTime = '" + x.ObservationTime + "'";

      var isAccepted = container.queryDocuments(
        containerLink,
        query, 
        requestOptions, 
        function(err, retrievedDupPairs, responseOptions)
        {
          if (err) throw err;
          if (retrievedDupPairs.length>0)
          {     
            tryMarkPeeked(retrievedDupPairs, x.maxTime);
          } else if (requestOptions.continuation) {
            tryMarkDuplicates(requestOptions.continuation);
          }  else {
          responseBody.continuation = false;
          response.setBody(responseBody);
          }
          docs.shift();
          tryMarkDuplicates(docs);
        }
      );
      // If we hit execution bounds - return continuation: true.
      if (!isAccepted) 
      {
        responseBody.continuation = true;
        response.setBody(responseBody);
      }
      } else {
        // If the document array is empty, query for more documents.
        tryQueryAndPeek();
      }
    } 

    function tryMarkPeeked(docs, maxTime)
    {
        // Validate input.
        if (!docs||!maxTime) throw new Error("tryMarkPeeked: At least one of the input params is undefined or null.");

        var docsLength = docs.length;
        if (docsLength == 0) {
          responseBody.continuation = false;
          reponse.setBody(responseBody);
          return;
        }
        var count = 0;
        tryPeek(docs[count], callback);

        function tryPeek(doc, callback)
        {
          // add JobID field
          doc.JobID = jobID;

          // Add new peeked field
          if (doc.EventHubEnqueueTime==maxTime) 
          {
            // add PeekStatus property and set it to Latest, meaning to be sent to actorid
            doc.PeekStatus = "Latest";
          }
          else // add PeekStatus property but leave to Old, meaning delete / do not send to actorid
          {
            doc.PeekStatus = "Old";
          }
          
          // Update the document.
          var isAccepted = container.upsertDocument(containerLink, doc,callback);
          if (!isAccepted) {
            responseBody.continuation = true;
            response.setBody(responseBody);
          }
        }

        function callback(err, doc, options)
        {
            if (err) throw err;

            // One more document has been inserted, increment the count.
            count++;
            responseBody.peeked++;

            if (count < docsLength) {
                // Peek next document.
                tryPeek(docs[count], callback);
            }
        }
    }
}