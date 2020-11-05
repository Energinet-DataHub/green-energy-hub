<!--
Copyright 2020 Energinet DataHub A/S 

Licensed under the Apache License, Version 2.0 (the "License"); 
you may not use this file except in compliance with the License. 
You may obtain a copy of the License at 

    http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software 
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
-->

# Fast and cheap ID storage and lookup

When we receive messages from our providers we want to verify that their Message reference, document identification and transaction ids are unique.  
To be able to do this, we need a cheap and fast storage type to handle lookups for preexisting IDs.

Redis seemed like the obvious candidate for this but it turned out to be too expensive due to the amount of data that needs to be stored.

This lead us to look at Azure Data Tables since it provides very cheap storage and hopefully also fast performance.

## Calculations

The calculations below are done based on the following values:

- We have 3.5M metering points in Denmark
- Each data row consists of a sample provider ID, a GUID (Without dashes) and a timestamp.
- A sample row takes up approx. 106 bytes in azure data tables (74 bytes on disk)

### Baseline calculation

- 3.5M metering points being updated once a day with 80% of messages being sent in twice **6.3M IDs per day**.  
- 6.3M IDs per day equals approx. **191M IDs every month**.

#### Transactions per month

Assuming we're able to batch insert those IDs somewhat efficiently, inserting 70 IDs at a time, that would result in **~2.8M insert transactions**.  
Since we're going to try to do the unique check **on insert** we won't need any read transactions.  
Meaning that the final monthly cost is `2.8M *  0,0000002266kr/transaction = ~0,6kr.`.

Notes:

- We are assuming that it's possible to batch our inserts. This might not be the case depending on when in our process the lookup happens.

#### Storage per month

This number depends on how many months of incoming IDs we want to store.
Storing just one month of data takes up ~107M IDs * ~106 bytes = **~10.5 GB**
Resulting in a monthly cost of `~10.5GB * 0,3777kr./GB = ~4kr.` with geo redundancy.

### Variables

There are quite a few variables that can be adjusted in these calculations.
This section will try to address this by giving some examples on how they affect the monthly price.

#### Storage period

Storing the IDs for a longer period of time increases the storage capacity needed each month.

| Months | Rows           | Table storage | Storage price / month |
|--------|----------------|---------------|-----------------------|
| 1      | 106.975.135    | 10,58 GB      | 4,00 kr.              |
| 3      | 320.925.405    | 31,75 GB      | 11,99 kr.             |
| 6      | 641.850.811    | 63,50 GB      | 23,99 kr.             |
| 12     | 1.283.701.622  | 127,01 GB     | 47,97 kr.             |
| 36     | 3.851.104.865  | 381,03 GB     | 143,91 kr.            |
| 96     | 10.269.612.973 | 1.016,07 GB   | 383,77 kr.            |

#### Report frequency

Increasing the number of times the measurements are sent to the system will increase the number of IDs we need to store.

| Reports per day | Months | Rows            | Table storage | Storage price / month |
|-----------------|--------|-----------------|---------------|-----------------------|
| 1x (current)    | 1      | 106.975.135     | 10,58 GB      | 4,00 kr.              |
|                 | 3      | 320.925.405     | 31,75 GB      | 11,99 kr.             |
|                 | 6      | 641.850.811     | 63,50 GB      | 23,99 kr.             |
|                 | 12     | 1.283.701.622   | 127,01 GB     | 47,97 kr.             |
|                 | 36     | 3.851.104.865   | 381,03 GB     | 143,91 kr.            |
|                 | 96     | 10.269.612.973  | 1.016,07 GB   | 383,77 kr.            |
|                 |        |                 |               |                       |
| 4x              | 1      | 427.900.541     | 42,34 GB      | 15,99 kr.             |
|                 | 3      | 1.283.701.622   | 127,01 GB     | 47,97 kr.             |
|                 | 6      | 2.567.403.243   | 254,02 GB     | 95,94 kr.             |
|                 | 12     | 5.134.806.486   | 508,04 GB     | 191,89 kr.            |
|                 | 36     | 15.404.419.459  | 1.524,11 GB   | 575,66 kr.            |
|                 | 96     | 41.078.451.892  | 4.064,29 GB   | 1.535,08 kr.          |
|                 |        |                 |               |                       |
| 12x             | 1      | 1.283.701.622   | 127,01 GB     | 47,97 kr.             |
|                 | 3      | 3.851.104.865   | 381,03 GB     | 143,91 kr.            |
|                 | 6      | 7.702.209.730   | 762,05 GB     | 287,83 kr.            |
|                 | 12     | 15.404.419.459  | 1.524,11 GB   | 575,66 kr.            |
|                 | 36     | 46.213.258.378  | 4.572,32 GB   | 1.726,97 kr.          |
|                 | 96     | 123.235.355.676 | 12.192,86 GB  | 4.605,24 kr.          |

