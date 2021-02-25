using System;
using System.Collections.Generic;
using System.Text;
using CARLA_Hennery.Master.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CARLA_Hennery.Master.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<KnownCarlaVersion> KnownCarlaVersions { get; set; }

        public DbSet<KnownPythonVersion> KnownPythonVersions { get; set; }
        
    }
}