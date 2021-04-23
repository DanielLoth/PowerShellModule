using System;
using System.Collections.Generic;

namespace CSharpModule
{
    public sealed class TableType : IComparable<TableType>, IEquatable<TableType>, IHasColumns, IHasType
    {
        private readonly List<Column> tableTypeColumns = new List<Column>();

        public string SchemaName { get; }
        public int ObjectId { get; }
        public string TableTypeName { get; }
        public int SystemTypeId { get; }
        public int UserTypeId { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
        public string CollationName { get; }
        public bool IsNullable { get; }
        public bool IsUserDefined { get; }
        public bool IsAssemblyType { get; }
        public bool IsMemoryOptimized { get; }
        public SqlType Type { get; set; }

        public (string, string) Key => (SchemaName, TableTypeName);
        public string FullName => ObjectName.GetFullName(SchemaName, TableTypeName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, TableTypeName);

        public IList<Column> Columns => tableTypeColumns;

        public void AddColumns(IEnumerable<Column> columns)
        {
            tableTypeColumns.AddRange(columns);
            tableTypeColumns.Sort();
        }

        public int CompareTo(TableType other) => Key.CompareTo(other.Key);
        public bool Equals(TableType other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is TableType other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
