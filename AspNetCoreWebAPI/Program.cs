using AspNetCoreWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

//// 1. Add services to the container.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// добавл€ем свой репозиторий пользователей
// простой, хранит пользователей в пам€ти и при перезапуске они обнул€ютс€
// изначально есть пользователь с логином "a" и паролем "a" (латинницей)
// важно, что добавлен он с помощью AddSingleton, а не AddTransient или AddScoped
// (хот€ хз мб в данном случае поведение и не изменитс€,
// так как он получаетс€ только в одном контроллере в конструкторе,
// а не на каждый запрос)
builder.Services.AddSingleton<UserRepository>();

// конфигурирование аутентификации - аутентификаци€ с помощью кук (предоставл€ема€ ASP.NET Core)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // или просто "Cookies"
    .AddCookie(options =>
    {
        // без этой настройки не будет работать, если фронт хоститс€ в другом месте
        options.Cookie.SameSite = SameSiteMode.None;
        // куки будут HttpOnly - их нельз€ будет получить через JavaScript в браузере
        // это типа более безопасно
        options.Cookie.HttpOnly = true;
        // это хз что
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // исправл€ет ненужный редирект на /Account/Login (обработчика которого и нет)
        // без этой штуки при неудачной аутентификации все врем€ редиректило на /Account/Login,
        // а так как обработчика на такой путь нет, то возвращало по итогу код 404 not found.
        // а с этой штукой при неудачной аутентификации просто возвращает код 401 Not Authorized
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// чтобы фронтенд мог общатьс€ с сервером - CORS
// CORS нужен, так как фронт будет хоститьс€ отдельно от бэка
builder.Services.AddCors(conf =>
{
    conf.AddPolicy("forFrontend", policy =>
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
app.UseCors("forFrontend");

// Swagger - при запуске приложени€ показывает, что есть в API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// јутентификаци€
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
