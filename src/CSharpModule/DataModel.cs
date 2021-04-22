using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpModule
{
    public sealed class DataModel
    {
        public IList<Table> Tables { get; }

        public DataModel(IEnumerable<Table> tables)
        {
            Tables = tables.ToList();
        }
    }

    public sealed class Table : IComparable<Table>, IEquatable<Table>
    {
        private readonly List<Column> tableColumns = new List<Column>();

        public int ObjectId { get; }
        public string SchemaName { get; }
        public string TableName { get; }
        public string TypeCode { get; }
        public string TypeDescription { get; }

        public string FullName => ObjectName.GetFullName(SchemaName, TableName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, TableName);

        public IList<Column> Columns => tableColumns;

        public void Add(Column column) => tableColumns.Add(column);
        public void AddColumns(IEnumerable<Column> columns)
        {
            tableColumns.AddRange(columns);
            tableColumns.Sort();
        }

        public int CompareTo(Table other)
        {
            var comparisonThis = (SchemaName, TableName);
            var comparisonOther = (other.SchemaName, other.TableName);

            return comparisonThis.CompareTo(comparisonOther);
        }

        public bool Equals(Table other) => ObjectId == other.ObjectId;
        public override bool Equals(object obj) => obj is Table other && Equals(other);
        public override int GetHashCode() => ObjectId;
        public override string ToString() => $"{SchemaName}.{TableName}";
    }

    public sealed class Column : IComparable<Column>, IEquatable<Column>
    {
        public int ParentObjectId { get; }
        public string ParentSchemaName { get; }
        public string ParentName { get; }
        public string ParentTypeCode { get; }
        public string ParentTypeDescription { get; }
        public int ColumnId { get; }
        public string ColumnName { get; }
        public int UserTypeId { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
        public string CollationName { get; } = string.Empty;
        public int MaxCharacterLength { get; }
        public bool IsNullable { get; }
        public bool IsIdentity { get; }
        public bool IsComputed { get; }

        public string FullName => ObjectName.GetFullName(ParentSchemaName, ParentName, ColumnName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(ParentSchemaName, ParentName, ColumnName);

        public int CompareTo(Column other) => (ParentObjectId, ColumnId).CompareTo((other.ParentObjectId, other.ColumnId));
        public bool Equals(Column other) => ParentObjectId == other.ParentObjectId && ColumnId == other.ColumnId;
        public override bool Equals(object obj) => obj is Column other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(ParentObjectId, ColumnId);
        public override string ToString() => FullName;
    }
}
