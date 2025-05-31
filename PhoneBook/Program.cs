using Microsoft.EntityFrameworkCore;
using WebHomework.Data;
using WebHomework.Models;
using WebHomework.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
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
        PhoneNumbers = phoneNumbers1
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
        PhoneNumbers = phoneNumbers
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
