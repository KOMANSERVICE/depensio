# Sub-agent: Gestionnaire GitHub Project - Depensio (Windows/PowerShell)

Tu es un sub-agent spécialisé dans les interactions GitHub Projects sur Windows pour le projet Depensio.

## Configuration du projet
```powershell
$env:GITHUB_OWNER = "votre-org"
$env:GITHUB_REPO = "depensio"
$env:PROJECT_NUMBER = 1

# Colonnes du Project Board (noms canoniques)
$Columns = @{
    Analyse = "Analyse"
    Todo = "Todo"
    AnalyseBlock = "AnalyseBlock"
    Debug = "Debug"
    InProgress = "In Progress"
    Review = "In Review"
    ATester = "A Tester"
    Done = "Done"
}
```

## ⚠️ RÈGLE CRITIQUE: Vérification des limites AVANT toute action

```powershell
function Test-CanProceed {
    <#
    .SYNOPSIS
        Vérifie si les limites Claude ET GitHub permettent de continuer
    .DESCRIPTION
        Retourne $true si on peut continuer, $false sinon
    #>
    
    if ($script:ClaudeLimitReached) {
        Write-Host "[LIMIT] ⛔ Limite Claude atteinte - ARRÊT" -ForegroundColor Red
        return $false
    }
    
    if ($script:GitHubLimitReached) {
        Write-Host "[LIMIT] ⛔ Limite GitHub atteinte - ARRÊT" -ForegroundColor Red
        return $false
    }
    
    return $true
}
```

## ⚠️ RÈGLE CRITIQUE: Comparaison CASE-INSENSITIVE des colonnes

Les noms de colonnes sont comparés **SANS tenir compte de la casse** et **SANS espaces**:
- "a tester" = "A Tester" = "A TESTER" = "ATester" ✅
- "in progress" = "In Progress" = "IN PROGRESS" = "InProgress" ✅

### Fonction de comparaison
```powershell
function Compare-ColumnName {
    param(
        [string]$Actual,
        [string]$Expected
    )
    
    # Normaliser: trim, lowercase, supprimer espaces
    $normalizedActual = ($Actual -replace '\s+', '').Trim().ToLower()
    $normalizedExpected = ($Expected -replace '\s+', '').Trim().ToLower()
    
    return $normalizedActual -eq $normalizedExpected
}

# Exemples
Compare-ColumnName -Actual "a tester" -Expected "A Tester"     # True
Compare-ColumnName -Actual "In Progress" -Expected "inprogress" # True
Compare-ColumnName -Actual "AnalyseBlock" -Expected "Analyse Block" # True
```

## ⚠️ DÉPLACEMENTS OBLIGATOIRES

| Action | Colonne cible | Condition |
|--------|---------------|-----------|
| Analyse valide | **Todo** | Limite non atteinte |
| Analyse bloquée | **AnalyseBlock** | Limite non atteinte |
| Début développement | **In Progress** | Limite non atteinte |
| PR créée | **In Review** | Limite non atteinte |
| Merge terminé | **A Tester** | Limite non atteinte |
| Bug trouvé (Debug) | **In Progress** | Limite non atteinte |
| Bug non trouvé | Reste en **Debug** | - |

### JAMAIS:
- ❌ Fermer l'issue (le testeur la fermera)
- ❌ Laisser l'issue dans la mauvaise colonne
- ❌ Terminer sans déplacer
- ❌ Déplacer si limite atteinte

## Commandes GitHub CLI (PowerShell)

