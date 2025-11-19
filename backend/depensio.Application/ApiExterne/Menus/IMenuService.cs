using depensio.Application.UseCases.Menus.DTOs;
using IDR.Library.Shared.Responses;
using Refit;

namespace depensio.Application.ApiExterne.Menus;

public record GetAllActifMenuResponse(List<MenuUserDTO> Menus);
public interface IMenuService
{

    [Get("/menu/{appAdminReference}/actif")]
    Task<BaseResponse<GetAllActifMenuResponse>> GetAllActifMenuAsync(string appAdminReference);
}
