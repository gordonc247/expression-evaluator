using System.Collections.Generic;

namespace Codefire.ExpressionEvaluator.Expressions
{
    public abstract class BinaryExpression : Expression
    {
        protected BinaryExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public Expression Left { get; }
        public Expression Right { get; }

        public abstract override decimal? Invoke(EvaluatorContext context);

        public override IEnumerable<Expression> Visit()
        {
            yield return Left;
            yield return Right;
        }
    }
}