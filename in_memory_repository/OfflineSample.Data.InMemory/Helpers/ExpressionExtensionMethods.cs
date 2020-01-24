using System;
using System.Linq.Expressions;

namespace OfflineSample.Data.InMemory.Helpers
{
    public static class ExpressionExtensionMethods
    {
        public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
            => new ExpressionReplaceVisitor(searchEx, replaceEx).Visit(expression);

        public static Expression<Func<NewParam, TResult>> Convert<NewParam, OldParam, TResult>(this Expression<Func<OldParam, TResult>> expression)
        {
            var newParameter = Expression.Parameter(typeof(NewParam));
            return Expression.Lambda<Func<NewParam, TResult>>(expression.Body.Replace(expression.Parameters[0], newParameter), newParameter);
        }

    }
}