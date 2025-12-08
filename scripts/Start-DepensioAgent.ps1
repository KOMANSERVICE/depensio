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
    [ValidateSet("claude-opus-4-5-20251101", "claude-sonnet-4-5-20250514", "claude-sonnet-4-20250514")]
    [string]$Model = "claude-opus-4-5-20251101",
    [switch]$DryRun,
    [switch]$ShowDetails
)

$ErrorActionPreference = "Stop"

# ============================================
# CONFIGURATION
# ============================================

$ProjectPath = Resolve-Path $ProjectPath -ErrorAction Stop
Write-Host "[CONFIG] Repertoire du projet: $ProjectPath" -ForegroundColor Cyan

$claudeDir = Join-Path $ProjectPath ".claude"
if (-not (Test-Path $claudeDir)) {
    Write-Host "[ERREUR] Le repertoire .claude n'existe pas dans $ProjectPath" -ForegroundColor Red
    exit 1
}

# Verifier les fichiers agents
$agentFiles = @{
    Orchestrator = Join-Path $claudeDir "orchestrator.md"
    Coder = Join-Path $claudeDir "coder.md"
    Debug = Join-Path $claudeDir "debug-agent.md"
    BackendAnalyzer = Join-Path $claudeDir "backend-analyzer.md"
    FrontendAnalyzer = Join-Path $claudeDir "frontend-analyzer.md"
    GithubManager = Join-Path $claudeDir "github-manager.md"
    GherkinGenerator = Join-Path $claudeDir "gherkin-generator.md"
}

foreach ($agent in $agentFiles.GetEnumerator()) {
    if (-not (Test-Path $agent.Value)) {
        Write-Host "[WARN] Fichier agent manquant: $($agent.Key) -> $($agent.Value)" -ForegroundColor Yellow
    }
}

$env:GITHUB_OWNER = $Owner
$env:GITHUB_REPO = $Repo
$env:PROJECT_NUMBER = $ProjectNumber
$env:GITHUB_OWNER_PACKAGE = $Owner_package
$env:GITHUB_REPO_PACKAGE = $Repo_package
$env:PROJECT_NUMBER_PACKAGE = $ProjectNumber_package
$env:CLAUDE_MODEL = $Model

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
$script:LastClaudeOutput = ""

# ============================================
# FONCTIONS DE VERIFICATION DES LIMITES
# ============================================

function Test-CanProceed {
    if ($script:ClaudeLimitReached) {
        Write-Host "[LIMIT] Limite Claude atteinte - ARRET" -ForegroundColor Red
        return $false
    }
    if ($script:GitHubLimitReached) {
        Write-Host "[LIMIT] Limite GitHub atteinte - ARRET" -ForegroundColor Red
        return $false
    }
    if ($script:CriticalErrorOccurred) {
        Write-Host "[CRITICAL] Erreur critique - ARRET" -ForegroundColor Red
        return $false
    }
    return $true
}

# ============================================
# FONCTION DE COMPARAISON DE COLONNES
# ============================================

function Compare-ColumnName {
    param(
        [string]$Actual,
        [string]$Expected
    )
    if (-not $Actual -or -not $Expected) { return $false }
    $normalizedActual = ($Actual -replace '\s+', '').Trim().ToLower()
    $normalizedExpected = ($Expected -replace '\s+', '').Trim().ToLower()
    return $normalizedActual -eq $normalizedExpected
}

# ============================================
# FONCTION POUR CHARGER UN AGENT
# ============================================

function Get-AgentInstructions {
    param([string]$AgentPath)
    
    if (Test-Path $AgentPath) {
        return Get-Content $AgentPath -Raw
    }
    return ""
}

# ============================================
# FONCTION POUR AJOUTER UN COMMENTAIRE
# ============================================

