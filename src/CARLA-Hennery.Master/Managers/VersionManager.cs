using System.Linq;
using CARLA_Hennery.Master.Data;
using CARLA_Hennery.Master.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CARLA_Hennery.Master.Managers
{
    public class VersionManager
    {
        private readonly ILogger<VersionManager> _logger;
        private readonly ApplicationDbContext _db;

        public VersionManager(ILogger<VersionManager> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IQueryable<KnownCarlaVersion> KnownCarlaVersions => _db.KnownCarlaVersions;

        public IQueryable<KnownPythonVersion> KnownPythonVersions => _db.KnownPythonVersions;

        public bool IsPythonVersionKnown(Library.Python.Version other)
        {
            return KnownPythonVersions.Cast<Library.Python.Version>().Any(i => Equals(i, other));
        }
        
        public bool IsCarlaVersionKnown(Library.CARLA.Version other)
        {
            return KnownPythonVersions.Cast<Library.CARLA.Version>().Any(i => Equals(i, other));
        }

        public void AddPythonVersion(Library.Python.Version version)
        {
            if (IsPythonVersionKnown(version)) return;
            var known = new KnownPythonVersion(version);
            _db.KnownPythonVersions.Add(known);
            _db.SaveChanges();
        }
        
        public void AddCarlaVersion(Library.CARLA.Version version)
        {
            if (IsCarlaVersionKnown(version)) return;
            var known = new KnownCarlaVersion(version);
            _db.KnownCarlaVersions.Add(known);
            _db.SaveChanges();
        }
    }
}