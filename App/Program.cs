using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi;
using PhotoEditor.Data;
using PhotoEditor.Services;
using PhotoEditor.Services.ImageProcessing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

// static void AddAuthServices(WebApplicationBuilder builder)
// {
//     builder.Services.AddScoped<IAuthService, SessionService>();

//     var repositorySettings = builder.Configuration.GetSection("Repository");
//     if (repositorySettings["AuthType"] == "InMemory")
//     {
//         builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
//     }
//     else
//     {
//         throw new InvalidOperationException($"Unsupported repository type: '{repositorySettings["Type"]}'");
//     }

//     builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
//     builder.Services.AddAuthorization();
// }

// static void AddRunTaskService(WebApplicationBuilder builder)
// {
//     builder.Services.AddScoped<RunTaskScheduler>();
// }

// static void AddCache(WebApplicationBuilder builder)
// {
//     builder.Services.AddDistributedMemoryCache();
// }

// static void AddSessionManagement(WebApplicationBuilder builder)
// {
//     builder.Services.AddSession(options =>
//     {
//         options.Cookie.Name = ".PhotoEditor.Session";
//         options.IdleTimeout = TimeSpan.FromSeconds(10);
//         options.Cookie.HttpOnly = true;
//         options.Cookie.IsEssential = true;
//     });
// }

// var builder = WebApplication.CreateBuilder(args);

// // ---------- SETUP ---------- //
// builder.Services.AddControllers();
// builder.Services.AddHttpContextAccessor();

// // swagger docs
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.AddSecurityDefinition(
//         "Bearer",
//         new OpenApiSecurityScheme
//         {
//             Name = "Authorization",
//             Type = SecuritySchemeType.Http,
//             Scheme = "bearer",
//             BearerFormat = "JWT",
//             In = ParameterLocation.Header,
//             Description = "Enter your JWT token"
//         }
//     );

//     options.AddSecurityRequirement(document =>
//     new OpenApiSecurityRequirement
//     {
//         [new OpenApiSecuritySchemeReference("Bearer", document)] = []
//     });
// });

// // services
// AddAuthServices(builder);
// AddRunTaskService(builder);
// AddCache(builder);
// AddSessionManagement(builder);

// // ---------- BUILD ---------- //
// var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseSession();

// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();

// app.Run();

string path = "abc.png";
using FileStream image = File.OpenRead(path);
var editor = new ImageProcessing();
await editor.ProcessPixelization(image, 100, 100);