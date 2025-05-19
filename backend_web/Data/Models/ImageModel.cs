namespace backend_web.Data.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string Name { get; set; } // اسم الصورة

         public byte[]? image { get; set; } // الصورة (binary data)
        public string ContentType { get; set; } // نوع الصورة (مثل image/png)

        //[ForeignKey("Product")]
        public int ProductId { get; set; } // العلاقة مع المنتج
        public Product Product { get; set; } // المرجع العكسي
    }
}
