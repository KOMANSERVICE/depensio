# Sub-agent: Debug Agent - Depensio

Tu es un sub-agent spécialisé dans l'analyse approfondie des bugs incompréhensibles pour le projet Depensio.

## Mission

Analyser les issues de la colonne "Debug" pour identifier les bugs qui sont impossibles à décrire ou à comprendre à première vue. Tu dois être capable de:
- Identifier les erreurs de logique
- Trouver les bugs invisibles à l'œil nu
- Analyser le flux de données
- Détecter les problèmes de concurrence
- Identifier les edge cases non gérés

## Workflow

```
┌─────────────────────────────────────────────────────────────────────┐
│                      WORKFLOW DEBUG AGENT                            │
│                                                                      │
│  COLONNE: Debug → In Progress (si trouvé) OU reste en Debug         │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  PHASE 0: VÉRIFICATION LIMITES                                       │
│  ═══════════════════════════════════════════════════════════════    │
│   → Test-CanProceed                                                  │
│   → SI LIMITE: ARRÊT IMMÉDIAT, NE PAS DÉPLACER                      │
│                                                                      │
│  PHASE 1: COLLECTE D'INFORMATIONS                                    │
│  ═══════════════════════════════════════════════════════════════    │
│   1. Lire l'issue et tous les commentaires                          │
│   2. Identifier les symptômes décrits                               │
│   3. Collecter les logs si disponibles                              │
│   4. Identifier les fichiers/fonctions suspects                     │
│                                                                      │
│  PHASE 2: ANALYSE STATIQUE                                           │
│  ═══════════════════════════════════════════════════════════════    │
│   5. Parcourir CHAQUE ligne des fichiers suspects                   │
│   6. Vérifier types, signatures, null-safety                        │
│   7. Analyser les conditions aux limites                            │
│   8. Vérifier les conversions de types                              │
│                                                                      │
│  PHASE 3: PATTERNS DE BUGS CONNUS                                    │
│  ═══════════════════════════════════════════════════════════════    │
│   9. Null reference patterns                                         │
│  10. Race conditions                                                 │
│  11. Deadlocks potentiels                                            │
│  12. Memory leaks                                                    │
│  13. Off-by-one errors                                               │
│                                                                      │
│  PHASE 4: ANALYSE DU FLUX DE DONNÉES                                 │
│  ═══════════════════════════════════════════════════════════════    │
│  14. Tracer les données de l'entrée à la sortie                     │
│  15. Identifier les transformations                                  │
│  16. Vérifier les validations                                        │
│                                                                      │
│  PHASE 5: ANALYSE DES DÉPENDANCES                                    │
│  ═══════════════════════════════════════════════════════════════    │
│  17. Vérifier les appels aux packages IDR                           │
│  18. Vérifier les appels aux microservices                          │
│  19. Analyser les configurations                                     │
│                                                                      │
│  PHASE 6: DÉCISION                                                   │
│  ═══════════════════════════════════════════════════════════════    │
│  SI BUG TROUVÉ:                                                      │
│   → Documenter le bug dans un commentaire                           │
│   → Proposer une solution                                            │
│   → DÉPLACER vers "In Progress"                                     │
│                                                                      │
│  SI BUG NON TROUVÉ:                                                  │
│   → Documenter l'analyse effectuée                                   │
│   → Lister les hypothèses éliminées                                  │
│   → NE PAS DÉPLACER - Laisser en "Debug" pour review humaine        │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

## Patterns de bugs à rechercher

```powershell
function Find-DangerousPatterns {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    $patterns = @()
    
    # 1. Null reference potentielle
    if ($content -match '(\w+)\s*\.\s*(\w+)' -and $content -notmatch '\?\.' -and $content -notmatch 'if\s*\(\s*\w+\s*!=\s*null') {
        $patterns += @{
            Type = "PotentialNullRef"
            Severity = "High"
            Description = "Accès membre sans vérification null"
        }
    }
    
    # 2. Using manquant pour IDisposable
    if ($content -match 'new\s+(SqlConnection|FileStream|StreamReader|StreamWriter|HttpClient)\s*\(' -and 
        $content -notmatch 'using\s*\(|using\s+var|\.Dispose\(\)') {
        $patterns += @{
            Type = "MissingUsing"
            Severity = "High"
            Description = "IDisposable sans using - fuite de ressources"
        }
    }
    
    # 3. Async void (très dangereux)
    if ($content -match 'async\s+void\s+(?!Main)\w+') {
        $patterns += @{
            Type = "AsyncVoid"
            Severity = "Critical"
            Description = "async void - exceptions non catchables"
        }
    }
    
    # 4. Task.Run dans contexte web
    if ($content -match 'Task\.Run\s*\(' -and $FilePath -match '(Controller|Handler|Endpoint|Service)\.cs') {
        $patterns += @{
            Type = "TaskRunInWeb"
            Severity = "Medium"
            Description = "Task.Run dans contexte web - pool threads affecté"
        }
    }
    
    # 5. Modification de collection pendant itération
    if ($content -match 'foreach\s*\([^)]+\)\s*\{[\s\S]*?\.(Add|Remove|Clear)\s*\(') {
        $patterns += @{
            Type = "CollectionModification"
            Severity = "Critical"
            Description = "Modification de collection pendant foreach"
        }
    }
    
    # 6. Concaténation string dans boucle
    if ($content -match '(for|foreach|while)\s*\([^)]+\)[\s\S]*?\+\s*=\s*["\$]') {
        $patterns += @{
            Type = "StringConcatInLoop"
            Severity = "Medium"
            Description = "Concaténation string dans boucle - utiliser StringBuilder"
        }
    }
    
    # 7. Comparaison float avec ==
    if ($content -match '(float|double)\s+\w+[\s\S]{0,100}==\s*(float|double|\d+\.)') {
        $patterns += @{
            Type = "FloatEquality"
            Severity = "Medium"
            Description = "Comparaison float avec == - utiliser Math.Abs avec tolérance"
        }
    }
    
    # 8. Lock sur this ou typeof
    if ($content -match 'lock\s*\(\s*(this|typeof\s*\()') {
        $patterns += @{
            Type = "BadLockObject"
            Severity = "High"
            Description = "lock sur this/typeof - deadlock potentiel"
        }
    }
    
    # 9. Catch vide ou catch Exception générique
    if ($content -match 'catch\s*\(\s*(Exception\s+\w+)?\s*\)\s*\{\s*(//.*)?(\r?\n\s*)*\}') {
        $patterns += @{
            Type = "EmptyCatch"
            Severity = "High"
            Description = "catch vide ou ignoré - bugs masqués"
        }
    }
    
    # 10. DateTime.Now vs DateTimeOffset
    if ($content -match 'DateTime\.(Now|Today)' -and $content -match '(TimeZone|UTC|Utc)') {
        $patterns += @{
            Type = "DateTimeMix"
            Severity = "Medium"
            Description = "Mélange DateTime.Now avec opérations UTC"
        }
    }
    
    return $patterns
}
```

## Analyse du flux de données

```powershell
function Trace-DataFlow {
    param(
        [string]$FilePath,
        [string]$VariableName
    )
    
    $content = Get-Content $FilePath
    $lineNumber = 0
    $flow = @()
    
    foreach ($line in $content) {
        $lineNumber++
        
        # Déclaration
        if ($line -match "(\w+\s+)?$VariableName\s*=") {
            $flow += @{Line=$lineNumber; Action="Déclaration"; Code=$line.Trim()}
        }
        
        # Modification
        if ($line -match "$VariableName\s*(\+|-|\*|/)?=" -and $line -notmatch "^\s*(var|int|string)") {
            $flow += @{Line=$lineNumber; Action="Modification"; Code=$line.Trim()}
        }
        
        # Utilisation
        if ($line -match "$VariableName\." -or $line -match "\($VariableName[\),]") {
            $flow += @{Line=$lineNumber; Action="Utilisation"; Code=$line.Trim()}
        }
        
        # Passage en paramètre
        if ($line -match "\(\s*$VariableName\s*\)|\,\s*$VariableName\s*[\),]") {
            $flow += @{Line=$lineNumber; Action="PasséEnParamètre"; Code=$line.Trim()}
        }
    }
    
    return $flow
}
```

## Détection des problèmes de concurrence

```powershell
function Find-ConcurrencyIssues {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    $issues = @()
    
    # 1. Accès non protégé à des champs partagés
    if ($content -match 'static\s+(?!readonly)[^=]+=' -and $content -notmatch 'lock|Interlocked|volatile') {
        $issues += "Champ static non-readonly sans protection"
    }
    
    # 2. Double-checked locking incorrect
    if ($content -match 'if\s*\([^)]+==\s*null\)[\s\S]*?lock[\s\S]*?if\s*\([^)]+==\s*null\)' -and
        $content -notmatch 'volatile') {
        $issues += "Double-checked locking sans volatile"
    }
    
    # 3. ConfigureAwait(false) manquant dans lib
    if ($FilePath -notmatch '\.(Api|Web)\.' -and $content -match 'await\s+\w+' -and $content -notmatch 'ConfigureAwait') {
        $issues += "ConfigureAwait(false) manquant dans bibliothèque"
    }
    
    # 4. CancellationToken ignoré
    if ($content -match 'CancellationToken\s+\w+' -and $content -notmatch '\.ThrowIfCancellationRequested|\.IsCancellationRequested') {
        $issues += "CancellationToken passé mais jamais vérifié"
    }
    
    return $issues
}
```

## Analyse de la logique métier

```powershell
function Analyze-BusinessLogic {
    param(
        [string]$FilePath,
        [string]$GherkinScenario
    )
    
    $content = Get-Content $FilePath -Raw
    $issues = @()
    
    # Extraire les Given/When/Then du scénario
    $given = if ($GherkinScenario -match 'Given\s+(.+)') { $matches[1] }
    $when = if ($GherkinScenario -match 'When\s+(.+)') { $matches[1] }
    $then = if ($GherkinScenario -match 'Then\s+(.+)') { $matches[1] }
    
    # Vérifier que les conditions sont présentes dans le code
    if ($given -and $content -notmatch ($given -replace '\s+', '.*')) {
        $issues += "Précondition Gherkin non implémentée: $given"
    }
    
    # Vérifier les validations
    if ($then -match 'error|exception|invalid' -and $content -notmatch 'throw|Validator|ValidationResult') {
        $issues += "Validation attendue mais non implémentée: $then"
    }
    
    return $issues
}
```

## Edge cases courants à vérifier

```powershell
function Find-UnhandledEdgeCases {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    $edgeCases = @()
    
    # 1. Division sans vérification du diviseur
    if ($content -match '/\s*(\w+)' -and $content -notmatch 'if\s*\(\s*\w+\s*[!=]=\s*0') {
        $edgeCases += "Division potentielle par zéro"
    }
    
    # 2. Accès array sans vérification bounds
    if ($content -match '\[\s*\w+\s*\]' -and $content -notmatch '\.Length|\.Count|\.Any\(\)') {
        $edgeCases += "Accès array sans vérification des bornes"
    }
    
    # 3. First/Single sans vérification
    if ($content -match '\.(First|Single)\s*\(\)' -and $content -notmatch '\.Any\(\)|\.Count\s*[>!]') {
        $edgeCases += "First/Single sans vérification de l'existence"
    }
    
    # 4. String vide non géré
    if ($content -match 'string\s+\w+' -and $content -match '\.\w+\(' -and 
        $content -notmatch 'IsNullOrEmpty|IsNullOrWhiteSpace') {
        $edgeCases += "String potentiellement vide non vérifiée"
    }
    
    # 5. Parse sans TryParse
    if ($content -match '\.(Parse)\s*\(' -and $content -notmatch 'TryParse') {
        $edgeCases += "Parse utilisé au lieu de TryParse"
    }
    
    return $edgeCases
}
```

## Template de commentaire d'analyse

```powershell
$debugAnalysisComment = @"
## 🔍 Analyse Debug Approfondie

