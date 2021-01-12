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
import datetime
import uuid


class MonitoredStopwatch:
    """Class used to track timing of an operation in Application Insights.
    The class can be used to build a hierarchy of stopwatches which will be tracked with the provided hierachy in Application Insights.
    If an operation id is provided the hierachy will be attached to the existing operation, otherwise a new operation will be started.
    """

    @staticmethod
    def start_timer(telemetry_client, name, operation_id="", parent_operation_id=""):
        """Creates a new monitored stopwatch and starts the timer on it

        Parameters:
            telemetry_client (TelemetryClient): Client used to track the stopwatch to Application Insights
            name (str): Name of the stopwatch, which will be viewable in Application Insights
            operation_id (str): The ID of the operation this watch will be part of in Application Insights (default is empty, where a new operation will be started)
            parent_operation_id (str): The ID of the parent within the operation (default is empty, which will be pointing the top element of the operation)

        Returns:
            A MonitoredStopwatch object which will be tracking the time to application insights

        """
        start_time = datetime.datetime.now()
        watch = MonitoredStopwatch(telemetry_client, name, start_time, operation_id, parent_operation_id)
        return watch

    def __init__(self, telemetry_client, name, start_time, operation_id, parent_operation_id):
        """Creates a new monitored stopwatch.

        Please use the method start_timer, as that will create the object as well as start the timer.

        """
        if not operation_id:
            operation_id = str(uuid.uuid4())
        if not parent_operation_id:
            parent_operation_id = operation_id

        self.watch_id = str(uuid.uuid4())
        self.telemetry_client = telemetry_client
        self.name = name
        self.start_time = start_time
        self.operation_id = operation_id
        self.parent_operation_id = parent_operation_id

    def stop_timer(self, item_count=None):
        """Stops the monitored stopwatch and sends the resulting time to Application Insights.
        If the item_count property is provided then the average time per item will be calculated and attached to the result.

        Parameters:
            item_count (int): The number of items that were processed as part of what is being tracked by the stopwatch (default is none)

        """
        self.stop_time = datetime.datetime.now()
        self.duration_ms = round((self.stop_time - self.start_time).total_seconds() * 1000, 1)
        self.item_count = item_count
        self._track()

    def start_sub_timer(self, name):
        """Stars a new stopwatch will be the child of the current stopwatch. In Application Insights this new stopwatch will be shown as a child of the parent stopwatch.

        Parameters:
            name (str): The name of the new sub-stopwatch, which will be viewable in Application Insights

        Returns:
            A MonitoredStopwatch object setup to be the sub-stopwatch of this stopwatch

        """
        sub_timer = MonitoredStopwatch.start_timer(self.telemetry_client, name, self.operation_id, self.watch_id)
        return sub_timer

    def _track(self):
        """Sends the duration timed by the stopwatch to Application Insights.
        If an item count has been provided, also calculates the average time spent per item.
        Called when the stopwatch is stopped to report the result

        """
        data = None
        dependency_type = "databricks"
        measurements = None

        if self.item_count:
            average_per_item = self.duration_ms / self.item_count
            measurements = {"ItemCount": self.item_count, "AverageMsPerItem": average_per_item}

        self.telemetry_client.context.operation.id = self.operation_id
        self.telemetry_client.context.operation.parent_id = self.parent_operation_id
        self.telemetry_client.track_dependency(self.name, data, type=dependency_type, duration=self.duration_ms, measurements=measurements, dependency_id=self.watch_id)
        self.telemetry_client.flush()
