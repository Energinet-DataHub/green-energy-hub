# Protobuf POC findings

Due to a lot of issues debugging our cross service communication while using JSON we decided to explore Protobuf as an alternative.

The advantage of Protobuf is the contract DTO that both the sending and receiving party needs to adhere to to be able to talk to each other.  
Unlike JSON which is much more lenient, Protobuf will throw an error when trying to pass an object that doesn't adhere to the given contract

## Implementation thoughts

### Multiple messages in the same queue

Protobuf doesn't provide any method of appending metadata or properties to an object.
Due to this we need to come up with our own solution for this to be able to handle multiple objects in the same queue.

We suggest handling this by using a "message envelope" - A top level object that wraps the original object along with an identifier for what object it contains.
Example code:

```protobuf
message MarketDataMessage {  
  string messageType = 1; // The number here indicates the order of the fields in the contract
  google.protobuf.Any message = 2;
}
```

The receiving party will then be able to unpack the message according to whatever string is provided `messageType`.

### Unpacking messages

The simple solution for unpacking the messages would be a switch statement that looks at the `messageType` field and then runs the code for that specific type.  
This does however make adding a new type a bit more error prone since you have to remember to add a new case in the switch statement every time.

Instead a more optimal approach could be to make it part of the configuration which types are allowed.
When receiving a message the mapping method could then run through the list from the configuration and check if any of them match the incoming type.

### Generating DTO class

To actually use the contracts in the code we have to generate a C# class from the `.proto` file.
Usually this can be done by downloading the Protobuf binary and executing a generate command.

To avoid having a dependency that every developer needs to install it is instead possible to use the `Grpc.Tools` NuGet package.  
See more here: <https://github.com/grpc/grpc/blob/master/src/csharp/BUILD-INTEGRATION.md>

This package makes it possible to configure the generator command in the `csproj` file like this:

```xml
  <ItemGroup>
    <Protobuf Include="**/*.proto" OutputDir="%(RelativeDir)" CompileOutputs="false"  />
  </ItemGroup>
```

After setting this up classes will be generated when the project is being built without having to manually install anything.

### Location of proto files

We suggest categorizing the proto files by their queue name like this `samples/energinet/communication-contracts/{queue-name}/`.
This way it should be easy to identify the proto files needed for a specific service.

### Versioning

The POC didn't take versioning of the contracts into account. A simple versioning scheme with the version number in the name is probably efficient enough though e.g. `change-of-supplier-v2`.

## Still left to figure out

### Encryption

It's been mentioned that our queues should be encrypted. Is this something that should be handled by the services or by the queue itself?
