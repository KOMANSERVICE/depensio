# scripts/Start-DepensioAgent.ps1
# Agent autonome pour Depensio

param(
    [string]$Owner = "KOMANSERVICE",
    [string]$Repo = "depensio",
    [int]$ProjectNumber = 1,
    [int]$PollingInterval = 60,

    # Chemin vers le projet (OBLIGATOIRE)
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = ".",
    
    # Configuration du repo des packages IDR
    [string]$Owner_package = "KOMANSERVICE",
    [string]$Repo_package = "IDR.Library",
    [int]$ProjectNumber_package = 4,
    
    # Modes de fonctionnement
    [switch]$AnalysisOnly,
    [switch]$CoderOnly,
    
    # Choix du modèle Claude
    [ValidateSet("claude-opus-4-5-20251101", "claude-sonnet-4-5-20250514", "claude-sonnet-4-20250514")]
    [string]$Model = "claude-opus-4-5-20251101",
    
    # Options avancées
    [switch]$DryRun,
    [switch]$ShowDetails
)

$ErrorActionPreference = "Stop"

# ============================================
# CONFIGURATION
# ============================================

$ProjectPath = Resolve-Path $ProjectPath -ErrorAction Stop
Write-Host "[CONFIG] Répertoire du projet: $ProjectPath" -ForegroundColor Cyan

if (-not (Test-Path (Join-Path $ProjectPath ".claude"))) {
    Write-Host "[ERREUR] Le répertoire .claude n'existe pas dans $ProjectPath" -ForegroundColor Red
    exit 1
}

# Variables projet principal
$env:GITHUB_OWNER = $Owner
$env:GITHUB_REPO = $Repo
$env:PROJECT_NUMBER = $ProjectNumber

# Variables repo packages IDR
$env:GITHUB_OWNER_PACKAGE = $Owner_package
$env:GITHUB_REPO_PACKAGE = $Repo_package
$env:PROJECT_NUMBER_PACKAGE = $ProjectNumber_package

$env:CLAUDE_MODEL = $Model

# Colonnes du Project Board
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

# ============================================
# VARIABLES GLOBALES - LIMITES
# ============================================

$script:ClaudeLimitReached = $false
$script:GitHubLimitReached = $false
$script:CriticalErrorOccurred = $false
$script:CriticalErrorMessage = ""

# ============================================
# FONCTIONS DE VÉRIFICATION DES LIMITES
# ============================================

function Test-CanProceed {
    <#
    .SYNOPSIS
        Vérifie si les limites Claude ET GitHub permettent de continuer
    #>
    
    if ($script:ClaudeLimitReached) {
        Write-Host "[LIMIT] ⛔ Limite Claude atteinte - ARRÊT" -ForegroundColor Red
        return $false
    }
    
    if ($script:GitHubLimitReached) {
        Write-Host "[LIMIT] ⛔ Limite GitHub atteinte - ARRÊT" -ForegroundColor Red
        return $false
    }
    
    if ($script:CriticalErrorOccurred) {
        Write-Host "[CRITICAL] ⛔ Erreur critique - ARRÊT" -ForegroundColor Red
        return $false
    }
    
    return $true
}

function Stop-OnCriticalError {
    param(
        [string]$ErrorMessage,
        [string]$Component = "Système"
    )
    
    $script:CriticalErrorOccurred = $true
    $script:CriticalErrorMessage = $ErrorMessage
    
    Write-Host ""
    Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Red
    Write-Host "║                    ERREUR CRITIQUE - ARRÊT                   ║" -ForegroundColor Red
    Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Red
    Write-Host ""
    Write-Host "  Composant: $Component" -ForegroundColor Red
    Write-Host "  Erreur: $ErrorMessage" -ForegroundColor Red
    Write-Host ""
}

# ============================================
# FONCTION DE COMPARAISON DE COLONNES
# ============================================

function Compare-ColumnName {
    param(
        [string]$Actual,
        [string]$Expected
    )
    
    $normalizedActual = ($Actual -replace '\s+', '').Trim().ToLower()
    $normalizedExpected = ($Expected -replace '\s+', '').Trim().ToLower()
    
    return $normalizedActual -eq $normalizedExpected
}

