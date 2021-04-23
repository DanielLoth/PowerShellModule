using System.Collections.Generic;
using System.Linq;

namespace CSharpModule
{
    public sealed class DataModelBuilder
    {
        private readonly Dictionary<int, List<Column>> columnMap;
        private readonly Dictionary<int, Procedure> procedureMap;
        private readonly Dictionary<int, Table> tableMap;
        private readonly Dictionary<(int, int), SqlType> typeMap;
        private readonly Dictionary<(int, int), TableType> tableTypeMap;
        private readonly Dictionary<int, View> viewMap;

        public DataModelBuilder(SqlMetadata metadata)
        {
            this.columnMap = metadata.Columns.GroupBy(column => column.ParentObjectId).ToDictionary(group => group.Key, group => group.AsList());
            this.procedureMap = metadata.Procedures.ToDictionary(procedure => procedure.ObjectId);
            this.tableMap = metadata.Tables.ToDictionary(table => table.ObjectId);
            this.typeMap = metadata.Types.ToDictionary(type => TypeKey.GetTypeKey(type));
            this.tableTypeMap = metadata.TableTypes.ToDictionary(type => TypeKey.GetTypeKey(type));
            this.viewMap = metadata.Views.ToDictionary(view => view.ObjectId);

            LinkColumns();
            LinkProcedures();
            LinkTables();
            LinkTypesToTableTypes();
            LinkViews();
        }

        public DataModel ToModel()
        {
            return new DataModel(procedureMap.Values, tableMap.Values, viewMap.Values, typeMap.Values);
        }

        private void LinkColumns()
        {
            LinkColumnsToTypes();
        }

        private void LinkColumnsToTypes()
        {
            foreach (var column in columnMap.Values.SelectMany(c => c.AsList()))
            {
                var typeKey = TypeKey.GetTypeKey(column);

                if (typeMap.TryGetValue(typeKey, out var type))
                {
                    column.Type = type;
                }
            }
        }

        private void LinkProcedures()
        {

        }

        private void LinkTables()
        {
            LinkTablesToColumns();
        }

        private void LinkTablesToColumns()
        {
            foreach (var table in tableMap.Values)
            {
                if (columnMap.TryGetValue(table.ObjectId, out var columns))
                {
                    table.AddColumns(columns);
                }
            }
        }

        private void LinkTypesToTableTypes()
        {
            foreach (var type in typeMap.Values)
            {
                var typeKey = TypeKey.GetTypeKey(type);

                if (type.IsTableType && tableTypeMap.TryGetValue(typeKey, out var tableType))
                {
                    type.TableType = tableType;
                    tableType.Type = type;
                }
            }
        }

        private void LinkViews()
        {
            LinkViewsToColumns();
        }

        private void LinkViewsToColumns()
        {
            foreach (var view in viewMap.Values)
            {
                if (columnMap.TryGetValue(view.ObjectId, out var columns))
                {
                    view.AddColumns(columns);
                }
            }
        }
    }
}
