using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace CSharpModule
{
    public sealed class View : IComparable<View>, IEquatable<View>, IHasColumns, IHasObjectTypeCode
    {
        private readonly List<Column> viewColumns = new List<Column>();

        public int ObjectId { get; }
        public string SchemaName { get; }
        public string ViewName { get; }
        public string ObjectTypeCode { get; }
        public string ObjectTypeDescription { get; }

        [Hidden]
        public (string, string) Key => (SchemaName, ViewName);

        public string FullName => ObjectName.GetFullName(SchemaName, ViewName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, ViewName);

        public IList<Column> Columns => viewColumns;

        public void AddColumns(IEnumerable<Column> columns) => viewColumns.AddRangeAndSort(columns);

        public int CompareTo(View other) => Key.CompareTo(other.Key);
        public bool Equals(View other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is View other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
