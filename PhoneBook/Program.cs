using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.Models;
using PhoneBook.Repository;
using PhoneBook.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Добавяме конфигурацията за JWT ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var key = Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // for production - true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// create database and tables
// adds a couple of contacts
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PhoneBookContext>();
    await dbContext.Database.MigrateAsync();

    if (dbContext.PhoneNumbers.Any())
    {
        dbContext.PhoneNumbers.RemoveRange(dbContext.PhoneNumbers);
        await dbContext.SaveChangesAsync();
    }

    if (dbContext.Contacts.Any())
    {
        dbContext.Contacts.RemoveRange(dbContext.Contacts);
        await dbContext.SaveChangesAsync();
    }

    if (dbContext.Users.Any())
    {
        dbContext.Users.RemoveRange(dbContext.Users);
        await dbContext.SaveChangesAsync();
    }

    // 1. Създай потребител
    var user = new User
    {
        Username = "admin",
        PasswordHash = "somehash", // В продукция използвай истински хеш!
        Email = "admin@example.com"
    };
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    // 2. Използвай user.Id за контактите
    var phoneNumbers1 = new List<PhoneNumber>
    {
        new PhoneNumber { Number = "0888123456" },
        new PhoneNumber { Number = "0899123456" }
    };

    var contact = new Contact
    {
        Name = "Ivan Ivanov",
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        Address = "dr Ivan Straski",
        PhoneNumbers = phoneNumbers1,
        UserId = 0, // използвай валидно Id
        photo = null,
        User = user
    };

    var phoneNumbers = new List<PhoneNumber>
    {
        new PhoneNumber { Number = "0888000000" },
        new PhoneNumber { Number = "0888999999" }
    };

    var contact2 = new Contact
    {
        Name = "Dragan Ivanov",
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        Address = "Varna",
        PhoneNumbers = phoneNumbers,
        UserId = 0,
        photo = null,
        User = user
    };

    dbContext.Contacts.AddRange(contact, contact2);
    await dbContext.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

// Activating authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
