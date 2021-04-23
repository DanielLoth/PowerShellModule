using System.Management.Automation;

namespace CSharpModule
{
    [Cmdlet(VerbsCommon.Get, "DataModel")]
    [OutputType(typeof(DataModel))]
    public class GetDataModelCmdlet : PSCmdlet
    {
        private Queries queries;

        [Parameter(Mandatory = false)]
        public string ServerInstance { get; set; } = ".";

        [Parameter(Mandatory = true)]
        public string DatabaseName { get; set; }

        protected override void BeginProcessing()
        {
            queries = new Queries(ServerInstance, DatabaseName);
            queries.Open();

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            var builder = new DataModelBuilder(queries.GetMetadata());

            WriteObject(builder.ToModel());

            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            queries.Close();

            base.EndProcessing();
        }
    }
}
