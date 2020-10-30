
# HDInsight cluster

HDInsight is a fully-featured _provider-agnostic_ Hadoop/Spark distribution that comes at the cost of much bigger management and support overhead.

Architecture top level view:

* Single or multiple HDInsight clusters to hold the whole infrastructure with configuration.
* Data is stored on Azure Data Lake connected to the cluster nodes.
* Applications are jar/python applications deployed to the cluster.
* Real-time spark application managed with YARN, Batch processing is organized through cron jobs.
* Data duplicates and updates are stored "as-is" in append-only format and then filtered during the first stage of processing and aggregation queries.
* Delta Lake can be installed as a library to all cluster nodes, so Delta Lake specific features from above are available as well.
* Data federation (connection to external storage) is used to copy reference data into memory of spark job.
  The streaming job is restarted daily to update reference cache.
