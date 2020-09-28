# Raising Custom Events

You can raise any kind of event from anywhere in your application for rules to watch for.  To do so, simply call the [`Push(TMessage)`](/api/GravyBot.MessageQueueService.html#GravyBot_MessageQueueService_Push__1___0_)  method on the [`MessageQueueService`](/api/GravyBot.MessageQueueService.html) singleton.  

This abridged example from [ChatBeet](https://github.com/halomademeapc/ChatBeet) shows an event being raised from a Razor page.

**Event Class**
```csharp
public class HighGroundClaim
{
    public string Nick { get; set; }
    public string Channel { get; set; }
}
```

**Raising the Event**
```csharp
public class HighGroundModel : PageModel
{
    private readonly MessageQueueService queueService;

    [BindProperty]
    public string Channel { get; set; }

    public HighGroundModel(MessageQueueService queueService)
    {
        this.queueService = queueService;
    }
    
    [Authorize]
    public async Task<IActionResult> OnPost()
    {
        if (User?.Identity?.IsAuthenticated ?? false && !string.IsNullOrEmpty(Channel) && Channel.StartsWith("#"))
        {
            queueService.Push(new HighGroundClaim { Nick = User.GetNick(), Channel = Channel });
        }
        return RedirectToPage("/HighGround");
    }
}
```

**Handling the Event**
```csharp
public class HighGroundRule : IMessageRule<PrivateMessage>, IMessageRule<HighGroundClaim>
{
    private readonly Regex filter;
    public static readonly Dictionary<string, string> HighestNicks = new Dictionary<string, string>();

    public HighGroundRule(IOptions<IrcBotConfiguration> options)
    {
        filter = new Regex($@"^{Regex.Escape(options.Value.CommandPrefix)}((climb)|(jump)|(high ground))$", RegexOptions.IgnoreCase);
    }

    public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
    {
        if (filter.IsMatch(incomingMessage.Message))
        {
            if (incomingMessage.IsChannelMessage)
            {
                yield return GetResponse(incomingMessage.From, incomingMessage.To, incomingMessage.GetResponseTarget());
            }
            else
            {
                yield return new PrivateMessage(incomingMessage.From, $"You must be in a channel to claim the high ground.");
            }
        }
    }

    public IEnumerable<IClientMessage> Respond(HighGroundClaim incomingMessage)
    {
        yield return GetResponse(incomingMessage.Nick, incomingMessage.Channel, incomingMessage.Channel);
    }

    private PrivateMessage GetResponse(string nick, string chan, string target)
    {
        if (!HighestNicks.ContainsKey(chan))
        {
            HighestNicks[chan] = nick;
            return new PrivateMessage(target, $"{nick} has the high ground.");
        }
        else if (nick == HighestNicks[chan])
        {
            HighestNicks.Remove(chan);
            return new PrivateMessage(target, $"{nick} trips and falls off the high ground.");
        }
        else
        {
            var oldKing = HighestNicks[chan];
            HighestNicks[chan] = nick;
            return new PrivateMessage(target, $"It's over, {oldKing}! {nick} has the high ground!");
        }
    }
}
```

*Note that this rule can respond to both `PrivateMessage` and `HighGroundClaim` events.*

**Registering the Rule**
```csharp
services.AddIrcBot(Configuration.GetSection("Irc"), pipeline => {
    pipeline.RegisterAsyncRule<HighGroundRule, PrivateMessage>();
    pipeline.RegisterAsyncRule<HighGroundRule, HighGroundClaim>();
});
```