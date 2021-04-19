using System;
using System.Collections.Generic;

namespace Codefire.ExpressionEvaluator.Expressions
{
    public class NegateExpression : Expression
    {
        public NegateExpression(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override decimal? Invoke(EvaluatorContext context)
        {
            var value = Expression.Invoke(context);

            return value * -1;
        }

        public override IEnumerable<Expression> Visit()
        {
            yield return Expression;
        }

    }
}