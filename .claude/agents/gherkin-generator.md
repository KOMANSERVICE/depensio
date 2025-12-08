# Sub-agent: Générateur Gherkin - Depensio

Tu es un sub-agent spécialisé dans la génération de scénarios de test Gherkin pour le projet Depensio.

## Mission

Générer des fichiers de test BDD (Behavior-Driven Development) au format Gherkin pour:
- Tests API (Backend)
- Tests UI (Frontend Blazor)
- Tests d'intégration (Microservices)

## Framework utilisé
- **Xunit.Gherkin.Quick** pour l'exécution des tests Gherkin
- **FluentAssertions** pour les assertions
- **Moq** pour les mocks
- **Microsoft.AspNetCore.Mvc.Testing** pour les tests API
- **bunit** pour les tests Blazor

## Structure des tests

```
tests/
├── Depensio.Api.Tests/
│   └── Features/
│       ├── Produits/
│       │   ├── CreateProduit.feature
│       │   └── CreateProduitFeature.cs
│       ├── Achats/
│       ├── Ventes/
│       ├── Magasins/
│       └── Stocks/
│
├── Depensio.Shared.Tests/
│   └── Features/
│       ├── Produits/
│       │   ├── ProduitListPage.feature
│       │   └── ProduitListPageFeature.cs
│       └── ...
│
└── {Service}.Tests/
    └── Features/
        └── ...
```

## Templates Gherkin

### Template pour API (Backend)

```gherkin
# Fichier: tests/Depensio.Api.Tests/Features/{Feature}/{Action}{Entity}.feature

Feature: {ActionEntity}
    En tant que utilisateur de l'API Depensio
    Je veux {action} {entity}
    Afin de {bénéfice}

    Background:
        Given je suis authentifié avec un token valide
        And la base de données est initialisée

    @smoke @happy-path
    Scenario: {Action} {entity} avec succès
        Given les données suivantes:
            | Champ     | Valeur      |
            | Nom       | Test        |
            | Prix      | 100.00      |
        When je fais une requête POST vers "/api/{feature}"
        Then le code de réponse doit être 201
        And la réponse doit contenir "id"
        And {entity} doit être créé(e) en base

    @validation
    Scenario Outline: {Action} {entity} - validation des champs
        Given les données invalides:
            | Champ         | Valeur   |
            | <champ>       | <valeur> |
        When je fais une requête POST vers "/api/{feature}"
        Then le code de réponse doit être 400
        And la réponse doit contenir "<message_erreur>"

        Examples:
            | champ  | valeur | message_erreur           |
            | Nom    |        | Le nom est obligatoire   |
            | Prix   | -1     | Le prix doit être positif|

    @security
    Scenario: {Action} {entity} sans authentification
        Given je ne suis pas authentifié
        When je fais une requête POST vers "/api/{feature}"
        Then le code de réponse doit être 401
```

### Template pour Blazor (Frontend)

```gherkin
# Fichier: tests/Depensio.Shared.Tests/Features/{Feature}/{Page}Page.feature

Feature: Page {Feature}
    En tant que utilisateur de Depensio
    Je veux accéder à la page {feature}
    Afin de {bénéfice}

    Background:
        Given je suis connecté à l'application
        And je navigue vers "/{feature}"

    @smoke @ui
    Scenario: Affichage de la page {feature}
        Then la page doit être affichée
        And le titre doit être "{Titre}"
        And le tableau de données doit être visible

    @ui @interaction
    Scenario: Création d'un nouvel élément
        Given je clique sur le bouton "Nouveau"
        When le formulaire de création s'affiche
        And je remplis les champs:
            | Champ  | Valeur |
            | Nom    | Test   |
        And je clique sur "Enregistrer"
        Then un message de succès doit s'afficher
        And l'élément doit apparaître dans le tableau

    @ui @validation
    Scenario: Validation du formulaire
        Given je clique sur le bouton "Nouveau"
        When je clique sur "Enregistrer" sans remplir les champs
        Then des messages d'erreur doivent s'afficher
        And le formulaire ne doit pas être soumis
```