function Add-IssueComment {
    param(
        [int]$IssueNumber,
        [string]$Comment
    )
    
    if (-not $Comment -or $Comment.Trim().Length -eq 0) {
        return $false
    }
    
    Write-Host "[COMMENT] Ajout commentaire sur #$IssueNumber..." -ForegroundColor Cyan
    
    try {
        $tempFile = Join-Path $env:TEMP "comment-$IssueNumber-$(Get-Date -Format 'yyyyMMddHHmmss').md"
        $Comment | Out-File -FilePath $tempFile -Encoding utf8
        
        $result = gh issue comment $IssueNumber --repo "$($env:GITHUB_OWNER)/$($env:GITHUB_REPO)" --body-file $tempFile 2>&1
        
        Remove-Item $tempFile -ErrorAction SilentlyContinue
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] Commentaire ajoute sur #$IssueNumber" -ForegroundColor Green
            return $true
        }
        else {
            Write-Host "[ERREUR] Echec ajout commentaire: $result" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "[ERREUR] Exception commentaire: $_" -ForegroundColor Red
        return $false
    }
}

# ============================================
# FONCTION POUR EXECUTER CLAUDE AVEC AGENT
# ============================================

function Invoke-ClaudeWithAgent {
    param(
        [Parameter(Mandatory)]
        [string]$AgentFile,
        [Parameter(Mandatory)]
        [string]$TaskPrompt,
        [string]$TaskDescription = "Tache",
        [int]$IssueNumber = 0
    )
    
    if (-not (Test-CanProceed)) {
        Write-Host "[SKIP] $TaskDescription - limite atteinte" -ForegroundColor Yellow
        return @{ Success = $false; Output = "" }
    }
    
    # Charger les instructions de l'agent
    $agentInstructions = Get-AgentInstructions -AgentPath $AgentFile
    
    if (-not $agentInstructions) {
        Write-Host "[WARN] Instructions agent non trouvees: $AgentFile" -ForegroundColor Yellow
    }
    
    # Construire le prompt complet
    $fullPrompt = @"
$agentInstructions

---
# TACHE ACTUELLE
$TaskPrompt
"@
    
    $promptFile = Join-Path $env:TEMP "claude-prompt-$(Get-Date -Format 'yyyyMMddHHmmss').md"
    $fullPrompt | Out-File $promptFile -Encoding utf8
    
    $originalLocation = Get-Location
    $success = $false
    $output = ""
    
    try {
        Set-Location $ProjectPath
        Write-Host "[CLAUDE] Execution: $TaskDescription..." -ForegroundColor Cyan
        
        $promptContent = Get-Content $promptFile -Raw
        $result = $promptContent | claude --model $env:CLAUDE_MODEL 2>&1
        $output = $result -join "`n"
        
        $script:LastClaudeOutput = $output
        
        if ($result -match "rate limit|limit reached|too many requests|429") {
            $script:ClaudeLimitReached = $true
            Write-Host "[LIMIT] Limite Claude detectee" -ForegroundColor Red
            return @{ Success = $false; Output = $output }
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] $TaskDescription termine" -ForegroundColor Green
            $success = $true
            
            # Ajouter automatiquement un commentaire si IssueNumber est fourni
            if ($IssueNumber -gt 0 -and $output.Length -gt 0) {
                Add-IssueComment -IssueNumber $IssueNumber -Comment $output
            }
        }
        else {
            Write-Host "[ERREUR] $TaskDescription echoue (code: $LASTEXITCODE)" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "[ERREUR] Exception: $_" -ForegroundColor Red
    }
    finally {
        Set-Location $originalLocation
        Remove-Item $promptFile -ErrorAction SilentlyContinue
    }
    
    return @{ Success = $success; Output = $output }
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
        Write-Host "[ERREUR] Impossible de recuperer les issues: $_" -ForegroundColor Red
        return @()
    }
}

function Get-IssueDetails {
    param([int]$IssueNumber)
    
    try {
        $issue = gh issue view $IssueNumber --repo "$($env:GITHUB_OWNER)/$($env:GITHUB_REPO)" --json title,body,labels,comments 2>&1 | ConvertFrom-Json
        return $issue
    }
    catch {
        return $null
    }
}

