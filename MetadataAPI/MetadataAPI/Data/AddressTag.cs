using System;
using System.Linq;

namespace MetadataAPI.Data;

public sealed record class AddressTag : IEquatable<AddressTag>
{
    public string Sublocation { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ProvinceState { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;

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
}
