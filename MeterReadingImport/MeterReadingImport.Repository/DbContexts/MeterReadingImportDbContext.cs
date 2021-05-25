﻿using MeterReadingImport.Domain.Entities.MeterReadingImport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MeterReadingImport.Repository.DbContexts
{
    public class MeterReadingImportDbContext : DbContext
    {
        private IConfiguration _config;

        public MeterReadingImportDbContext() { }

        public MeterReadingImportDbContext(DbContextOptions<MeterReadingImportDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}