function Move-IssueToColumn {
    param(
        [int]$IssueNumber,
        [string]$TargetColumn
    )
    
    if (-not (Test-CanProceed)) {
        Write-Host "[ABORT] Deplacement #$IssueNumber annule - limite atteinte" -ForegroundColor Red
        return $false
    }
    
    Write-Host "[MOVE] #$IssueNumber vers '$TargetColumn'..." -ForegroundColor Cyan
    
    try {
        $projectNumber = [int]$env:PROJECT_NUMBER
        $owner = $env:GITHUB_OWNER
        
        $projectJson = gh project view $projectNumber --owner $owner --format json 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERREUR] Impossible de recuperer le projet: $projectJson" -ForegroundColor Red
            return $false
        }
        
        $project = $projectJson | ConvertFrom-Json
        $projectId = $project.id
        
        if (-not $projectId) {
            Write-Host "[ERREUR] ID du projet non trouve" -ForegroundColor Red
            return $false
        }
        
        $itemsJson = gh project item-list $projectNumber --owner $owner --format json 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERREUR] Impossible de recuperer les items: $itemsJson" -ForegroundColor Red
            return $false
        }
        
        $items = $itemsJson | ConvertFrom-Json
        
        if (-not $items -or -not $items.items) {
            Write-Host "[ERREUR] Aucun item dans le projet" -ForegroundColor Red
            return $false
        }
        
        $item = $items.items | Where-Object { $_.content.number -eq $IssueNumber } | Select-Object -First 1
        
        if (-not $item) {
            Write-Host "[ERREUR] Issue #$IssueNumber non trouvee dans le projet" -ForegroundColor Red
            return $false
        }
        
        $itemId = $item.id
        
        $fieldsJson = gh project field-list $projectNumber --owner $owner --format json 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERREUR] Impossible de recuperer les champs: $fieldsJson" -ForegroundColor Red
            return $false
        }
        
        $fields = $fieldsJson | ConvertFrom-Json
        $statusField = $fields.fields | Where-Object { $_.name -eq "Status" } | Select-Object -First 1
        
        if (-not $statusField) {
            Write-Host "[ERREUR] Champ 'Status' non trouve" -ForegroundColor Red
            return $false
        }
        
        $targetOption = $statusField.options | Where-Object { 
            Compare-ColumnName -Actual $_.name -Expected $TargetColumn
        } | Select-Object -First 1
        
        if (-not $targetOption) {
            Write-Host "[ERREUR] Colonne '$TargetColumn' non trouvee" -ForegroundColor Red
            Write-Host "[DEBUG] Colonnes disponibles: $($statusField.options.name -join ', ')" -ForegroundColor DarkGray
            return $false
        }
        
        $result = gh project item-edit --project-id $projectId --id $itemId --field-id $statusField.id --single-select-option-id $targetOption.id 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] #$IssueNumber vers '$($targetOption.name)'" -ForegroundColor Green
            return $true
        }
        else {
            Write-Host "[ERREUR] Echec deplacement: $result" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "[ERREUR] Exception: $_" -ForegroundColor Red
        return $false
    }
}

function Get-CurrentIssueColumn {
    param([int]$IssueNumber)
    
    try {
        $items = gh project item-list $env:PROJECT_NUMBER --owner $env:GITHUB_OWNER --format json 2>$null | ConvertFrom-Json
        $item = $items.items | Where-Object { $_.content.number -eq $IssueNumber } | Select-Object -First 1
        if ($item) {
            return $item.status
        }
    }
    catch { }
    return $null
}

# ============================================
# GESTION DES BRANCHES GIT
# ============================================

function New-FeatureBranch {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    Write-Host "[GIT] Creation de la branche pour #$IssueNumber..." -ForegroundColor Cyan
    
    $originalLocation = Get-Location
    Set-Location $ProjectPath
    
    try {
        $safeName = $Title -replace '[^a-zA-Z0-9]', '-' -replace '-+', '-' -replace '^-|-$', ''
        $safeName = $safeName.Substring(0, [Math]::Min(30, $safeName.Length)).ToLower()
        $branchName = "feature/$IssueNumber-$safeName"
        
        $status = git status --porcelain 2>&1
        if ($status) {
            Write-Host "[GIT] Sauvegarde des changements en cours (stash)..." -ForegroundColor Yellow
            git stash push -m "WIP before issue #$IssueNumber" 2>&1 | Out-Null
        }
        
        Write-Host "[GIT] Checkout main et pull..." -ForegroundColor DarkGray
        git checkout main 2>&1 | Out-Null
        git pull origin main 2>&1 | Out-Null
        
        $existingBranch = git branch --list $branchName 2>&1
        if ($existingBranch) {
            Write-Host "[GIT] Branche '$branchName' existe deja - checkout" -ForegroundColor Yellow
            git checkout $branchName 2>&1 | Out-Null
        }
        else {
            Write-Host "[GIT] Creation branche '$branchName'..." -ForegroundColor DarkGray
            git checkout -b $branchName 2>&1 | Out-Null
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] Branche '$branchName' prete" -ForegroundColor Green
            return $branchName
        }
        else {
            Write-Host "[ERREUR] Echec creation branche" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "[ERREUR] Exception Git: $_" -ForegroundColor Red
        return $null
    }
    finally {
        Set-Location $originalLocation
    }
}

