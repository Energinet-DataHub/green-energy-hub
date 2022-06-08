# ADR0004 Charges: Post Office integration

* Status: approved
* Deciders: `@bemwen, @bjarkemeier, @prtandrup`
* Date: 2021-10-07

Technical Story: [Design talk on postal office implementation, longer term implications and risk mitigation](https://github.com/Energinet-DataHub/geh-charges/issues/660]) <!-- optional -->

## Context and Problem Statement

What information do we need to store about charge link notifications to the post office, so that we can bundle the data correctly once the post office request it?

## Decision Drivers <!-- optional -->

* Ease of maintainability
* Ease of complexity
* Reuseability
* Ease of support in production

## Considered Options

* Storing the event that initiated the notification
* Generate XML at the time of notification
* Store information about the specific charge link
* Store information about the charge and period affected
* Store information about the metering point and period affected

## Decision Outcome

Chosen option: "Store information about the metering point and period affected".

The option was chosen because it is easy to comprehend, allows us to extend it with sending charge links as part of other processes later and can support both sending all changes and only latest changes to market participants pending the outcome of that discussion.

### Positive Consequences <!-- optional -->

* For each new event that might trigger sending charge links to a market participant, we only need to make functionality to determine which period of the metering point the change will affect
* Various use scenarios, like changes to the links, or triggering of transmission from other processes can be supported by the model
* Bundling functionality can be made once and for all, instead of having to continuously extend with for each new event
* The model makes it easy to make a support tool to retransmit a period to a market participants if bugs or support request warrent it
* The current discussion about whether market participants should receive all changes or only the sum of the changes since their last peek can be handled by the model if the revision time is included in the stored data
* The design has auto-repairing abilities as it will always send the full charge link state for the specified period of the metering point

### Negative Consequences <!-- optional -->

* For each event that triggers the flow, functionality needs to be made to map the event to the period it affects
* The message will not be generated until the bundle request is received
* We will send out more data than might be needed

## Pros and Cons of the Options <!-- optional -->

### Storing the event that initiated the notification

The event that triggers the transfer of the charge link is stored so that the original event can be fetched at the time of receiving the request to bundle messages.

* Good, because it will be easy to make functionality that support sending all changes to a market participant
* Good, because the functionality needed in notifying the post office is simple and easy to comprehend
* Good, because it precisly send the information that is needed
* Bad, because while the notification gets easier, the already complex bundling functionality gets additional layers of complexity
* Bad, because support tools will require more event specific implementation
* Bad, because the message will not be generated until the bundle request is received

### Generate XML at the time of notification

A XML document is generated when the request is received.

* Good, alot of document generation can be done at the time of notification instead of waiting
* Good, because it will be easy to make functionality that support sending all changes to the market participant
* Good, because bunding can be made once and for all, based on generated XML documents which has to adhere to CIM/XML
* Bad, because it will require us to merge XML documents at the time of bundling, which will neither be intuitive, or in the spirit of clean code.
* Bad, because support tools will be harder to make because of the unintuitive design
* Bad, because the time gained by generating the XML at the time of notification might be lost at the time of merging

### Store information about the charge and period affected

When an event to send is received, information about the metering point, the charge the link concerns and the period affected is stored.

* Good, because for each new event that might trigger sending charge links to a market participant, we only need to make functionality to determine which period of the metering point the change will affect, for the specific charge
* Good, because various use scenarios, like changes to the links, or triggering of transmission from other processes can be supported by the model
* Good, because bundling functionality can be made once and for all, instead of having to continuously extend with for each new event
* Good, because the model makes it easy to make a support tool to retransmit a period of a charge to a market participants if bugs or support request warrent it
* Good, because the design has auto-repairing abilities as it will always send the full charge link state for the specified period on the specific charge of the metering point
* Good, because the current discussion about whether market participants should receive all changes or only the sum of the changes since their last peek can be handled by the model if the revision time is included in the stored data
* Bad, because for each event that triggers the flow, functionality needs to be made to map the event to the period it affects
* Bad, because the message will not be generated until the bundle request is received
* Bad, because we will send out more data than might be needed, although less than if on metering point level
* Bad, because it will be as auto-repairing as using the metering point level
