using backend_web.Data;
using backend_web.Data.Models;
using backend_web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostItemsController : ControllerBase
    {
        private readonly    WebDbContext _context;

        public LostItemsController(WebDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] LostItemDto dto)
        {
            var item = new LostItem
            {
                Type = dto.Type,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Date = dto.Date,
                PhoneNumber = dto.PhoneNumber,
                UserId = dto.UserId,
                Images = new List<LostItemImage>()
            };

            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                foreach (var file in dto.ImageFiles)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var img = new LostItemImage
                    {
                        Name = file.FileName,
                        Image = ms.ToArray(),
                        ContentType = file.ContentType
                    };
                    item.Images.Add(img);
                }
            }

            _context.LostItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم نشر الإعلان بنجاح", item.Id });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.LostItems
                .Include(i => i.User)
                .Include(i => i.Images)
                .Select(i => new
                {
                    i.Id,
                    i.Type,
                    i.Title,
                    i.Description,
                    i.Location,
                    i.Date,
                    i.PhoneNumber,
                    i.UserId,
                    UserName = i.User.Name,
                    Images = i.Images.Select(img => new
                    {
                        img.Name,
                        img.ContentType,
                        ImageBase64 = Convert.ToBase64String(img.Image)
                    }).ToList()
                }).ToListAsync();

            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int userId, [FromQuery] bool isAdmin = false)
        {
            var item = await _context.LostItems
                .Include(i => i.Images)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound("العنصر غير موجود");

            if (!isAdmin && item.UserId != userId)
                return Forbid("لا يمكنك حذف منشور لا يخصك");

            _context.LostItemImages.RemoveRange(item.Images);
            _context.LostItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("تم حذف المنشور بنجاح");
        }

    }

}
