using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.Models;
using PhoneBook.Repository;

var builder = WebApplication.CreateBuilder(args);

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
        UserId = 0, // Set to a valid UserId if you have users
        photo = null,
        User = null
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
        UserId = 0, // Set to a valid UserId if you have users
        photo = null,
        User = null
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
app.UseAuthorization();
app.MapControllers();
app.Run();
