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
from pyspark.sql import DataFrame

from geh_stream.monitoring import Telemetry, MonitoredStopwatch


def get_rows_in_batch(batch_df: DataFrame, watch):
    """Retrieves the amount of rows in the batch currently being processed.

    Parameters:
        batch_df (DataFrame): The batch currently being processed which will be used to determine which correlation ID's are represented in the batch.
        watch (MonitoredStopwatch): The stopwatch currently tracking the performance of the batch. Will be used to start a sub-stopwatch monitoring this method.

    Returns:
        count (int): The amount of rows in the current batch
    """
    current_method_name = get_rows_in_batch.__name__
    timer = watch.start_sub_timer(current_method_name)
    count = batch_df.count()
    timer.stop_timer()
    return count


def __track(correlation_ids_with_count, telemetry_instrumentation_key, *, batch_row_count: int, batch_dependency_id, batch_duration_ms):
    correlation_id = correlation_ids_with_count[0]
    correlation_row_count = correlation_ids_with_count[1]

    name = "TimeSeriesHandledInBatch"
    data = None
    dependency_type = "databricks"
    properties = {"BatchDependencyId": batch_dependency_id}
    measurements = None
    if batch_row_count:
        average_per_item = batch_duration_ms / batch_row_count
        measurements = {"TotalCountInBatch": batch_row_count, "CorrelationItemsInBatch": correlation_row_count, "AverageMsPerItem": average_per_item}

    telemetry_client = Telemetry.create_telemetry_client(telemetry_instrumentation_key)
    telemetry_client.context.operation.id = correlation_id
    telemetry_client.context.operation.parent_id = None
    telemetry_client.track_dependency(name, data, type=dependency_type, duration=batch_duration_ms, properties=properties, measurements=measurements)
    telemetry_client.flush()


def track_batch_back_to_original_correlation_requests(time_series_points_df: DataFrame, batch_info, telemetry_instrumentation_key):
    """Tracks the performance of the batch back to the original requests each atomic value came from.
    The correlation ID of an atomic value is the way to track it back to the original request where it was added.
    Atomic values from the same batch may be split into multiple batches when processing the stream, but we add information
    about each batch to Application Insights to be able to see when the individual parts of the original request were processed.
    Typically a batch will contain multiple atomic values from the same original request, so we will not have the same number
    of correlation IDs as rows in the dataframe. Usually alot less.
    For each correlation ID, the number of items in the batch that belonged to the the request, the total rows in the batch and
    the average time spent per row is reported back to the original request.

    Parameters:
        correlation_ids (list of class Row): The correlation IDs that were part of the batch
        batch_count (int): The number of rows in the current batch
        watch (MonitoredStopwatch): The stopwatch used to monitor the performance of the batch, which will contain the time used on the batch.
    """
    from functools import partial
    # Use 'partial' to solve the problem that UDFs don't take arguments
    track_function = partial(__track, telemetry_instrumentation_key=telemetry_instrumentation_key, **batch_info)
    track_function.__doc__ = """User defined function with single argument feeded by the spark engine.
                             It is necessary that the function doesn't hold object instance references
                             because the function will be serialized and sent to all worker nodes."""

    time_series_points_df \
        .groupBy("CorrelationId") \
        .count() \
        .foreach(track_function)
