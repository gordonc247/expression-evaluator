namespace Codefire.ExpressionEvaluator.Expressions
{
    public class DivideExpression : BinaryExpression
    {
        public DivideExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public override decimal? Invoke(EvaluatorContext context)
        {
            var leftValue = Left.Invoke(context);
            var rightValue = Right.Invoke(context);

            // Don't throw a divide by zero exception, return 0
            // Buuuutttt, if the left value is null then let's return null rather.
            if (rightValue == 0) return leftValue == null ? (decimal?)null : 0M;

            return leftValue / rightValue;
        }
    }
}