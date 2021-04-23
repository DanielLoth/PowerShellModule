using System;

namespace CSharpModule
{
    public sealed class SqlType : IComparable<SqlType>, IEquatable<SqlType>, IHasType
    {
        public string SchemaName { get; }
        public string TypeName { get; }
        public int SystemTypeId { get; }
        public int UserTypeId { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
        public string CollationName { get; }
        public bool IsNullable { get; }
        public bool IsUserDefined { get; }
        public bool IsAssemblyType { get; }
        public bool IsTableType { get; }
        public TableType TableType { get; set; }

        public (string, string) Key => (SchemaName, TypeName);
        public string FullName => ObjectName.GetFullName(SchemaName, TypeName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, TypeName);

        public int CompareTo(SqlType other) => Key.CompareTo(other.Key);
        public bool Equals(SqlType other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is SqlType other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
