using System;

namespace MetadataAPI.Data
{

    public sealed class PeopleTag : IEquatable<PeopleTag>
    {

        public string Name { get; set; } = string.Empty;
        public FaceRect Rectangle { get; set; } = FaceRect.Empty;
        public string EmailDigest { get; set; } = string.Empty;
        public string LiveCID { get; set; } = string.Empty;

        public PeopleTag(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }
            Name = name;
        }

        public PeopleTag(string name, FaceRect rectangle) : this(name)
        {
            Rectangle = rectangle;
        }

        public PeopleTag(string name, FaceRect rectangle, string eMailDigest) : this(name)
        {
            Rectangle = rectangle;
            EmailDigest = eMailDigest;
        }

        public PeopleTag(string name, FaceRect rectangle, string eMailDigest, string liveId) : this(name)
        {
            Rectangle = rectangle;
            EmailDigest = eMailDigest;
            LiveCID = liveId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PeopleTag);
        }

        public bool Equals(PeopleTag? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(Name, other.Name)
                && Equals(Rectangle, other.Rectangle)
                && Equals(EmailDigest, other.EmailDigest)
                && Equals(LiveCID, other.LiveCID);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Rectangle, EmailDigest, LiveCID);
        }

        public static bool operator ==(PeopleTag? obj1, PeopleTag? obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(PeopleTag? obj1, PeopleTag? obj2)
        {
            return !Equals(obj1, obj2);
        }

    }

}
