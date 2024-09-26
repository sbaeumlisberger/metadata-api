using System;

namespace MetadataAPI.Data;

public readonly struct Fraction : IEquatable<Fraction>
{

    public static Fraction Zero = new Fraction(0, 1);

    public long Numerator { get; }
    public long Denominator { get; }

    public bool IsZero => Numerator == 0;

    public Fraction(long numerator, long denominator)
    {
        ArgumentOutOfRangeException.ThrowIfZero(denominator, nameof(denominator));
        Numerator = numerator;
        Denominator = denominator;
    }

    public Fraction GetReduced()
    {
        long gcd = GetGCD(Numerator, Denominator);
        long numerator = Numerator / gcd;
        long denominator = Denominator / gcd;
        if (Denominator < 0)
        {
            numerator = -Numerator;
            denominator = -Denominator;
        }
        return new Fraction(numerator, denominator);
    }

    public Fraction GetReciprocal()
    {
        if (Denominator == 0)
        {
            throw new ArithmeticException("There is no reciprocal for a fraction equal to 0.");
        }
        return new Fraction(Denominator, Numerator);
    }

    public static Fraction operator +(Fraction fraction1, Fraction fraction2)
    {
        return new Fraction(fraction1.Numerator * fraction2.Denominator + fraction2.Numerator * fraction1.Denominator, fraction1.Denominator * fraction2.Denominator).GetReduced();
    }

    public static Fraction operator +(Fraction fraction, long integer)
    {
        return new Fraction(fraction.Numerator + integer * fraction.Denominator, fraction.Denominator).GetReduced();
    }

    public static Fraction operator -(Fraction fraction1, Fraction fraction2)
    {
        return new Fraction(fraction1.Numerator * fraction2.Denominator - fraction2.Numerator * fraction1.Denominator, fraction1.Denominator * fraction2.Denominator).GetReduced();
    }

    public static Fraction operator -(Fraction fraction, long integer)
    {
        return new Fraction(fraction.Numerator - integer * fraction.Denominator, fraction.Denominator).GetReduced();
    }

    public static Fraction operator *(Fraction fraction1, Fraction fraction2)
    {
        return new Fraction(fraction1.Numerator * fraction2.Numerator, fraction1.Denominator * fraction2.Denominator).GetReduced();
    }

    public static Fraction operator *(Fraction fraction, long integer)
    {
        return new Fraction(fraction.Numerator * integer, fraction.Denominator).GetReduced();
    }

    public static Fraction operator /(Fraction fraction1, Fraction fraction2)
    {
        return fraction1 * fraction2.GetReciprocal();
    }

    public static Fraction operator /(Fraction fraction, long integer)
    {
        return fraction * new Fraction(1, integer);
    }

    public static bool operator ==(Fraction fraction1, Fraction fraction2)
    {
        return Equals(fraction1, fraction2);
    }

    public static bool operator ==(Fraction fraction, long integer)
    {
        return Equals(fraction, new Fraction(integer, 1));
    }

    public static bool operator !=(Fraction fraction1, Fraction fraction2)
    {
        return !Equals(fraction1, fraction2);
    }

    public static bool operator !=(Fraction fraction, long integer)
    {
        return !Equals(fraction, new Fraction(integer, 1));
    }

    public double ToDouble()
    {
        return (double)Numerator / Denominator;
    }

    public override string ToString()
    {
        if (Denominator == 1)
        {
            return Numerator.ToString();
        }
        return Numerator + "/" + Denominator;
    }

    public override int GetHashCode()
    {
        return Numerator.GetHashCode() ^ Denominator.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Fraction other && Equals(other);
    }

    public bool Equals(Fraction other)
    {
        Fraction reduced = GetReduced();
        Fraction otherReduced = other.GetReduced();
        return reduced.Numerator == otherReduced.Numerator
            && reduced.Denominator == otherReduced.Denominator;
    }

    private static long GetGCD(long a, long b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (a != 0 && b != 0)
        {
            if (a > b) a %= b;
            else b %= a;
        }
        if (a == 0) return b;
        else return a;
    }

}
