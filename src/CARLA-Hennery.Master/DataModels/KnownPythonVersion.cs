using System;
using System.ComponentModel.DataAnnotations;
using Version = CARLA_Hennery.Library.Python.Version;

namespace CARLA_Hennery.Master.DataModels
{
    public class KnownPythonVersion : Version 
    {
        [Key]
        public int Id { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public KnownPythonVersion()
        {
        }

        public KnownPythonVersion(Version version) : base(version.Major,version.Major,version.Patch)
        {
            UpdateDateTime = DateTime.Now;
        }
    }
}