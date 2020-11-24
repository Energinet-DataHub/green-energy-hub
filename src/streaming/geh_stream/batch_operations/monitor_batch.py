# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
import inspect
from pyspark.sql import DataFrame


def get_involved_correlation_ids(batch_df: DataFrame, watch):
    """Determines which correlation ID's are represented in the dataframe batch currently being processed.
    The correlation ID is used to track the various atomic values of a time series back to the original request where they were reported.
    In the original request, multiple times series, each with multiple atomic values were sent in a single request.

    Parameters:
        batch_df (DataFrame): The batch currently being processed which will be used to determine which correlation ID's are represented in the batch.
        watch (MonitoredStopwatch): The stopwatch currently tracking the performance of the batch. Will be used to start a sub-stopwatch monitoring this method.

    Returns:
        ids (list of class Row): List with a row per correlation id, with the id itself and the amount of times it was represented in the batch.

    """
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    ids = batch_df \
        .groupBy("CorrelationId") \
        .count() \
        .collect()

    timer.stop_timer()
    return ids


def get_rows_in_batch(batch_df: DataFrame, watch):
    """Retrieves the amount of rows in the batch currently being processed.

    Parameters:
        batch_df (DataFrame): The batch currently being processed which will be used to determine which correlation ID's are represented in the batch.
        watch (MonitoredStopwatch): The stopwatch currently tracking the performance of the batch. Will be used to start a sub-stopwatch monitoring this method.

    Returns:
        count (int): The amount of rows in the current batch

    """
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    count = batch_df \
        .count()
    timer.stop_timer()
    return count


def track_batch_back_to_original_correlation_requests(correlation_ids, batch_count, telemetry_client, watch):
    """Tracks the performance of the batch back to the original requests each atomic value came from.
    The correlation ID of an atomic value is the way to track it back to the original request where it was added.
    Atomic values from the same batch may be split into multiple batches when processing the stream, but we add information about each batch to Application Insights to be able to see when the individual parts of the original request were processed.
    Typically a batch will contain multiple atomic values from the same original request, so we will not have the same number of correlation IDs as rows in the dataframe. Usually alot less.
    For each correlation ID, the number of items in the batch that belonged to the the request, the total rows in the batch and the average time spent per row is reported back to the original request.

    Parameters:
        correlation_ids (list of class Row): The correlation IDs that were part of the batch
        batch_count (int): The number of rows in the current batch
        telemetry_client (TelemetryClient): The client used to track the result to Application Insights
        watch (MonitoredStopwatch): The stopwatch used to monitor the performance of the batch, which will contain the time used on the batch.

    """
    name = "AtomicValueHandledInBatch"
    data = None
    dependency_type = "databricks"
    properties = {"BatchDependencyId": watch.watch_id}

    # row[0] will be CorrelationId, row[1] count, see above
    for row in correlation_ids:
        measurements = None
        if batch_count:
            average_per_item = watch.duration_ms / batch_count
            measurements = {"TotalCountInBatch": batch_count, "CorrelationItemsInBatch": row[1], "AverageMsPerItem": average_per_item}

        telemetry_client.context.operation.id = row[0]
        telemetry_client.context.operation.parent_id = None
        telemetry_client.track_dependency(name, data, type=dependency_type, duration=watch.duration_ms, properties=properties, measurements=measurements)

    telemetry_client.flush()
