using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Querying
{
    public class Query<T>
    {

        public Query()
        {
            _BuilderQuery = PredicateBuilder.True<T>();
        }

        private Expression<Func<T, bool>> _BuilderQuery;
        public Expression<Func<T, bool>> BuilderQuery { get { return _BuilderQuery; } }
        


        IList<OrderByCla<T>> _OrderByList;
        public IList<OrderByCla<T>> OrderByList { get { return _OrderByList; } }

        public void And(Expression<Func<T, bool>> q)
        {

            _BuilderQuery = _BuilderQuery.And(q);
        }

        public void Or(Expression<Func<T, bool>> q)
        {

            _BuilderQuery = _BuilderQuery.Or(q);
        }


        public void OrderBy(Expression<Func<T, object>> orderby, bool isDesc = false)
        {
            if (_OrderByList == null)
            {
                _OrderByList = new List<OrderByCla<T>>();

            }
            _OrderByList.Add(new OrderByCla<T>(orderby, isDesc));
        }


    }


    public class OrderByCla<T>
    {
        public OrderByCla(Expression<Func<T, object>> orderby, bool isDesc = false)
        {
            this.Orderby = orderby;
            this.IsDesc = isDesc;
        }
        public Expression<Func<T, object>> Orderby { get; set; }
        public bool IsDesc { get; set; }
    }
}
