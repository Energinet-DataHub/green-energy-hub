# Performance Measurements

## Measurements

Time in seconds it requires to process a batch with the following parameters (as of 2020-12-14):

- `MaxEventsPerTrigger`: 10 000 time series (240 000 atomic)
- Trigger: 1 second
- Nodes: 9 workers + 1 master

## Results

Data processing (measured with databricks display). The numbers are cumulative. So e.g. validation adds 0.4s to the batch processing time. All measurements are for a single batch with 10.000 time series.

- event hub ingestion (raw_data) -- 0.7s
- parsing (parsed_data) -- 1s
- explosion (exploded_data) -- 1.2s
- enrichment (enriched_data) -- 1.8s
- validation (validated_data) -- 2.2s

Biggest delays happens while storing data to the storage. Below is the performance of the whole processing pipeline while storing results to Azure Data Lake Gen2 (both standard and premium). Additionally to the current design, parquet storing format as well as impact of partitioning, appinsights logic and eventhub streaming were taken into the consideration. As forwarding to Event Hub (or some other sink) is required, this scenarios only serve the purpose of analyzing the impact of the various parts of the batch operations:

- `forEachBatch` with features (current design):
    - delta with appinsights, eventhub and partitioning (current code) -- 13.7s
    - delta with appinsights and partitioning without eventhub-- 9.5s
- `forEachBatch` (without appinsights and eventhub):
    - delta with partitioning: Standard -- 7.2s, Premium -- 7.8s
    - delta without partitioning: Standard -- 4.6s, Premium -- 4.1s
    - parquet with partitioning: Standard -- 4.5s, Premium -- 4.4s (degrades with time)
    - parquet without partitioning: Standard -- 2.8s, Premium -- 2.7s (degrades with time)
- `writeStream` (without appinsights and eventhub):
    - delta with partitioning: Standard -- 5.5s, Premium -- 5.4s
    - delta without partitioning: Standard -- 2.9s, Premium -- 2.5s
    - parquet with partitioning: Standard -- 2.9s, Premium -- 2.7s (degrades with time)
    - parquet without partitioning: Standard -- 1.5s, Premium -- 1.2s (degrades with time)

Partially the performance is affected by the fixed costs related to spark operations and storage writes. Below is the numbers for different values of `MaxEventsPerTrigger` that affects batch size per trigger (without appinsights and eventhub):

- `forEachBatch`:
    - delta with partitioning: 10 000 -- 7.8s, 20 000 -- 11.4s, 40 000 -- 20.4s, 80 000 -- 36.0s
    - parquet with partitioning: 10 000 -- 5s, 20 000 -- 7.5s, 40 000 -- 11.9s, 80 000 -- 20.4s
- `writeStream`:
    - delta with partitioning: 10 000 -- 6.1s, 20 000 - 9.8, 40 000 -- 17.6, 80 000 -- 30.6
    - parquet with partitioning: 10 000 -- 3.0s, 20 000 -- 5.0, 40 000 -- 9.3, 80 000 -- 16.6

Scalability with node counts (without appinsights and eventhub):

- `forEachBatch`:
    - delta with partitioning: 3 -- 9.2s, 9 -- 7.2s
    - delta without partitioning: 3 - 7.5s, 9 -- 4.6s
    - parquet with partitioning: 3 -- 6.0s, 9 -- 4.5s (degrades with time)
    - parquet without partitioning: 3 -- 5.9s, 9 -- 2.8s (degrades with time)
- `writeStream`:
    - delta with partitioning: 3 -- 6.9s, 9 - 5.5s
    - delta without partitioning: 3 -- 4.0s, 9 -- 2.9s
    - parquet with partitioning: 3 -- 3.9s, 9 -- 2.9s (degrades with time)
    - parquet without partitioning: 3 -- 2.4s, 9 -- 1.5s (degrades with time)

## Conclusions

- implementation meets the current performance target:
    - target: 5 000 000 metering points sending 1 time series per day with 24 hourly measurements: 5 000 000 / 24h / 60m = 3 472 time series per minute
    - current performance: 10 000 / 13.7s x 60s = 43 795 time series per minute
- implementation should meet the next performance target:
    - target: 5 000 000 metering points sending 1 time series per day with 96 points corresponding to a measurement every 15 minutes: approx. 3 472 x 4 = 13 888
    - current performance: 10 000 / 13.7s x 60s = 43 795 time series per minute
- extensive processing leads to comparable performance degradation compared to simple demo and there is still a large room for optimization:
    - baseline: max recorded demo performance of receiving messages from event hub, parsing them and storing in delta lake was 30 000 atomic messages per second
    - current performance: 10 000 x 24 / 13.7 = 17 518 atomic messages per second
    - optimized (minimize appinsights overhead and sending to eventhub): up to 10 000 x 24 / 5.5 = 43 363
    - extreme optimization might give even much bigger numbers
- in the case of spikes event hub will store messages and the streaming job will process them on the first come first served basis: job runs far from its performance limits, so that no backpressure should happen
- processing pipeline suffers from early explosion that multiplies amount of messages from 10 000 to 240 000 in the test scenario, that leads to a possible performance degradation of the following join operation in enrichment and where clauses in validation. Moving explosion down the processing pipeline should considerably improve processing performance, but won't affect storage write times (that has the bigger impact).
- appinsights calculate the amount of rows with 2s overhead and most probably the same information should be available with minimal impact from spark API
- event hub sending is going to be replaced with CosmosDB whose performance is yet to be find out
- premium data lake gen2 performance is generally slightly better compared to standard HDD based solution (mind the price)
- partitioning by year, month and day, while being required for aggregation jobs, doesn't help writing as it is mainly streaming data to a single "today" partition, but the required shuffling adds a considerable performance overhead
- parquet performs better as it has less overhead compared to delta (timestamps, commits etc) but degrades with time (worth an additional research); delta numbers are mainly stable
- direct `writeStream` to storage allows to save about 2 seconds of processing time per trigger at the cost of adding another potentially smaller cluster for streaming to CosmosDB and other consumers (`forEachBatch` is the only known method to fork the stream; consider delta lake streaming source functionality)
- amplification of `MaxEventsPerTrigger` allows to get better streaming performance at the cost of longer delays (lower fixed costs per row)
- performance rate scales with node count so it is always an option, but its form is not linear potentially due to the growing fixed costs
