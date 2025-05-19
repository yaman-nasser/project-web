
    using Microsoft.AspNetCore.SignalR;

    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // نحصل على userId من Query String مثل: /chatHub?userId=someId
            return connection.GetHttpContext().Request.Query["userId"];
        }
    }


