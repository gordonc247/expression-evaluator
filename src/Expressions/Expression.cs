using System.Collections.Generic;
using System.Linq;

namespace Codefire.ExpressionEvaluator.Expressions
{
    public abstract class Expression
    {
        public abstract decimal? Invoke(EvaluatorContext context);

        public virtual IEnumerable<Expression> Visit()
        {
            return Enumerable.Empty<Expression>();
        }
    }
}