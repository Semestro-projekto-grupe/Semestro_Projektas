using Microsoft.EntityFrameworkCore;
using Semestro_projektas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Post> Posts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }


    }
}
