using backend_web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly WebDbContext _context;

        public MessagesController(WebDbContext context)
        {
            _context = context;
        }

        // جلب الرسائل بين المستخدمين (sender & receiver)
        [HttpGet]
        public async Task<IActionResult> GetMessages(string userId, string otherUserId)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }
    }

}
