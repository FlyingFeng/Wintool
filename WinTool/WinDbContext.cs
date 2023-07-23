using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinTool.Entities;

namespace WinTool
{
    internal class WinDbContext : DbContext
    {
        internal static string DbPath = Path.Combine(AppContext.BaseDirectory, "Db", "WinTool.db");



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //DbPath = Path.Combine(AppContext.BaseDirectory, "Db", "WinTool.db");
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }



        public DbSet<MyPasswordEntity> MyPasswords { get; set; }

    }
}
