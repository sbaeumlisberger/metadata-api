using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Definitions
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

        public IList<PeopleTag> Read(IReadMetadata metadataReader)
        {
            var peopleTasgs = new List<PeopleTag>();

            var regions = metadataReader.GetMetadataBlock(RegionsBlockKey);

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

        public void Write(IWriteMetadata metadataWriter, IList<PeopleTag> people)
        {
            int existingCount = metadataWriter.GetMetadataBlock(RegionsBlockKey).GetKeys().Count();

            for (int n = 0; n < Math.Max(people.Count, existingCount); n++)
            {
                string name = null;
                string rect = null;
                string emailDigest = null;
                string liveCID = null;

                if (n < people.Count)
                {
                    name = people[n].Name;
                    rect = RectToString(people[n].Rectangle);
                    emailDigest = people[n].EmailDigest;
                    liveCID = people[n].LiveCID;
                }

                metadataWriter.SetMetadata(CreateKey(n, NameKey), name);
                metadataWriter.SetMetadata(CreateKey(n, RectangleKey), rect);
                metadataWriter.SetMetadata(CreateKey(n, EmailDigestKey), emailDigest);
                metadataWriter.SetMetadata(CreateKey(n, LiveCIDKey), liveCID);
            }
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (IList<PeopleTag>)value);
        }

        private int ParseIndexFromKey(string key)
        {
            // key = "/{ulong=index}"
            var indexString = key.Substring(8, key.Length - 9);
            return int.Parse(indexString);
        }

        private string CreateKey(int n, string propertyKey)
        {
            return RegionsBlockKey + "/<xmpstruct>{ulong=" + n + "}" + propertyKey;
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
