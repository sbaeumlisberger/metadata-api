using System;
using System.Collections.Generic;
using MetadataAPI.Data;
using WIC;

namespace MetadataAPI.Properties;

public class FocalLengthMetadataProperty : MetadataPropertyBase<double?>
{
    public static FocalLengthMetadataProperty Instance { get; } = new FocalLengthMetadataProperty();

    public override string Identifier { get; } = nameof(FocalLengthMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private FocalLengthMetadataProperty() { }

    public override double? Read(IMetadataReader metadataReader)
    {
        return (double?)metadataReader.GetMetadata("System.Photo.FocalLength");
    }

    public override void Write(IMetadataWriter metadataWriter, double? value)
    {
        if (value is double doubleValue)
        {
            var fraction = ToFraction(doubleValue);
            metadataWriter.SetMetadata("System.Photo.FocalLengthNumerator", fraction.Numerator);
            metadataWriter.SetMetadata("System.Photo.FocalLengthDenominator", fraction.Denominator);
        }
        else
        {
            metadataWriter.SetMetadata("System.Photo.FocalLengthNumerator", null);
            metadataWriter.SetMetadata("System.Photo.FocalLengthDenominator", null);
        }
    }

    private Fraction ToFraction(double doubleValue)
    {
        long denominator = 10000; // precession of 4 decimal places
        long numerator = (long)Math.Round(doubleValue * denominator);
        return new Fraction(numerator, denominator).GetReduced();
    }
}
