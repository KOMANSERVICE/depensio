Tu es un expert en développement .NET et tests unitaires comportementaux (BDD).  
Ta tâche est de générer les tests unitaires du module **backend/Depensio.Application/UseCases/Products** dans le projet de test **tests/Depensio.Tests.Acceptance**.

### Objectif :
Créer un ou plusieurs fichiers de test complets basés sur la méthode **Gherkin (Given / When / Then)** en utilisant les packages :
- `Xunit`
- `Xunit.Gherkin.Quick`
- `FluentAssertions`
- `Moq`
- `Microsoft.EntityFrameworkCore`

### Contexte :
- Le module **UseCases/Products** contient les cas d’usage métier relatifs à la gestion des produits.
- Les tests doivent vérifier le comportement métier (non seulement le résultat mais aussi les interactions avec les dépendances).
- Utilise des **mocks** pour les dépendances (repositories, context EF, services).
- Utilise **FluentAssertions** pour des assertions lisibles et expressives.
- Chaque scénario Gherkin doit correspondre à une classe de test distincte.

### Structure du code attendue :
- Dossier de sortie Spec.cs : `tests/Depensio.Tests.Acceptance/Steps/Products`
- Dossier de sortie .Feature : `tests/Depensio.Tests.Acceptance/Features/Products`
- Chaque fichier de test doit suivre la convention : `Product<NomDuCas>Spec.cs`
- Chaque classe de test doit hériter de `Feature` (de `Xunit.Gherkin.Quick`)
- Inclure un ou plusieurs scénarios avec les étapes : `Given`, `When`, `Then`
- Utiliser `Fact` ou `Scenario` selon le contexte
- Implémenter les étapes comme des méthodes publiques marquées `[Given]`, `[When]`, `[Then]`

### Exemple minimal attendu :
```csharp
using Xunit;
using Xunit.Gherkin.Quick;
using FluentAssertions;
using Moq;
using Depensio.Application.UseCases.Products;
using Depensio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Depensio.Tests.Acceptance.Products;

[FeatureFile("./Products/ProductCreation.feature")]
public sealed class ProductCreationSteps : Feature
{
    private readonly Mock<IProductRepository> _productRepository;
    private Product _newProduct;
    private ProductHandler _handler;

    public Product_CreationSteps()
    {
        _productRepository = new Mock<IProductRepository>();
        _handler = new ProductHandler(_productRepository.Object);
    }

    [Given("un produit valide")]
    public void GivenAValidProduct()
    {
        _newProduct = new Product { Name = "Stylo Bleu", Price = 2.5M };
    }

    [When("je crée le produit")]
    public void WhenICreateTheProduct()
    {
        _handler.Create(_newProduct);
    }

    [Then("le produit est ajouté au dépôt")]
    public void ThenTheProductIsAddedToRepository()
    {
        _productRepository.Verify(repo => repo.Add(It.Is<Product>(p => p.Name == "Stylo Bleu")), Times.Once);
    }
}