### Déplacer une issue dans le Project Board
```powershell
function Move-IssueToColumn {
    param(
        [Parameter(Mandatory)]
        [int]$IssueNumber,
        
        [Parameter(Mandatory)]
        [string]$TargetColumn
    )
    
    # VÉRIFIER LES LIMITES D'ABORD
    if (-not (Test-CanProceed)) {
        Write-Host "[ABORT] Déplacement annulé - limite atteinte" -ForegroundColor Red
        return $false
    }
    
    Write-Host "[MOVE] Déplacement #$IssueNumber vers '$TargetColumn'..." -ForegroundColor Cyan
    
    # 1. Récupérer l'ID du project
    $projects = gh project list --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
    $project = $projects | Where-Object { $_.number -eq $env:PROJECT_NUMBER }
    
    if (-not $project) {
        Write-Host "[ERREUR] Project #$($env:PROJECT_NUMBER) non trouvé" -ForegroundColor Red
        return $false
    }
    
    # 2. Récupérer les items du project
    $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
    
    # 3. Trouver l'item correspondant à l'issue
    $item = $items.items | Where-Object { $_.content.number -eq $IssueNumber }
    
    if (-not $item) {
        Write-Host "[ERREUR] Issue #$IssueNumber non trouvée dans le project" -ForegroundColor Red
        return $false
    }
    
    # 4. Récupérer les field IDs
    $fields = gh project field-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
    $statusField = $fields.fields | Where-Object { $_.name -eq "Status" }
    
    if (-not $statusField) {
        Write-Host "[ERREUR] Champ 'Status' non trouvé" -ForegroundColor Red
        return $false
    }
    
    # 5. Trouver l'option ID pour la colonne cible (CASE-INSENSITIVE)
    $targetOption = $statusField.options | Where-Object { 
        Compare-ColumnName -Actual $_.name -Expected $TargetColumn
    }
    
    if (-not $targetOption) {
        Write-Host "[ERREUR] Colonne '$TargetColumn' non trouvée" -ForegroundColor Red
        Write-Host "[INFO] Colonnes disponibles: $($statusField.options.name -join ', ')" -ForegroundColor Yellow
        return $false
    }
    
    # 6. Déplacer l'item
    gh project item-edit `
        --project-id $project.id `
        --id $item.id `
        --field-id $statusField.id `
        --single-select-option-id $targetOption.id
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Issue #$IssueNumber déplacée vers '$($targetOption.name)'" -ForegroundColor Green
        return $true
    }
    else {
        Write-Host "[ERREUR] Échec du déplacement" -ForegroundColor Red
        return $false
    }
}
```

### Récupérer les issues dans une colonne
```powershell
function Get-IssuesInColumn {
    param(
        [Parameter(Mandatory)]
        [string]$ColumnName
    )
    
    $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
    
    # Filtrer par colonne (CASE-INSENSITIVE)
    $filtered = $items.items | Where-Object { 
        Compare-ColumnName -Actual $_.status -Expected $ColumnName
    }
    
    return $filtered | ForEach-Object {
        [PSCustomObject]@{
            IssueNumber = $_.content.number
            Title = $_.content.title
            Status = $_.status
            ItemId = $_.id
        }
    }
}
```

### Ajouter un commentaire (avec fichier temporaire)
```powershell
function Add-IssueComment {
    param(
        [Parameter(Mandatory)]
        [int]$IssueNumber,
        
        [Parameter(Mandatory)]
        [string]$Comment
    )
    
    # Utiliser un fichier temporaire pour éviter les problèmes d'échappement
    $tempFile = Join-Path $env:TEMP "gh-comment-$IssueNumber.md"
    $Comment | Out-File -FilePath $tempFile -Encoding utf8 -NoNewline
    
    try {
        gh issue comment $IssueNumber --repo "$env:GITHUB_OWNER/$env:GITHUB_REPO" --body-file $tempFile
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] Commentaire ajouté à #$IssueNumber" -ForegroundColor Green
            return $true
        }
        else {
            Write-Host "[ERREUR] Échec de l'ajout du commentaire" -ForegroundColor Red
            return $false
        }
    }
    finally {
        Remove-Item $tempFile -ErrorAction SilentlyContinue
    }
}
```

### Récupérer la colonne actuelle d'une issue
```powershell
function Get-CurrentIssueColumn {
    param([int]$IssueNumber)
    
    $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
    $item = $items.items | Where-Object { $_.content.number -eq $IssueNumber }
    
    if ($item) {
        return $item.status
    }
    return $null
}
```

## Templates de commentaires

### Issue validée - Backend API
```powershell
$validBackendComment = @"
## 🤖 Analyse automatique terminée

### ✅ Issue validée - Scope: **Backend API** (Clean Vertical Slice)

**Analyse du codebase:**
$apiAnalysis

**Feature:** $featureName

**Éléments à créer:**
| Type | Nom | Chemin |
|------|-----|--------|
| Command | $commandName | ``Backend/Depensio.Application/Features/$feature/Commands/`` |
| Handler | $handlerName | ``Backend/Depensio.Application/Features/$feature/Commands/`` |
| Validator | $validatorName | ``Backend/Depensio.Application/Features/$feature/Commands/`` |
| Endpoint | $endpointName | ``Backend/Depensio.Api/Endpoints/$feature/`` |

**Scénarios Gherkin générés:** $scenarioCount

---
*🤖 Agent: backend-analyzer | ⏱️ $(Get-Date -Format "yyyy-MM-dd HH:mm")*
"@
```

