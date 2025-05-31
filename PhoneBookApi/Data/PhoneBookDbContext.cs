using Microsoft.EntityFrameworkCore;
using PhoneBookApi.Models;

namespace PhoneBookApi.Data
{
    public class PhoneBookDbContext : DbContext
    {
        public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options)
            : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
