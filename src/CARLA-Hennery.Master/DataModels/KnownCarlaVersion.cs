using System;
using System.ComponentModel.DataAnnotations;
using Version = CARLA_Hennery.Library.CARLA.Version;

namespace CARLA_Hennery.Master.DataModels
{
    public class KnownCarlaVersion : Version
    {
        [Key]
        public int Id { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public KnownCarlaVersion()
        {
        }

        public KnownCarlaVersion(Version version) : base(version.Major, version.Minor, version.Patch,
            version.Additional)
        {
            
        }
    }
}