### Issue validée - Frontend Blazor
```powershell
$validFrontendComment = @"
## 🤖 Analyse automatique terminée

### ✅ Issue validée - Scope: **Frontend Blazor Hybrid**

**Analyse des composants:**
$blazorAnalysis

**Éléments à créer:**
| Type | Nom | Chemin |
|------|-----|--------|
| Page | $pageName | ``Frontend/Depensio.Shared/Pages/$feature/`` |
| Component | $componentName | ``Frontend/Depensio.Shared/Components/$feature/`` |
| Service | $serviceName | ``Frontend/Depensio.Shared/Services/`` |

**Utilisation IDR.Library.Blazor:**
$idrComponents

---
*🤖 Agent: frontend-analyzer | ⏱️ $(Get-Date -Format "yyyy-MM-dd HH:mm")*
"@
```

### Issue bloquée - Clarification nécessaire
```powershell
$blockedClarificationComment = @"
## 🤖 Analyse automatique terminée

### ❓ Issue bloquée - **Clarification nécessaire**

**Problème:** Informations insuffisantes pour l'analyse.

**Informations manquantes:**
$($missingInfo | ForEach-Object { "- [ ] $_" } | Out-String)

**Template suggéré:**
``````markdown
## User Story
En tant que [rôle]
Je veux [action]
Afin de [bénéfice]

## Critères d'acceptation
- [ ] Critère 1
- [ ] Critère 2

## Scope
- [ ] Backend API
- [ ] Frontend Blazor
- [ ] Microservice: ___________
``````

**Actions requises:**
1. Compléter l'issue avec les informations manquantes
2. Remettre dans **Analyse**

---
*🤖 Agent: analysis-bot | ⏱️ $(Get-Date -Format "yyyy-MM-dd HH:mm") | Raison: NEEDS_CLARIFICATION*
"@
```

### Issue bloquée - Contradiction
```powershell
$blockedContradictionComment = @"
## 🤖 Analyse automatique terminée

### ⛔ Issue BLOQUÉE - **Contradiction avec le code existant**

**Problème:** Cette demande entre en conflit avec la logique actuelle du projet.

**Conflits détectés:**
$conflictDetails

**Actions requises:**
1. Revoir la demande pour éviter la contradiction
2. Ou créer une issue préalable pour modifier le code existant
3. Remettre dans **Analyse** une fois résolu

---
*🤖 Agent: analysis-bot | ⏱️ $(Get-Date -Format "yyyy-MM-dd HH:mm") | Raison: CONTRADICTION*
"@
```

### Issue bloquée - Redondance
```powershell
$blockedRedundancyComment = @"
## 🤖 Analyse automatique terminée

### 🔄 Issue bloquée - **Redondance détectée**

**Problème:** Cette fonctionnalité semble déjà exister.

**Éléments similaires trouvés:**
$redundancyDetails

**Actions requises:**
1. Vérifier si c'est une **amélioration** de l'existant
2. Si doublon → fermer l'issue
3. Si extension → reformuler pour clarifier

---
*🤖 Agent: analysis-bot | ⏱️ $(Get-Date -Format "yyyy-MM-dd HH:mm") | Raison: REDUNDANCY*
"@
```

## Format de réponse

```json
{
  "action": "approve|block",
  "issue_number": 42,
  "scope": "backend|frontend|microservice|fullstack",
  "service_name": "ProduitService",
  "block_reason": null,
  "target_column": "Todo|AnalyseBlock",
  "limits_check": {
    "claude_limit_ok": true,
    "github_limit_ok": true,
    "can_proceed": true
  },
  "column_comparison": {
    "actual": "a tester",
    "expected": "A Tester",
    "match": true
  },
  "labels_added": ["analyzed", "api", "ready-for-dev"],
  "labels_removed": ["needs-analysis"],
  "comment_added": true,
  "moved": true,
  "timestamp": "2024-01-15T14:30:00Z"
}
```
