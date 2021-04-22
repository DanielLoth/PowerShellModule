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

            var columnQuery = @"
                select
                    o.object_id as ParentObjectId
                    , object_schema_name(o.object_id) as ParentSchemaName
                    , o.name as ParentName
                    , o.type as ParentTypeCode
                    , o.type_desc as ParentTypeDescription
                    , c.column_id as ColumnId
                    , c.name as ColumnName
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
                inner join sys.tables o on o.object_id = c.object_id
                where o.is_ms_shipped = 0
                order by object_schema_name(o.object_id), o.name, c.column_id;";

            var columnMap = connection.Query<Column>(columnQuery)
                                      .AsList()
                                      .GroupBy(column => column.ParentObjectId)
                                      .ToDictionary(group => group.Key, group => group.AsList());

            foreach (var table in tableMap.Values)
            {
                if (columnMap.TryGetValue(table.ObjectId, out var columns))
                {
                    table.AddColumns(columns);
                }
            }

            WriteObject(new DataModel(tableMap.Values));

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
