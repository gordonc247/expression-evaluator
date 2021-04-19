namespace Codefire.ExpressionEvaluator.Expressions
{
    public class ConstantExpression : Expression
    {
        public ConstantExpression(decimal? value)
        {
            Value = value;
        }

        public decimal? Value { get; }

        public override decimal? Invoke(EvaluatorContext context)
        {
            return Value;
        }
    }
}