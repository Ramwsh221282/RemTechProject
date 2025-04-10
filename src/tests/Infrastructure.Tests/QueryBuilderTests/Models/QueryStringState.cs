namespace Infrastructure.Tests.QueryBuilderTests.Models;

public enum QueryStringState
{
    NotStarted = 0,
    Select = 1,
    From = 2,
    Where = 3,
    And = 4,
}
