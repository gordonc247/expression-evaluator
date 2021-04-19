using System;

namespace Codefire.ExpressionEvaluator.Expressions
{
    public class PowerExpression : BinaryExpression
    {
        public PowerExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public override decimal? Invoke(EvaluatorContext context)
        {
            var leftValue = Left.Invoke(context);
            var rightValue = Right.Invoke(context);

            if (leftValue == null || rightValue == null) return null;

            return (decimal)Math.Pow((double)leftValue.Value, (double)rightValue.Value);
        }
    }
}