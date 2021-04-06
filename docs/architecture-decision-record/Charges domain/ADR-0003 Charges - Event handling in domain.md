# Event handling in the Charges domain

* Status: proposed
* Deciders: `@bemwen, @bjarkemeier, @Mech0z, @lasrinnil, @prtandrup`
* Date: 2021-03-31

Technical Story: ADO-119260

## Context and Problem Statement

This address how we want to handle events in the charge domain, which we have identified to be highly event-driven domain.

## Decision Drivers

* We want to make our architecture event driven
* We want to prepare for the possibility to use event sourcing in the domain
* We want to support multiple subscribers for events

## Considered Options

* Option 1 - Passing `DTO's` though `EventHub's` (like previously)
* Option 2 - Event sourcing, by use of Service Bus retention
* __Option 3 - Preparing for event sourcing, saving events in external storage (for example table storage)__

## Decision Outcome

Chosen option: "Option 3 - Preparing for event sourcing, saving events in external storage (for example table storage)".

In this option, we work towards using Event Sourcing. This means that we work towards letting the events determine the state of our system.

In addition, we aim to save the events themselves in some external storage (for example a table storage).

This options was chosen because it gives us the most freedom in how to design and implement the charges domain, and is also the option most likely to keep the cost of using event sourcing lower (should we choose to go that way).

In comparison, option 1 does not live up to our requirements of multiple subscribers and does not really promote event driven architecture in the degree that we want to.

Option 2, while simple, will take about as much work as option 3, but will most likely be more expensive to run later. In addition, it ties us more to the chosen technologies.

### Positive Consequences <!-- optional -->

* Good: Promotes low coupling
* Good: Total history - You can go back to any specific point in time and recreate the state once event sourcing is implemented
* Good: You are less likely to need locks -> Higher performance
* Good: Works well with given/when/then testing, making testing scenarios easier
* Good: Components do not rely on other components running.
* Good: Refactoring can be less of an issue as you can rebuild your model based on existing events
* Good: Topics allow multiple subscribers to listen to any type of event
* Good: Saving the events to table storage or similar means that we can potentially keep the cost lower as storage is usually cheaper.
* Good: We can implement in stages, starting with event driven architecture and going towards event sourcing if needed
* Good: The SQL server will start owning the data, but can then later be demoted to a query model; we are working towards event sourcing

### Negative Consequences <!-- optional -->

* Bad: We need to build mechanisms for replaying events from the table storage; it does not come out of the box
* Bad: We need to build mechanisms for saving events to the table storage
* Bad: With only events, you cannot easily see the current state of your application; a query model is needed to make it perform
* Bad: It takes more boiler plating
* Bad: Data will be replicated (both event and query model are needed, so the cost is higher than option 1)
* Bad: You are stuck with your events; It will be a log of all your bad design decisions
* Bad: It is another way to work; you have to think differently

### Implementation notes

To support multiple subscribers, we are using topics on the Service Bus rather than queues. Queues serve events to the first one wanting it, meaning that multiple subscribers are not supported.

To keep things simple, we will start by using only one topic for all internal domain events and then use subscription filtering to make sure an internal subscriber only gets the events they want. We can add integration events later using other topics once the need arise.

Charge domain event guidelines used:

* Events are in the past, they are immutable
* As a result, they cannot be deleted
* Events should only contain information needed for the state, and specifically not computed values from previous events

## Pros and Cons of the Options <!-- optional -->

### Option 1 - Passing `DTO's` though `EventHub's` (like previously)

In this options, we use `EventHub` to pass `DTO's` or events between components or services.

* Good: It is a simple design
* Good: It is what we have been doing up until now, so we have the knowledge
* Good: It is cheap
* Good: Components do not rely on other components running
* Good: Events are fire and forget and can be refactored more easily themselves as long as both sender and receiver agree
* Bad: Refactoring the rest of the system can be more of an issue
* Bad: Events cannot be used by multiple subscribers at the same time (easily)
* Bad: We do not save events; we cannot replay them or recreate historical states
* Bad: Model changes require potentially more complex migrations of data
* Consequence: Data is owned by whichever model we use, for example in the SQL server

### Option 2 - Event sourcing, by use of Service Bus retention

In this option, we aim to use Event Sourcing. This means that we let the events determine the state of our system.
In addition, we aim to save the events in the Service Bus itself.

* Good: Promotes low coupling
* Good: Total history - You can go back to any specific point in time and recreate the state
* Good: You are less likely to need locks -> Higher performance
* Good: Works well with given/when/then testing, making testing scenarios easier
* Good: Components do not rely on other components running.
* Good: Refactoring can be less of an issue as you can rebuild your model based on existing events
* Good: Topics allow multiple subscribers to listen to any type of event
* Good: Saving the events in the Service Bus means we do not have to build a new place to store them
* Bad: Saving the events in the Service Bus means retention will be an issue; you need earlier events to build your state from scratch so care will need to be taken in determining when an event is no longer needed.
* Bad: We do not have an out of the box way of replaying older events as subscribers will only get events from the time they subscribe and onward
* Bad: With only events, you cannot easily see the current state of your application; a query model is needed to make it perform
* Bad: It takes more boiler plating
* Bad: Data will be replicated (both event and query model are needed, so the cost is higher than option 1)
* Bad: You are stuck with your events; It will be a log of all your bad design decisions
* Bad: It is another way to work; you have to think differently
* Bad: We rely a lot on Service Bus; we are more tied to it
* Consequence: Data is owned by events; query models (like SQL server data) can be wiped and rebuild with a new model
