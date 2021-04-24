namespace CSharpModule
{
    public sealed class CheckConstraint : IHasObjectTypeCode, IHasParentObjectTypeCode
    {
        public int ParentObjectId { get; }
        public string ParentSchemaName { get; }
        public string ParentName { get; }
        public string ParentObjectTypeCode { get; }
        public string ParentObjectTypeDescription { get; }

        public int ObjectId { get; }
        public string ObjectTypeCode { get; }
        public string ObjectTypeDescription { get; }
        public string CheckConstraintName { get; }
        public bool IsDisabled { get; }
        public bool IsNotTrusted { get; }
        public bool IsSystemNamed { get; }
        public int ParentColumnId { get; }
        public string Definition { get; }
    }
}
