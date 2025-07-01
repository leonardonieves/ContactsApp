using Microsoft.EntityFrameworkCore;
using ContactosApp.Models;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ContactosApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
    }
}