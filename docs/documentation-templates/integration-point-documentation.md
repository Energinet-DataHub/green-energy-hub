# Integration Points

When working in a micro-service and domain driven context a large part of the architecture is relying on the bounded context and the integration points between individual domains and services.

To ensure that we are able to define specific contracts containing all relevant information for an integration point, we will try and template all the mandatory information needed to define an integration point.

This document is not a definition of the technical solution needed to run an integration point between two domains - but ensure that all relevant information on what is needed is documented.

## The Bounded Context

We encourage that all domains have a visual overview on the contexts that the domain is a part of. But futher more each of the integration points needs to be defined with following information if applicaple.

## Expected Behavior (rejections, passing etc)

## Ownership of Integration Point (which domain are in charge of defining, building and maintaining the integration point function)

## Dataflow

## Technical Solution

## Message Types and Message Content + the use of Protobuf

## Configurations

## Version Control/Changelog

## Test Data

## Integration monitoring (Reporting, alarms etc)

## How To's

### How to simulate the integration point - using test doubles

### How to test and debug
