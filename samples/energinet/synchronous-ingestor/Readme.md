# Synchronous ingestor

## Motivation

We want to be able to perform a subset of validation on received request from known market actors to validate if the request fulfills the requirements set by the TSO so the request can be persisted and sent to a queue for further processing.

See architecture:
<https://github.com/microsoft/green-energy-hub/blob/main/docs/images/TechStack.png>

## Description

When a market actor e.g. a balance supplier wants to change a supplier on behalf of a costumer(or other request) they send a request to the Green Energy Hub. The message goes through the system an reaches the synchronous ingestor where the ingestor performs a subset of validations to validate the if the request fulfills the requirement's. If the message fails the validations it is still persisted. If the validation passes it is sent to an inbound queue for further processing.

The synchronous ingestor is a single instance, that can be invoked from different URL paths, each paths matches a Business Process.
This instance will then, based on the path, run a type specific piece of code.
All types contain the same core steps, but with different outcomes.

### Business Process Examples

- ChangeOfSupplier
- MoveIn
- MoveOut
- SendTimeSeries