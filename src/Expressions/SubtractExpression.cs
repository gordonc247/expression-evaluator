namespace Codefire.ExpressionEvaluator.Expressions
{
    public class SubtractExpression : BinaryExpression
    {
        public SubtractExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public override decimal? Invoke(EvaluatorContext context)
        {
            var leftValue = Left.Invoke(context);
            var rightValue = Right.Invoke(context);

            return leftValue - rightValue;
        }
    }
}