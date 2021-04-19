using System;

namespace Codefire.ExpressionEvaluator
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string msg)
            : base(msg) {
        }
    }
}