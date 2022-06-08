# External domain interaction

* Status: accepted
* Deciders: @sondergaard, @martinfhansen
* Date: 2022-06-08

## Context and Problem Statement

As a system we must support different formats (CIM Xml & CIM json) and versions.

## Decision Outcome

Every domain is responsible for handling the transformation to respond to a request.

Eg. When a domain receives a request for processing a business document, it must handle all known supported formats.

It is stated within the contract for `MessageHub` the format and version that needs to be returned in a response.
