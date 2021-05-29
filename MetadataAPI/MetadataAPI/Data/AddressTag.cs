using System;
using System.Linq;

namespace MetadataAPI.Data
{

    public sealed class AddressTag : IEquatable<AddressTag>
    {

        public string Sublocation { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ProvinceState { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public AddressTag() { }

        public AddressTag(string sublocation, string city, string provinceState, string country)
        {
            Sublocation = sublocation;
            City = city;
            ProvinceState = provinceState;
            Country = country;
        }

        public override string ToString()
        {
            var allParts = new[] { Sublocation, City, ProvinceState, Country };
            var nonEmptyParts = allParts.Where(part => !string.IsNullOrEmpty(part));
            return string.Join(" ", nonEmptyParts);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AddressTag);
        }

        public bool Equals(AddressTag? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Sublocation == other.Sublocation
                && City == other.City
                && ProvinceState == other.ProvinceState
                && Country == other.Country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Sublocation, City, ProvinceState, Country);
        }

        public static bool operator ==(AddressTag? obj1, AddressTag? obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(AddressTag? obj1, AddressTag? obj2)
        {
            return !Equals(obj1, obj2);
        }

    }

}
