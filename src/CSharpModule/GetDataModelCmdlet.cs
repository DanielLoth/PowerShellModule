using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using Dapper;

namespace CSharpModule
{
    [Cmdlet(VerbsCommon.Get, "DataModel")]
    [OutputType(typeof(DataModel))]
    public class GetDataModelCmdlet : PSCmdlet
    {
        private SqlConnection connection;

        [Parameter(Mandatory = false)]
        public string ServerInstance { get; set; } = ".";

        [Parameter(Mandatory = true)]
        public string DatabaseName { get; set; }

        protected override void BeginProcessing()
        {
            var builder = GetConnectionStringBuilder();

            connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            var procedureQuery = @$"
                select
                    object_schema_name(p.object_id) as {nameof(Procedure.SchemaName)}
                    , p.object_id as {nameof(Procedure.ObjectId)}
                    , p.name as {nameof(Procedure.ProcedureName)}
                    , p.type as {nameof(Procedure.ObjectTypeCode)}
                    , p.type_desc as {nameof(Procedure.ObjectTypeDescription)}
                from sys.procedures p
                where p.is_ms_shipped = 0
                order by object_schema_name(p.object_id), p.name;";

            var procedureMap = connection.Query<Procedure>(procedureQuery)
                                         .AsList()
                                         .ToDictionary(procedure => procedure.ObjectId);

            var tableQuery = @$"
                select
                    object_schema_name(t.object_id) as {nameof(Table.SchemaName)}
                    , t.object_id as {nameof(Table.ObjectId)}
                    , t.name as {nameof(Table.TableName)}
                    , t.type as {nameof(Table.ObjectTypeCode)}
                    , t.type_desc as {nameof(Table.ObjectTypeDescription)}
                from sys.tables t
                where t.is_ms_shipped = 0
                order by object_schema_name(t.object_id), t.name;";

            var tableMap = connection.Query<Table>(tableQuery)
                                     .AsList()
                                     .ToDictionary(table => table.ObjectId);

            var typeQuery = @$"
                select
                    schema_name(t.schema_id) as {nameof(SqlType.SchemaName)}
                    , t.name as {nameof(SqlType.TypeName)}
                    , t.system_type_id as {nameof(SqlType.SystemTypeId)}
                    , t.user_type_id as {nameof(SqlType.UserTypeId)}
                    , t.max_length as {nameof(SqlType.MaxLength)}
                    , t.precision as {nameof(SqlType.Precision)}
                    , t.scale as {nameof(SqlType.Scale)}
                    , t.collation_name as {nameof(SqlType.CollationName)}
                    , t.is_nullable as {nameof(SqlType.IsNullable)}
                    , t.is_user_defined as {nameof(SqlType.IsUserDefined)}
                    , t.is_assembly_type as {nameof(SqlType.IsAssemblyType)}
                    , t.is_table_type as {nameof(SqlType.IsTableType)}
                from sys.types t
                order by schema_name(t.schema_id), t.name;";

            var typeMap = connection.Query<SqlType>(typeQuery)
                                    .AsList()
                                    .ToDictionary(type => TypeKey.GetTypeKey(type));

            var tableTypeQuery = @$"
                select
                    schema_name(t.schema_id) as {nameof(TableType.SchemaName)}
                    , t.type_table_object_id as {nameof(TableType.ObjectId)}
                    , t.name as {nameof(TableType.TableTypeName)}
                    , t.system_type_id as {nameof(TableType.SystemTypeId)}
                    , t.user_type_id as {nameof(TableType.UserTypeId)}
                    , t.max_length as {nameof(TableType.MaxLength)}
                    , t.precision as {nameof(TableType.Precision)}
                    , t.scale as {nameof(TableType.Scale)}
                    , t.collation_name as {nameof(TableType.CollationName)}
                    , t.is_nullable as {nameof(TableType.IsNullable)}
                    , t.is_user_defined as {nameof(TableType.IsUserDefined)}
                    , t.is_assembly_type as {nameof(TableType.IsAssemblyType)}
                    , t.is_memory_optimized as {nameof(TableType.IsMemoryOptimized)}
                from sys.table_types t
                order by schema_name(t.schema_id), t.name;";

            var tableTypeMap = connection.Query<TableType>(tableTypeQuery)
                                         .AsList()
                                         .ToDictionary(type => TypeKey.GetTypeKey(type));

            var viewQuery = @$"
                select
                    object_schema_name(v.object_id) as {nameof(View.SchemaName)}
                    , v.object_id as {nameof(View.ObjectId)}
                    , v.name as {nameof(View.ViewName)}
                    , v.type as {nameof(View.ObjectTypeCode)}
                    , v.type_desc as {nameof(View.ObjectTypeDescription)}
                from sys.views v
                where v.is_ms_shipped = 0
                order by object_schema_name(v.object_id), v.name;";

            var viewMap = connection.Query<View>(viewQuery)
                                    .AsList()
                                    .ToDictionary(view => view.ObjectId);

            var columnQuery = @$"
                select
                    o.object_id as {nameof(Column.ParentObjectId)}
                    , object_schema_name(o.object_id) as {nameof(Column.ParentSchemaName)}
                    , o.name as {nameof(Column.ParentName)}
                    , o.type as {nameof(Column.ParentObjectTypeCode)}
                    , o.type_desc as {nameof(Column.ParentObjectTypeDescription)}
                    , c.column_id as {nameof(Column.ColumnId)}
                    , c.name as {nameof(Column.ColumnName)}
                    , c.system_type_id as {nameof(Column.SystemTypeId)}
                    , c.user_type_id as {nameof(Column.UserTypeId)}
                    , c.max_length as {nameof(Column.MaxLength)}
                    , c.precision as {nameof(Column.Precision)}
                    , c.scale as {nameof(Column.Scale)}
                    , c.collation_name as {nameof(Column.CollationName)}
                    , columnproperty(c.object_id, c.name, 'charmaxlen') as {nameof(Column.MaxCharacterLength)}
                    , c.is_nullable as {nameof(Column.IsNullable)}
                    , c.is_identity as {nameof(Column.IsIdentity)}
                    , c.is_computed as {nameof(Column.IsComputed)}
                from sys.columns c
                inner join sys.objects o on o.object_id = c.object_id
                where o.is_ms_shipped = 0
                order by object_schema_name(o.object_id), o.name, c.column_id;";

            var columnMap = connection.Query<Column>(columnQuery)
                                      .AsList()
                                      .GroupBy(column => column.ParentObjectId)
                                      .ToDictionary(group => group.Key, group => group.AsList());

            foreach (var type in typeMap.Values)
            {
                var typeKey = TypeKey.GetTypeKey(type);

                if (type.IsTableType && tableTypeMap.TryGetValue(typeKey, out var tableType))
                {
                    type.TableType = tableType;
                    tableType.Type = type;
                }
            }

            foreach (var column in columnMap.Values.SelectMany(c => c.AsList()))
            {
                var typeKey = TypeKey.GetTypeKey(column);

                if (typeMap.TryGetValue(typeKey, out var type))
                {
                    column.Type = type;
                }
            }

            foreach (var table in tableMap.Values)
            {
                if (columnMap.TryGetValue(table.ObjectId, out var columns))
                {
                    table.AddColumns(columns);
                }
            }

            foreach (var view in viewMap.Values)
            {
                if (columnMap.TryGetValue(view.ObjectId, out var columns))
                {
                    view.AddColumns(columns);
                }
            }

            WriteObject(new DataModel(procedureMap.Values, tableMap.Values, viewMap.Values, typeMap.Values));

            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            base.EndProcessing();
        }

        private SqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            var builder = new SqlConnectionStringBuilder();

            builder.ApplicationIntent = ApplicationIntent.ReadOnly;
            builder.ApplicationName = "PowerShellModule";
            //builder.CommandTimeout = 30;
            builder.ConnectTimeout = 20;
            builder.DataSource = ServerInstance;
            builder.InitialCatalog = DatabaseName;
            builder.IntegratedSecurity = true;
            builder.MultipleActiveResultSets = false;
            builder.Pooling = true;

            return builder;
        }
    }
}
