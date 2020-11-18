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
