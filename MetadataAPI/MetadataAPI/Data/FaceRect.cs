using System;
using System.Collections.Generic;
using System.Text;
using DotNetToolkit.Foundation;

namespace MetadataAPI.Data
{
    public struct FaceRect
    {
        public static FaceRect Empty { get; } = new FaceRect(double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity);

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public FaceRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            return obj is FaceRect other ? Equals(other) : false;
        }

        public bool Equals(FaceRect other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return X == other.X
                && Y == other.Y
                && Width == other.Width
                && Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Of(X, Y, Width, Height);
        }

        public static bool operator ==(FaceRect obj1, FaceRect obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(FaceRect obj1, FaceRect obj2)
        {
            return !Equals(obj1, obj2);
        }

    }
}
