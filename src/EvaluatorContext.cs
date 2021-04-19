using System.Collections.Generic;
using System.Linq;

namespace Codefire.ExpressionEvaluator
{
    public class EvaluatorContext
    {
        public EvaluatorContext(IEnumerable<FunctionDefinition> functions, IEnumerable<Parameter> parameters)
        {
            Functions = functions.ToList();
            Parameters = parameters.ToList();
        }

        public List<FunctionDefinition> Functions { get; }
        public List<Parameter> Parameters { get; }

        public FunctionDelegate GetFunction(string name)
        {
            var f = Functions.FirstOrDefault(x => x.Name == name);

            return f?.Definition;
        }

        public decimal? GetParameter(string name)
        {
            var p = Parameters.FirstOrDefault(x => x.Name == name);

            return p?.Value;
        }
    }
}