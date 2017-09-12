using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace HighLoad.Framework.Data
{
    public interface IVisitViewDbConnectionFactory : IDbConnectionFactory
    {
    }

    public class VisitViewDbConnectionFactory : OrmLiteConnectionFactory,
        IVisitViewDbConnectionFactory
    {
        public VisitViewDbConnectionFactory(string s, IOrmLiteDialectProvider dialectProvider) : base(s, dialectProvider)
        {
        }
    }
}