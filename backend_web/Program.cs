using backend_web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// إضافة إعدادات CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// إضافة الخدمات
builder.Services.AddControllers();

// تفعيل AllowSynchronousIO
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// إضافة DbContext مع إعدادات الاتصال
builder.Services.AddDbContext<WebDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// تفعيل Swagger فقط في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// تفعيل HTTPS
app.UseHttpsRedirection();

// تفعيل سياسة CORS
app.UseCors("AllowAll");

app.UseAuthorization();

// ربط الـ Controllers
app.MapControllers();

// تشغيل التطبيق
app.Run();
