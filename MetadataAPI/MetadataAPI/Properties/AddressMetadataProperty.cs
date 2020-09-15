using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Properties
{
    public class AddressMetadataProperty : IMetadataProperty<AddressTag>
    {
        public static AddressMetadataProperty Instance { get; } = new AddressMetadataProperty();

        public string Identifier { get; } = nameof(AddressMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private const string COUNTRY_NAME_KEY = "/xmp/<xmpbag>http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:LocationCreated/<xmpstruct>{ulong=0}/http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:CountryName";
        private const string PROVINCE_STATE_KEY = "/xmp/<xmpbag>http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:LocationCreated/<xmpstruct>{ulong=0}/http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:ProvinceState";
        private const string CITY_KEY = "/xmp/<xmpbag>http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:LocationCreated/<xmpstruct>{ulong=0}/http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:City";
        private const string SUBLOCATION_KEY = "/xmp/<xmpbag>http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:LocationCreated/<xmpstruct>{ulong=0}/http\\:\\/\\/iptc.org\\/std\\/Iptc4xmpExt\\/2008-02-29\\/:Sublocation";

        private AddressMetadataProperty() { }

        public AddressTag Read(IMetadataReader metadataReader)
        {
            string sublocation = (string)metadataReader.GetMetadata(SUBLOCATION_KEY);
            string city = (string)metadataReader.GetMetadata(CITY_KEY);
            string provinceState = (string)metadataReader.GetMetadata(PROVINCE_STATE_KEY);
            string country = (string)metadataReader.GetMetadata(COUNTRY_NAME_KEY);

            if (sublocation is null && city is null && provinceState is null && country is null)
            {
                return null;
            }

            return new AddressTag()
            {
                Sublocation = sublocation ?? string.Empty,
                City = city ?? string.Empty,
                ProvinceState = provinceState ?? string.Empty,
                Country = country ?? string.Empty
            };
        }

        public void Write(IMetadataWriter metadataWriter, AddressTag value)
        {
            metadataWriter.SetMetadata(COUNTRY_NAME_KEY, value?.Country);
            metadataWriter.SetMetadata(PROVINCE_STATE_KEY, value?.ProvinceState);
            metadataWriter.SetMetadata(CITY_KEY, value?.City);
            metadataWriter.SetMetadata(SUBLOCATION_KEY, value?.Sublocation);
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (AddressTag)value);
        }

    }
}
