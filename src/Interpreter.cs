using System.Collections.Generic;
using System.Linq;
using Codefire.ExpressionEvaluator.Expressions;

namespace Codefire.ExpressionEvaluator
{
    public class Interpreter
    {
        private static readonly Dictionary<string, Expression> Cache = new Dictionary<string, Expression>();
        private static readonly object Lock = new object();

        public Interpreter()
        {
            Functions = new List<FunctionDefinition>();
        }

        private List<FunctionDefinition> Functions { get; }
        public bool UseCache { get; set; }

        public Expression Parse(string text)
        {
            Expression compiledExpression = null;
            if (UseCache)
            {
                lock (Lock)
                {
                    Cache.TryGetValue(text, out compiledExpression);
                }
            }

            if (compiledExpression == null)
            {
                var parser = new Parser();
                compiledExpression = parser.Parse(text);

                if (UseCache)
                {
                    lock (Lock)
                    {
                        Cache[text] = compiledExpression;
                    }
                }
            }

            return compiledExpression;
        }

        public decimal? Eval(string text, IEnumerable<Parameter> parameters)
        {
            var compiledExpression = Parse(text);

            var context = new EvaluatorContext(Functions, parameters);

            return compiledExpression.Invoke(context);
        }

        public IEnumerable<string> DetectIdentifiers(string text)
        {
            var expression = Parse(text);

            return Flatten(expression).OfType<ParameterExpression>()
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
        }

        public void RegisterFunction(string name, FunctionDelegate func)
        {
            Functions.Add(new FunctionDefinition {Name = name, Definition = func});
        }

        private IEnumerable<Expression> Flatten(Expression expression)
        {
            var results = new List<Expression>();

            var stack = new Stack<Expression>();
            stack.Push(expression);
            while (stack.Any())
            {
                var next = stack.Pop();

                results.Add(next);

                foreach (var child in next.Visit())
                    stack.Push(child);
            }

            return results;
        }
    }
}