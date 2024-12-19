using System.Linq.Expressions;

namespace AuroBank_SoftwareProject.Data.DataAccess
{
    public class QueryOptions<T>
    {
        public Expression<Func<T, object>> OrderBy { get; set; }
        //public string OrderByDirection { get; set; } = "asc"; // default
        public Expression<Func<T, bool>> Where { get; set; }
    }
}
