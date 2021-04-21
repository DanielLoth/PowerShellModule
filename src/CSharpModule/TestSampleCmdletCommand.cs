using System.Management.Automation;
using System.Threading;

namespace CSharpModule
{
    [Cmdlet(VerbsDiagnostic.Test,"SampleCmdlet")]
    [OutputType(typeof(FavoriteStuff))]
    public class TestSampleCmdletCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int FavoriteNumber { get; set; }

        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        [ValidateSet("Cat", "Dog", "Horse")]
        public string FavoritePet { get; set; } = "Dog";

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            var progress = new ProgressRecord(1, "Testing", "Description")
            {
                PercentComplete = 0
            };

            WriteProgress(progress);

            WriteVerbose("Begin!");
            //Thread.Sleep(100);
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteObject(new FavoriteStuff {
                FavoriteNumber = FavoriteNumber,
                FavoritePet = FavoritePet
            });

            var progress = new ProgressRecord(1, "Testing", "Description")
            {
                PercentComplete = 50
            };

            WriteProgress(progress);

            //Thread.Sleep(100);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            var progress = new ProgressRecord(1, "Testing", "Description")
            {
                PercentComplete = 100
            };

            WriteProgress(progress);

            WriteVerbose("End!");
        }
    }

    public class FavoriteStuff
    {
        public int FavoriteNumber { get; set; }
        public string FavoritePet { get; set; }
        public string Hi { get; set; } = "Hi :::)!";
    }
}
