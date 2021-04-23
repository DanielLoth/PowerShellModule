using System.Collections.Generic;

namespace CSharpModule
{
    public sealed class SqlMetadata
    {
        public List<Column> Columns { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<Procedure> Procedures { get; set; }
        public List<SqlType> Types { get; set; }
        public List<Table> Tables { get; set; }
        public List<TableType> TableTypes { get; set; }
        public List<View> Views { get; set; }
    }
}
