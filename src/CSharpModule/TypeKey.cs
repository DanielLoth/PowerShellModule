namespace CSharpModule
{
    public static class TypeKey
    {
        public static (int, int) GetTypeKey(IHasType hasType) => (hasType.SystemTypeId, hasType.UserTypeId);
    }
}
