using backend_web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);


// إعداد CORS للسماح للفرونتند بالاتصال
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();  // مهم للسماح بإرسال الكوكيز والاتصالات مع الاعتماديات
            });
});




// إضافة الخدمات
builder.Services.AddControllers();



//////////////////////////////////////////////////
builder.Services.AddSignalR();
//////////////////////////////////////////////////
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
///////////////////////////////////////////////////////////////////////


// تفعيل AllowSynchronousIO
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// إضافة DbContext مع إعدادات الاتصال بقاعدة البيانات
builder.Services.AddDbContext<WebDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// إضافة Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// بناء التطبيق
var app = builder.Build();


// تفعيل Swagger فقط في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

/////////////////////////////////
app.MapHub<ChatHub>("/chatHub");
/////////////////////////////////

app.UseStaticFiles();

// تفعيل HTTPS
app.UseHttpsRedirection();

// تفعيل سياسة CORS
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

// ربط الـ Controllers
app.MapControllers();

// تشغيل التطبيق
app.Run();
