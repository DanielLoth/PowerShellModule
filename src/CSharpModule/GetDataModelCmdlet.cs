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
            var procedureQuery = @"
                select
                    object_schema_name(p.object_id) as SchemaName
                    , p.object_id as ObjectId
                    , p.name as ProcedureName
                    , p.type as TypeCode
                    , p.type_desc as TypeDescription
                from sys.procedures p
                where p.is_ms_shipped = 0
                order by object_schema_name(p.object_id), p.name;";

            var procedureMap = connection.Query<Procedure>(procedureQuery)
                                         .AsList()
                                         .ToDictionary(procedure => procedure.ObjectId);

            var tableQuery = @"
                select
                    object_schema_name(t.object_id) as SchemaName 
                    , t.object_id as ObjectId
                    , t.name as TableName
                    , t.type as TypeCode
                    , t.type_desc as TypeDescription
                from sys.tables t
                where t.is_ms_shipped = 0
                order by object_schema_name(t.object_id), t.name;";

            var tableMap = connection.Query<Table>(tableQuery)
                                     .AsList()
                                     .ToDictionary(table => table.ObjectId);

            var typeQuery = @"
                select
                    schema_name(t.schema_id) as SchemaName 
                    , t.name as TypeName
                    , t.system_type_id as SystemTypeId
                    , t.user_type_id as UserTypeId
                    , t.max_length as MaxLength
                    , t.precision as Precision
                    , t.scale as Scale
                    , t.collation_name as CollationName
                    , t.is_nullable as IsNullable
                    , t.is_user_defined as IsUserDefined
                    , t.is_assembly_type as IsAssemblyType
                    , t.is_table_type as IsTableType
                from sys.types t
                order by schema_name(t.schema_id), t.name;";

            var typeMap = connection.Query<SqlType>(typeQuery)
                                    .AsList()
                                    .ToDictionary(type => TypeKey.GetTypeKey(type));

            var viewQuery = @"
                select
                    object_schema_name(v.object_id) as SchemaName
                    , v.object_id as ObjectId
                    , v.name as ViewName
                    , v.type as TypeCode
                    , v.type_desc as TypeDescription
                from sys.views v
                where v.is_ms_shipped = 0
                order by object_schema_name(v.object_id), v.name;";

            var viewMap = connection.Query<View>(viewQuery)
                                    .AsList()
                                    .ToDictionary(view => view.ObjectId);

            var columnQuery = @"
                select
                    o.object_id as ParentObjectId
                    , object_schema_name(o.object_id) as ParentSchemaName
                    , o.name as ParentName
                    , o.type as ParentTypeCode
                    , o.type_desc as ParentTypeDescription
                    , c.column_id as ColumnId
                    , c.name as ColumnName
                    , c.system_type_id as SystemTypeId
                    , c.user_type_id as UserTypeId
                    , c.max_length as MaxLength
                    , c.precision as Precision
                    , c.scale as Scale
                    , c.collation_name as CollationName
                    , columnproperty(c.object_id, c.name, 'charmaxlen') as MaxCharacterLength
                    , c.is_nullable as IsNullable
                    , c.is_identity as IsIdentity
                    , c.is_computed as IsComputed
                from sys.columns c
                inner join sys.objects o on o.object_id = c.object_id
                where o.is_ms_shipped = 0
                order by object_schema_name(o.object_id), o.name, c.column_id;";

            var columnMap = connection.Query<Column>(columnQuery)
                                      .AsList()
                                      .GroupBy(column => column.ParentObjectId)
                                      .ToDictionary(group => group.Key, group => group.AsList());

            foreach (var column in columnMap.Values.SelectMany(c => c.AsList()))
            {
                var typeKey = TypeKey.GetTypeKey(column);

                if (typeMap.TryGetValue(typeKey, out var type))
                {
                    column.SqlType = type;
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
