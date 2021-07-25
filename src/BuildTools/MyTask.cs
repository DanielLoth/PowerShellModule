using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BuildTools
{
    public sealed class MyTask : Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "MyTask message");
            return true;
        }
    }
}