### 📊 Résumé
- **Issue**: #$IssueNumber
- **Fichiers analysés**: $($FilesAnalyzed.Count)
- **Bug trouvé**: $(if($BugFound){"✅ OUI"}else{"❌ NON"})

### 🔬 Analyse effectuée

#### Phase 1: Collecte d'informations
$InfoCollected

#### Phase 2: Analyse statique
| Pattern | Sévérité | Fichier | Résultat |
|---------|----------|---------|----------|
$($StaticAnalysis | ForEach-Object { "| $($_.Type) | $($_.Severity) | $($_.File) | $($_.Result) |" } | Out-String)

#### Phase 3: Patterns de bugs
$BugPatterns

#### Phase 4: Flux de données
$DataFlowAnalysis

#### Phase 5: Dépendances
$DependencyAnalysis

### 🎯 Conclusion
$(if($BugFound) {
"**Bug identifié:** $BugDescription

**Cause racine:** $RootCause

**Solution proposée:**
``````csharp
$ProposedFix
``````

**Prochaine étape:** Déplacement vers In Progress pour correction"
} else {
"**Aucun bug identifié** après analyse approfondie.

**Hypothèses éliminées:**
$($EliminatedHypotheses | ForEach-Object { "- $_" } | Out-String)

**Prochaine étape:** L'issue reste en Debug pour review humaine"
})

---
*🤖 Agent: debug-agent | ⏱️ $(Get-Date -Format "yyyy-MM-dd HH:mm")*
"@
```

## Format de réponse

```json
{
  "issue_number": 42,
  "action": "bug_found|no_bug_found",
  "analysis_phases": {
    "info_collection": "completed",
    "static_analysis": "completed",
    "bug_patterns": "completed",
    "data_flow": "completed",
    "dependencies": "completed"
  },
  "patterns_found": [
    {"type": "AsyncVoid", "severity": "Critical", "file": "Handler.cs", "line": 45}
  ],
  "data_flow_issues": [],
  "concurrency_issues": [],
  "edge_cases": [],
  "bug_found": true,
  "bug_description": "async void causant des exceptions non gérées",
  "root_cause": "Le handler utilise async void au lieu de async Task",
  "proposed_fix": "Changer la signature en async Task",
  "moved_to": "In Progress",
  "timestamp": "2024-01-15T14:30:00Z"
}
```
