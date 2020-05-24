﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Definitions
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

        public AddressTag Read(IReadMetadata metadataReader)
        {
            AddressTag address = new AddressTag();

            if (metadataReader.GetMetadata(COUNTRY_NAME_KEY) is string country)
            {
                address.Country = country;
            }
            if (metadataReader.GetMetadata(PROVINCE_STATE_KEY) is string provinceState)
            {
                address.ProvinceState = provinceState;
            }
            if (metadataReader.GetMetadata(CITY_KEY) is string city)
            {
                address.City = city;
            }
            if (metadataReader.GetMetadata(SUBLOCATION_KEY) is string sublocation)
            {
                address.Sublocation = sublocation;
            }

            //if (address.IsEmpty)
            //{
            //    return null;
            //}

            return address;
        } 

        public void Write(IWriteMetadata metadataWriter, AddressTag value)
        {
            metadataWriter.SetMetadata(COUNTRY_NAME_KEY, value.Country);
            metadataWriter.SetMetadata(PROVINCE_STATE_KEY, value.ProvinceState);
            metadataWriter.SetMetadata(CITY_KEY, value.City);
            metadataWriter.SetMetadata(SUBLOCATION_KEY, value.Sublocation);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (AddressTag)value);
        }

    }
}
