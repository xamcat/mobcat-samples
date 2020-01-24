using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OfflineSample.Data.InMemory.Helpers
{
    public class ExpressionReturnTypeVisitor<TSource, TReturnValue> : ExpressionVisitor
    {
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(TReturnValue), typeof(bool));
            return Expression.Lambda(delegateType, Visit(node.Body), node.Parameters);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType == typeof(TSource))
            {
                var expression = Expression.Property(Visit(node.Expression), node.Member.Name);
                return expression;
            }
            return base.VisitMember(node);
        }

    }
}