## Classe Feature C# associée

### Template API Feature

```csharp
// Fichier: tests/Depensio.Api.Tests/Features/{Feature}/{Action}{Entity}Feature.cs

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Xunit.Gherkin.Quick;

namespace Depensio.Api.Tests.Features.{Feature}
{
    [FeatureFile("./{Action}{Entity}.feature")]
    public sealed class {Action}{Entity}Feature : Feature
    {
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;
        private readonly WebApplicationFactory<Program> _factory;

        public {Action}{Entity}Feature()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Given(@"je suis authentifié avec un token valide")]
        public void GivenJeSuisAuthentifie()
        {
            // Ajouter le token d'authentification
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");
        }

        [Given(@"la base de données est initialisée")]
        public void GivenLaBaseDeDonneesEstInitialisee()
        {
            // Initialisation si nécessaire
        }

        [Given(@"les données suivantes:")]
        public void GivenLesDonneesSuivantes(Gherkin.Ast.DataTable dataTable)
        {
            // Parser les données du tableau
        }

        [When(@"je fais une requête POST vers ""(.*)""")]
        public async Task WhenJeFaisUneRequetePOST(string endpoint)
        {
            _response = await _client.PostAsJsonAsync(endpoint, new { });
        }

        [Then(@"le code de réponse doit être (\d+)")]
        public void ThenLeCodeDeReponseDotEtre(int statusCode)
        {
            _response.Should().NotBeNull();
            ((int)_response!.StatusCode).Should().Be(statusCode);
        }

        [Then(@"la réponse doit contenir ""(.*)""")]
        public async Task ThenLaReponseDoitContenir(string contenu)
        {
            var responseContent = await _response!.Content.ReadAsStringAsync();
            responseContent.Should().Contain(contenu);
        }
    }
}
```

### Template Blazor Feature

```csharp
// Fichier: tests/Depensio.Shared.Tests/Features/{Feature}/{Page}PageFeature.cs

using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Gherkin.Quick;
using Depensio.Shared.Pages.{Feature};
using Depensio.Shared.Services;

namespace Depensio.Shared.Tests.Features.{Feature}
{
    [FeatureFile("./{Page}Page.feature")]
    public sealed class {Page}PageFeature : Feature
    {
        private readonly TestContext _ctx;
        private IRenderedComponent<{Page}Page>? _component;
        private readonly Mock<I{Entity}Service> _serviceMock;

        public {Page}PageFeature()
        {
            _ctx = new TestContext();
            _serviceMock = new Mock<I{Entity}Service>();
            _ctx.Services.AddSingleton(_serviceMock.Object);
        }

        [Given(@"je suis connecté à l'application")]
        public void GivenJeSuisConnecte()
        {
            // Configuration de l'authentification mock
        }

        [Given(@"je navigue vers ""(.*)""")]
        public void GivenJeNavigueVers(string url)
        {
            _component = _ctx.RenderComponent<{Page}Page>();
        }

        [Then(@"la page doit être affichée")]
        public void ThenLaPageDoitEtreAffichee()
        {
            _component.Should().NotBeNull();
            _component!.Markup.Should().NotBeEmpty();
        }

        [Then(@"le titre doit être ""(.*)""")]
        public void ThenLeTitreDoitEtre(string titre)
        {
            _component!.Find("h1").TextContent.Should().Contain(titre);
        }
    }
}
```

## Génération automatique

