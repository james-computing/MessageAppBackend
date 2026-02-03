using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Message.Kafka.Producer;

/*
    The Hub is transient, it can be disposed fast. For this reason, it can't be used for long tasks.
    The Hub.Context is null in the constructor, as far as my tests show, only use it in the methods.
*/

namespace Message.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
    }
}
