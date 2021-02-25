#nullable enable
using System;

namespace CARLA_Hennery.Library.Python
{
    public class Version : IComparable<Version>
    {
        public Version()
        {
        }

        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public int CompareTo(Version? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0) return majorComparison;
            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0) return minorComparison;
            return Patch.CompareTo(other.Patch);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is Version other) return Equals(other);
            return false;
        }

        private bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch);
        }
    }
}