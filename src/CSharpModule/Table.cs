using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace CSharpModule
{
    public sealed class Table : IComparable<Table>, IEquatable<Table>, IHasColumns, IHasObjectTypeCode
    {
        private readonly List<CheckConstraint> tableCheckConstraints = new List<CheckConstraint>();
        private readonly List<Column> tableColumns = new List<Column>();
        private readonly List<KeyConstraint> tableKeyConstraints = new List<KeyConstraint>();

        public int ObjectId { get; }
        public string SchemaName { get; }
        public string TableName { get; }
        public string ObjectTypeCode { get; }
        public string ObjectTypeDescription { get; }

        [Hidden]
        public (string, string) Key => (SchemaName, TableName);

        public string FullName => ObjectName.GetFullName(SchemaName, TableName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, TableName);

        public IList<CheckConstraint> CheckConstraints => tableCheckConstraints;
        public IList<Column> Columns => tableColumns;
        public IList<KeyConstraint> KeyConstraints => tableKeyConstraints;

        public void AddCheckConstraints(IEnumerable<CheckConstraint> checkConstraints) => tableCheckConstraints.AddRangeAndSort(checkConstraints);
        public void AddColumns(IEnumerable<Column> columns) => tableColumns.AddRangeAndSort(columns);
        public void AddKeyConstraints(IEnumerable<KeyConstraint> keyConstraints) => tableKeyConstraints.AddRangeAndSort(keyConstraints);

        public int CompareTo(Table other) => Key.CompareTo(other.Key);
        public bool Equals(Table other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is Table other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
