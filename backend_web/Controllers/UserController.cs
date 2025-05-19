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

        //GETallUser
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<mdlUser>>> GetAllUsers()
        {
            var users = await db.users
                .Select(u => new mdlUser
                {
                    Id=u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Major = u.Major,
                    PostCount = u.products.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        //sign up by user.....................
        [HttpPost]
        public async Task<IActionResult> AddUser([FromForm] mdlUser mdl)
        {
            try
            {
                if (mdl == null || string.IsNullOrWhiteSpace(mdl.Name) || string.IsNullOrWhiteSpace(mdl.Password))
                {
                    return BadRequest("Invalid user data.");
                }

                var user = new User
                {
                    Name = mdl.Name,
                    Email = mdl.Email,
                    Password = mdl.Password,
                    Phone = mdl.Phone,
                   // Major = mdl.Major
                };

                db.users.Add(user);
                await db.SaveChangesAsync(); // أولاً نحفظ المستخدم للحصول على User.Id

                if (mdl.Image != null && mdl.Image.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await mdl.Image.CopyToAsync(stream);

                    var userImage = new UserImage
                    {
                        Name = mdl.Image.FileName,
                        ContentType = mdl.Image.ContentType,
                        Image = stream.ToArray(),
                        UserId = user.Id
                    };

                    db.Userimg.Add(userImage);
                    await db.SaveChangesAsync(); // نحفظ الصورة بعد حفظ المستخدم
                }

                return Ok(new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    HasImage = mdl.Image != null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await db.users
                .Include(u => u.products)
                    .ThenInclude(p => p.Images)
                .Include(u => u.Images) // ← تضمين صور المستخدم
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("User not found.");

            // جلب أول صورة (إن وجدت)
            var userImage = user.Images?.FirstOrDefault();

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Password,
                user.Major,

                ProfileImage = userImage == null ? null : new
                {
                    userImage.Name,
                    ImageBase64 = Convert.ToBase64String(userImage.Image),
                    userImage.ContentType
                },

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
                    }).ToList()
                }).ToList()
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] mdlUser mdl)
        {
            var user = await db.users
                .Include(u => u.Images) // لجلب الصورة القديمة
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            // تحديث البيانات النصية
            if (!string.IsNullOrEmpty(mdl.Name)) user.Name = mdl.Name;
            if (!string.IsNullOrEmpty(mdl.Email)) user.Email = mdl.Email;
            if (!string.IsNullOrEmpty(mdl.Phone)) user.Phone = mdl.Phone;
            if (!string.IsNullOrEmpty(mdl.Major)) user.Major = mdl.Major;

            // ✅ تحديث الصورة إذا تم إرسالها
            if (mdl.Image != null && mdl.Image.Length > 0)
            {
                // حذف الصورة القديمة (اختياري)
                var oldImage = user.Images.FirstOrDefault();
                if (oldImage != null)
                {
                    db.Userimg.Remove(oldImage);
                }

                using var ms = new MemoryStream();
                await mdl.Image.CopyToAsync(ms);

                var newImage = new UserImage
                {
                    Name = mdl.Image.FileName,
                    ContentType = mdl.Image.ContentType,
                    Image = ms.ToArray(),
                    UserId = user.Id
                };

                db.Userimg.Add(newImage);
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully",
                updatedUser = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Phone,
                    user.Major
                }
            });
        }




        //delete user...............
        [HttpDelete("{id}")]
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
