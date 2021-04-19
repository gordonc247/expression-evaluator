namespace Codefire.ExpressionEvaluator
{
    public delegate decimal? FunctionDelegate(decimal?[] values);

    public class FunctionDefinition
    {
        public string Name { get; set; }
        public FunctionDelegate Definition { get; set; }
    }
}