using System;
using System.Collections.Generic;

namespace CSharpModule
{
    public sealed class Table : IComparable<Table>, IEquatable<Table>, IHasColumns
    {
        private readonly List<Column> tableColumns = new List<Column>();

        public int ObjectId { get; }
        public string SchemaName { get; }
        public string TableName { get; }
        public string TypeCode { get; }
        public string TypeDescription { get; }

        public (string, string) Key => (SchemaName, TableName);
        public string FullName => ObjectName.GetFullName(SchemaName, TableName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, TableName);

        public IList<Column> Columns => tableColumns;

        public void AddColumns(IEnumerable<Column> columns)
        {
            tableColumns.AddRange(columns);
            tableColumns.Sort();
        }

        public int CompareTo(Table other) => Key.CompareTo(other.Key);
        public bool Equals(Table other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is Table other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
