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
                CheckConstraints = GetCheckConstraints(),
                Columns = GetColumns(),
                KeyConstraints = GetKeyConstraints(),
                Parameters = GetParameters(),
                Procedures = GetProcedures(),
                Types = GetTypes(),
                Tables = GetTables(),
                TableTypes = GetTableTypes(),
                Views = GetViews()
            };

            return metadata;
        }

        private List<CheckConstraint> GetCheckConstraints()
        {
            var checkConstraintQuery = $@"
                select
                    o.object_id as {nameof(CheckConstraint.ParentObjectId)}
                    , schema_name(isnull(tt.schema_id, o.schema_id)) as {nameof(CheckConstraint.ParentSchemaName)}
                    , isnull(tt.name, o.name) as {nameof(CheckConstraint.ParentName)}
                    , o.type as {nameof(CheckConstraint.ParentObjectTypeCode)}
                    , o.type_desc as {nameof(CheckConstraint.ParentObjectTypeDescription)}

                    , cc.object_id as {nameof(CheckConstraint.ObjectId)}
                    , cc.type as {nameof(CheckConstraint.ObjectTypeCode)}
                    , cc.type_desc as {nameof(CheckConstraint.ObjectTypeDescription)}
                    , cc.name as {nameof(CheckConstraint.CheckConstraintName)}
                    , cc.is_disabled as {nameof(CheckConstraint.IsDisabled)}
                    , cc.is_not_trusted as {nameof(CheckConstraint.IsNotTrusted)}
                    , cc.parent_column_id as {nameof(CheckConstraint.ParentColumnId)}
                    , cc.definition as {nameof(CheckConstraint.Definition)}
                    , cc.is_system_named as {nameof(CheckConstraint.IsSystemNamed)}
                from sys.check_constraints cc
                inner join sys.objects o on cc.parent_object_id = o.object_id
                left join sys.table_types tt on cc.parent_object_id = tt.type_table_object_id
                where cc.is_ms_shipped = 0 or (cc.is_ms_shipped = 1 and tt.is_user_defined = 1)
                order by object_schema_name(o.object_id), cc.name;";

            return Get<CheckConstraint>(checkConstraintQuery);
        }

        private List<Column> GetColumns()
        {
            var columnQuery = @$"
                select
                    o.object_id as {nameof(Column.ParentObjectId)}
                    , schema_name(isnull(tt.schema_id, o.schema_id)) as {nameof(Column.ParentSchemaName)}
                    , isnull(tt.name, o.name) as {nameof(Column.ParentName)}
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
                    left join sys.table_types tt on tt.type_table_object_id = o.object_id and o.type = 'TT'
                where o.is_ms_shipped = 0 or (o.is_ms_shipped = 1 and tt.is_user_defined = 1)
                order by object_schema_name(o.object_id), o.name, c.column_id;";

            return Get<Column>(columnQuery);
        }

        private List<KeyConstraint> GetKeyConstraints()
        {
            var keyConstraintQuery = $@"
                select
                    o.object_id as [{nameof(KeyConstraint.ParentObjectId)}]
                    , schema_name(isnull(tt.schema_id, o.schema_id)) as [{nameof(KeyConstraint.ParentSchemaName)}]
                    , isnull(tt.name, o.name) as [{nameof(KeyConstraint.ParentName)}]
                    , o.type as [{nameof(KeyConstraint.ParentObjectTypeCode)}]
                    , o.type_desc as [{nameof(KeyConstraint.ParentObjectTypeDescription)}]
                    , kc.object_id as [{nameof(KeyConstraint.ObjectId)}]
                    , kc.type as [{nameof(KeyConstraint.ObjectTypeCode)}]
                    , kc.type_desc as [{nameof(KeyConstraint.ObjectTypeDescription)}]
                    , kc.name as [{nameof(KeyConstraint.KeyConstraintName)}]
                    , kc.unique_index_id as [{nameof(KeyConstraint.UniqueIndexId)}]
                    , kc.is_enforced as [{nameof(KeyConstraint.IsEnforced)}]
                    , kc.is_system_named as [{nameof(KeyConstraint.IsSystemNamed)}]
                from sys.key_constraints kc
                inner join sys.objects o on kc.parent_object_id = o.object_id
                left join sys.table_types tt on kc.parent_object_id = tt.type_table_object_id
                where kc.is_ms_shipped = 0 or (kc.is_ms_shipped = 1 and tt.is_user_defined = 1)
                order by schema_name(isnull(tt.schema_id, o.schema_id)), isnull(tt.name, o.name), kc.name;";

            return Get<KeyConstraint>(keyConstraintQuery);
        }

        private List<Parameter> GetParameters()
        {
            var parameterQuery = $@"
                select
                    o.object_id as {nameof(Parameter.ParentObjectId)}
                    , object_schema_name(o.object_id) as {nameof(Parameter.ParentSchemaName)}
                    , o.name as {nameof(Parameter.ParentName)}
                    , o.type as {nameof(Parameter.ParentObjectTypeCode)}
                    , o.type_desc as {nameof(Parameter.ParentObjectTypeDescription)}
                    , p.parameter_id as {nameof(Parameter.ParameterId)}
                    , p.name as {nameof(Parameter.ParameterName)}
                    , p.system_type_id as {nameof(Parameter.SystemTypeId)}
                    , p.user_type_id as {nameof(Parameter.UserTypeId)}
                    , p.max_length as {nameof(Parameter.MaxLength)}
                    , p.precision as {nameof(Parameter.Precision)}
                    , p.scale as {nameof(Parameter.Scale)}
                    , columnproperty(p.object_id, p.name, 'charmaxlen') as {nameof(Parameter.MaxCharacterLength)}
                    , p.is_nullable as {nameof(Parameter.IsNullable)}
                from sys.parameters p
                inner join sys.objects o on o.object_id = p.object_id
                order by object_schema_name(o.object_id), o.name, p.parameter_id;";

            return Get<Parameter>(parameterQuery);
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
