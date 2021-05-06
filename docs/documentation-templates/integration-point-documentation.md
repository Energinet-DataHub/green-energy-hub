# Integration Points

When working in a micro-service and domain driven context a large part of the architecture is relying on the bounded context and the integration points between individual domains and services.

To ensure that we are able to define specific contracts containing all relevant information for an integration point, we have a template for all the mandatory information needed to define an integration point.

## The Bounded Context

We encourage that all domains have a visual overview on the contexts, that the domain is a part of. But futher more each of the integration points needs to be defined with following information when applicable.

## Ownership of Integration Point (which domain are in charge of defining, building and maintaining the integration point function)

Before setting up an integration point we want to establish ownership. This may vary depending on the function of the integration. Having ownership does not mean that other domains cannot make changes through PR's, but the changes must be approved by code owners and must happen to the owning repository. That is why the integration point must also be documented in the domain of the ownership.

## Dataflow

The documentation must contain a description of the dataflow ( e.g. direction, cadence etc).

## Technical Solution

Outline what kind of technical solution the integration point is build upon and how it works.

## Message Types and Message Content + the use of Protobuf

What is being exchanged between domains on the specific integration point.

## Configurations

Are there any specifics configurations or requirements needed to run the integration.

## Expected Behavior (rejections, passing etc)

Please elaborate the business flow(s) that uses the integration point.

## Version Control/Changelog

Ensure that you have considered how to run maintenance on it, and control of changes - so that other domains always are in loop on the updated functionality, changes, bug fixes etc.

## Test Data

We would like to provide our project with sample data that are used to test the integration point. Please refer to this from your documentation.

## Integration Monitoring (Reporting, alarms etc)

Describe how to set up monitoring on the integration point and what it should monitor - or describe what has already been implemented and where to get more information.

## How To's

Furthermore please consider other topics that would bring value to share on your specific integration point - so that others are self-helped to start working on and with the solution, both integrated domains but also in our community. We would like to inspire and knowledge share our solution - including the bounded contexts.

### How to simulate the integration point - using test doubles

### How to test and debug
