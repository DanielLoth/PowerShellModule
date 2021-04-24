using System.Collections.Generic;
using System.Linq;

namespace CSharpModule
{
    public sealed class DataModel
    {
        public IList<Procedure> Procedures { get; }
        public IList<Table> Tables { get; }
        public IList<TableType> TableTypes { get; }
        public IList<View> Views { get; }
        public IList<SqlType> Types { get; }

        public DataModel(IEnumerable<Procedure> procedures,
                         IEnumerable<Table> tables,
                         IEnumerable<TableType> tableTypes,
                         IEnumerable<View> views,
                         IEnumerable<SqlType> types)
        {
            Procedures = procedures.ToList();
            Tables = tables.ToList();
            TableTypes = tableTypes.ToList();
            Views = views.ToList();
            Types = types.ToList();
        }
    }
}