function Remove-FeatureBranch {
    param([string]$BranchName)
    
    if (-not $BranchName) { return }
    
    Write-Host "[GIT] Suppression de la branche '$BranchName'..." -ForegroundColor Cyan
    
    $originalLocation = Get-Location
    Set-Location $ProjectPath
    
    try {
        git checkout main 2>&1 | Out-Null
        git branch -D $BranchName 2>&1 | Out-Null
        git push origin --delete $BranchName 2>&1 | Out-Null
        git fetch --prune 2>&1 | Out-Null
        
        Write-Host "[OK] Branche '$BranchName' supprimee" -ForegroundColor Green
    }
    catch {
        Write-Host "[WARN] Erreur suppression branche: $_" -ForegroundColor Yellow
    }
    finally {
        Set-Location $originalLocation
    }
}

# ============================================
# GESTION DES ISSUES TRAITEES
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
# AGENTS UTILISANT LES FICHIERS .MD
# ============================================

function Invoke-AnalysisAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return @{ Success = $false; IsBlocked = $false } }
    
    # Charger les details de l'issue
    $issueDetails = Get-IssueDetails -IssueNumber $IssueNumber
    $issueBody = if ($issueDetails) { $issueDetails.body } else { "" }
    
    $taskPrompt = @"
## Issue a analyser: #$IssueNumber - $Title

### Description de l'issue:
$issueBody

### Ta mission:
1. Analyse cette issue selon les instructions de l'agent
2. Verifie si elle est claire et complete
3. Genere les scenarios Gherkin
4. Formate ta reponse selon le format de l'agent

IMPORTANT - A la fin de ton analyse, indique clairement:
- [STATUT: VALIDE] si l'issue est prete pour le developpement
- [STATUT: BLOQUE] si des informations manquent (et explique ce qui manque)
"@
    
    $result = Invoke-ClaudeWithAgent `
        -AgentFile $agentFiles.BackendAnalyzer `
        -TaskPrompt $taskPrompt `
        -TaskDescription "Analyse #$IssueNumber" `
        -IssueNumber $IssueNumber
    
    # Detecter si l'issue est bloquee
    $isBlocked = $false
    if ($result.Output -match "\[STATUT:\s*BLOQUE\]|\[STATUT:\s*BLOCKED\]") {
        $isBlocked = $true
    }
    
    return @{ Success = $result.Success; IsBlocked = $isBlocked }
}

function Invoke-CoderAgent {
    param(
        [int]$IssueNumber,
        [string]$Title,
        [string]$BranchName
    )
    
    if (-not (Test-CanProceed)) { return $false }
    
    # Charger les details de l'issue
    $issueDetails = Get-IssueDetails -IssueNumber $IssueNumber
    $issueBody = if ($issueDetails) { $issueDetails.body } else { "" }
    
    $taskPrompt = @"
## Issue a implementer: #$IssueNumber - $Title

### Description de l'issue:
$issueBody

### Branche de travail: $BranchName
La branche a deja ete creee. Tu es deja dessus.

### Ta mission:
1. Implemente cette issue selon les instructions de l'agent coder.md
2. La branche '$BranchName' est deja creee et active
3. Suis le workflow complet: implementation -> tests -> commit -> PR -> merge
4. Commandes a utiliser:
   - git add . && git commit -m "feat: $Title (#$IssueNumber)"
   - git push -u origin $BranchName
   - gh pr create --title "$Title" --body "Closes #$IssueNumber" --base main
   - gh pr merge --squash --delete-branch

IMPORTANT:
- Utilise UNIQUEMENT IDR.Library.BuildingBlocks dans Domain
- Utilise UNIQUEMENT IDR.Library.Blazor dans Shared
- NE FERME PAS l'issue
- La branche sera supprimee automatiquement par --delete-branch
"@
    
    $result = Invoke-ClaudeWithAgent `
        -AgentFile $agentFiles.Coder `
        -TaskPrompt $taskPrompt `
        -TaskDescription "Developpement #$IssueNumber" `
        -IssueNumber $IssueNumber
    
    return $result.Success
}

