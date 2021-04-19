using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Codefire.ExpressionEvaluator.Expressions;
using Codefire.Expressions;

namespace Codefire.ExpressionEvaluator
{
    public class Parser
    {
        private static readonly char[] Delimiters = { '(', ')', '.', ',', '=', '<', '>', '!', '&', '|', '*', '/', '%', '+', '-', '^' };
        private static readonly string[] MultOperators = { "*", "/", "%", "^" };
        private static readonly string[] AddOperators = { "+", "-" };

        public Expression Parse(string s)
        {
            var parseResult = ParseAdditive(s, 0);
            var lastLexem = Read(s, parseResult.End);
            if (lastLexem.NodeType != NodeType.End)
                throw new ParseException(s, parseResult.End, "Invalid expression");

            return parseResult.Expr;
        }

        protected Node Read(string s, int start)
        {
            var node = new Node
            {
                NodeType = NodeType.Unknown,
                Expression = s,
                Start = start,
                End = start
            };

            while (node.End < s.Length)
            {
                if (Array.IndexOf(Delimiters, s[node.End]) >= 0)
                {
                    if (node.NodeType == NodeType.Unknown)
                    {
                        node.End++;
                        node.NodeType = NodeType.Delimiter;
                        return node;
                    }

                    if (node.NodeType != NodeType.Constant || s[node.End] != '.')
                        return node; // stop
                }
                else if (Char.IsSeparator(s[node.End]))
                {
                    if (node.NodeType != NodeType.Unknown)
                        return node; // stop
                }
                else if (Char.IsLetter(s[node.End]))
                {
                    if (node.NodeType == NodeType.Unknown)
                        node.NodeType = NodeType.Name;
                }
                else if (Char.IsDigit(s[node.End]))
                {
                    if (node.NodeType == NodeType.Unknown)
                        node.NodeType = NodeType.Constant;
                }
                else if (Char.IsControl(s[node.End]) && node.NodeType != NodeType.Unknown)
                    return node;

                // goto next char
                node.End++;
            }

            if (node.NodeType == NodeType.Unknown)
            {
                node.NodeType = NodeType.End;
                return node;
            }

            return node;
        }

        protected ParseResult ParseAdditive(string expr, int start)
        {
            var firstOp = ParseMultiplicative(expr, start);

            do
            {
                var opNode = Read(expr, firstOp.End);
                if (opNode.NodeType == NodeType.Delimiter && AddOperators.Contains(opNode.GetValue()))
                {
                    var secondOp = ParseMultiplicative(expr, opNode.End);
                    var res = new ParseResult { End = secondOp.End };
                    switch (opNode.GetValue())
                    {
                        case "+":
                            res.Expr = new AddExpression(firstOp.Expr, secondOp.Expr);
                            break;
                        case "-":
                            res.Expr = new SubtractExpression(firstOp.Expr, secondOp.Expr);
                            break;
                    }
                    firstOp = res;
                    continue;
                }
                break;
            } while (true);

            return firstOp;
        }

        protected ParseResult ParseMultiplicative(string expr, int start)
        {
            var firstOp = ParseUnary(expr, start);

            do
            {
                var opLexem = Read(expr, firstOp.End);
                if (opLexem.NodeType == NodeType.Delimiter && MultOperators.Contains(opLexem.GetValue()))
                {
                    var secondOp = ParseUnary(expr, opLexem.End);
                    var res = new ParseResult { End = secondOp.End };
                    switch (opLexem.GetValue())
                    {
                        case "*":
                            res.Expr = new MultiplyExpression(firstOp.Expr, secondOp.Expr);
                            break;
                        case "/":
                            res.Expr = new DivideExpression(firstOp.Expr, secondOp.Expr);
                            break;
                        case "^":
                            res.Expr = new PowerExpression(firstOp.Expr, secondOp.Expr);
                            break;
                    }
                    firstOp = res;
                    continue;
                }
                break;
            } while (true);

            return firstOp;
        }

        protected ParseResult ParseUnary(string expr, int start)
        {
            var opLexem = Read(expr, start);

            if (opLexem.NodeType == NodeType.Delimiter)
            {
                switch (opLexem.GetValue())
                {
                    case "-":
                    {
                        var operand = ParsePrimary(expr, opLexem.End);
                        operand.Expr = new NegateExpression(operand.Expr);
                        return operand;
                    }
                }
            }

            return ParsePrimary(expr, start);
        }

        protected int ReadCallArguments(string expr, int start, List<Expression> args)
        {
            var end = start;
            do
            {
                var lexem = Read(expr, end);

                if (IsCloseBracket(lexem))
                {
                    return lexem.End;
                }

                if (IsComma(lexem))
                {
                    if (args.Count == 0)
                    {
                        throw new ParseException(expr, lexem.Start, "Expected method call parameter");
                    }
                    end = lexem.End;
                }
                
                // read parameter
                var paramExpr = ParseAdditive(expr, end);
                args.Add(paramExpr.Expr);
                end = paramExpr.End;
            } while (true);
        }

        protected ParseResult ParsePrimary(string expr, int start)
        {
            var node = Read(expr, start);

            if (IsOpenBracket(node))
            {
                var groupRes = ParseAdditive(expr, node.End);

                var endNode = Read(expr, groupRes.End);
                if (!IsCloseBracket(endNode))
                    throw new ParseException(expr, endNode.Start, "Expected ')'");

                groupRes.End = endNode.End;

                return groupRes;
            }

            if (node.NodeType == NodeType.Constant)
            {
                if (!Decimal.TryParse(node.GetValue(), NumberStyles.Any, CultureInfo.InvariantCulture, out var numberConst))
                {
                    throw new Exception(String.Format("Invalid number: {0}", node.GetValue()));
                }

                return new ParseResult
                {
                    End = node.End,
                    Expr = new ConstantExpression(numberConst)
                };
            }

            if (node.NodeType == NodeType.Name)
            {
                var val = node.GetValue();
                var next = Read(expr, node.End);
                if (IsOpenBracket(next))
                {
                    var methodParams = new List<Expression>();
                    var paramsEnd = ReadCallArguments(expr, next.End, methodParams);
                    return new ParseResult
                    {
                        End = paramsEnd,
                        Expr = new MethodExpression(val, methodParams)
                    };
                }

                return new ParseResult
                {
                    End = node.End,
                    Expr = new ParameterExpression(val)
                };
            }

            throw new ParseException(expr, start, "Expected value");
        }

        private bool IsOpenBracket(Node node)
        {
            return node.NodeType == NodeType.Delimiter && node.GetValue() == "(";
        }

        private bool IsComma(Node node)
        {
            return node.NodeType == NodeType.Delimiter && node.GetValue() == ",";
        }

        private bool IsCloseBracket(Node node)
        {
            return node.NodeType == NodeType.Delimiter && node.GetValue() == ")";
        }
    }
}