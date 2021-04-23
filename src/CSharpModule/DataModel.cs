using System.Collections.Generic;
using System.Linq;

namespace CSharpModule
{
    public sealed class DataModel
    {
        public IList<Procedure> Procedures { get; }
        public IList<Table> Tables { get; }
        public IList<View> Views { get; }
        public IList<SqlType> Types { get; }

        public DataModel(IEnumerable<Procedure> procedures,
                         IEnumerable<Table> tables,
                         IEnumerable<View> views,
                         IEnumerable<SqlType> types)
        {
            Procedures = procedures.ToList();
            Tables = tables.ToList();
            Views = views.ToList();
            Types = types.ToList();
        }
    }
}
