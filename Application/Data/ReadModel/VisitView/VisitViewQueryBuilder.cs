using System;
using System.Linq.Expressions;

namespace HighLoad.Application.Data.ReadModel.VisitView
{
    public class VisitViewQueryBuilder:QueryBuilderBase<VisitView>
    {
       public VisitViewQueryBuilder WithFromDate(DateTime? fromDate)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!fromDate.HasValue) return this;

            Expression<Func<VisitView, bool>> expression = v => v.VisitedAt > fromDate.Value;
            CombineIntoAndExpression(expression);

            return this;
        }

        public VisitViewQueryBuilder WithToDate(DateTime? toDate)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!toDate.HasValue) return this;

            Expression<Func<VisitView, bool>> expression = v => v.VisitedAt <= toDate.Value;
            CombineIntoAndExpression(expression);

            return this;
        }

        public VisitViewQueryBuilder WithCountry(string country)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (string.IsNullOrEmpty(country)) return this;

            Expression<Func<VisitView, bool>> expression = v => v.Country == country;
            CombineIntoAndExpression(expression);

            return this;
        }

        public VisitViewQueryBuilder WithToDistance(int? toDistance)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!toDistance.HasValue) return this;

            Expression<Func<VisitView, bool>> expression = v => v.Distance < toDistance.Value;
            CombineIntoAndExpression(expression);

            return this;
        }

        public VisitViewQueryBuilder WithUserId(int userId)
        {
            if (_isSealed) throw new InvalidOperationException();

            Expression<Func<VisitView, bool>> expression = v => v.UserId == userId;
            CombineIntoAndExpression(expression);

            return this;
        }

        public static VisitViewQueryBuilder GetBuilder()
        {
            return new VisitViewQueryBuilder();
        }
    }
}