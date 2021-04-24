using System;
using System.Management.Automation;

namespace CSharpModule
{
    public sealed class KeyConstraint : IComparable<KeyConstraint>, IEquatable<KeyConstraint>, IHasParentObjectTypeCode, IHasObjectTypeCode
    {
        public int ParentObjectId { get; }
        public string ParentSchemaName { get; }
        public string ParentName { get; }
        public string ParentObjectTypeCode { get; }
        public string ParentObjectTypeDescription { get; }

        public int ObjectId { get; }
        public string ObjectTypeCode { get; }
        public string ObjectTypeDescription { get; }
        public string KeyConstraintName { get; }
        public int UniqueIndexId { get; }
        public bool IsEnforced { get; }
        public bool IsSystemNamed { get; }

        [Hidden]
        public (string, string) Key => (KeyConstraintName, string.Empty);

        public (string, string, string) OrdinalKey => (ParentSchemaName, ParentName, KeyConstraintName);

        public int CompareTo(KeyConstraint other) => OrdinalKey.CompareTo(other.OrdinalKey);
        public bool Equals(KeyConstraint other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is KeyConstraint other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => KeyConstraintName;
    }
}
