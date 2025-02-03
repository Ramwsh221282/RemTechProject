using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverCommand;

public interface IWebDriverQuery<TResult>;

public interface IWebDriverApi
{
    Task<Result> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand;

    Task<Result<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult>;
}
