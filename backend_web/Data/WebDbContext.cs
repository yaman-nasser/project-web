using backend_web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_web.Data
{
    public class WebDbContext : DbContext
    {

        public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<LikedProduct> LikedProducts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // العلاقة بين User و Product: عند حذف المستخدم، يتم حذف منتجاته
            modelBuilder.Entity<User>()
                .HasMany(u => u.products)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة بين Product و Image: عند حذف المنتج، يتم حذف الصور الخاصة به
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة بين LikedProduct و User: عند حذف المستخدم، يتم حذف إعجاباته
            modelBuilder.Entity<LikedProduct>()
                .HasOne(lp => lp.User)
                .WithMany(u => u.LikedProducts)
                .HasForeignKey(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة بين LikedProduct و Product: عند حذف المنتج، لا يتم حذف إعجاباته تلقائيًا
            modelBuilder.Entity<LikedProduct>()
                .HasOne(lp => lp.Product)
                .WithMany(p => p.LikedProducts)
                .HasForeignKey(lp => lp.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتتالي لتجنب Multiple Cascade Paths

            // جعل البريد الإلكتروني فريداً لكل مستخدم
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }



    }
}
