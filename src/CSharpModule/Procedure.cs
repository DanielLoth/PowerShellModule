using System;

namespace CSharpModule
{
    public sealed class Procedure : IComparable<Procedure>, IEquatable<Procedure>
    {
        public int ObjectId { get; }
        public string SchemaName { get; }
        public string ProcedureName { get; }
        public string TypeCode { get; }
        public string TypeDescription { get; }

        public (string, string) Key => (SchemaName, ProcedureName);
        public string FullName => ObjectName.GetFullName(SchemaName, ProcedureName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, ProcedureName);

        public int CompareTo(Procedure other) => Key.CompareTo(other.Key);
        public bool Equals(Procedure other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is Procedure other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
