using System.Collections.Generic;

namespace CSharpModule
{
    public interface IHasColumns
    {
        IList<Column> Columns { get; }
        void AddColumns(IEnumerable<Column> columns);
    }
}
