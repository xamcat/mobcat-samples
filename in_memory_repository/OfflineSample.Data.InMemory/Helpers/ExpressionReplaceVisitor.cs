using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OfflineSample.Data.InMemory.Helpers
{
    internal class ExpressionReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ExpressionReplaceVisitor(Expression from, Expression to)
        {
            this._from = from;
            this._to = to;
        }

        public override Expression Visit(Expression node)
        {
            Expression exp = default(Expression);
            try
            {
                if (node == _from)
                {
                    return _to;
                }
                return base.Visit(node);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return exp;
        }
    }
}