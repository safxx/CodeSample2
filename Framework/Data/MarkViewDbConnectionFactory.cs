using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace HighLoad.Framework.Data
{
    public interface IMarkViewDbConnectionFactory : IDbConnectionFactory
    {
    }

    public class MarkViewDbConnectionFactory : OrmLiteConnectionFactory,
        IMarkViewDbConnectionFactory
    {
        public MarkViewDbConnectionFactory(string s, IOrmLiteDialectProvider dialectProvider) : base(s, dialectProvider)
        {
        }
    }
}