# ============================================
# FONCTION POUR EXÉCUTER CLAUDE
# ============================================

function Invoke-ClaudeInProject {
    param(
        [Parameter(Mandatory)]
        [string]$PromptFile,
        
        [string]$TaskDescription = "Tâche"
    )
    
    # Vérifier les limites
    if (-not (Test-CanProceed)) {
        Write-Host "[SKIP] $TaskDescription - limite atteinte" -ForegroundColor Yellow
        Remove-Item $PromptFile -ErrorAction SilentlyContinue
        return $false
    }
    
    $originalLocation = Get-Location
    $success = $false
    
    try {
        Set-Location $ProjectPath
        
        Write-Host "[CLAUDE] Exécution: $TaskDescription..." -ForegroundColor Cyan
        
        # Exécuter Claude
        $result = claude --model $env:CLAUDE_MODEL --print-markdown < $PromptFile 2>&1
        
        # Vérifier si limite atteinte
        if ($result -match "rate limit|limit reached|too many requests|429") {
            $script:ClaudeLimitReached = $true
            Write-Host "[LIMIT] Limite Claude détectée" -ForegroundColor Red
            return $false
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] $TaskDescription terminé" -ForegroundColor Green
            $success = $true
        }
        else {
            Write-Host "[ERREUR] $TaskDescription échoué (code: $LASTEXITCODE)" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "[ERREUR] Exception: $_" -ForegroundColor Red
    }
    finally {
        Set-Location $originalLocation
        Remove-Item $PromptFile -ErrorAction SilentlyContinue
    }
    
    return $success
}

# ============================================
# FONCTIONS GITHUB
# ============================================

function Get-IssuesInColumn {
    param([string]$ColumnName)
    
    try {
        $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json 2>$null | ConvertFrom-Json
        
        if (-not $items -or -not $items.items) {
            return @()
        }
        
        # Filtrer par colonne (CASE-INSENSITIVE)
        $filtered = $items.items | Where-Object { 
            Compare-ColumnName -Actual $_.status -Expected $ColumnName
        }
        
        return @($filtered | ForEach-Object {
            [PSCustomObject]@{
                IssueNumber = $_.content.number
                Title = $_.content.title
                Status = $_.status
                ItemId = $_.id
            }
        })
    }
    catch {
        Write-Host "[ERREUR] Impossible de récupérer les issues: $_" -ForegroundColor Red
        return @()
    }
}

function Move-IssueToColumn {
    param(
        [int]$IssueNumber,
        [string]$TargetColumn
    )
    
    # VÉRIFIER LES LIMITES D'ABORD
    if (-not (Test-CanProceed)) {
        Write-Host "[ABORT] Déplacement #$IssueNumber annulé - limite atteinte" -ForegroundColor Red
        return $false
    }
    
    Write-Host "[MOVE] #$IssueNumber → '$TargetColumn'..." -ForegroundColor Cyan
    
    try {
        $projects = gh project list --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
        $project = $projects | Where-Object { $_.number -eq $env:PROJECT_NUMBER }
        
        $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
        $item = $items.items | Where-Object { $_.content.number -eq $IssueNumber }
        
        if (-not $item) {
            Write-Host "[ERREUR] Issue #$IssueNumber non trouvée" -ForegroundColor Red
            return $false
        }
        
        $fields = gh project field-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
        $statusField = $fields.fields | Where-Object { $_.name -eq "Status" }
        
        # Trouver la colonne (CASE-INSENSITIVE)
        $targetOption = $statusField.options | Where-Object { 
            Compare-ColumnName -Actual $_.name -Expected $TargetColumn
        }
        
        if (-not $targetOption) {
            Write-Host "[ERREUR] Colonne '$TargetColumn' non trouvée" -ForegroundColor Red
            return $false
        }
        
        gh project item-edit `
            --project-id $project.id `
            --id $item.id `
            --field-id $statusField.id `
            --single-select-option-id $targetOption.id
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] #$IssueNumber → '$($targetOption.name)'" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "[ERREUR] Exception: $_" -ForegroundColor Red
    }
    
    return $false
}

