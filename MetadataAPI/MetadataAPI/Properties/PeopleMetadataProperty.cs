using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MetadataAPI.Data;
using WIC;

namespace MetadataAPI.Properties;

public class PeopleMetadataProperty : MetadataPropertyBase<IList<PeopleTag>>
{
    public static PeopleMetadataProperty Instance { get; } = new PeopleMetadataProperty();

    public override string Identifier { get; } = nameof(PeopleMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private const string RegionsBlockKey = "/xmp/<xmpstruct>MP:RegionInfo/<xmpbag>MPRI:Regions";
    private const string NameKey = "/MPReg:PersonDisplayName";
    private const string RectangleKey = "/MPReg:Rectangle";
    private const string EmailDigestKey = "/MPReg:PersonEmailDigest";
    private const string LiveCIDKey = "/MPReg:PersonLiveCID";

    private PeopleMetadataProperty() { }

    public override IList<PeopleTag> Read(IMetadataReader metadataReader)
    {
        var peopleTasgs = new List<PeopleTag>();

        var regions = metadataReader.GetMetadataBlock(RegionsBlockKey);

        if (regions is null)
        {
            return peopleTasgs;
        }

        foreach (string key in regions.GetKeys().OrderBy(key => ParseIndexFromKey(key)))
        {
            var region = regions.GetMetadataBlock(key)!;

            if (region.GetMetadata(NameKey) is string name && !string.IsNullOrWhiteSpace(name))
            {
                FaceRect? rectangle = null;

                if (region.GetMetadata(RectangleKey) is string rectangleRaw)
                {
                    rectangle = ParseRect(rectangleRaw);
                }

                string? emailDigest = region.GetMetadata(EmailDigestKey) as string;
                string? liveCID = region.GetMetadata(LiveCIDKey) as string;

                peopleTasgs.Add(new PeopleTag(name, rectangle, emailDigest, liveCID));
            }
        }

        return peopleTasgs;
    }

    public override void Write(IMetadataWriter metadataWriter, IList<PeopleTag> people)
    {
        if (metadataWriter.GetMetadataBlock(RegionsBlockKey) is not null)
        {
            metadataWriter.SetMetadata(RegionsBlockKey, null);
        }
        for (int i = 0; i < people.Count; i++)
        {
            string entryKey = RegionsBlockKey + "/<xmpstruct>{ulong=" + i + "}";
            string name = people[i].Name;
            string? rect = RectToString(people[i].Rectangle);
            string? emailDigest = people[i].EmailDigest;
            string? liveCID = people[i].LiveCID;
            metadataWriter.SetMetadata(entryKey + NameKey, name);
            metadataWriter.SetMetadata(entryKey + RectangleKey, rect);
            metadataWriter.SetMetadata(entryKey + EmailDigestKey, emailDigest);
            metadataWriter.SetMetadata(entryKey + LiveCIDKey, liveCID);
        }
    }

    private int ParseIndexFromKey(string key)
    {
        // key = "/{ulong=index}"
        var indexString = key.Substring(8, key.Length - 9);
        return int.Parse(indexString);
    }

    private FaceRect? ParseRect(string s)
    {
        if (string.IsNullOrEmpty(s)) return null;
        string[] values = s.Split(',');
        double x = double.Parse(values[0], CultureInfo.InvariantCulture);
        double y = double.Parse(values[1], CultureInfo.InvariantCulture);
        double width = double.Parse(values[2], CultureInfo.InvariantCulture);
        double height = double.Parse(values[3], CultureInfo.InvariantCulture);
        return new FaceRect(x, y, width, height);
    }

    private string? RectToString(FaceRect? rect)
    {
        if (rect is null) return null;
        string x = rect.Value.X.ToString(CultureInfo.InvariantCulture);
        string y = rect.Value.Y.ToString(CultureInfo.InvariantCulture);
        string width = rect.Value.Width.ToString(CultureInfo.InvariantCulture);
        string height = rect.Value.Height.ToString(CultureInfo.InvariantCulture);
        return x + ", " + y + ", " + width + ", " + height;
    }
}
