using System;

namespace MetadataAPI.Data;

public sealed record class PeopleTag : IEquatable<PeopleTag>
{
    public string Name { get; }
    public FaceRect? Rectangle { get; init; }
    public string? EmailDigest { get; init; }
    public string? LiveCID { get; init; }

    public PeopleTag(string name, FaceRect? rectangle = null, string? eMailDigest = null, string? liveId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(null, nameof(name));
        }
        Name = name;
        Rectangle = rectangle;
        EmailDigest = eMailDigest;
        LiveCID = liveId;
    }
}
