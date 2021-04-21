using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using Dapper;

namespace CSharpModule
{

    [Cmdlet(VerbsCommon.Get, "SqlTables")]
    [OutputType(typeof(IEnumerable<Table>))]
    public class GetTablesCmdlet : PSCmdlet
    {
        private SqlConnection connection;

        [Parameter(Mandatory = false)]
        public string ServerInstance { get; set; } = ".";

        [Parameter(Mandatory = true)]
        public string DatabaseName { get; set; }

        [Parameter]
        public string SchemaName { get; set; }

        protected override void BeginProcessing()
        {
            var builder = GetConnectionStringBuilder();

            connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            var query = @"
                select
                    /* COLUMN ORDER MATTERS HERE DUE TO HOW DAPPER WORKS */

                    /* Select all columns to be bound to Table */
                    object_schema_name(t.object_id) as SchemaName
                    , t.object_id as TableObjectId
                    , t.name as TableName
                    , t.type as TypeCode
                    , t.type_desc as TypeDescription

                    /* AND THEN all columns to be bound to Column */
                    , c.object_id as ColumnObjectId
                    , c.column_id as ColumnId
                    , c.name as ColumnName
                    , c.user_type_id as UserTypeId
                    , c.max_length as MaxLength
                    , c.precision as Precision
                    , c.scale as Scale
                    , c.collation_name as CollationName
                    , c.is_nullable as IsNullable
                    , c.is_identity as IsIdentity
                    , c.is_computed as IsComputed
                from sys.tables t
                inner join sys.columns c
                    on t.object_id = c.object_id
                where t.is_ms_shipped = 0
                order by object_schema_name(t.object_id), t.name, c.column_id;";

            var tableMap = new Dictionary<int, Table>();

            _ = connection.Query<Table, Column, Table>(
                query,
                (table, column) =>
                {
                    tableMap.TryGetValue(table.TableObjectId, out var existingTable);

                    var newOrExistingTable = existingTable ?? table;
                    newOrExistingTable.Add(column);

                    tableMap.TryAdd(table.TableObjectId, newOrExistingTable);

                    return table;
                },
                splitOn: "ColumnObjectId").AsList();

            WriteObject(tableMap.Values);

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

    public sealed class Table
    {
        private readonly List<Column> columns = new List<Column>();

        public int TableObjectId { get; }
        public string SchemaName { get; }
        public string TableName { get; }
        public string TypeCode { get; }
        public string TypeDescription { get; }

        public IEnumerable<Column> Columns => columns.OrderBy(x => x.ColumnObjectId).ThenBy(x => x.ColumnId);

        public void Add(Column column) => columns.Add(column);
    }

    public sealed class Column : IEquatable<Column>
    {
        public int ColumnObjectId { get; }
        public int ColumnId { get; }
        public string ColumnName { get; }
        public int UserTypeId { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
        public bool IsNullable { get; }
        public bool IsIdentity { get; }
        public bool IsComputed { get; }

        public bool Equals(Column other) => ColumnObjectId == other.ColumnObjectId && ColumnId == other.ColumnId;
        public override bool Equals(object obj) => obj is Column other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(ColumnObjectId, ColumnId);
    }
}
