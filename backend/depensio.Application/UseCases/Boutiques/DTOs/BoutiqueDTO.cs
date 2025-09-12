namespace depensio.Application.UserCases.Boutiques.DTOs;

public record BoutiqueDTO(Guid Id, string Name,string Location, DateOnly CreatedAt);