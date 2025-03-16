using GuardValidationLibrary.GuardedFactory;
using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Models;
using RemTech.MainApi.ParsersManagement.Requests;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Services;

public sealed class ParserManagementService
{
    // private readonly ParserDataServiceMessager _messager;
    //
    // public ParserManagementService(ParserDataServiceMessager messager) => (_messager) = (messager);
    //
    // public async Task<Result<ParserResponse>> CreateDefault(
    //     string name,
    //     CancellationToken ct = default
    // )
    // {
    //     var parserCreation = GuardedCreator.Create<Parser>(
    //         [
    //             new ParserName(name),
    //             new ParserSchedule(1, DateTime.Now.Date, DateTime.Now.AddHours(1)),
    //             new ParserState("Выключен"),
    //             Array.Empty<ParserLink>(),
    //         ]
    //     );
    //     if (!parserCreation.IsSuccess)
    //         return new Error(parserCreation.Error);
    //
    //     var message = new SaveParserMessage(parserCreation.Object.ToDto());
    //     ContractActionResult saving = await _messager.Send(message, ct);
    //     return saving.IsSuccess
    //         ? parserCreation.Object.ToResponse()
    //         : new Error(parserCreation.Error);
    // }
    //
    // public async Task<Result<ParserResponse>> Update(string name, UpdateParserRequest request, CancellationToken ct = default)
    // {
    //     var parserCreation = GuardedCreator.Create<Parser>(
    //         [
    //             new ParserName(name),
    //             new ParserSchedule(request.RepeatEveryHours, request.LastRun, request.NextRun),
    //             new ParserState(request.ParserState),
    //             request.Links.Select(l => new ParserLink(l)).ToArray(),
    //         ]
    //     );
    //     if (!parserCreation.IsSuccess)
    //         return new Error(parserCreation.Error);
    //
    //     var message = new UpdateParserMessage(parserCreation.Object.ToDto());
    //     ContractActionResult updating = await _messager.Send(message)
    //
    //     ContractActionResult updating = await _factory
    //         .CreateSingleCommunicationPublisher()
    //         .Send(new UpdateParserMessage(parserCreation.Object.ToDto()));
    //
    //     return updating.IsSuccess
    //         ? updating.FromResult<ParserDaoResponse>().MapToParser().ToResponse()
    //         : new Error(updating.Error);
    // }
    //
    // public async Task<Result> Delete(string name)
    // {
    //     DeleteParserMessage message = new DeleteParserMessage(name);
    //     ContractActionResult deleting = await _factory
    //         .CreateSingleCommunicationPublisher()
    //         .Send(message);
    //     return deleting.IsSuccess ? Result.Success() : new Error(deleting.Error);
    // }
    //
    // public async Task<Result<ParserResponse>> Get(string name)
    // {
    //     GetConcreteParserMessage message = new GetConcreteParserMessage(name);
    //     ContractActionResult querying = await _factory
    //         .CreateSingleCommunicationPublisher()
    //         .Send(message);
    //
    //     return querying.IsSuccess
    //         ? querying.FromResult<ParserDaoResponse>().MapToParser().ToResponse()
    //         : new Error(querying.Error);
    // }
    //
    // public async Task<Result<IEnumerable<ParserResponse>>> GetAll()
    // {
    //     ContractActionResult querying = await _factory
    //         .CreateSingleCommunicationPublisher()
    //         .Send(new GetAllParsersMessage());
    //
    //     return querying.IsSuccess
    //         ? Result<IEnumerable<ParserResponse>>.Success(
    //             querying
    //                 .FromResult<ParserDaoResponse[]>()
    //                 .Select(p => p.MapToParser())
    //                 .Select(p => p.ToResponse())
    //         )
    //         : new Error("Data Service Might be disabled.");
    // }
}
