using System;
using System.Linq.Expressions;

namespace HighLoad.Application.Data.ReadModel.MarkView
{
    public class MarkViewQueryBuilder:QueryBuilderBase<MarkView>
    {
        private MarkViewQueryBuilder()
        {
        }

        public MarkViewQueryBuilder WithFromDate(DateTime? fromDate)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!fromDate.HasValue) return this;

            Expression<Func<MarkView, bool>> expression = mv => mv.VisitedAt > fromDate.Value;
           CombineIntoAndExpression(expression);

            return this;
        }

        public MarkViewQueryBuilder WithToDate(DateTime? toDate)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!toDate.HasValue) return this;

            Expression<Func<MarkView, bool>> expression = mv => mv.VisitedAt <= toDate.Value;
            CombineIntoAndExpression(expression);

            return this;
        }

        public MarkViewQueryBuilder WithFromAge(int? fromAge)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!fromAge.HasValue) return this;

            Expression<Func<MarkView, bool>> expression = mv => mv.Age > fromAge.Value;
           CombineIntoAndExpression(expression);

            return this;
        }

        public MarkViewQueryBuilder WithToAge(int? toAge)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!toAge.HasValue) return this;

            Expression<Func<MarkView, bool>> expression = mv => mv.Age <= toAge.Value;
            CombineIntoAndExpression(expression);

            return this;
        }

        public MarkViewQueryBuilder WithGender(char? gender)
        {
            if (_isSealed) throw new InvalidOperationException();

            if (!gender.HasValue) return this;

            var g = gender.Value == 'm';
            Expression<Func<MarkView, bool>> expression = mv => mv.Gender == g;
            CombineIntoAndExpression(expression);

            return this;
        }

        public MarkViewQueryBuilder WithLocationId(int locationId)
        {
            if (_isSealed) throw new InvalidOperationException();

            Expression<Func<MarkView, bool>> expression = mv => mv.LocationId == locationId;
            CombineIntoAndExpression(expression);

            return this;
        }

        public static MarkViewQueryBuilder GetBuilder()
        {
            return new MarkViewQueryBuilder();
        }
    }
}