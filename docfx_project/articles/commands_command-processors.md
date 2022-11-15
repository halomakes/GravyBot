# Command Processors

Command processors are the bread and butter of the commands package.  These parallel the functionality of controllers in web API development.  Your command processors should inherit from the [CommandProcessor](/api/GravyBot.Commands.CommandProcessor.html) base class.  You can then add methods with a [CommandAttribute](/api/GravyBot.Commands.CommandAttribute.html) to handle invocations.  Your methods should return `IClientMessage` or `Task<IClientMessage>` if you have a single response, or you can use `IEnumerable<IClientMessage>` or `IAsyncEnumerable<IClientMessage>` if you want to send multiple responses.

## Basic Usage

You can have multiple methods in a single command processor.
```csharp
public class HelloCommandProcessor : CommandProcessor
{
    [Command("hello", Description = "Say hello to your GravyBot.")]
    public IClientMessage SayHello() => new PrivateMessage(IncomingMessage.From, $"Hello, {IncomingMessage.From}!");

    [Command("goodbye", Description = "Say goodbye to your GravyBot.")]
    public IClientMessage SayGoodBye() => new PrivateMessage(IncomingMessage.From, $"Goodbye, {IncomingMessage.From}!");
}
```

## Message Context

See available properties of the [CommandProcessor](/api/GravyBot.Commands.CommandProcessor.html) base class in the API documentation.

Commands are invoked when a [PrivateMessage](http://gravyirc.halomademeapc.com/api/GravyIrc.Messages.PrivateMessage.html) matches the command prefix and definition.  You have access to the [PrivateMessage](http://gravyirc.halomademeapc.com/api/GravyIrc.Messages.PrivateMessage.html) that triggered the command on the [IncomingMessage](/api/GravyBot.Commands.CommandProcessor.html#GravyBot_Commands_CommandProcessor_IncomingMessage) property.  This is useful for obtaining information about the user that sent the message or what channel the message was in.  If your command method is bound to multiple trigger strings, you can access the [TriggeringCommandName](/api/GravyBot.Commands.CommandProcessor.html#GravyBot_Commands_CommandProcessor_TriggeringCommandName) property to see which was used to invoke that method.


## Dependency Injection

Command processors get registered into the dependency injection container in a scoped context and have normal access to other services in a transient, scoped, or singleton lifecycle.

```csharp
public class ComplimentCommandProcessor : CommandProcessor
{
    private readonly ComplimentService service;

    public ComplimentCommandProcessor(ComplimentService service)
    {
        this.service = service;
    }

    [Command("compliment", Description = "Have your GravyBot compliment you.")]
    public async Task<IClientMessage> Compliment()
    {
        var compliment = await service.GetComplimentAsync();
        return new PrivateMessage(IncomingMessage.From, $"{IncomingMessage.From}: {compliment}");
    }
}
```

## Multiple Bindings

You can bind one method to multiple command names.

```csharp
public class PotatoCommandProcessor : CommandProcessor
{
    [Command("spud")]
    [Command("tater")]
    public IClientMessage GetPotato() => new PrivateMessage(IncomingMessage.From, "potato");
}