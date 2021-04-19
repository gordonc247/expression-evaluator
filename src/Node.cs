namespace Codefire.Expressions
{
    public class Node
    {
        public NodeType NodeType;
        public int Start;
        public int End;
        public string Expression;
        internal string Value;

        public string GetValue()
        {
            if (Value == null)
            {
                Value = Expression.Substring(Start, End - Start).Trim();
            }

            return Value;
        }
    }
}