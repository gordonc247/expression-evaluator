using System;

namespace Codefire.ExpressionEvaluator
{
    public class ParseException : Exception
    {
        /// <summary>
        /// Lambda expression
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Parser position where syntax error occurs 
        /// </summary>
        public int Index { get; }

        public ParseException(string expr, int idx, string msg)
            : base( String.Format("{0} at {1}: {2}", msg, idx, expr) ) {
            Expression = expr;
            Index = idx;
        }
    }
}