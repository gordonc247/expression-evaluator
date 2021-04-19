using System.Collections.Generic;

namespace Codefire.ExpressionEvaluator.Expressions
{
    public class MethodExpression : Expression
    {
        public MethodExpression(string name, IEnumerable<Expression> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public string Name { get; }
        public IEnumerable<Expression> Parameters { get; }

        public override decimal? Invoke(EvaluatorContext context)
        {
            var values = new List<decimal?>();
            foreach (var p in Parameters)
            {
                values.Add(p.Invoke(context));
            }

            var func = context.GetFunction(Name);

            return func?.Invoke(values.ToArray());
        }

        public override IEnumerable<Expression> Visit()
        {
            return Parameters;
        }
    }
}