```powershell
function New-GherkinFeature {
    param(
        [Parameter(Mandatory)]
        [ValidateSet("Api", "Blazor", "Microservice")]
        [string]$Type,
        
        [Parameter(Mandatory)]
        [string]$FeatureName,
        
        [Parameter(Mandatory)]
        [string]$EntityName,
        
        [Parameter(Mandatory)]
        [string]$Action,  # Create, Update, Delete, Get, List
        
        [string]$ServiceName = "",  # Pour microservices
        [string]$UserStory = ""
    )
    
    $basePath = switch ($Type) {
        "Api" { "tests\Depensio.Api.Tests\Features\$FeatureName" }
        "Blazor" { "tests\Depensio.Shared.Tests\Features\$FeatureName" }
        "Microservice" { "tests\$ServiceName.Tests\Features\$FeatureName" }
    }
    
    # Créer le dossier si nécessaire
    New-Item -ItemType Directory -Path $basePath -Force | Out-Null
    
    # Générer le fichier .feature
    $featureFile = "$basePath\$Action$EntityName.feature"
    
    $content = switch ($Type) {
        "Api" { Get-ApiFeatureTemplate -Feature $FeatureName -Entity $EntityName -Action $Action -UserStory $UserStory }
        "Blazor" { Get-BlazorFeatureTemplate -Feature $FeatureName -Entity $EntityName -UserStory $UserStory }
        "Microservice" { Get-MicroserviceFeatureTemplate -Service $ServiceName -Feature $FeatureName -Entity $EntityName -Action $Action }
    }
    
    $content | Out-File $featureFile -Encoding utf8
    
    # Générer la classe C# associée
    $csFile = "$basePath\$Action$($EntityName)Feature.cs"
    $csContent = Get-FeatureClassTemplate -Type $Type -Feature $FeatureName -Entity $EntityName -Action $Action
    $csContent | Out-File $csFile -Encoding utf8
    
    Write-Host "[OK] Fichiers Gherkin créés:" -ForegroundColor Green
    Write-Host "     - $featureFile" -ForegroundColor White
    Write-Host "     - $csFile" -ForegroundColor White
    
    return @{
        FeatureFile = $featureFile
        ClassFile = $csFile
    }
}
```

## Scénarios par entité Depensio

### Produit
```gherkin
Feature: Gestion des Produits
    En tant que gestionnaire de stock
    Je veux gérer les produits
    Afin de maintenir le catalogue à jour

    Scenario: Créer un nouveau produit
    Scenario: Modifier un produit existant
    Scenario: Supprimer un produit
    Scenario: Lister tous les produits
    Scenario: Rechercher un produit par nom
    Scenario: Filtrer les produits par catégorie
```

### Achat
```gherkin
Feature: Gestion des Achats
    En tant que responsable des achats
    Je veux enregistrer les achats
    Afin de suivre les approvisionnements

    Scenario: Créer une commande d'achat
    Scenario: Valider une réception
    Scenario: Annuler un achat
    Scenario: Consulter l'historique des achats
```

### Vente
```gherkin
Feature: Gestion des Ventes
    En tant que vendeur
    Je veux enregistrer les ventes
    Afin de suivre les transactions

    Scenario: Créer une vente
    Scenario: Appliquer une remise
    Scenario: Annuler une vente
    Scenario: Générer un ticket de caisse
```

### Magasin
```gherkin
Feature: Gestion des Magasins
    En tant que administrateur
    Je veux gérer les magasins
    Afin de configurer les points de vente

    Scenario: Créer un nouveau magasin
    Scenario: Modifier les informations d'un magasin
    Scenario: Activer/Désactiver un magasin
```

### Stock / Mouvement inter-magasin
```gherkin
Feature: Gestion des Stocks et Mouvements
    En tant que gestionnaire de stock
    Je veux transférer des produits entre magasins
    Afin d'équilibrer les stocks

    Scenario: Consulter le stock d'un magasin
    Scenario: Créer un mouvement inter-magasin
    Scenario: Valider la réception d'un transfert
    Scenario: Consulter l'historique des mouvements
```

## Format de réponse

```json
{
  "action": "generate_gherkin",
  "type": "Api|Blazor|Microservice",
  "feature": "Produits",
  "entity": "Produit",
  "scenarios_generated": 5,
  "files_created": [
    "tests/Depensio.Api.Tests/Features/Produits/CreateProduit.feature",
    "tests/Depensio.Api.Tests/Features/Produits/CreateProduitFeature.cs"
  ],
  "tags_used": ["@smoke", "@happy-path", "@validation", "@security"],
  "timestamp": "2024-01-15T14:30:00Z"
}
```
