# Integration events communication

* Status: proposed
* Deciders: @sondergaard
* Date: 2021-06-15

## Context and Problem Statement

The system is designed as an `micro service architecture`. This promotes that the different components eg. `micro services`, publishes events and/or subscribe to events.

When designing `micro services` there are some design principles. One of these are that services are `Loose coupled`. What this means is that a service should know as little as possible about the surrounding services. This promotes event based integrations over request/response. With event based integrations, an event is published and an infrastructure component forwards the event to those services that are interested in the event.

## Decision Drivers

* teams are starting to reach out and define integration event
* integration events needs to be exchanged in a uniform way across domains

## Considered Options

* [Azure Event Hubs](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-about)
* [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)

## Decision Outcome

Chosen option: "[option 1]", because [justification. e.g., only option, which meets k.o. criterion decision driver | which resolves force force | … | comes out best (see below)].

### Positive Consequences <!-- optional -->

* [e.g., improvement of quality attribute satisfaction, follow-up decisions required, …]
* …

### Negative Consequences <!-- optional -->

* [e.g., compromising quality attribute, follow-up decisions required, …]
* …

## Pros and Cons of the Options <!-- optional -->

### Azure Event Hubs

Azure Event Hubs is designed for big data and streaming. It is capable of scaling from megebytes to gigabytes of data.

It builds upon the concept of pub/sub and their by decoupling producer and consumer of events.

* Can process huge amount of event with low latency
* Guaranties delivery at least once
* Supports ordering of events, if they are pinned to a specific partition. If high availability is important, then don't target a specific partition.

### Azure Service Bus

Azure Service Bus is an enterprise grade message broker with message queues and pub/sub topics. This can be utilized to decouple services.

* Support for message sessions that supports message ordering
* Topics and subscriptions that cater for 1:n relations ships for publishers and consumers
* Load balancing that ensures messages read from the same queue are delivered exclusively to one consumer.
* It does not support the same level of throughput as Azure Event Hubs

## Provisioning of infrastructure

How should the shared infrastructure be provisioned?