function Get-CurrentIssueColumn {
    param([int]$IssueNumber)
    
    try {
        $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json | ConvertFrom-Json
        $item = $items.items | Where-Object { $_.content.number -eq $IssueNumber }
        
        if ($item) {
            return $item.status
        }
    }
    catch { }
    
    return $null
}

# ============================================
# GESTION DES ISSUES TRAITÉES
# ============================================

$architectureDir = Join-Path $ProjectPath ".architecture"
if (-not (Test-Path $architectureDir)) {
    New-Item -ItemType Directory -Path $architectureDir -Force | Out-Null
}

$processedAnalysisFile = Join-Path $architectureDir "processed-analysis.txt"
$processedCoderFile = Join-Path $architectureDir "processed-coder.txt"

function Get-ProcessedIssues {
    param([string]$FilePath)
    
    if (Test-Path $FilePath) {
        return @(Get-Content $FilePath | Where-Object { $_ -match '^\d+$' } | ForEach-Object { [int]$_ })
    }
    return @()
}

function Add-ProcessedIssue {
    param([string]$FilePath, [int]$IssueNumber)
    
    $IssueNumber | Out-File -Append -FilePath $FilePath -Encoding utf8
}

# ============================================
# AGENTS
# ============================================

function Invoke-AnalysisAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return $false }
    
    $promptFile = Join-Path $env:TEMP "analysis-prompt-$IssueNumber.md"
    
    @"
Tu es l'agent d'analyse pour Depensio.

Analyse l'issue #$IssueNumber: "$Title"

1. Lis la documentation IDR.Library (agent-docs)
2. Analyse le code existant pour comprendre l'architecture
3. Vérifie si l'issue est claire et complète
4. Génère les scénarios Gherkin
5. Commente l'issue avec ton analyse
6. **DÉPLACE L'ISSUE** vers "Todo" si valide, "AnalyseBlock" si bloquée

Format User Story: En tant que... Je veux... Afin de...

IMPORTANT: 
- Comparaison de colonnes CASE-INSENSITIVE
- NE JAMAIS déplacer si limite atteinte
- Toujours commenter l'issue
"@ | Out-File $promptFile -Encoding utf8

    return Invoke-ClaudeInProject -PromptFile $promptFile -TaskDescription "Analyse #$IssueNumber"
}

function Invoke-CoderAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return $false }
    
    $promptFile = Join-Path $env:TEMP "coder-prompt-$IssueNumber.md"
    
    @"
Tu es l'agent codeur pour Depensio.

Implémente l'issue #$IssueNumber: "$Title"

Workflow:
1. Lis la documentation IDR.Library (agent-docs)
2. Crée une branche depuis main: git checkout main && git pull && git checkout -b feature/$IssueNumber-xxx
3. Implémente le code
4. Fais une analyse debug approfondie
5. Exécute les tests: dotnet test
6. Commit et push
7. Crée la PR
8. Merge la PR
9. **SUPPRIME LA BRANCHE** (local + remote)
10. **DÉPLACE L'ISSUE** vers "A Tester"

IMPORTANT:
- Utiliser ICommand, IQuery, AbstractValidator de IDR.Library.BuildingBlocks
- Utiliser les composants Idr* de IDR.Library.Blazor
- NE JAMAIS fermer l'issue
- TOUJOURS supprimer la branche après merge
- NE JAMAIS déplacer si limite atteinte
"@ | Out-File $promptFile -Encoding utf8

    return Invoke-ClaudeInProject -PromptFile $promptFile -TaskDescription "Développement #$IssueNumber"
}

function Invoke-DebugAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return $false }
    
    $promptFile = Join-Path $env:TEMP "debug-prompt-$IssueNumber.md"
    
    @"
Tu es l'agent de debug pour Depensio.

Analyse en profondeur l'issue #$IssueNumber: "$Title"

1. Collecte les informations (logs, symptômes)
2. Analyse statique du code suspect
3. Recherche les patterns de bugs connus
4. Trace le flux de données
5. Analyse les dépendances

SI BUG TROUVÉ:
- Documente le bug
- Propose une solution
- Déplace vers "In Progress"

