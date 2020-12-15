# Databricks API

An aggregation process should run as a collection of scheduled jobs created using the [Databricks API](https://docs.databricks.com/dev-tools/api/latest/jobs.html).
Time period and when to run an aggregation process is configurable.

## Create new aggregation job

Properties to configure in request body:

`name`  
`existing_cluster_id`  
`python_file`  
`parameters`  
`quartz_cron_expression`  
`timezone_id`  

## Delete aggregation job

Property to configure in request body:

`job_id`
