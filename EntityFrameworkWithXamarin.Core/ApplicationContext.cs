using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkWithXamarin.Core
{

    public class ApplicationContext : DbContext
    {
        private string DatabasePath { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reguest> Reguests { get; set; }
        public DbSet<TableItem> TableItems { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        public ApplicationContext(string databasePath)
        {
            // Database.EnsureCreated();
            DatabasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DatabasePath}");
        }
       
    }
}

