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
"""
Utility methods for using application insights telemetry
"""

import applicationinsights


class Telemetry:

    @staticmethod
    def create_telemetry_client(instrumentationKey, operationId="", parentOperationId=""):
        tc = applicationinsights.TelemetryClient(instrumentationKey)
        tc.context.application.id = "streamingprocessing"
        tc.context.application.ver = "1.0.0"
        tc.context.device.id = "spark"
        tc.context.operation.id = operationId
        tc.context.operation.parentId = parentOperationId
        return tc
