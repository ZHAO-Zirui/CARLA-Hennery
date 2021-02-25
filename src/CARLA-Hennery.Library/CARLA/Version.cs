#nullable enable
using System;

namespace CARLA_Hennery.Library.CARLA
{
    public class Version : IComparable<Version>
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public int Additional { get; set; }

        public Version()
        {
        }

        public Version(int major, int minor, int patch, int additional = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Additional = additional;
        }

        public int CompareTo(Version? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0) return majorComparison;
            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0) return minorComparison;
            var patchComparison = Patch.CompareTo(other.Patch);
            if (patchComparison != 0) return patchComparison;
            return Additional.CompareTo(other.Additional);
        }

        public override string ToString()
        {
            if (Additional == 0) return $"{Major}.{Minor}.{Patch}";
            return $"{Major}.{Minor}.{Patch}.{Additional}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is Version other) return Equals(other);
            return false;
        }

        protected bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch && Additional == other.Additional;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Additional);
        }
    }
}