using System.Linq.Expressions;

namespace Utilities.Queries
{
    public class QueryCombinator : ExpressionVisitor
    {
        private readonly Expression _from, _to;

        public QueryCombinator(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public override Expression? Visit(Expression node)
        {
            return node == _from ? _to : base.Visit(node);
        }

        public static Expression<Func<T, bool>> MergeWithAnd<T>(
            Expression<Func<T, bool>> firstClause,
            Expression<Func<T, bool>> secondClause)
        {
            // rewrite e1, using the prarmeter from e2; "&&"
            Expression<Func<T, bool>> lamda1 = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(
                new QueryCombinator(firstClause.Parameters[0], secondClause.Parameters[0]).Visit(firstClause.Body),
                secondClause.Body), secondClause.Parameters);

            return lamda1;
        }

        public static Expression<Func<T, bool>> MergeWithOr<T>(
            Expression<Func<T, bool>> firstClause,
            Expression<Func<T, bool>> secondClause)
        {
            // rewrite e1, using the prarmeter from e2; "||"
            Expression<Func<T, bool>> lamda1 = Expression.Lambda<Func<T, bool>>(Expression.OrElse(
                new QueryCombinator(firstClause.Parameters[0], secondClause.Parameters[0]).Visit(firstClause.Body),
                secondClause.Body), secondClause.Parameters);

            return lamda1;
        }

    }
}
