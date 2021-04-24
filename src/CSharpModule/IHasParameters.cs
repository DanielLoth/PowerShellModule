using System.Collections.Generic;

namespace CSharpModule
{
    public interface IHasParameters
    {
        IList<Parameter> Parameters { get; }
        void AddParameters(IEnumerable<Parameter> parameters);
    }
}
