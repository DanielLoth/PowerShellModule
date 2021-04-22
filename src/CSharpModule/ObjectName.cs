using System.Linq;

namespace CSharpModule
{
    public static class ObjectName
    {
        public static string GetFullName(params string[] nameParts) => string.Join('.', nameParts);

        public static string GetFullNameQuoted(params string[] nameParts) => string.Join('.', nameParts.Select(x => GetQuotedName(x)));

        private static string GetQuotedName(string namePart) => $"[{namePart}]";
    }
}
