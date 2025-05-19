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
        public DbSet<Rating> ratings { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<BookExchange> BookExchange { get; set; }
        public DbSet<UserImage> Userimg { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<LostItem> LostItems { get; set; }
        public DbSet<LostItemImage> LostItemImages { get; set; }



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

            // إضافة في OnModelCreating في WebDbContext.cs
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // استخدام Restrict لتجنب مشاكل الحذف المتتالي

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // إضافة قيد فريد لمنع تكرار تقييم المستخدم لنفس المنتج
            modelBuilder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.ProductId })
                .IsUnique();


            //comments
            // عند حذف المنتج، احذف تعليقاته
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Product)
                .WithMany(p => p.comments)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // عند حذف المستخدم، لا تحذف تعليقاته تلقائيًا (اجعلها Restrict أو NoAction)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // ← هذا يمنع الخطأ

            modelBuilder.Entity<UserImage>()
               .HasOne(img => img.User)
               .WithMany(user => user.Images)
               .HasForeignKey(img => img.UserId)
               .OnDelete(DeleteBehavior.Cascade); // هذه السطر يحقق المطلوب



            // عند حذف المستخدم → حذف كل المنشورات التابعة له
            modelBuilder.Entity<User>()
                .HasMany(u => u.LostItems)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // عند حذف المنشور → حذف كل الصور التابعة له
            modelBuilder.Entity<LostItem>()
                .HasMany(i => i.Images)
                .WithOne(img => img.LostItem)
                .HasForeignKey(img => img.LostItemId)
                .OnDelete(DeleteBehavior.Cascade);


        }




    }
}
