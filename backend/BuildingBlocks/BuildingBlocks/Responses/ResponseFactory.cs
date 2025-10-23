
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Responses;

public static class ResponseFactory
{
    public static BaseResponse<T> Success<T>(T data, string? message = null, int statusCode = StatusCodes.Status200OK)
        => new(data, message ?? "Succès", statusCode, DateTime.UtcNow);

    public static BaseResponse<string> Deleted(string? message = null)
        => Success("Ressource supprimée", message ?? "Supprimé avec succès", StatusCodes.Status200OK);
}
