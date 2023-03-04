using P010Store.Data;
using P010Store.Data.Abstract;
using P010Store.Data.Concrete;
using P010Store.Service.Abstract;
using P010Store.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies; //oturum iþlemi için gerekli kütüphane


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(); // Entityframework iþlemlerini yapabilmek için bu satýrý ekliyoruz


builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient(typeof(IService<>), typeof(Service<>)); //Veritabaný iþlemleri yapacaðýmýz servisleri ekledik.Burada .net core a eðer sana IService interface i kullanma isteði gelirse Service sýnýfýndan bir nesne oluþtur demiþ olduk
//. net core 3 farklý yöntemle servisleri ekleyebiliyoruz:

//builder.Services.AddSingleton(); : AddSingleton kullanarak oluþturduðumuz nesneden 1 tane örnek oluþtur ve her seferinde bu örnek kullanýlýr.

//builder.Services.AddTransient(); : yönteminde ise önceden oluþmuþ nesne varsa o kullanýlýr yoksa yenisi oluþuturulur

//builder.Services.AddScoped(); : yönteminde ise yapýlan her istek için yeni bir nesne oluþturulur

builder.Services.AddTransient<IProductService, ProductService>(); //producta özel yazdýðýmýz servis
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
//HttpContextAccessor ile uygulama içerisindeki giriþ yapan kullanýcý,session verileri,cookieler gibi içeriklere view lardan veya controllerdan ulaþabilmemizi saðlar.

// Authentication : Oturum açma servisi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x => 
{
    x.LoginPath = "/Admin/Login/"; //giriþyapma sayfasý
    x.AccessDeniedPath = "/AccessDenied/"; //giriþ yapan kullancýnýn admin yetkisi yoksa AccessDenied sayfasýna yönlendir
    
    x.LogoutPath = "/Admin/Login/Logout/"; //çýkýþ sayfasý
    x.Cookie.Name = "Administrator"; //Oluþacak cookie nin adý
    x.Cookie.MaxAge = TimeSpan.FromDays(1); //Oluþacak cookie nin yaþam süresi
});
//Authorization : Yetkilendirme
builder.Services.AddAuthorization( x =>
{
    x.AddPolicy("AdminPolicy", policy=>policy.RequireClaim("Role","Admin"));
    x.AddPolicy("UserPolicy", policy => policy.RequireClaim("Role", "User"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//UseAuthentication() : oturum açma-giriþ yapma yeri
app.UseAuthentication(); //admin login için. UseAuthentication() ýn UseAuthorization() dan önce gelmesi zorunlu!!!

//UseAuthorization() : yetkilendirme (oturum açan kullanýcýnýn admine giriþ yetkisi var mý)
app.UseAuthorization(); 

app.MapControllerRoute(
            name: "admin",
            pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "custom",
    pattern: "{customur1?}/{controller=Home}/{action=Index}/{id?}");

app.Run();



//Katmanlý Mimari Ýliþki Hiyerarþisi

/*
*Web UI üzerinde veritabaný iþlemlerini yapabilmek için WebUI ýn dependencies(referanslarýna) Service katmanýný dependencies e sað týklayýp as project diyerek açýlan pencereden Service katmanýna tik koyup ok butonuyla pencereyi kapatýp baðlantý kurduk.
*Service katmaný da veritabaný iþlemlerini yapabilmek için Data katmanýna eriþmesi gerekiyor, yine dpendencies e sað týklayýp add project refences diyerek açýlan pencereden Data katmanýna iþaret koyup ekliyoruz.
*Data katmanýn da entity lere eriþmesi gerekiyor ki class larý kullanarak veritabaný iþlemleri yapabilsin.yine ayný yolu izleyerek veya class larýn üzerine gelip ampul e týklayýp add project references diyerek data dan entities e eriþim vermemiz gerekiyor.
*/