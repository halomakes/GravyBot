# Command Policies

You can use policies to control when and where users can invoke commands.  Users will recieve an announcement message explaining why they cannot use a message if they violate a policy.

## Rate Limiting

Apply a [RateLimitAttribute](/api/GravyBot.Commands.RateLimitAttribute.html) to a command method to limit how often users can invoke it.

```csharp
[Command("spam", Description = "Do something spammy.")]
[RateLimit(2, TimeUnit.Minute)]
public IClientMessage Spam()
{
    return new PrivateMessage(IncomingMessage.From, "This command can only be used once every two minutes");
}
```

## Channel-Only Commands

Apply a [ChannelOnlyAttribute](/api/GravyBot.Commands.ChannelOnlyAttribute.html) to specify that a command cannot be used in direct messages.

```csharp
[Command("announce", Description = "Do something in public.")]
[ChannelOnly]
public IClientMessage Announce()
{
    return new PrivateMessage(IncomingMessage.To, "This command can only be used in channels");
}
```

## Direct-Only Commands

Apply a [DirectOnlyAttribute](/api/GravyBot.Commands.DirectOnlyAttribute.html) to specify that a command cannot be used in channel messages.

```csharp
[Command("whisper", Description = "Do something in private :).")]
[DirectOnly]
public IClientMessage Whisper()
{
    return new PrivateMessage(IncomingMessage.From, "This command can only be used in direct messages");
}
```

## Specific Channels

You can also define custom policies that specify when commands can only be used in specific channels.  Register the policy during startup first:

```csharp
services.AddCommandOrchestrator(builder =>
{
    /* register rules... */

    builder.AddChannelPolicy("NoMain", botConfig.Policies["NoMain"]);
});
```

```json
{
    "BotConfig:Policies": {
        "NoMain": {
            "Channels": [
                "#mainchannel",
                "#otherimportantchannel"
            ],
            "Mode": "Blacklist"
        }
    }
}
```

See [ChannelPolicy](/api/GravyBot.Commands.ChannelPolicy.html) for more details on configuration. 

Apply the restriction with a [ChannelPolicyAttribute](/api/GravyBot.Commands.CommandAttribute.html)

```csharp
[Command("whisper", Description = "Do something in private :).")]
[ChannelPolicy("NoMain")]
public IClientMessage BeAnnoying()
{
    return new PrivateMessage(IncomingMessage.From, "This command can only be used in direct messages");
}
```