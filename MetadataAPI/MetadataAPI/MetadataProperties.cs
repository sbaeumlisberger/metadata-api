using System;
using System.Collections.Generic;
using System.Text;
using MetadataAPI.Data;
using MetadataAPI.Properties;

namespace MetadataAPI
{
    public static partial class MetadataProperties
    {
        public static IMetadataProperty<AddressTag?> Address => AddressMetadataProperty.Instance;
        public static IMetadataProperty<string[]?> Author => AuthorMetadataProperty.Instance;
        public static IMetadataProperty<string?> CameraManufacturer => CameraManufacturerMetadataProperty.Instance;
        public static IMetadataProperty<string?> CameraModel => CameraModelMetadataProperty.Instance;
        public static IMetadataProperty<string?> Copyright => CopyrightMetadataProperty.Instance;
        public static IMetadataProperty<DateTime?> DateTaken => DateTakenMetadataProperty.Instance;
        public static IMetadataProperty<Fraction?> ExposureTime => ExposureTimeMetadataProperty.Instance;
        public static IMetadataProperty<Fraction?> FNumber => FNumberMetadataProperty.Instance;
        public static IMetadataProperty<ushort?> FocalLengthInFilm => FocalLengthInFilmMetadataProperty.Instance;
        public static IMetadataProperty<double?> FocalLength => FocalLengthMetadataProperty.Instance;
        public static IMetadataProperty<GeoTag?> GeoTag => GeoTagMetadataProperty.Instance;
        public static IMetadataProperty<ushort?> ISOSpeed => ISOSpeedMetadataProperty.Instance;
        public static IMetadataProperty<string[]?> Keywords => KeywordsMetadataProperty.Instance;
        public static IMetadataProperty<PhotoOrientation> Orientation => OrientationMetadataProperty.Instance;
        public static IMetadataProperty<IList<PeopleTag>?> People => PeopleMetadataProperty.Instance;
        public static IMetadataProperty<int?> Rating => RatingMetadataProperty.Instance;
        public static IMetadataProperty<string?> Title => TitleMetadataProperty.Instance;
    }
}
