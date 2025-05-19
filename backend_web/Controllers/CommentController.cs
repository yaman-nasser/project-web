using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_web.Data.Models; // يحتوي على DTOs والكيانات
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_web.Data.Models;
using backend_web.Data;
using backend_web.Model;
using backend_web.Migrations;

namespace YourProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly WebDbContext _context;

        public CommentController(WebDbContext context)
        {
            _context = context;
        }

        // POST: api/Comment
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto dto)
        {
            var user = await _context.users.FindAsync(dto.UserId);
            var product = await _context.products.FindAsync(dto.ProductId);

            if (user == null || product == null)
                return NotFound("User or Product not found.");

            var comment = new Comment
            {
                Content = dto.Content,
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment added successfully.");
        }

        // GET: api/Comment/product/5
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<CommentReadDto>>> GetCommentsForProduct(int productId)
        {
            var comments = await _context.comments
                .Where(c => c.ProductId == productId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentReadDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Name = c.User.Name, // أو .Name إذا كان عندك اسم
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        // DELETE: api/Comment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            _context.comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted.");
        }
    }
}
