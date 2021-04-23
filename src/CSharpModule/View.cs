﻿using System;
using System.Collections.Generic;

namespace CSharpModule
{
    public sealed class View : IComparable<View>, IEquatable<View>, IHasColumns
    {
        private readonly List<Column> viewColumns = new List<Column>();

        public int ObjectId { get; }
        public string SchemaName { get; }
        public string ViewName { get; }
        public string TypeCode { get; }
        public string TypeDescription { get; }

        public (string, string) Key => (SchemaName, ViewName);
        public string FullName => ObjectName.GetFullName(SchemaName, ViewName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, ViewName);

        public IList<Column> Columns => viewColumns;

        public void AddColumns(IEnumerable<Column> columns)
        {
            viewColumns.AddRange(columns);
            viewColumns.Sort();
        }

        public int CompareTo(View other) => Key.CompareTo(other.Key);
        public bool Equals(View other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is View other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}