SI BUG NON TROUVÉ:
- Documente l'analyse
- Liste les hypothèses éliminées
- LAISSE en "Debug" pour review humaine
"@ | Out-File $promptFile -Encoding utf8

    return Invoke-ClaudeInProject -PromptFile $promptFile -TaskDescription "Debug #$IssueNumber"
}

function Invoke-ReviewAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return $false }
    
    $promptFile = Join-Path $env:TEMP "review-prompt-$IssueNumber.md"
    
    @"
Tu es l'agent de review pour Depensio.

Finalise l'issue #$IssueNumber: "$Title"

1. Vérifie s'il y a une PR ouverte
2. Si PR existe: review et merge
3. Après merge: **SUPPRIME LA BRANCHE**
4. **DÉPLACE L'ISSUE** vers "A Tester"

IMPORTANT:
- NE JAMAIS fermer l'issue
- TOUJOURS supprimer la branche
"@ | Out-File $promptFile -Encoding utf8

    return Invoke-ClaudeInProject -PromptFile $promptFile -TaskDescription "Review #$IssueNumber"
}

# ============================================
# MISE À JOUR DES PACKAGES IDR
# ============================================

function Update-IDRPackages {
    Write-Host "[PACKAGES] Mise à jour automatique des packages IDR..." -ForegroundColor Cyan
    
    $originalLocation = Get-Location
    Set-Location $ProjectPath
    
    try {
        Get-ChildItem -Path "." -Filter "*.csproj" -Recurse | ForEach-Object {
            dotnet add $_.FullName package IDR.Library.BuildingBlocks --prerelease 2>$null
            dotnet add $_.FullName package IDR.Library.Blazor --prerelease 2>$null
        }
        Write-Host "[OK] Packages IDR mis à jour" -ForegroundColor Green
    }
    catch {
        Write-Host "[WARN] Erreur mise à jour packages: $_" -ForegroundColor Yellow
    }
    finally {
        Set-Location $originalLocation
    }
}

# ============================================
# BOUCLE PRINCIPALE
# ============================================

Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║              AGENT AUTONOME DEPENSIO                         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Owner: $Owner" -ForegroundColor White
Write-Host "  Repo: $Repo" -ForegroundColor White
Write-Host "  Project: #$ProjectNumber" -ForegroundColor White
Write-Host "  Modèle: $Model" -ForegroundColor White
Write-Host "  Intervalle: ${PollingInterval}s" -ForegroundColor White
Write-Host ""

# Mise à jour initiale des packages
Update-IDRPackages

