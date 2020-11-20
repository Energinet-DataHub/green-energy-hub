# Message ingest

## General

This solution serves as the basis for the message ingestion processing part of the Green Energy Hub, and proves out the ability to have a single entry point (could be an Azure Function) that is able to accept different requests types and logically route them to classes fulfilling the respective business processes for that request.

It provides an opinionated framework that self-registers classes to make adding new requests, handlers, and rule sets easy, and currently uses NRules as the rules framework.

It serves HTTP endpoints for each of the discovered request types, and if validation succeeds, pushes the accepted messages onto a queue.

## Concepts

Core concepts for customizing the request processing flow are as follows:

* *Hub Action Requests*: A small POCO-like class implementing `IHubActionRequest` that defining the properties expected on a request of that type.
* *Hub Action Handlers*: Provides the business processing logic for handling a request of that type, namely validating the message against business rules. Must derive from `IngestionHandler<HubActionRequestType>` and handles any validation processes in `ValidateAsync()`, persists the request for further processing in `AcceptAsync()`, and crafts a `HubActionResponse` to return the caller in `RespondAsync()`.
* *Rule Sets*: Provides a static enumerable collection of `Rule` types that should be applied for validation of the request. Must implement `IRuleSet<HubActionRequestType>`.
* *Rules*: Provides custom validation logic and yields a `RuleResult` to document the outcome. Extends the `Rule` class as defined per [NRules documentation](http://nrules.net/api/html/N_NRules.htm).

## Requests and handlers

To implement logic for a new request, classes for each of the types above should be created. `HubRequestAttribute` can be used to create a friendly name for a concrete hub action request type.

All of the classes implementing `IHubActionRequest` and `IHubActionHandler<HubActionRequestType>` are auto-discovered and registered via `AddGreenEnergyHub()` service extension in the `HandlerExtensions` class, permitting requests of that type to flow to the correct handler automatically when called at `http://localhost:7071/api/HubActionRequestType`. `RequestRegistration` helps with this auto-registration.

## Rules and rule sets

### Validation logic

NRules rules match against fact types (in this case, the action request instance), and then execute if their match is successful. Because we intend to detect validation rule execution (required for rule chaining) and whether that validation was successful or not, **each rule must execute** no matter the validity of the data. Therefore, keep in mind when authoring rules that they should match on request type, but any further validation should take place in the rule's action portion.

A rule's action can then validate request properties and then yield a new `RuleResult` instance to document the execution of the rule and whether or not its validation criteria were met.

### Generics and rule matching

All rules declarations must be generic (`where TRequest : IActionRequest`) because NRules matching happens against fact *types*. While matching the fact against the common interface `IActionRequest` will cause rules to execute, the validation cannot access any properties properties on the object; using generics, the `TRequest` can represent the expected and specific shape of the request and if we constrain `TRequest` using property-based interfaces, we can also guarantee their presence at compile time.

Thus, the solution defines interfaces for various properties available on requests; for example `IRequestHasCustomerId` defines a single property `CustomerId`. A rule that requires `CustomerId` would then constrain its `where TRequest : IActionRequest, IRequestHasCustomerId` to expose the property for validation in the rule action.

Rule sets bind a group of `Rule`s to a particular request type by implementing `IRuleSet<HubActionRequestType>`. However, type references to rules in their static enumeration should be kept generic (i.e. `typeof(NonNegativeCustomerIdRule<>)`) so that the rule set can be composable; for example one may wish to create a "Customer" rule set that applies a set of validations to any request that references a `CustomerId` (via `IRequestHasCustomerId`).

As such, having rule sets listing generic `Rule` instances permits for composing rule sets at compile time and binding the `Rule` instances to the correct `TRequest` at runtime (which happens in the `NRulesEngine` class).