function Invoke-DebugAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return @{ Success = $false; BugFound = $false } }
    
    $issueDetails = Get-IssueDetails -IssueNumber $IssueNumber
    $issueBody = if ($issueDetails) { $issueDetails.body } else { "" }
    
    $taskPrompt = @"
## Bug a analyser: #$IssueNumber - $Title

### Description du bug:
$issueBody

### Ta mission:
1. Analyse ce bug en profondeur selon les instructions de debug-agent.md
2. Utilise toutes les techniques d'analyse: statique, flux de donnees, patterns
3. Documente ton analyse

IMPORTANT - A la fin de ton analyse, indique clairement:
- [STATUT: BUG_TROUVE] si tu as identifie le bug et propose une solution
- [STATUT: BUG_NON_TROUVE] si tu n'as pas trouve le bug (liste les hypotheses eliminees)
"@
    
    $result = Invoke-ClaudeWithAgent `
        -AgentFile $agentFiles.Debug `
        -TaskPrompt $taskPrompt `
        -TaskDescription "Debug #$IssueNumber" `
        -IssueNumber $IssueNumber
    
    $bugFound = $false
    if ($result.Output -match "\[STATUT:\s*BUG_TROUVE\]|\[STATUT:\s*BUG_FOUND\]") {
        $bugFound = $true
    }
    
    return @{ Success = $result.Success; BugFound = $bugFound }
}

function Invoke-ReviewAgent {
    param(
        [int]$IssueNumber,
        [string]$Title
    )
    
    if (-not (Test-CanProceed)) { return $false }
    
    $taskPrompt = @"
## Issue a finaliser: #$IssueNumber - $Title

### Ta mission:
1. Verifie s'il y a une PR ouverte pour cette issue
2. Si PR existe: review le code et merge
3. Apres merge: verifie que la branche est supprimee

Commandes utiles:
- gh pr list --search "$IssueNumber"
- gh pr view <pr-number>
- gh pr merge <pr-number> --squash --delete-branch

IMPORTANT:
- NE FERME PAS l'issue (le testeur le fera)
- Assure-toi que la branche est supprimee
"@
    
    $result = Invoke-ClaudeWithAgent `
        -AgentFile $agentFiles.GithubManager `
        -TaskPrompt $taskPrompt `
        -TaskDescription "Review #$IssueNumber" `
        -IssueNumber $IssueNumber
    
    return $result.Success
}

# ============================================
# MISE A JOUR DES PACKAGES IDR
# ============================================

function Update-IDRPackages {
    Write-Host "[PACKAGES] Verification des mises a jour IDR..." -ForegroundColor Cyan
    
    $originalLocation = Get-Location
    Set-Location $ProjectPath
    
    try {
        # IDR.Library.BuildingBlocks - UNIQUEMENT depensio.Domain
        $domainProject = Get-ChildItem -Path "." -Filter "*.Domain.csproj" -Recurse | Select-Object -First 1
        if ($domainProject) {
            $content = Get-Content $domainProject.FullName -Raw
            if ($content -match "IDR\.Library\.BuildingBlocks") {
                Write-Host "[PACKAGES] Verification BuildingBlocks dans $($domainProject.Name)..." -ForegroundColor DarkGray
                dotnet add $domainProject.FullName package IDR.Library.BuildingBlocks --prerelease 2>$null
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "[OK] BuildingBlocks verifie/mis a jour" -ForegroundColor Green
                }
            }
        }
        
        # IDR.Library.Blazor - UNIQUEMENT depensio.Shared
        $sharedProject = Get-ChildItem -Path "." -Filter "*.Shared.csproj" -Recurse | Select-Object -First 1
        if ($sharedProject) {
            $content = Get-Content $sharedProject.FullName -Raw
            if ($content -match "IDR\.Library\.Blazor") {
                Write-Host "[PACKAGES] Verification Blazor dans $($sharedProject.Name)..." -ForegroundColor DarkGray
                dotnet add $sharedProject.FullName package IDR.Library.Blazor --prerelease 2>$null
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "[OK] Blazor verifie/mis a jour" -ForegroundColor Green
                }
            }
        }
    }
    catch {
        Write-Host "[WARN] Erreur verification packages: $_" -ForegroundColor Yellow
    }
    finally {
        Set-Location $originalLocation
    }
}

