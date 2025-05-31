using Microsoft.EntityFrameworkCore;
using PhoneBookApi.Data;
using PhoneBookApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PhoneBookDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PhoneBookDbContext>();
    await dbContext.Database.MigrateAsync();

    dbContext.PhoneNumbers.RemoveRange(dbContext.PhoneNumbers);
    dbContext.Addresses.RemoveRange(dbContext.Addresses);
    dbContext.Contacts.RemoveRange(dbContext.Contacts);
    await dbContext.SaveChangesAsync();

    // Добавяне на адреси
    var address1 = new Address
    {
        Street = "dr Ivan Straski",
        City = "Sofia",
        PostalCode = "1000"
    };

    var address2 = new Address
    {
        Street = "Somewhere",
        City = "Varna",
        PostalCode = "9000"
    };

    dbContext.Addresses.AddRange(address1, address2);
    await dbContext.SaveChangesAsync();

    // Добавяне на контакти (без връзка към Address обект, тъй като Contact.Address е string)
    var contact1 = new Contact
    {
        Name = "Ivan Ivanov",
        Address = $"{address1.Street}, {address1.City}, {address1.PostalCode}",
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        PhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0888123456" },
            new PhoneNumber { Number = "0899123456" }
        }
    };

    var contact2 = new Contact
    {
        Name = "Dragan Ivanov",
        Address = $"{address2.Street}, {address2.City}, {address2.PostalCode}",
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        PhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0888000000" },
            new PhoneNumber { Number = "0888999999" }
        }
    };

    dbContext.Contacts.AddRange(contact1, contact2);
    await dbContext.SaveChangesAsync();
}

app.MapControllers();
app.Run();
