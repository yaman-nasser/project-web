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
    public class ProductController : ControllerBase
    {
        public ProductController(WebDbContext _db, IConfiguration configuration, ILogger<ProductController> logger)
        {
            db = _db;
            _configuration = configuration;
            _logger = logger;
        }
        private readonly WebDbContext db;
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _configuration;


        //Get all products...............
        [HttpGet]
        public async Task<IActionResult> AllProductsWithImages()
        {
            // جلب جميع المنتجات مع الصور المرتبطة بها
            var products = await db.products
                .Include(p => p.Images)  // تضمين الصور المرتبطة بكل منتج
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("لا توجد منتجات.");
            }

            // تحويل المنتجات إلى الشكل المطلوب ليشمل الصور وبيانات المنتج
            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.caption,
                p.price,
                p.type,
                p.PHnum,
                Images = p.Images.Select(i => new
                {
                    i.Name,
                    ImageBase64 = Convert.ToBase64String(i.image),  // تحويل البيانات الثنائية إلى Base64 لعرض الصورة
                    i.ContentType
                }).ToList()
            }).ToList();

            return Ok(result);
        }



        //Get product by id-product...............
        [HttpGet("id")]
        public async Task<IActionResult> GetProductWithImages(int id)
        {
            var product = await db.products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found.");

            var result = new
            {
                product.Id,
                product.Name,
                product.price,
                product.caption,
                product.type,
                product.UserId,
                Images = product.Images.Select(i => new
                {
                    i.Name,
                    ImageBase64 = Convert.ToBase64String(i.image),
                    i.ContentType
                })
            };

            return Ok(result);
        }

        //Get product by id-User...............


        //Get product by product type...............
        [HttpGet("by-type")]
        public async Task<IActionResult> GetProductsByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest(new { message = "Type parameter is required." });
            }

            // البحث عن المنتجات التي تطابق النوع
            var products = await db.products
                .Where(p => p.type == type)
                 .Select(p => new
                 {
                     p.Id,
                     p.Name,
                     p.price,
                     p.type,
                     Images = p.Images.Select(img => new
                     {
                         img.Id,
                         ImageBase64 = Convert.ToBase64String(img.image),
                         img.ContentType
                     }).ToList()
                 })
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound(new { message = "No products found for the specified type." });
            }

            return Ok(products);
        }



        //Add a product for a specific User with a set of images........... 
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromForm] mdlProduct mdl)
        {
            try
            {
                if (mdl == null || string.IsNullOrWhiteSpace(mdl.Name) || mdl.price <= 0)
                {
                    return BadRequest("Invalid product data.");
                }

                var pro = new Product
                {
                    Name = mdl.Name,
                    caption = mdl.caption,
                    price = mdl.price,
                    type = mdl.type,
                    PHnum = mdl.PHnum,
                    UserId = mdl.userId,
                    Images = new List<ImageModel>()
                };

                if (mdl.images != null && mdl.images.Count > 0)
                {
                    foreach (var img in mdl.images)
                    {

                        using var Stream = new MemoryStream();
                        await img.CopyToAsync(Stream);

                        var image = new ImageModel
                        {
                            Name = img.FileName,
                            image = Stream.ToArray(),
                            ContentType = img.ContentType
                        };

                        pro.Images.Add(image);
                    }
                }

                // إضافة المنتج مع الصور إلى قاعدة البيانات
                db.products.Add(pro);
                await db.SaveChangesAsync();

                return Ok(new { pro.Id, pro.Name, pro.price, ImageCount = pro.Images.Count });
            }
            catch (Exception ex)
            {
                // إرجاع الخطأ كتفاصيل للعميل
                return StatusCode(500, new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }

        }




        //Update Product data................
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductPartial(int productId, [FromForm] mdlProduct updatedProduct)
        {
            if (productId <= 0 || updatedProduct == null)
            {
                return BadRequest("Invalid product data.");
            }

            var existingProduct = await db.products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId);

            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // تحديث الحقول المعدلة
            if (!string.IsNullOrEmpty(updatedProduct.Name)) existingProduct.Name = updatedProduct.Name;
            if (!string.IsNullOrEmpty(updatedProduct.caption)) existingProduct.caption = updatedProduct.caption;
            if (updatedProduct.price > 0) existingProduct.price = updatedProduct.price;
            if (!string.IsNullOrEmpty(updatedProduct.type)) existingProduct.type = updatedProduct.type;
            if (!string.IsNullOrEmpty(updatedProduct.PHnum)) existingProduct.PHnum = updatedProduct.PHnum;

            if (updatedProduct.userId > 0) existingProduct.UserId = updatedProduct.userId;

            // معالجة الصور
            if (updatedProduct.images != null && updatedProduct.images.Any())
            {
                // حذف جميع الصور القديمة المرتبطة بالمنتج
                db.Images.RemoveRange(existingProduct.Images);

                var newImages = new List<ImageModel>();

                foreach (var img in updatedProduct.images)
                {
                    using var stream = new MemoryStream();
                    await img.CopyToAsync(stream);

                    var image = new ImageModel
                    {
                        Name = img.FileName,
                        image = stream.ToArray(),
                        ContentType = img.ContentType,
                        ProductId = existingProduct.Id  // تأكد من إضافة ProductId الصحيح
                    };

                    newImages.Add(image);
                }

                await db.Images.AddRangeAsync(newImages);
            }

            // حفظ التعديلات في قاعدة البيانات
            await db.SaveChangesAsync();

            return NoContent(); // إرجاع حالة ناجحة بدون محتوى
        }







        //Delete Product.....................
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // البحث عن المنتج في قاعدة البيانات
            var product = await db.products
                .Include(p => p.LikedProducts) // تضمين جدول الإعجابات المرتبطة بالمنتج
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            // حذف جميع الإعجابات المرتبطة بالمنتج
            db.LikedProducts.RemoveRange(product.LikedProducts);

            // حذف المنتج نفسه
            db.products.Remove(product);

            // حفظ التغييرات في قاعدة البيانات
            await db.SaveChangesAsync();

            return Ok(new { message = "Product and its associated likes deleted successfully." });
        }



    }
}
