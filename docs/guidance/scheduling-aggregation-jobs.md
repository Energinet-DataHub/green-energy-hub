# Scheduling Aggregation Jobs

Based on business requirements to create aggregation job schedules on the fly using Job Scheduling Application there was need to rethink the approach on how the aggregation jobs will be scheduled.
Originally we used schedule property of [Databricks Terraform Job resource](https://registry.terraform.io/providers/databrickslabs/databricks/latest/docs/resources/job), which gave us possibility to predefine job schedule using CRON expression.
This approach however did not provide enough agility when creating or altering schedules.
If person responsible for altering the job schedule would like to perform such change, it would need to be translated into workflow file and job would need to be redeployed with new schedule setting, what is definitely complicated process.

Due to this fact we have decided to create job and its schedule using Databricks API calls. Idea is that UI used for scheduling the jobs will send request to Azure Function, which will hold correct connection details to Databricks workspace/cluster.
This Azure Function will construct proper Databricks API call and post it to the workspace to create job together with its schedule.  Below we are stating simplified architecture:

![Scheduling mechanism diagram](C:\Users\mlani.EUROPE.000\OneDrive - Microsoft\Ascends\Energinet\scheduling.png)

## Determining aggregation period for the job

Currently aggregation jobs are built in a way, that they expect *beginning* and *end time* input parameters, which needs to be specified when sending create job request to Databricks API.
These parameters do not play well with schedule setting of an aggregation job. Even though it is possible to run the job with desired frequency or at desired times, the aggregation results would be calculated always for the same period defined by fixed values of beginning and end time parameter.
In order to be able to schedule the job and run it for valid time period there should be new parameter introduced.
This parameter would be used to determine length of the period for which the aggregation job should be ran (e.g., day, week, month, three years) and the job logic itself would be responsible to figure out the date time boundary of the aggregation period.

Keeping original parameters in play will enable to run the job on demand for particular non predefined period without scheduling it.

## Modeling dependencies between the jobs

During the discussion about requirements for the job aggregation we found out that there is need to enable dependencies between aggregation jobs, meaning aggregation job B can only be executed if aggregation job A already calculated and output the results.
There are two different approaches we can take:

- **Defining dependencies on job level**

  In this approach aggregation job python code needs to be altered so it implements wait loop in the beginning of the job.
  This loop should contain check for existence of the results of other job which are required for the current job to start aggregation work.

  Disadvantage of this approach is, that it doesn't allow adding new dependencies between the jobs without performing the change of aggregation job code.

- **Custom Scheduling Configuration**

  In this approach the scheduling logic is shifted to Aggregation Jobs Manager Azure Function.
  There should be scheduling configuration maintained by this Function App (e.g., in form of file or even database), while this configuration file can take in not only schedule settings but also dependencies between the jobs.
  This function would run in regular intervals (triggered by time trigger), it would check which jobs are due to run and it would start those (without defining schedule on Databricks level)

  Disadvantage of this approach is that implementation of management of schedule configuration is nontrivial task, however it would provide higher level of agility when it comes to job scheduling in comparison to previously stated approach.

  When using this approach, the logic for determining beginning and end date time of aggregation period can be shifted to Azure Function logic and there would be no need to introduce new input parameter to aggregation jobs discussed in [Determining aggregation period for the job section](#Determining aggregation period for the job).

Note: Before decision on the approach of modeling dependencies between the jobs is done, there should be thorough discussion on business requirements connected to aggregation job scheduling.
