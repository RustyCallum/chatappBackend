using chatappBackend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection"));
});

builder.Services.AddCors(options => options.AddPolicy(name: "ChatAppAccess", policy => { policy.WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithExposedHeaders("X-CSRF-TOKEN");
}));
builder.Services.AddAuthentication(y =>
{
    y.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    y.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    y.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.Authority = "http://localhost:7094/";
        x.TokenValidationParameters = new TokenValidationParameters
        {
            
            ValidateLifetime = false,
            ValidAudience = "http://localhost:7094/",
            ValidIssuer = "http://localhost:7094/",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Aleksander51HaHaXDbekazwasxDxDnienawidzewas"))

        };
        x.Events = new JwtBearerEvents
        {
            OnMessageReceived = y =>
            {
                y.Token = y.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    }
);

builder.Services.AddAuthorization();

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ChatAppAccess");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
