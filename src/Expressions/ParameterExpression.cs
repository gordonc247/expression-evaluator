namespace Codefire.ExpressionEvaluator.Expressions
{
    public class ParameterExpression : Expression
    {
        public ParameterExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override decimal? Invoke(EvaluatorContext context)
        {
            return context.GetParameter(Name);
        }
    }
}