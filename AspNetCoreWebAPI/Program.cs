using AspNetCoreWebAPI.Models;
using AspNetCoreWebAPI.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

//// 1. Add services to the container.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// добавляем хранилище пользователей
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

// конфигурирование аутентификации - аутентификация с помощью кук (предоставляемая ASP.NET Core)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // или просто "Cookies"
    .AddCookie(options =>
    {
        // без этой настройки не будет работать, если фронт хостится в другом месте
        options.Cookie.SameSite = SameSiteMode.None;

        // куки будут HttpOnly - их нельзя будет получить через JavaScript в браузере
        // это типа более безопасно
        options.Cookie.HttpOnly = true;

        // точно не знаю что делает, наверное требует,
        // чтобы аутентификация всегда проводилась через HTTPS, а не HTTP
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // без этой штуки при неудачной аутентификации все время редиректило на "/Account/Login",
        // а так как обработчика на такой путь нет, то возвращало по итогу код 404 (Not Found).
        // а с этой штукой при неудачной аутентификации просто возвращает код 401 (Not Authorized)
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// чтобы фронтенд мог общаться с сервером, нужен CORS
// (CORS нужен, так как фронт будет хоститься отдельно)
var corsPolicyName = "frontend";
builder.Services.AddCors(conf =>
{
    conf.AddPolicy(corsPolicyName, policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();

        // разрешить получать запросы только из фронтенда
        string whereIsFrontend = "http://localhost:3000";
        policy.WithOrigins(whereIsFrontend);

        // так же можно разрешить получать запросы отовсюду (но наверное это небезопасно)
        //policy.SetIsOriginAllowed(origin => true);
    });
});

//// 2. Build the WebApplication
var app = builder.Build();

//// 3. Configure the HTTP request pipeline.

// устанавливаем сконфигурированную ранее политику CORS
app.UseCors(corsPolicyName);

// Swagger - при запуске приложения показывает, что есть в API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Аутентификация
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
