﻿using System;

namespace CSharpModule
{
    public sealed class Parameter : IComparable<Parameter>, IEquatable<Parameter>, IHasParentObjectTypeCode, IHasType
    {
        public int ParentObjectId { get; }
        public string ParentSchemaName { get; }
        public string ParentName { get; }
        public string ParentObjectTypeCode { get; }
        public string ParentObjectTypeDescription { get; }
        public int ParameterId { get; }
        public string ParameterName { get; }
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

        public (string, string, string) Key => (ParentSchemaName, ParentName, ParameterName);
        public string FullName => ObjectName.GetFullName(ParentSchemaName, ParentName, ParameterName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(ParentSchemaName, ParentName, ParameterName);

        public int CompareTo(Parameter other) => Key.CompareTo(other.Key);
        public bool Equals(Parameter other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is Parameter other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}