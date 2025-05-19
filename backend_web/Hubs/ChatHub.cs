using backend_web.Data;
using backend_web.Data.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    private static ConcurrentDictionary<string, string> UserConnections = new();

    private readonly WebDbContext _context;

    public ChatHub(WebDbContext context)
    {
        _context = context;
    }

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            UserConnections[userId] = Context.ConnectionId;
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var userEntry = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (!string.IsNullOrEmpty(userEntry.Key))
        {
            UserConnections.TryRemove(userEntry.Key, out _);
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string senderId, string receiverId, string messageText)
    {
        // حفظ الرسالة في قاعدة البيانات
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            MessageText = messageText,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // إرسال الرسالة للمستقبل إذا متصل
        if (UserConnections.TryGetValue(receiverId, out string receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, messageText);
        }

        // إرسال الرسالة للمرسل لتحديث الواجهة (باستخدام senderId الحقيقي)
        if (UserConnections.TryGetValue(senderId, out string senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderId, messageText);
        }
    }
}
