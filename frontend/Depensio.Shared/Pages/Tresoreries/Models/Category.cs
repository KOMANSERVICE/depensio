namespace depensio.Shared.Pages.Tresoreries.Models;

public enum CategoryType
{
    INCOME = 1,   // Catégorie de revenus
    EXPENSE = 2   // Catégorie de dépenses
}

public record CreateCategoryRequest(
    string Name,
    CategoryType Type,
    string? Icon
);

public record CreateCategoryResponse(CategoryDTO Category);

public record CategoryDTO(
    Guid Id,
    string ApplicationId,
    string Name,
    CategoryType Type,
    string? Icon,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public class CategoryCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; } = CategoryType.EXPENSE;
    public string? Icon { get; set; }
}

public record GetCategoriesResponse(
    IReadOnlyList<CategoryDTO> Categories,
    int TotalCount
);
