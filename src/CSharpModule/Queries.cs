using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace CSharpModule
{
    public sealed class Queries
    {
        private readonly SqlConnection connection;

        public Queries(string serverInstance, string databaseName)
        {
            var builder = GetConnectionStringBuilder(serverInstance, databaseName);

            this.connection = new SqlConnection(builder.ConnectionString);
        }

        public void Open() => connection.Open();
        
        public void Close()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public SqlMetadata GetMetadata()
        {
            var metadata = new SqlMetadata
            {
                Columns = GetColumns(),
                Procedures = GetProcedures(),
                Types = GetTypes(),
                Tables = GetTables(),
                TableTypes = GetTableTypes(),
                Views = GetViews()
            };

            return metadata;
        }

        private List<Column> GetColumns()
        {
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

            return Get<Column>(columnQuery);
        }

        private List<Procedure> GetProcedures()
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

            return Get<Procedure>(procedureQuery);
        }

        private List<SqlType> GetTypes()
        {
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

            return Get<SqlType>(typeQuery);
        }

        private List<Table> GetTables()
        {
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

            return Get<Table>(tableQuery);
        }

        private List<TableType> GetTableTypes()
        {
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

            return Get<TableType>(tableTypeQuery);
        }

        private List<View> GetViews()
        {
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

            return Get<View>(viewQuery);
        }

        private List<T> Get<T>(string query) => connection.Query<T>(query).AsList();

        private SqlConnectionStringBuilder GetConnectionStringBuilder(string serverInstance, string databaseName)
        {
            var builder = new SqlConnectionStringBuilder();

            builder.ApplicationIntent = ApplicationIntent.ReadOnly;
            builder.ApplicationName = "PowerShellModule";
            //builder.CommandTimeout = 30;
            builder.ConnectTimeout = 20;
            builder.DataSource = serverInstance;
            builder.InitialCatalog = databaseName;
            builder.IntegratedSecurity = true;
            builder.MultipleActiveResultSets = false;
            builder.Pooling = true;

            return builder;
        }
    }
}
