using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ViagemCRUDProject.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("MyConnection") { }

        public DbSet<User> Table_Users { get; set; }
        public DbSet<Admin> Table_Admins { get; set; }
    }
}