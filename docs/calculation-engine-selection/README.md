# Calculation Engine Selection

Calculation Engine is the core part of Green Energy Hub solution, responsible for calculating and aggregating individual data points.
Key parts of calculation engine are:

* **Streaming flow** - semi-real time data processing flow executing data enrichment, validation, collection and aggregation.
  Result data is stored into data lake.
* **Aggregation** - daily/monthly batch data processing to calculate business-critical metrics on top of the data collected by real-time flow.
* **Reference data** - daily updated data used to enrich ingested data points.

Preliminary technology decisions:

* **Spark** is the primary choice of calculation engine to implement data processing flows.
* **Delta Lake** is an optional (but recommended) storage layer that brings ACID transactions, time travel, schema enforcement and evolution functionalities to Spark.
* **Azure Event Hub (potentially with Kafka mode)** serves as high-throughput data collection endpoint consumed by Spark engine.
* **Relational SQL Store (Azure SQL Database)** used as relational data store for reference (master) data.

## Technology Options

Primary options considered in detail in the matrix below:

* [Azure Databricks](technology-options/azure-databricks.md)
* [Azure Databricks with Delta Lake](technology-options/azure-databricks-with-delta-lake.md)
* [Apache Flink](technology-options/apache-flink.md)
* [Azure Synapse](technology-options/azure-synapse.md)
* [Azure Functions](technology-options/azure-functions.md)

Additional options have been reviewed, however it was identified that they only partially satisfy [selection criteria](selection-criteria.md):

* [HDInsight](technology-options/azure-hdinsights.md)
* [Apache Spark](technology-options/apache-spark.md)
* [Azure Stream Analytics](technology-options/azure-stream-analytics.md)
* [Azure Time Series Insights](technology-options/azure-time-series-insights.md)
* [Azure Data Explorer](technology-options/azure-data-explorer.md)
* [Azure Data Warehouse](technology-options/azure-data-warehouse.md)

## Selection Criteria

Selection criteria described in detail in [this document](selection-criteria.md).

## Decision Matrix

<table>
<tr>
    <td></td>
    <td>Azure Databricks</td>
    <td>Apache Flink</td>
    <td>Azure Synapse (Spark)/</td>
    <td>Azure Functions</td>