## Benchmark

The benchmarks in this section have been performed to verify that Azure Table Storage will live up to our performance requirements.

### Setup

The most critical part to test is if we're able to do the ID lookups fast enough since we're going to be receiving a lot of messages.
To test this a Table Storage has been loaded with 92M sample rows consisting of provider ID, Message ID (GUID) and a timestamp.

### System requirements

To fulfill the requirements of the system we need to be able to keep up with the amount of incoming IDs.
The system is currently running with a report frequency of once per day. Looking at the **Calculations** section we can see that that means we need to support 6.3M IDs per day.
Distributing 6.3M IDs evenly across the day results in `6.3M / 24 / 60 / 60 = ~73` IDs per second.

As mentioned in our calculations above we might be able to make do only with inserts.
This would open up for the option to do batch inserts instead. Assuming we can insert 70 IDs at a time that would leave us with a requirement of `6.3M / 70 / 24 / 60 / 60 = ~1` transaction per second - Giving us quite the performance boost.

### Test requests

To perform the test requests we use a subset of the total sample rows and query the Data Table for them.

#### Initial test

To start off with we execute a range of lookup requests synchronously.
Each iteration ran three times in an attempt to detect and mitigate variations in the lookup speed.

##### Synchronous

| Number of lookups | Run time - 1 | Run time - 2 | Run time - 3 | Average lookup time |
|-------------------|--------------|--------------|--------------|---------------------|
| 1                 | 555ms        | 548ms        | 547ms        | 550,000ms           |
| 10                | 850ms        | 584ms        | 568ms        | 66,733ms            |
| 100               | 1372ms       | 943ms        | 975ms        | 10,967ms            |
| 1000              | 4488ms       | 5012ms       | 4630ms       | 4,710ms             |
| 10000             | 45270ms      | 40909ms      | 40245ms      | 4,214ms             |

Firing just one request results in a rather high request time. This seems to be caused by the first request establishing the connection after which it's kept open.
The subsequent requests are a lot faster and the more requests we execute the closer we get to a baseline of approx. 5ms per request.

Since each file contains ~370 transactions we end up in the 4-11ms territory leaving us with about `1000ms / 10ms = 100` requests per second.

To optimize the performance the requests can be parallelized by doing them asynchronously.
Making this change results in the following performance:

##### Asynchronous

| Number of lookups | Run time - 1 | Run time - 2 | Run time - 3 | Average lookup time |
|-------------------|--------------|--------------|--------------|---------------------|
| 100               | 1280ms       | 648ms        | 640ms        | 8,560ms             |
| 1000              | 1785ms       | 1189ms       | 2007ms       | 1,660ms             |
| 10000             | 3517ms       | 3463ms       | 3117ms       | 0,337ms             |
| 30000             | 7907ms       | 7069ms       | 7023ms       | 0,244ms             |

Redoing the rps calculation again we arrive at the following: `1000ms / 8ms = 125` requests per second.

## Partial conclusion

The initial impression of our tests and calculations show that Azure Table Storage is able to fulfill our requirements as a fast yet cheap ID storage.
If we're not able to utilize the batch inserts, the benchmarks do make it seem as if we're only barely able to keep up with the incoming requests. However considering the following key points we can rest assured that we're well above the required performance.
Furthermore the savings are still tangible. Assuming we're going to store IDs for 6 months that would account for approx. 60GB of data. Our calculations show that that would amount to **~23kr/month**.
The largest Basic Redis config provides 54GB of storage and costs **3800kr/month**.

### Read VS Write

The benchmarks were done for read speeds.
This means that our benchmarks are not quite correct since reading will most likely be slower than writing.
Seeing as we need a write speed of around 1 transaction per second and our benchmarks showed read speeds of around 125 we should be well within what we need.

### Partitioning and parallel requests

The benchmark results are based on the assumption that the incoming requests are done one after the other.
This is obviously not the case since we're receiving requests from multiple providers and therefore these processes are able to run in parallel on multiple different partitions.
Spreading the load over multiple partitions means that we can assume that the performance will scale close to linearly with the amount of simultaneous requests.
If that is the case the limit for the system will be the ~22k request per second limit on a Storage Container.  
