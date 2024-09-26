using Xunit.Sdk;

namespace MetadataAPITest;

class AssertUtil
{

    public static void GreaterThan(long than, long actual)
    {
        if (!(actual > than))
        {
            throw new XunitException($"{actual} is not greater than {than}.");
        }
    }

    public static void LessThan(long than, long actual)
    {
        if (!(actual < than))
        {
            throw new XunitException($"{actual} is not less than {than}.");
        }
    }
}
