# Integration events communication

* Status: proposed
* Deciders: @sondergaard
* Date: 2021-06-15

## Context and Problem Statement

The system is designed as a `micro service architecture`. This promotes that the different components eg. `micro services`, publishes events and/or subscribe to events.

When designing `micro services` there are some design principles. One of these are that services are `Loose coupled`. What this means is that a service should know as little as possible about the surrounding services. This promotes event based integrations over request/response. With event based integrations, an event is published and an infrastructure component forwards the event to those services that are interested in the event.

## Decision Drivers

* teams are starting to reach out and define integration event
* integration events need to be exchanged in a uniform way across domains

## Considered Options

* [Azure Event Hubs](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-about)
* [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)

## Decision Outcome

Both technologies support message pub/sub. The difference lies in the volume and the guarantees they give. `Azure Event Hub` is capable of handling massive amount of events, but it comes as the cost of ordering of messages and guarantees about delivering messages. `Azure Service Bus` does not support as high throughput but is capable of storing events for a defined time-limit, ordering of messages, sessions and can route messages based on topics and queues.

With this in mind each technology should be used for specific tasks.

* When publishing time series events we have to deal with high volume - this fits with the model of `Azure Event Hub`.
* For other events the hypothesis is that the volume is very low compared to time series. The volume is expected to fit within the limitations of `Azure Service Bus`.

## Pros and Cons of the Options <!-- optional -->

### Azure Event Hubs

Azure Event Hubs is designed for big data and streaming. It is capable of scaling from megebytes to gigabytes of data.

It builds upon the concept of pub/sub and thereby decoupling producer and consumer of events.

* Can process huge amounts of events with low latency
* Guarantees delivery at least once
* Supports ordering of events, if they are pinned to a specific partition. If high availability is important, then don't target a specific partition.

### Azure Service Bus

Azure Service Bus is an enterprise grade message broker with message queues and pub/sub topics. This can be utilized to decouple services.

* Support for message sessions that supports message ordering
* Topics and subscriptions that cater for 1:n relationships for publishers and consumers
* Load balancing that ensures messages read from the same queue are delivered exclusively to one consumer.
* It does not support the same level of throughput as Azure Event Hubs

## Provisioning of infrastructure

To empower the teams in being able to start creating integration events, the infrastructure needs to be in place.
Team Mighty Duck will create and maintain the Azure Service Bus Namespace, and Azure Event Hub Namespace.
They will also deliver documentation on how to use these shared ressources.

Each individual team will then be responsible for creating and maintaining the Topics/Queues on the Service Bus namespace and Event Hubs on the Event Hub namespaced needed for their integration events to work.
They will have to put these inside the shared ressources created by Team Mighthy Ducks.

To read more about how you can start using the shared ressources, follow this [link](https://github.com/Energinet-DataHub/geh-shared-resources#using-the-shared-resources).