</tr>
<tr valign="top">
    <td>Summary</td>
    <td>
        <li>(+) Managed Spark with decoupled cluster that is easy to use.
        <li>(+) Vast Spark ecosystem of libraries, data connectors etc to reuse.
        <li>(+) Feature-rich solution with Enterprise optimization, support and SLA.
        <li>(-) Limited implementation-lock because of unique DataBricks primitives like notebooks, jobs, widgets etc and limited access to original Spark API.
    </td>
    <td>
        <li>(+) Dedicated real-time streaming engine with **high performance target**.
        <li>(-) No managed hosting options compared to Spark.
        <li>(-) Significantly smaller community compared to Spark.
        <li>(!) Batch processing is work-in-progress, but follows streaming semantics.
        <li>(!) Low-level development primitives compared to Spark.
    </td>
    <td>
      <li>(+) Offers managed SQL DWH and managed Spark with decoupled cluster (pool) that is easy to use.
      <li>(+) Delta Lake Support
      <li>(+) Single Synapse workspace for relational and Data Lake processing (Spark pool).
      <li>(+) Supports Python, Scala, Spark SQL, .NET for Spark (C#)
      <li>(+) Vast Spark ecosystem of libraries, data connectors etc to reuse.
      <li>(-) In Technical Preview with unknown pricing model and SLA
      <li>(-) Similar to Databricks, certain level of vendor lock, adaptation of notebooks/jobs would be needed.
      <li>(-) In comparison to Databricks offers worse performance and capabilities when it comes to stream processing e.g. Z-order clustering when using Delta, join optimizations etc.
     <li>(!) Great resource on [comparison of Synapse and Databricks](https://www.element61.be/en/resource/when-use-azure-synapse-analytics-andor-azure-databricks)
     <li>(!) Uses Open Source implementation of Apache Spark, thus lacking on performance of Databricks
    </td>
    <td>
        <li>(+) Azure Functions is a serverless technology offering total abstraction from underlying infrastructure that can be used for per-item data operations.
        <li>(+) This option is limited to per-item processing with per-collection operations left to Spark processing. Otherwise, it will have too large development overhead.
        <li>(+) Supporting various programming languages while C# is first class citizen
        <li>(+) Provides abstraction level when connecting to various Azure Services, whether it is on input, trigger or output side
        <li>(+) Provides a way how to build loosely coupled microservice oriented solutions with independent scaling
        <li>(+) Provides possibility to use Durable Functions extensions which allow to create asynchronous reliable processing workflows.
        <li>(-) Not suitable for long running tasks and heavy data processing
        <li>(!) highly scalable based on actual demand, nevertheless still need to test if it will be capable to meet scale of 60k/s
    </td>
</tr>
<tr>
    <td>Data in transit</td>
    <td>
        <li>(+) Meets requirement: ingestion/processing rate scales semi-linearly with cluster size (autoscaling)
        <li>(+) DataBricks Spark is additionally optimized compared to OSS version.
        <li>(!) Deeper investigation is required to find the right processing parameter values. Periodic reevaluation is required to accommodate growth.
        <li>(!) Micro-batch streaming processing may add latency and limit scalability.
    </td>
    <td>
        <li>(+) Real-time processing engine giving better performance compared to micro-batch model of Spark.
        <li>(+) Ingestion/processing rate scales semi-linearly with cluster size.
        <li>(!) Low-level development primitives that are ideologically close to data-aware multi-agent systems. Higher level primitives are available but not yet feature-rich.
        <li>(!) As a rather low-level solution, deep investigation is required to find the right processing parameter values and approach. Periodic reevaluation is required to accommodate growth.
    </td>
    <td>
        <li>(+) Ingestion/processing rate scales semi-linearly with cluster size (autoscaling).
        <li>(+) Expressive fluent API adds flexibility: full access to scala/python/.NET for Spark (C#/F#) language features and libraries.
        <li>(+) Synapse Spark is using OSS version of Spark and lacks stream processing optimization available in Databricks.
        <li>(!) Deep investigation is required to find the right processing parameter values. Periodic reevaluation is required to accommodate growth.
        <li>(!) Micro-batch streaming processing may add latency and limit scalability.
    </td>
    <td>
        <li>(+) Ingestion/processing rate scales linearly with number of instances (autoscaling).
        <li>(+) Abstraction on the side of  execution triggers and inputs enables Functions to be invoked in various ways (timer triggered, http triggered, triggered by incoming Event Hub messages etc.)
        <li>(-) Azure Functions are not meant for implementing complex processing logic over big amounts of data
        <li>(-) If denormalized reference data is needed, it should be cached with Redis for quick access by Azure Functions. Additional effort is needed to keep cache in up-to-date state.
    </td>
</tr>
<tr>
    <td>Data at rest</td>
    <td>
        <li>(+) Meets requirement: ingestion/processing rate scales semi-linearly with cluster size (autoscaling).
        <li>(+) DataBricks Spark is an additionally optimized compared to OSS version.
        <li>(+) As execution strategy for micro-batch streaming and batch processing are the same, code can be reused for up to 90%.
    </td>
    <td>
        <li>(+) Batch processing is streaming-inspired that makes code reuse among pipelines easier.
        <li>(!) Batch processing is still "experimental".
        <li>(!) Low-level development primitives compared to Spark.
    </td>
    <td>
        <li>(+) Processing rate scales semi-linearly with cluster size (autoscaling).
        <li>(+) Expressive fluent API adds flexibility: full access to scala/python or .NET for Spark language features and libraries.
        <li>(+) Besides Spark it offers SQL warehousing and querying capabilities
        <li>(+) As execution strategy for micro-batch streaming and batch processing are the same, code can be reused for up to 90%.
    </td>
    <td>
        <li>(+) Azure Functions can use input abstraction and existing code libraries to obtain data from various data stores.
        <li>(-) Azure Functions are not suitable to load big volumes of data into memory
        <li>(-) Scaling is not applicable in this case, as it would be difficult to implement parallel data processing using Azure Functions when it comes to processing data at rest
    </td>
</tr>
<tr>
    <td>Developer confidence</td>
    <td>
        <li>(+) Spark is a regular skill for data engineers.
        <li>(+) Skills: Scala, python and spark predominantly. Databricks specifics can be considered as details.
        <li>(+) Technology is mature, there community around Spark and Databricks
        <li>(+) Learning curve is common for all Spark solutions: 5 days to get used to. Team awareness is moderate.
        <li>(!) Well-established and widely used, it is easy to find new hires with existing set of skills
    </td>
    <td>
        <li>Skills: Scala, java and Flink predominantly.
        <li>(-) Flink knowledge is rare on the market: onboarding should be planned.
        <li>(!) Steep learning curve.
        <li>(!) Limited awareness and expertise among the team about.
        <li>(!) Smaller adoption compared to Spark, but there is a few well-known companies using it for business-critical workloads.
    </td>
    <td>
        <li>Skills: Scala, python or .NET for Spark (C#,F#) and spark predominantly.
        <li>(+) Spark is a regular skill for data engineers.
        <li>(+) Out of the box C# support, which plays well with team experience, nevertheless C# approach in combination with Spark is not common
        <li>(!) Steepness of learning curve.
        <li>(!) Existing familiarity in the team with the technology
        <li>(!) Well-established and widely used, is it easy to find new hires with existing set of skills
        <li>(!) Spark and supported languages are used quite broadly, nevertheless the envelope service Azure Synapse is quite new offering.
    </td>
    <td>
        <li>Skills: .Net and Azure general knowledge predominantly.
        <li>.NET is a common skill for software developers. Azure can be upskilled.
        <li>(!) Steepness of learning curve.
        <li>(!) Existing familiarity in the team with the technology
        <li>(!) Well-established and widely used, so that it should be easy to find new hires with existing skill set
        <li>(!) People already use it and there is community around it.
   </td>
</tr>
<tr>
    <td>Cost</td>
    <td>
        <li> details of estimated price per month are lower than expected
        <li>(!) Infrastructure requirements are demanding.
        <li>(!) Additional licence fee of $0.07-$0.55 per 4 core x hour.
    </td>
    <td>
        <li>(+) Only infra cost behind open-source solution.
        <li>(!) Additional evaluation is required to evaluate the price and performance characteristics.
        <li>(!) Infrastructure requirements are demanding.
    </td>
    <td>
       <li>(!) Costs structure not know yet, probably Synapse Workspace won't be billed, only pool nodes and storage will be.
    </td>
    <td>
        <li> details of estimated price per month are lower than expected
        <li>(!) Azure Functions Premium is billed based on number of vCPU-s and GB-s, 4 cores, 14 GB RAM 250 GB storage non stop running - $609.26  per month
    </td>
</tr>
<tr>
    <td>Stability</td>
    <td>
        <li>(+) SLA 99.9% SLA for Databricks workspace
        <li>(+) Calculation can be paused and resumed after maintenance
        <li>(+) Cluster is decoupled from configuration that removes the largest Spark's point of failure.
        <li>(+) Spark is aware of streaming jobs and is able to monitor and restart them if needed.
        <li>(+) Spark checkpoints stored on Azure Data Lake allows seamless restarts.
        <li>(!) Checkpoints may fail after significant logic updates.
        <li>(!) Spark jobs are not normally high available, but fault tolerant if checkpoints and precautions are used.
    </td>
    <td>
        <li>(+) System uptime is guaranteed through clustering, system can be recreated from scratch
        <li>(+) Calculation can be paused and resumed after maintenance
        <li>(+) Flink is aware of streaming and batch processing jobs and is able to monitor and restart them if needed.
        <li>(+) Real-time checkpoint strategy to avoid blocking processing.
        <li>(-) SLA No SLA or support except community.
        <li>(!) Checkpoints may fail after significant logic updates.
        <li>(!) Jobs are normally high available, but fault tolerant is easily achievable if checkpoints and precautions are used.
    </td>
    <td>
        <li>(+) Cluster is decoupled from configuration that removes the largest Spark's point of failure.
        <li>(+) Spark is aware of streaming jobs and is able to monitor and restart them if needed.
        <li>(+) Spark checkpoints stored on Azure Data Lake allows seamless restarts.
        <li>(!) SLA of Synapse Workspace not determined yet.
        <li>(!) Checkpoints may fail after significant logic updates.
        <li>(!) Spark jobs are not normally high available, but fault tolerant if checkpoints and precautions are used.
    </td>
    <td>
        <li>SLA 99.95% SLA for Azure Function
        <li>(+) Azure Functions Runtime automatically secures that enough function instances are running to handle the load
        <li>(+) In case of error during Event Hub message processing, Event Hub Checkpoint is not progressed and message gets delivered to another function instance (applies to Event Hub message processing)
        <li>(!) Retry policy might be needed when exception is thrown during message processing (applies to Event Hub message processing)
    </td>
</tr>
<tr>
    <td>DevOps</td>
    <td>
        <li>(+) DataBricks terraform provider is available.
        <li>(+) Git integration for Notebooks
        <li>(-) Notebooks complicates unit and integration testing compared to ordinary Spark jars.
        <li>(!) No access to default Spark endpoints and management that requires use of vendor-specific tooling.
    </td>
    <td>
        <li>(+) Application is a scala/java application that can run locally so that classic unit/integration testing approaches are available.
        <li>(+) CLI is available to automate deployment to the cluster.
        <li>(-) No managed options for cluster hosting, so deployment automation should be developed from scratch.
    </td>
    <td>
      <li> (-) No git integration for Notebooks
      <li> (-) Automated deployment either thru Azure Dev Ops or Terraform not possible at this point
      <li> (!) Terraform support for deployment of workspace and pools nevertheless with sparse documentation
    </td>
    <td>
       <li>(+) Function App creation can be automated in various ways incl. Terraform.
       <li>(+) Function code can be deployed automatically from CI/CD pipeline, while docker containerization of functions is supported as well.
       <li>(+) Same runtime running on local dev machines enables local testing
    </td>
</tr>
<tr>
    <td>Maintainability</td>
    <td>
        <li>(+) Expressive fluent API adds flexibility: full access to scala/python language features and libraries.
        <li>(+) Cluster decoupling significantly simplifies Spark management.
        <li>(+) Same engine as OSS Spark so that the main code is fully transferrable.
        <li>(+) DataBricks is available on both Azure and Amazon.
        <li>(-) Packaging format (notebooks), parametrization through widgets, DevOps tooling, cluster decoupling are unique features that will require full overwrite to adapt to other options.
        <li>(-) Notebooks complicates unit and integration testing compared to ordinary Spark jars.
    </td>
    <td>
        <li>(+) Fully open-source solution so that there is no vendor-lock.
        <li>(-) "Pet problem" of having cluster being bound to configuration and jobs.
        <li>(-) No managed options for cluster hosting that adds management overhead.
    </td>
    <td>
        <li>(+) Cluster decoupling significantly simplifies Spark management.
        <li>(+) Same engine as OSS Spark so that the code is fully transferrable.
        <li>(-) Synapse Workspace available only on Azure
    </td>
    <td>
        <li>(+) When running in Azure, full server abstraction reduces management only to maintaining functions logic/code
        <li>(+) Same runtime running on local dev machines enables local testing
        <li>(!) Running production deployment outside of Azure of course requires management of underlying infrastructure. This approach is not recommended.
    </td>
</tr>
<tr>
    <td>OSS and reuse</td>
    <td>
        <li>(+) Vast ecosystem of spark drivers and libraries is available.
        <li>(+) Access to full scala/python language features (functions, classes, modules) allows to implement an arbitrary modular design.
        <li>(+) Spark as an OSS solution has a vast community (libraries, StackOverflow, GitHub issues). DataBricks community is smaller but it is only relevant to unique features.
        <li>(+) Spark is a feature-proof and has wide adoption.
        <li>(-) Limited access to Spark ecosystem solutions: DevOps, security, RBAC, logging.
    </td>
    <td>
        <li>(+) Access to full scala/java language features (functions, classes, modules) allows to implement an arbitrary modular design.
        <li>(+) Vast ecosystem of java libraries simplifies development but it still operates on rather low-level primitives.
        <li>(-) Flink is a high-class Apache citizen, but has much less adoption and community around it compared to Spark.
        <li>(-) Limited ecosystem of solutions for DevOps, security, RBAC, logging.
        <li>(!) Moderate community and adoption compared to Spark, but Flink is considered the main real-time streaming technology.
    </td>
    <td>
        <li>(+) Vast ecosystem of spark drivers and libraries is available.
        <li>(+) Access to full scala/python/.NET for Spark language features (functions, classes, modules) allows to implement an arbitrary modular design.
        <li>(+) Spark as an OSS solution has a vast community (libraries, StackOverflow, GitHub issues).
    </td>
    <td>
        <li>(+) Libraries for selected language can be used (NuGet packages in case of C#)
        <li>(+) Connecting to input/output services is abstracted and can be done in a very simple way
        <li>(+) Functions runtime and underlying WebJobs SDK are open source projects
        <li>(+) Azure Functions Runtime is portable and can be ran outside of Azure
        <li>(+) Wide usage of Azure Functions results in strong active community
        <li>(+) Runtime v2 released in 2018 brought large performance improvements and is already well tuned.
    </td>
</tr>
<tr>
    <td>Observability</td>
    <td>
        <li>(+) Notebooks (including Jobs) allows to visualize issues and show stack traces next to the failing code.
        <li>(!) Low-level cluster, storage etc logs can be sent to storage or Azure Log Analytics. Functionality is optimized for DataBricks, but limited in extensibility compared to OSS ecosystem.
    </td>
    <td>
        <li>(+) Flink exports basic metrics and logs about usage, jobs status and etc, that can be consumed by systems and represented with Grafana.
    </td>
    <td>
       <li>(+) Notebooks (including Jobs) allows to visualize issues and show stack traces next to the failing code.
       <li>(+) Default monitoring of jobs provided thru Azure portal, displaying information such as job status, data read/write size etc.
    </td>
    <td>(+) Offers built-in integration with Application Insights. Runtime/system and function level logs and metrics can be collected
        (+) Application level logging can use various technologies and can be defined in code
        (!) Data from Application Insights (Log Analytics) can be exported to other solutions
    </td>
</tr>
<tr>
    <td>Unique functionality</td>
    <td>
        <li>(+) Effortless Spark: cluster is decoupled from configuration that makes management easier.
        <li>(+) Notebooks and collaboration workspaces simplifies data experimenting and development.
    </td>
    <td>
        <li>(+) Built for real-time data processing with high performance and throughput targets.
    </td>
    <td>
        <li>(+) .NET for Spark support out of the box
    </td>
    <td>
        <li>(+) Abstraction of inputs, outputs and triggers and simple programming model
        <li>(+) Automatic on demand scaling
        <li>(+) Allow creation of asynchronous workflows using Durable Functions extensions
    </td>
</tr>
<tr>
    <td>Documentation</td>
    <td>
        <li>(+) Spark documentation is extensive, but hard to search.
        <li>(!) DataBricks documentation covers main scenarios but lacks details.
    </td>
    <td>
        <li>(+) Good documentation and books are available, covering core concepts.
    </td>
    <td>
        <li>(+) Initial official documentation exists, as it is new service, no resources from community (such as blogs, SA questions etc.)
    </td>
    <td>
        <li>(+) Detailed extensive official documentation exists, includes also large amount of samples.
    </td>
</tr>
</table>
