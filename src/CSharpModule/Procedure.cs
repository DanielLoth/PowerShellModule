using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace CSharpModule
{
    public sealed class Procedure : IComparable<Procedure>, IEquatable<Procedure>, IHasObjectTypeCode
    {
        private readonly List<Parameter> procedureParameters = new List<Parameter>();

        public int ObjectId { get; }
        public string SchemaName { get; }
        public string ProcedureName { get; }
        public string ObjectTypeCode { get; }
        public string ObjectTypeDescription { get; }

        [Hidden]
        public (string, string) Key => (SchemaName, ProcedureName);

        public string FullName => ObjectName.GetFullName(SchemaName, ProcedureName);
        public string FullNameQuoted => ObjectName.GetFullNameQuoted(SchemaName, ProcedureName);

        public IList<Parameter> Parameters => procedureParameters;
        public void AddParameters(IEnumerable<Parameter> parameters) => procedureParameters.AddRangeAndSort(parameters);

        public int CompareTo(Procedure other) => Key.CompareTo(other.Key);
        public bool Equals(Procedure other) => Key.Equals(other.Key);
        public override bool Equals(object obj) => obj is Procedure other && Equals(other);
        public override int GetHashCode() => Key.GetHashCode();
        public override string ToString() => FullName;
    }
}
