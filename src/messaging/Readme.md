# Messaging in GreenEnergyHub

## Context

We've tried to create an opinionated framework for sending and handling messages. This document will discuss the usage of the base handlers.

## Framework

### Messages

To send messages they must extend the `IHubMessage` interface.

### Handlers

There are two kinds of handlers in the system; `RequestHandlers` and `CommandHandlers`.

`RequestHandlers` have a response whereas `CommandHandlers` do not.

### Getting started

To start using the framework you should extend either `HubRequestHandler` or `HubCommandHandler` depending on the specific use case, ie. if it's a synchronous action that requires a response extend the `HubRequestHandler`.

Common methods for handlers:

- `ValidateAsync` is invoked when validating the current message.
- `AcceptAsync` is invoked when the message is being accepted, this could be handling/saving/forwarding/business logic etc.
- `OnErrorAsync` is invoked on uncaught exceptions.

`HubRequestHandler`:

- `RespondAsync` is invoked when creating the response.

`HubCommandHandler`:

- `RejectAsync` is invoked when the message can't be handled.

In your Startup.cs you should call the extension method `AddGreenEnergyHub` in the `GreenEnergyHub.Messaging.Integration.ServiceCollection` package, in order to bootstrap the framework. Provide a list of assemblies that will be scanned for HubRequestHandlers, HubCommandHandlers and IHubMessages.

Example Startup:

```C#
builder.Services.AddGreenEnergyHub(typeof(MyRequestHandler).Assembly);
```

Example `CommandHandler`:

```C#
/// <summary>
/// Override Accept to save in the database
/// Validate and Reject are not overridden thus using the default implementation
/// </summary>
public class ChangeOfSupplierCommandHandler : HubCommandHandler<ChangeOfSupplierMessage>
{
    protected override async Task AcceptAsync(ChangeOfSupplierMessage actionData, CancellationToken cancellationToken)
    {
        // Save to database
    }
}
```

For more examples look in the `samples\energinet\ingestion` folder.
