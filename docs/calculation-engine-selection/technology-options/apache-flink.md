# Apache Flink

Flink is a lower-level (compared to Spark) stateful streaming engine.
Compared to Spark, which was originally built for batch and then extended for streaming, Flink was built originally for streaming and then extended for batch processing.

Architecture top level view:

* No managed services on Azure, Flink requires either Kubernetes or VMSS deployment.
* Connection to Event Hub in Kafka mode through DataStream connector.
* Applications are prebuilt jar/python applications deployed to the cluster.
* Streaming and batch processing use internal Flink scheduling and recovery mechanisms.
* SQL Master data can be imported through JDBC Table connector.
* Data duplicates and updates are stored "as-is" in append-only format and then filtered during the first stage of processing and aggregation queries.
* Flink has its own serverless implementation allowing to compose app as a set of related small functions.
