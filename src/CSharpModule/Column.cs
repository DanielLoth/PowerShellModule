using System;
using System.Management.Automation;

namespace CSharpModule
{
    public sealed class Column : IComparable<Column>, IEquatable<Column>, IHasParentObjectTypeCode, IHasType
    {
        public int ParentObjectId { get; }
        public string ParentSchemaName { get; }
        public string ParentName { get; }
        public string ParentObjectTypeCode { get; }
        public string ParentObjectTypeDescription { get; }
        public int ColumnId { get; }
        public string ColumnName { get; }
        public int SystemTypeId { get; }
        public int UserTypeId { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
        public string CollationName { get; } = string.Empty;
        public int MaxCharacterLength { get; }
        public bool IsNullable { get; }
        public bool IsIdentity { get; }
        public bool IsComputed { get; }

        public SqlType Type { get; set; }

        [Hidden]
        public (string, string, string) Key => (ParentSchemaName, ParentName, ColumnName);

        public string FullName => ObjectName.GetFullName(ParentSchemaName, ParentName, ColumnName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(ParentSchemaName, ParentName, ColumnName);

        public int CompareTo(Column other) => Key.CompareTo(other.Key);
        public bool Equals(Column other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is Column other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
