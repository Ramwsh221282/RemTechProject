using RemTech.Application.ParserContext.Contracts;
using RemTech.Domain.ParserContext;
using RemTech.Domain.ParserContext.ValueObjects;

namespace RemTech.WebApi.Extras;

public static class AddInitialParsers
{
    public static void AddInitialParsersInDb(this IServiceCollection serviceCollection)
    {
        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IServiceScopeFactory scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        using IServiceScope scope = scopeFactory.CreateScope();

        try
        {
            Parser avito = new(ParserId.New(), ParserName.Create("AVITO"));
            Parser drom = new(ParserId.New(), ParserName.Create("DROM"));
            IParserWriteRepository writeRepository =
                scope.ServiceProvider.GetRequiredService<IParserWriteRepository>();
            writeRepository.Add(avito).Wait();
            writeRepository.Add(drom).Wait();
        }
        catch
        {
            Console.WriteLine("Initial parsers already exist.");
        }
    }
}
