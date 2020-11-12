import enum


class SchemaNames(enum.Enum):
    Master = 1       # the schema of the Master data
    MessageBody = 2  # the schema of the json content of the message itself
    Parsed = 3       # the schema of the json content plus the Enqueued time from Event Hub
