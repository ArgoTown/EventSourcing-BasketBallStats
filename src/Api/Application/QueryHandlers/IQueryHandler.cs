namespace BasketballStats.Api.Application.QueryHandlers;

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> Query(TQuery query);
}
