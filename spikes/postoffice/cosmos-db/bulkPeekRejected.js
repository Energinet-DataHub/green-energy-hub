/*
 * A Cosmos DB stored procedure that bulk peeks documents in the partition it is executed in.<br/>
 * It marks as peeked all existing documents (Rejected meters readings) for a given Market Actor (which is the partitionkey for our collection).</br>
 * Once the actor will acknowledge they have received the meters, then another SProc will be called to delete all docs with a defined peeked field. </br>
 * Note: You may need to execute this sproc multiple times (depending whether the sproc is able to peek every document within the execution timeout limit).
 *
 * @function
 * @params {string} jobID - the Job / Request Correlation ID generated when the actor requested for new data
 * @returns {Object.<number, boolean>} Returns an object with the two properties:<br/>
 *   peeked - contains a count of documents that have been peeked<br/>
 *   continuation - a boolean whether you should execute the sproc again (true if there are more documents to peek; false otherwise).
*/

function bulkPeekRejected(jobID) {
  var container = getContext().getCollection();
  var containerLink = container.getSelfLink();
  var response = getContext().getResponse();
  var responseBody = {
    peeked: 0,
    continuation: false
  };

  if (!jobID) throw new Error("bulkPeekRejected: Job ID parameter was not set. Exiting.");

  // The following query determines if there are any Rejected messages
  // Rejected messages must have the Reasons field defined. For future implementations we might want to consider having a messageType field
  query = "select * from c where c.MessageType='InvalidTimeSeries' and not is_defined(c.PeekStatus)";

  tryQueryAndPeek();

  function tryQueryAndPeek(continuation) {
    var requestOptions = { continuation: continuation };
    var isAccepted = container.queryDocuments(containerLink, query, requestOptions,
      function(err, retrievedDocs, responseOptions) {
        if (err) throw err;

        if (retrievedDocs.length>0)
        // Begin peeking documents as soon as documents are returned form the query results.
        // tryMarkPeek() resumes querying after marking; no need to page through continuation tokens.
        //  - this is to prioritize writes over reads given timeout constraints.
        {
          tryMarkPeeked(retrievedDocs);

        } else if (responseOptions.continuation) {
          // Else if the query came back empty, but with a continuation token; repeat the query w/ the token.
          tryQueryAndPeek(responseOptions.continuation);
        } else {
            // Else if there are no more documents and no continuation token - we are finished peeking documents.
          responseBody.continuation = false;
          response.setBody(responseBody);
        }
      }
    );

    // If we hit execution bounds - return continuation: true.
    if (!isAccepted) {
      responseBody.continuation = true;
      response.setBody(responseBody);
    }
  }

  function tryMarkPeeked(docs)
  {
    // Validate input.
    if (!docs) throw new Error("tryMarkPeeked: At least one of the input params is undefined or null.");

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
      // Add new PeekedStatus and JobID fields
      doc.PeekStatus = "Latest";
      doc.JobID = jobID;

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

      if (count >= docsLength) {
          // If we have upserted all documents, we are done for this batch. Query more docs.
          tryQueryAndPeek();
      } else {
          // Peek next document.
          tryPeek(docs[count], callback);
      }
    }
  }
}