using backend_web.Data.Models;
using backend_web.Data;
using backend_web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController(WebDbContext _db)
        {
            db = _db;
        }
        private readonly WebDbContext db;



        //sign up by user.....................
        [HttpPost]
        public async Task<IActionResult> Add(mdlUser addUser)
        {

            var emploee = new User()
            {
                Name = addUser.Name,
                Email = addUser.Email,
                Password = addUser.Password,
                Phone = addUser.Phone,
            };

            db.users.Add(emploee);
            await db.SaveChangesAsync();
            return Ok(new { emploee, id = emploee.Id });
        }

        //login by user "Authentication".............
        [HttpGet]
        public IActionResult Login([FromQuery] string email, [FromQuery] string password)
        {
            // التحقق من وجود البريد الإلكتروني وكلمة المرور
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            // البحث عن المستخدم في قاعدة البيانات باستخدام البريد الإلكتروني
            var user = db.users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // التحقق من مطابقة كلمة المرور
            if (user.Password != password)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // تسجيل الدخول ناجح - إرجاع بيانات الحساب
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                username = user.Name,
                message = "Login successful"
            });
        }

        //Get user with your products.......
        [HttpGet("id")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await db.users
            .Include(u => u.products)
                .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Password,
                Products = user.products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.caption,
                    p.price,
                    p.PHnum,
                    p.type,
                    Images = p.Images.Select(i => new
                    {
                        i.Name,
                        ImageBase64 = Convert.ToBase64String(i.image),
                        i.ContentType
                    })//#######################

                }).ToList()
            });
        }

        //Update data User...............
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] mdlUser mdl)
        {
            // البحث عن المستخدم في قاعدة البيانات
            var user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // تحديث الحقول إذا تم إرسالها فقط
            if (!string.IsNullOrEmpty(mdl.Name))
            {
                user.Name = mdl.Name;
            }
            if (!string.IsNullOrEmpty(mdl.Email))
            {
                user.Email = mdl.Email;
            }
            if (!string.IsNullOrEmpty(mdl.Phone))
            {
                user.Phone = mdl.Phone;
            }

            // حفظ التعديلات
            await db.SaveChangesAsync();

            // إرجاع رسالة مع القيم المحدثة
            return Ok(new
            {
                message = "User updated successfully",
                updatedUser = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Phone
                }
            });
        }



        //delete user...............
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // البحث عن المستخدم في قاعدة البيانات
            var user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // حذف المستخدم
            db.users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

        //ارجاع المنتجات التي اعجب بها المستخدم 
        [HttpGet("GetLikedProducts/{userId}")]
        public async Task<IActionResult> GetLikedProducts(int userId)
        {
            try
            {
                var likedProducts = await db.LikedProducts
                    .Where(lp => lp.UserId == userId)
                    .Select(lp => new
                    {
                        lp.Product.Id,
                        lp.Product.Name,
                        lp.Product.caption,
                        lp.Product.price,
                        lp.Product.PHnum,
                        lp.Product.type,
                        Images = lp.Product.Images
                        .Select(i => new
                        {
                            ImageBase64 = Convert.ToBase64String(i.image),
                            i.ContentType
                        })//#######################
                        .ToList() // جلب جميع الصور

                    })
                    .ToListAsync();

                if (!likedProducts.Any())
                {
                    return NotFound("لا توجد منتجات معجب بها.");
                }

                return Ok(likedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء استرجاع البيانات: {ex.Message}");
            }
        }

        //لاضافة الاعجاب لمنتج في حساب مستخد معين
       /* [HttpPost("AddLike")]
        public async Task<IActionResult> AddLike([FromBody] AddLikeDto likeDto)
        {
            try
            {
                // التحقق من وجود الإعجاب مسبقًا
                var existingLike = await db.LikedProducts
                    .FirstOrDefaultAsync(lp => lp.UserId == likeDto.UserId && lp.ProductId == likeDto.ProductId);

                if (existingLike != null)
                {
                    return BadRequest("الإعجاب موجود مسبقًا.");
                }

                // إضافة الإعجاب
                var likedProduct = new LikedProduct
                {
                    UserId = likeDto.UserId,
                    ProductId = likeDto.ProductId,
                };

                db.LikedProducts.Add(likedProduct);
                await db.SaveChangesAsync();

                return Ok("تم إضافة الإعجاب بنجاح.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء إضافة الإعجاب: {ex.Message}");
            }
        }
        */
       
        [HttpPost("ToggleLike")]
        public async Task<IActionResult> ToggleLike([FromBody] AddLikeDto likeDto)
        {
            try
            {
                // التحقق من وجود الإعجاب مسبقًا
                var existingLike = await db.LikedProducts
                    .FirstOrDefaultAsync(lp => lp.UserId == likeDto.UserId && lp.ProductId == likeDto.ProductId);

                if (existingLike != null)
                {
                    // إذا كان الإعجاب موجودًا، قم بحذفه
                    db.LikedProducts.Remove(existingLike);
                    await db.SaveChangesAsync();
                    return Ok("تم حذف الإعجاب .");
                }
                else
                {
                    // إذا لم يكن الإعجاب موجودًا، قم بإضافته
                    var likedProduct = new LikedProduct
                    {
                        UserId = likeDto.UserId,
                        ProductId = likeDto.ProductId,
                    };

                    db.LikedProducts.Add(likedProduct);
                    await db.SaveChangesAsync();
                    return Ok("تم إضافة الإعجاب.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء تبديل حالة الإعجاب: {ex.Message}");
            }
        }

    }

}
