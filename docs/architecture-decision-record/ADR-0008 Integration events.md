# ADR-0008 Integration events with protocol buffers

* Status: accepted
* Deciders: @sondergaard, @MartinFHansen
* Date: 2021-08-23

This document describes how integration events are defined, distributed and consumed in the system architecture of GreenEnergyHub.

## Defining integration events

All integration events are defined as [protocol buffer contracts](https://developers.google.com/protocol-buffers "Protocol Buffers | Google Developers"). By using this message technology, we get some benefits.

* Strong contracts
* Supports new fields fields to existing contracts
* Tooling that supports the programming languages used in GreenEnergyHub
* Fast and efficient (de)serializing
* Binary format that is fast to transmit over the wire

### Designing contracts

When a contract is made public available it's considered final. This means that all modifications to the contract is backwards compatible. You are allowed to add new fields to a contract. Ordering of fields, data types, removal of fields etc. are considered breaking changes. If a breaking change is unavoidable then the contract should be duplicated and given a new name.

A protocol buffer contract file should only contain one contract. If enums are used in the contract, then they are defined within the file as nested elements.

``` proto
message CreateMeteringPoint {
    enum MeteringPointType {
        CONSUMPTION = 1;
        PRODUCTION = 2;
        EXCHANGE = 3;
    }
    MeteringPointType metering_point_type = 1;
}
```

Please follow the [style guide](https://developers.google.com/protocol-buffers/docs/style "Style Guide | Google Developers") from Google when writing a contract.

Each field should be documented with a comment describing the purpose of the field. Also the datatype should be as explicit as possible.

* Use timestamp for date and time
* If a field contains a finite/practical number of valid values, use an enum
* Use corresponding numeric datatype, signed/unsigned etc.
* Use string for 'strings'

### Documentation

All public exposed contracts should be documented. If the comment can fit on a single line then use `///` annotation. If the comment is a block we use `/* */`

``` protobuf
syntax = "proto3";

/**
 * Create a metering point
 */
message CreateMeteringPoint {
    string metering_point_id = 1; /// GLN number identifying the metering point
}

```

### Caveats

`Enums` are treated in a special manner. The fields of a enum *must be* unique within a message. The following message would result in a compiler error.

``` protobuf
syntax = "proto3";

message CreateMeteringPoint {
    enum MeteringPointType {
        CONSUMPTION = 0;
        PRODUCTION = 1;
        EXCHANGE = 2;
    }

    enum PlantType {
        SOLAR = 0;
        COAL = 1;
        WIND = 2;
    }

    enum CustomerType {
        PRODUCTION = 0;
        CONSUMPTION = 1;
    }

    MeteringPointType metering_point_type = 1;
    PlantType plant_type = 2;
    CustomerType customer_type = 3;
}
```

When the contract is compiled it will report with an error:

> CMP.proto:17:9: "PRODUCTION" is already defined in "CreateMeteringPoint".
> CMP.proto:17:9: Note that enum values use C++ scoping rules, meaning that enum values are siblings of their type, not children of it.  Therefore, "PRODUCTION" must be unique within "CreateMeteringPoint", not just within "CustomerType".

To workaround this issue we have chosen to prefix every enum field with the name of the enum in abbreviated form.

``` protobuf
    enum MeteringPointType {
        MPT_CONSUMPTION = 0;
        MPT_PRODUCTION = 1;
        MPT_EXCHANGE = 2;
    }

    enum CustomerType {
        CT_PRODUCTION = 0;
        CT_CONSUMPTION = 1;
    }
```

## Distribute integration events

Integration events are distributed so that services are coupled as loosely as possible. The preferred way is to use the publisher / subscriber pattern. Azure Service Bus when the message volume is moderate. In high throughput scenarios Azure Event Hub should be used.

All the infrastructure for Azure Service Bus is defined in a [shared repository](https://github.com/Energinet-DataHub/geh-shared-resources "Shared Resources").

* `System team` is providing a shared Azure Service Bus namespace for all integration events
* The team that is defining an integration event is responsible for creating the corresponding topic in the shared namespace
* If a team needs to subscribe to an integration event, then they are responsible for creating the subscription in the corresponding topic
* Optionally, a subscription can forward all events to a queue. A service can then receive all events from a single endpoint

### Message metadata

We want metadata to travel along the message when it is sent as an integration event. The purpose of the metadata is to describe the event that is published so that a recipient can evaluate how the event should be processed and tracked.

If an event is retransmitted, all the values would match that of any previous copies that had been sent.

#### Timestamp

The `OperationTimestamp` key contains a UTC timestamp. The value represents the point in time, when the sending application created the event.

#### CorrelationId

The key `OperationCorrelationId` contains a string. The value is the correlation id for the current operation.

#### Message version

The key `MessageVersion` contains an integer. When an application publishes an event it is responsible for setting the message version. This is used on the receiving end to identify how the message should be processed if the contract has evolved.

#### Message type

The key `MessageType` contains a string. The value is identical to the name of message/event.

#### Event identification

The key `EventIdentification` contains a UUID. With this value an event can be uniquely identified.

### Message naming

When choosing a name for an event it should be clear what the intent is. The consumer of an event can expect to get all data related to the event - no need to contact the sending party to get the remaining data. A message name should follow the pattern `<entity><verb-in-past-tens>`.

Examples:

* MeteringPointCreated
* MeteringPointConnected
* EnergyConsumptionMeasured

### Naming infrastructure components

When publishing an event to a topic, the topic should have the same name as the event. This means that a topic is only used for one event type. If an application needs to publish more events, then it should create multiple topics.

Topic name would be `metering-point-created`.

When a subscription is added to a topic it is easy to identify the consumer of the topic. The naming convention is to include the receiving applications name.

Subscription name for `metering-point-created` and `time-series` would result in: `metering-point-created-sub-time-series`.

## Consuming integration events

When an application is consuming events a few things that should be considered.

### Logging and monitoring

It is the responsibility of the application to monitor the subscription. This would involve measuring the incoming message rate, processed message rate, if messages ends in dead-letter-queue etc.

There should be proper logging for when a message is not able to be processed. The log would contain details about the exception/reason to why the message failed, and what action that was taken when it failed.

### Design for idempotent

The same message will eventual end up being delivered more then once. The consumer of the message should be able to handle this situation.

A way to handle this could be with the use of `EventIdentification`.
