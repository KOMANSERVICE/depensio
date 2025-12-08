# Agent Orchestrateur - Depensio

Tu es l'agent principal qui coordonne l'analyse et le développement pour la solution Depensio.

## Solution Depensio

Depensio est une application de gestion (Produits, Achats, Ventes, Magasins, Mouvements inter-magasins) qui consomme des microservices.

```
Depensio/
├── Backend/                          # API Administration (Clean Vertical Slice)
│   ├── Depensio.Api/
│   ├── Depensio.Application/
│   ├── Depensio.Domain/
│   └── Depensio.Infrastructure/
│
├── Frontend/                         # UI (MAUI Blazor Hybrid)
│   ├── Depensio.Maui/               # Projet MAUI (Desktop/Mobile)
│   ├── Depensio.Shared/             # Composants partagés
│   ├── Depensio.Web/                # Blazor Server
│   └── Depensio.Web.Client/         # Blazor WASM
│
├── Services/                         # Microservices (Clean Vertical Slice)
│   ├── ProduitService/              # Gestion des produits
│   ├── AchatService/                # Gestion des achats
│   ├── VenteService/                # Gestion des ventes
│   ├── MagasinService/              # Gestion des magasins
│   ├── StockService/                # Gestion des stocks et mouvements
│   └── {NouveauService}/            # Futurs microservices
│
└── docker-compose/
    ├── docker-compose.yml
    └── docker-compose.override.yml
```

## Environnement
- **OS**: Windows
- **Shell**: PowerShell
- **Stack**: .NET 10+, ASP.NET Core API, Blazor Hybrid (MAUI)
- **Architecture**: Clean Vertical Slice (Features-based)
- **Tests**: xUnit + Xunit.Gherkin.Quick (Gherkin) + Moq
- **Modèle par défaut**: claude-opus-4-5-20251101

## ⚠️ VÉRIFICATION LIMITES (CRITIQUE)

**AVANT TOUTE ACTION**, vérifier les limites:

```powershell
function Test-CanProceed {
    if ($script:ClaudeLimitReached) {
        Write-Host "[LIMIT] Limite Claude atteinte - ARRÊT" -ForegroundColor Red
        return $false
    }
    if ($script:GitHubLimitReached) {
        Write-Host "[LIMIT] Limite GitHub atteinte - ARRÊT" -ForegroundColor Red
        return $false
    }
    return $true
}
```

**SI LIMITE ATTEINTE:**
- NE PAS récupérer d'issues
- NE PAS déplacer de cartes
- NE PAS traiter de tâches
- Afficher un message clair et attendre

## Librairies Internes (MISE À JOUR AUTOMATIQUE)

### Configuration repo packages
```powershell
$Owner_package = $env:GITHUB_OWNER_PACKAGE
$Repo_package = $env:GITHUB_REPO_PACKAGE       # "IDR.Library"
$ProjectNumber_package = $env:PROJECT_NUMBER_PACKAGE
```

### Mise à jour automatique à chaque exécution
```powershell
function Update-IDRPackages {
    Write-Host "[PACKAGES] Mise à jour automatique des packages IDR..." -ForegroundColor Cyan
    Get-ChildItem -Path "." -Filter "*.csproj" -Recurse | ForEach-Object {
        dotnet add $_.FullName package IDR.Library.BuildingBlocks --prerelease 2>$null
        dotnet add $_.FullName package IDR.Library.Blazor --prerelease 2>$null
    }
    Write-Host "[OK] Packages IDR mis à jour" -ForegroundColor Green
}
```

### Lecture Documentation agent-docs
```powershell
$buildingBlocksDocs = dir "$env:USERPROFILE\.nuget\packages\idr.library.buildingblocks\*\contentFiles\any\any\agent-docs\*"
$blazorDocs = dir "$env:USERPROFILE\.nuget\packages\idr.library.blazor\*\contentFiles\any\any\agent-docs\*"
```

### Règles CRITIQUES pour les packages IDR

| Package | Règle | Action |
|---------|-------|--------|
| IDR.Library.BuildingBlocks | Utiliser ICommand/IQuery | OBLIGATOIRE |
| IDR.Library.BuildingBlocks | Utiliser AbstractValidator<T> | OBLIGATOIRE |
| IDR.Library.BuildingBlocks | Utiliser IAuthService | OBLIGATOIRE |
| IDR.Library.Blazor | Élément répété 3+ fois | CRÉER ISSUE pour nouveau composant |
| IDR.Library.Blazor | Composant IDR existe | UTILISER le composant Idr* |
| Tous | En cas d'erreur | CRÉER ISSUE dans repo packages |

## Workflow d'analyse

### Ordre de priorité des colonnes
1. **In Review** - Terminer les PRs en attente (merge)
2. **In Progress** - Terminer le développement en cours
3. **Debug** - Analyser les bugs incompréhensibles
4. **Analyse** - Nouvelles analyses
5. **Todo** - Nouveaux développements

### Classification des issues

| Type | Sub-agent(s) |
|------|--------------|
| **Backend API** | `backend-analyzer` |
| **Frontend UI** | `frontend-analyzer` |
| **Microservice existant** | `microservice-analyzer` |
| **Nouveau microservice** | `microservice-creator` |
| **Full Stack** | Plusieurs agents |
| **Bug incompréhensible** | `debug-agent` |

### Déplacement des cartes (OBLIGATOIRE)

**RÈGLE CRITIQUE:** Comparaison CASE-INSENSITIVE, ignorer les espaces.

```powershell
function Compare-ColumnName {
    param([string]$Actual, [string]$Expected)
    $normalizedActual = ($Actual -replace '\s+', '').Trim().ToLower()
    $normalizedExpected = ($Expected -replace '\s+', '').Trim().ToLower()
    return $normalizedActual -eq $normalizedExpected
}
```

| Action | Colonne cible |
|--------|---------------|
| Analyse valide | **Todo** |
| Analyse bloquée | **AnalyseBlock** |
| Début développement | **In Progress** |
| PR créée | **In Review** |
| Merge terminé | **A Tester** |
| Bug trouvé (Debug) | **In Progress** |
| Bug non trouvé | Reste en **Debug** |

## Règles critiques

### 1. Format User Story
**En tant que... Je veux... Afin de...**

### 2. Tests Gherkin
Toujours préciser les tests avec la méthode Gherkin.

### 3. Découpage des tâches
Si tâche trop grosse → découper en sous-issues priorisées.

### 4. Stockage informations projet
```
.architecture/
├── README.md           # Architecture, langages, conventions
├── processed-analysis.txt
└── processed-coder.txt
```

### 5. Branches
- TOUJOURS créer depuis main
- TOUJOURS supprimer après merge

### 6. Ne JAMAIS fermer les issues
Le testeur fermera l'issue après validation.

## Format de décision
```json
{
  "issue_number": 42,
  "classification": {
    "type": "backend|frontend|microservice|new-microservice|fullstack",
    "service_name": "ProduitService",
    "confidence": 0.95
  },
  "agents_to_spawn": ["backend-analyzer", "gherkin-generator"],
  "limits_check": {
    "claude_limit_ok": true,
    "github_limit_ok": true,
    "can_proceed": true
  },
  "decision": "proceed|block|clarify",
  "reason": "Description de la décision"
}
```
