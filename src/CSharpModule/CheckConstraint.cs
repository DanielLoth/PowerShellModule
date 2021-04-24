using System;
using System.Management.Automation;

namespace CSharpModule
{
    public sealed class CheckConstraint : IComparable<CheckConstraint>, IEquatable<CheckConstraint>, IHasObjectTypeCode, IHasParentObjectTypeCode
    {
        public int ParentObjectId { get; }
        public string ParentSchemaName { get; }
        public string ParentName { get; }
        public string ParentObjectTypeCode { get; }
        public string ParentObjectTypeDescription { get; }

        public int ObjectId { get; }
        public string ObjectTypeCode { get; }
        public string ObjectTypeDescription { get; }
        public string CheckConstraintName { get; }
        public bool IsDisabled { get; }
        public bool IsNotTrusted { get; }
        public bool IsSystemNamed { get; }
        public int ParentColumnId { get; }
        public string Definition { get; }

        [Hidden]
        public (string, string) Key => (CheckConstraintName, string.Empty);

        [Hidden]
        public (string, string, string) OrdinalKey => (ParentSchemaName, ParentName, CheckConstraintName);

        public int CompareTo(CheckConstraint other) => OrdinalKey.CompareTo(other.OrdinalKey);
        public bool Equals(CheckConstraint other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is CheckConstraint other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => CheckConstraintName;
    }
}
