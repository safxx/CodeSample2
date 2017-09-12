using System;
using System.Linq.Expressions;

namespace HighLoad.Application.Data.ReadModel
{
    public class QueryBuilderBase<T>
    {
        protected bool _isSealed;
        private Expression _query;
        private readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof(T));

        public Expression<Func<T, bool>> Build()
        {
            _isSealed = true;

            ParameterReplacer.Get(_parameterExpression).Visit(_query);

            return Expression.Lambda<Func<T, bool>>(_query, _parameterExpression);
        }

        protected void CombineIntoAndExpression(Expression<Func<T, bool>> expression)
        {
            if (_query == null)
            {
                _query = expression.Body;
                return;
            }
            
            _query = Expression.AndAlso(_query, expression.Body);
        }

        internal class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(_parameter);
            }

            internal ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }

            public static ParameterReplacer Get(ParameterExpression parameterExpression)
            {
                return new ParameterReplacer(parameterExpression);
            }
        }
    }
}