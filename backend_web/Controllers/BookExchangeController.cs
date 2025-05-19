using backend_web.Data;
using backend_web.Data.Models;
using backend_web.Migrations;
using backend_web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;


namespace backend_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookExchangeController : ControllerBase
    {
        private readonly WebDbContext _context;

        public BookExchangeController(WebDbContext context)
        {
            _context = context;
        }

        // GET: api/BookExchange
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _context.BookExchange
             .Include(p => p.User)
                 .ThenInclude(u => u.Images)
             .OrderByDescending(p => p.CreatedAt)
             .Select(p => new
             {
                 p.Id,
                 p.BooksToGive,
                 p.BooksToReceive,
                 p.Major,
                 p.CreatedAt,
                 UserName = p.User.Name,
                 UserId= p.User.Id,

                 Image = p.User.Images.FirstOrDefault() != null ? new
                 {
                     p.User.Images.FirstOrDefault().Name,
                     ImageBase64 = Convert.ToBase64String(p.User.Images.FirstOrDefault().Image),
                     p.User.Images.FirstOrDefault().ContentType
                 } : null
             })
             .ToListAsync();



            return Ok(posts);
        }


        // POST: api/BookExchange
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] BookExchangePostCreateDto dto)
        {
            var user = await _context.users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("المستخدم غير موجود");

            var post = new BookExchange
            {
                UserId = dto.UserId,
                BooksToGive = dto.BooksToGive,
                BooksToReceive = dto.BooksToReceive,
                Major = user.Major // ✅ جلب التخصص من المستخدم
            };

            _context.BookExchange.Add(post);
            await _context.SaveChangesAsync();

            return Ok("تم نشر طلب تبادل الكتب بنجاح.");
        }

        //Delete Post
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id, [FromQuery] int userId, [FromQuery] bool isAdmin = false)
        {
            var post = isAdmin
                ? await _context.BookExchange.FirstOrDefaultAsync(p => p.Id == id)
                : await _context.BookExchange.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post == null)
                return NotFound(new { message = "لم يتم العثور على المنشور أو لا تملك صلاحية حذفه." });

            _context.BookExchange.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المنشور بنجاح." });
        }

    }
}