while ($true) {
    $timestamp = Get-Date -Format "HH:mm:ss"
    
    # Vérifier les erreurs critiques
    if ($script:CriticalErrorOccurred) {
        Write-Host "[$timestamp] [STOP] Erreur critique - arrêt" -ForegroundColor Red
        break
    }
    
    # Reset de la limite Claude après un certain temps
    if ($script:ClaudeLimitReached) {
        Write-Host "[$timestamp] [LIMIT] En attente - limite Claude" -ForegroundColor Yellow
        Start-Sleep -Seconds $PollingInterval
        $script:ClaudeLimitReached = $false
        continue
    }
    
    try {
        Write-Host ""
        Write-Host "[$timestamp] ═══════════════════════════════════════════" -ForegroundColor DarkGray
        
        # ============================================
        # PRIORITÉ 1: IN REVIEW (Terminer les PRs)
        # ============================================
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITÉ 1] Vérification 'In Review'..." -ForegroundColor Magenta
            
            $reviewIssues = @(Get-IssuesInColumn -ColumnName $Columns.Review)
            
            if ($reviewIssues.Count -gt 0) {
                $issue = $reviewIssues[0]
                Write-Host "[$timestamp]   → #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                
                $success = Invoke-ReviewAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
                
                if ($script:ClaudeLimitReached) {
                    Write-Host "[$timestamp]   [LIMIT] Limite atteinte pendant review" -ForegroundColor Red
                }
            }
            else {
                Write-Host "[$timestamp]   Aucune PR en attente" -ForegroundColor DarkGray
            }
        }
        
        # ============================================
        # PRIORITÉ 2: IN PROGRESS (Terminer le dev)
        # ============================================
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITÉ 2] Vérification 'In Progress'..." -ForegroundColor Yellow
            
            $progressIssues = @(Get-IssuesInColumn -ColumnName $Columns.InProgress)
            
            if ($progressIssues.Count -gt 0) {
                $issue = $progressIssues[0]
                Write-Host "[$timestamp]   → #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                
                $success = Invoke-CoderAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
            }
            else {
                Write-Host "[$timestamp]   Aucun développement en cours" -ForegroundColor DarkGray
            }
        }
        
        # ============================================
        # PRIORITÉ 3: DEBUG (Analyse des bugs)
        # ============================================
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITÉ 3] Vérification 'Debug'..." -ForegroundColor Red
            
            $debugIssues = @(Get-IssuesInColumn -ColumnName $Columns.Debug)
            
            if ($debugIssues.Count -gt 0) {
                $issue = $debugIssues[0]
                Write-Host "[$timestamp]   → #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                
                $success = Invoke-DebugAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
            }
            else {
                Write-Host "[$timestamp]   Aucun bug à analyser" -ForegroundColor DarkGray
            }
        }
        
        # ============================================
        # PRIORITÉ 4: ANALYSE (Nouvelles issues)
        # ============================================
        if (-not $CoderOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITÉ 4] Vérification 'Analyse'..." -ForegroundColor Blue
            
            $analysisIssues = @(Get-IssuesInColumn -ColumnName $Columns.Analyse)
            $processedAnalysis = @(Get-ProcessedIssues -FilePath $processedAnalysisFile)
            $newAnalysisIssues = @($analysisIssues | Where-Object { $_.IssueNumber -notin $processedAnalysis })
            
            if ($newAnalysisIssues.Count -gt 0) {
                foreach ($issue in $newAnalysisIssues) {
                    if (-not (Test-CanProceed)) { break }
                    
                    Write-Host "[$timestamp]   → #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                    
                    $success = Invoke-AnalysisAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
                    
                    if ($success -and (Test-CanProceed)) {
                        Add-ProcessedIssue -FilePath $processedAnalysisFile -IssueNumber $issue.IssueNumber
                        
                        # Vérifier le déplacement
                        $currentColumn = Get-CurrentIssueColumn -IssueNumber $issue.IssueNumber
                        if (Compare-ColumnName -Actual $currentColumn -Expected $Columns.Analyse) {
                            Write-Host "[$timestamp]   [WARN] Issue non déplacée - forçage vers 'Todo'" -ForegroundColor Yellow
                            Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.Todo
                        }
                    }
                }
            }
            else {
                Write-Host "[$timestamp]   Aucune nouvelle analyse" -ForegroundColor DarkGray
            }
        }
        
        # ============================================
        # PRIORITÉ 5: TODO (Nouveaux développements)
        # ============================================
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITÉ 5] Vérification 'Todo'..." -ForegroundColor Green
            
            $todoIssues = @(Get-IssuesInColumn -ColumnName $Columns.Todo)
            $processedCoder = @(Get-ProcessedIssues -FilePath $processedCoderFile)
            $newTodoIssues = @($todoIssues | Where-Object { $_.IssueNumber -notin $processedCoder })
            
            if ($newTodoIssues.Count -gt 0) {
                $issue = $newTodoIssues[0]
                Write-Host "[$timestamp]   → #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                
                # Déplacer vers In Progress AVANT
                $moved = Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.InProgress
                
                if ($moved) {
                    $success = Invoke-CoderAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
                    
                    if ($success -and (Test-CanProceed)) {
                        Add-ProcessedIssue -FilePath $processedCoderFile -IssueNumber $issue.IssueNumber
                    }
                    elseif ($script:ClaudeLimitReached) {
                        # Remettre dans Todo si limite atteinte
                        Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.Todo
                    }
                }
            }
            else {
                Write-Host "[$timestamp]   Aucune nouvelle tâche" -ForegroundColor DarkGray
            }
        }
    }
    catch {
        Write-Host "[$timestamp] [ERREUR] Exception: $_" -ForegroundColor Red
    }
    
    Write-Host "[$timestamp] [WAIT] Prochaine vérification dans ${PollingInterval}s..." -ForegroundColor DarkGray
    Start-Sleep -Seconds $PollingInterval
}
