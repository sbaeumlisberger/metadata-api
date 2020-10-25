using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Properties
{
    public class PeopleMetadataProperty : IMetadataProperty<IList<PeopleTag>>
    {
        public static PeopleMetadataProperty Instance { get; } = new PeopleMetadataProperty();

        public string Identifier { get; } = nameof(PeopleMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private const string RegionsBlockKey = "/xmp/<xmpstruct>MP:RegionInfo/<xmpbag>MPRI:Regions";
        private const string NameKey = "/MPReg:PersonDisplayName";
        private const string RectangleKey = "/MPReg:Rectangle";
        private const string EmailDigestKey = "/MPReg:PersonEmailDigest";
        private const string LiveCIDKey = "/MPReg:PersonLiveCID";

        private PeopleMetadataProperty() { }

        public IList<PeopleTag> Read(IMetadataReader metadataReader)
        {
            var peopleTasgs = new List<PeopleTag>();

            var regions = metadataReader.GetMetadataBlock(RegionsBlockKey);

            if (regions is null)
            {
                return peopleTasgs;
            }

            foreach (string key in regions.GetKeys().OrderBy(key => ParseIndexFromKey(key)))
            {
                var region = regions.GetMetadataBlock(key);

                if (region.GetMetadata(NameKey) is string name && !string.IsNullOrWhiteSpace(name))
                {
                    var peopleTag = new PeopleTag((string)name);

                    if (region.GetMetadata(RectangleKey) is string rectangle)
                    {
                        peopleTag.Rectangle = ParseRect(rectangle);
                    }
                    if (region.GetMetadata(EmailDigestKey) is string emailDigest)
                    {
                        peopleTag.EmailDigest = emailDigest;
                    }
                    if (region.GetMetadata(LiveCIDKey) is string liveCID)
                    {
                        peopleTag.LiveCID = liveCID;
                    }

                    peopleTasgs.Add(peopleTag);
                }
            }

            return peopleTasgs;
        }

        public void Write(IMetadataWriter metadataWriter, IList<PeopleTag> people)
        {
            people = people ?? new PeopleTag[0];

            int existingCount = metadataWriter.GetMetadataBlock(RegionsBlockKey)?.GetKeys().Count() ?? 0;

            for (int n = 0; n < Math.Max(people.Count, existingCount); n++)
            {
                string entryKey = RegionsBlockKey + "/<xmpstruct>{ulong=" + n + "}";

                if (n < people.Count)
                {
                    string name = people[n].Name;
                    string rect = RectToString(people[n].Rectangle);
                    string emailDigest = people[n].EmailDigest;
                    string liveCID = people[n].LiveCID;
                    metadataWriter.SetMetadata(entryKey + NameKey, name);
                    metadataWriter.SetMetadata(entryKey + RectangleKey, rect);
                    metadataWriter.SetMetadata(entryKey + EmailDigestKey, emailDigest);
                    metadataWriter.SetMetadata(entryKey + LiveCIDKey, liveCID);
                }
                else
                {
                    metadataWriter.SetMetadata(entryKey, null);
                }
            }
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (IList<PeopleTag>)value);
        }

        private int ParseIndexFromKey(string key)
        {
            // key = "/{ulong=index}"
            var indexString = key.Substring(8, key.Length - 9);
            return int.Parse(indexString);
        }

        private FaceRect ParseRect(string s)
        {
            if (string.IsNullOrEmpty(s)) return FaceRect.Empty;
            string[] values = s.Split(',');
            double x = double.Parse(values[0], CultureInfo.InvariantCulture);
            double y = double.Parse(values[1], CultureInfo.InvariantCulture);
            double width = double.Parse(values[2], CultureInfo.InvariantCulture);
            double height = double.Parse(values[3], CultureInfo.InvariantCulture);
            return new FaceRect(x, y, width, height);
        }

        private string RectToString(FaceRect rect)
        {
            if (rect == FaceRect.Empty) return "";
            string x = rect.X.ToString(CultureInfo.InvariantCulture);
            string y = rect.Y.ToString(CultureInfo.InvariantCulture);
            string width = rect.Width.ToString(CultureInfo.InvariantCulture);
            string height = rect.Height.ToString(CultureInfo.InvariantCulture);
            return x + ", " + y + ", " + width + ", " + height;
        }
    }
}
