using backend_web.Data.Models;

namespace backend_web.Model
{
    public class mdlProduct
    {
        public string? Name { get; set; }
        public string? caption { get; set; }
        public decimal price { get; set; }
        public string type { get; set; }
        public string? PHnum { get; set; }
        public List<IFormFile> images { get; set; } // الصور المرفقة

        public int userId { get; set; }
       

    }
}
