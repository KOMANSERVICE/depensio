using depensio.Application.UseCases.ChatBots.Commands;
using depensio.Application.UseCases.ChatBots.DTOs;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.ChatBots;


public record UseChatBotRequest(ChatbotDTO Chatbot);
public record UseChatBotResponse(string ResponseMessage);

public class UseChatBot : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/chatbot", async (UseChatBotRequest request, ISender sender) =>
        {
            var command = request.Adapt<UseChatBotCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UseChatBotResponse>();
            var baseResponse = ResponseFactory.Success(response, "Reponse récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("UseChatBot")
       .WithTags("ChatBot")
       .Produces<BaseResponse<UseChatBotResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("UseChatBot By Menu Id")
       .WithDescription("UseChatBot By Menu Id");
    }
}