# ============================================
# BOUCLE PRINCIPALE
# ============================================

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "              AGENT AUTONOME DEPENSIO                           " -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Owner: $Owner" -ForegroundColor White
Write-Host "  Repo: $Repo" -ForegroundColor White
Write-Host "  Project: #$ProjectNumber" -ForegroundColor White
Write-Host "  Modele: $Model" -ForegroundColor White
Write-Host "  Intervalle: ${PollingInterval}s" -ForegroundColor White
Write-Host "  Agents: $claudeDir" -ForegroundColor White
Write-Host ""

Update-IDRPackages

while ($true) {
    $timestamp = Get-Date -Format "HH:mm:ss"
    
    if ($script:CriticalErrorOccurred) {
        Write-Host "[$timestamp] [STOP] Erreur critique - arret" -ForegroundColor Red
        break
    }
    
    if ($script:ClaudeLimitReached) {
        Write-Host "[$timestamp] [LIMIT] En attente - limite Claude" -ForegroundColor Yellow
        Start-Sleep -Seconds $PollingInterval
        $script:ClaudeLimitReached = $false
        continue
    }
    
    try {
        Write-Host ""
        Write-Host "[$timestamp] ================================================" -ForegroundColor DarkGray
        
        # PRIORITE 1: IN REVIEW
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITE 1] Verification 'In Review'..." -ForegroundColor Magenta
            
            $reviewIssues = @(Get-IssuesInColumn -ColumnName $Columns.Review)
            
            if ($reviewIssues.Count -gt 0) {
                $issue = $reviewIssues[0]
                Write-Host "[$timestamp]   -> #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                $success = Invoke-ReviewAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
                
                if ($success) {
                    Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.ATester
                }
            }
            else {
                Write-Host "[$timestamp]   Aucune PR en attente" -ForegroundColor DarkGray
            }
        }
        
        # PRIORITE 2: IN PROGRESS
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITE 2] Verification 'In Progress'..." -ForegroundColor Yellow
            
            $progressIssues = @(Get-IssuesInColumn -ColumnName $Columns.InProgress)
            
            if ($progressIssues.Count -gt 0) {
                $issue = $progressIssues[0]
                Write-Host "[$timestamp]   -> #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                
                $branchName = New-FeatureBranch -IssueNumber $issue.IssueNumber -Title $issue.Title
                
                if ($branchName) {
                    $success = Invoke-CoderAgent -IssueNumber $issue.IssueNumber -Title $issue.Title -BranchName $branchName
                    
                    if ($success) {
                        Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.Review
                        Remove-FeatureBranch -BranchName $branchName
                    }
                }
            }
            else {
                Write-Host "[$timestamp]   Aucun developpement en cours" -ForegroundColor DarkGray
            }
        }
        
        # PRIORITE 3: DEBUG
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITE 3] Verification 'Debug'..." -ForegroundColor Red
            
            $debugIssues = @(Get-IssuesInColumn -ColumnName $Columns.Debug)
            
            if ($debugIssues.Count -gt 0) {
                $issue = $debugIssues[0]
                Write-Host "[$timestamp]   -> #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                $debugResult = Invoke-DebugAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
                
                if ($debugResult.Success -and (Test-CanProceed)) {
                    if ($debugResult.BugFound) {
                        Write-Host "[$timestamp]   [BUG] Bug trouve - deplacement vers 'In Progress'" -ForegroundColor Yellow
                        Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.InProgress
                    }
                    else {
                        Write-Host "[$timestamp]   [INFO] Bug non trouve - reste en 'Debug' pour review humaine" -ForegroundColor Cyan
                    }
                }
            }
            else {
                Write-Host "[$timestamp]   Aucun bug a analyser" -ForegroundColor DarkGray
            }
        }
        
        # PRIORITE 4: ANALYSE
        if (-not $CoderOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITE 4] Verification 'Analyse'..." -ForegroundColor Blue
            
            $analysisIssues = @(Get-IssuesInColumn -ColumnName $Columns.Analyse)
            $processedAnalysis = @(Get-ProcessedIssues -FilePath $processedAnalysisFile)
            $newAnalysisIssues = @($analysisIssues | Where-Object { $_.IssueNumber -notin $processedAnalysis })
            
            if ($newAnalysisIssues.Count -gt 0) {
                foreach ($issue in $newAnalysisIssues) {
                    if (-not (Test-CanProceed)) { break }
                    
                    Write-Host "[$timestamp]   -> #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                    $analysisResult = Invoke-AnalysisAgent -IssueNumber $issue.IssueNumber -Title $issue.Title
                    
                    if ($analysisResult.Success -and (Test-CanProceed)) {
                        if ($analysisResult.IsBlocked) {
                            Write-Host "[$timestamp]   [BLOCK] Issue #$($issue.IssueNumber) bloquee - informations manquantes" -ForegroundColor Yellow
                            $moved = Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.AnalyseBlock
                        }
                        else {
                            Write-Host "[$timestamp]   [VALID] Issue #$($issue.IssueNumber) valide" -ForegroundColor Green
                            $moved = Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.Todo
                        }
                        
                        if ($moved) {
                            Add-ProcessedIssue -FilePath $processedAnalysisFile -IssueNumber $issue.IssueNumber
                        }
                    }
                }
            }
            else {
                Write-Host "[$timestamp]   Aucune nouvelle analyse" -ForegroundColor DarkGray
            }
        }
        
        # PRIORITE 5: TODO
        if (-not $AnalysisOnly -and (Test-CanProceed)) {
            Write-Host "[$timestamp] [PRIORITE 5] Verification 'Todo'..." -ForegroundColor Green
            
            $todoIssues = @(Get-IssuesInColumn -ColumnName $Columns.Todo)
            $processedCoder = @(Get-ProcessedIssues -FilePath $processedCoderFile)
            $newTodoIssues = @($todoIssues | Where-Object { $_.IssueNumber -notin $processedCoder })
            
            if ($newTodoIssues.Count -gt 0) {
                $issue = $newTodoIssues[0]
                Write-Host "[$timestamp]   -> #$($issue.IssueNumber): $($issue.Title)" -ForegroundColor White
                
                $branchName = New-FeatureBranch -IssueNumber $issue.IssueNumber -Title $issue.Title
                
                if ($branchName) {
                    $moved = Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.InProgress
                    
                    if ($moved) {
                        $success = Invoke-CoderAgent -IssueNumber $issue.IssueNumber -Title $issue.Title -BranchName $branchName
                        
                        if ($success -and (Test-CanProceed)) {
                            $movedToReview = Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.Review
                            if ($movedToReview) {
                                Add-ProcessedIssue -FilePath $processedCoderFile -IssueNumber $issue.IssueNumber
                                Write-Host "[$timestamp]   [OK] Issue #$($issue.IssueNumber) developpee" -ForegroundColor Green
                                Remove-FeatureBranch -BranchName $branchName
                            }
                        }
                        elseif ($script:ClaudeLimitReached) {
                            Move-IssueToColumn -IssueNumber $issue.IssueNumber -TargetColumn $Columns.Todo
                        }
                    }
                }
            }
            else {
                Write-Host "[$timestamp]   Aucune nouvelle tache" -ForegroundColor DarkGray
            }
        }
    }
    catch {
        Write-Host "[$timestamp] [ERREUR] Exception: $_" -ForegroundColor Red
    }
    
    Write-Host "[$timestamp] [WAIT] Prochaine verification dans ${PollingInterval}s..." -ForegroundColor DarkGray
    Start-Sleep -Seconds $PollingInterval
}

