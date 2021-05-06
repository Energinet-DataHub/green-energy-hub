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

## Communication

What is being exchanged between domains on the specific integration point, and how is it being exchanged.

## Configurations

Are there any specifics configurations or requirements needed to run the integration.

## Expected Behavior (rejections, passing etc)

Please elaborate the business flow(s) that uses the integration point.

## Versioning

Does multiple versions of the integration point exist?

And what about the content it distributes, does it come in several versions?

## Test Data

We would like to provide our project with sample data that are used to test the integration point. Please refer to this from your documentation.

## Integration Monitoring (Reporting, alarms etc)

Describe how to set up monitoring on the integration point and what it should monitor - or describe what has already been implemented and where to get more information.

## How have we tested this integration point

Please share your test setup and example(s).
