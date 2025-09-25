## Configuration du SIO
https://search.google.com/search-console/welcome?hl=fr
https://search.google.com/search-console?resource_id=sc-domain%3Adepensio.com

## Configuration de la migration

# Installation de ef
dotnet tool install --global dotnet-ef

# commande de migration
dotnet ef migrations add InitialCreate --project backend/depensio.Infrastructure --startup-project backend/depensio.Api --output-dir Data/Migrations

dotnet ef database update  --project backend/depensio.Infrastructure  --startup-project backend/depensio.Api

# Generer un script SQL pour la migration
dotnet ef migrations script  --project backend/depensio.Infrasturcture  --startup-project backend/depensio.Api  --output ./deploiementsql/deploy-20260606.sql  --idempotent

# Remove migration
dotnet ef migrations remove --project backend/depensio.Infrastructure --startup-project backend/depensio.Api
dotnet ef migrations remove --force --project backend/depensio.Infrastructure --startup-project backend/depensio.Api
docker compose exec depensio.api dotnet ef migrations remove  --project backend/depensio.Infrastructure   --startup-project backend/depensio.Api
## Configuration des secrets

# Voir les logs en direct 
docker logs -f <nom_du_conteneur>

# Se connecter a HashiCorp Vault sur docker 

docker compose -f docker-compose.prod.yml down -v || true
docker compose -f docker-compose.prod.yml up -d --build

docker compose down // facultative
docker compose build --no-cache // facultative
docker compose up -d // facultative

vault kv get -format=json secret/depensio

# ✅ Pour aller plus loin : supprimer aussi les images
docker compose down --volumes --rmi all --remove-orphans

docker exec -it depensioVault /bin/sh
cat /etc/environment | grep Vault__
printenv | grep Vault__
export VAULT_ADDR="http://127.0.0.1:8200"
vault status
vault login "root-token"


# si vous voulez un token périodique
token_period = "60m"
token_explicit_max_ttl = "0"        # forçant une **durée illimitée**, sans renouvellement automatique
orphan = true                       # optionnel pour rendre le token indépendant

# Ajout de secret
vault auth enable approle

creer le fichier vault-env.export dans le repertoire /vault/shared/
creer le fichier depensio-policy.hcl dans le repertoire /vault/policies/
creer le fichier setup.sh dans le repertoire /vault/policies/

vault policy write depensio-policy /vault/policies/depensio-policy.hcl
vault write auth/approle/role/my-role token_policies="depensio-policy" token_ttl=1h token_max_ttl=4h token_explicit_max_ttl=0 token_period=60m  orphan=true

ou 
vault write auth/approle/role/my-role token_policies="depensio-policy"  token_ttl=0   token_max_ttl=0   token_explicit_max_ttl=0  token_period=0
vault write auth/approle/role/my-role token_policies="depensio"  token_ttl=0   token_max_ttl=0   token_explicit_max_ttl=0  token_period=0

vault write -f auth/approle/role/my-role/secret-id
vault read auth/approle/role/my-role/role-id

vault kv put secret/depensio \
DataBase="Host=localhost;Port=5436;Database=Database;Username=Username;Password=Password" \
 FromMailIdPassword=""


# Definir les variable d'environement sur powershell
[System.Environment]::SetEnvironmentVariable("Vault__Uri", "http://127.0.0.1:8200", "User")

vault read auth/approle/role/my-role/role-id // reccuperer la valeur de role-id
[System.Environment]::SetEnvironmentVariable("Vault__RoleId", "xxx", "User")

vault write -f auth/approle/role/my-role/secret-id // reccuperer la valeur de secret-id
[System.Environment]::SetEnvironmentVariable("Vault__SecretId", "yyy", "User")

# Se connecter au container de bd
docker exec -it {Container ID} bash

# Connexion a la BD line de command
psql -h depensioDB -p 5432 -U testRoot -d depensioDB -W

\dn   -- liste les schémas
\dt   -- liste toutes les tables 

SELECT * FROM "TABLE";

# Transférer un dossier local vers le serveur distant
scp -r build/ root@123.45.67.89:/var/www/mon-site/

# Configurer le serveur web sur ton VPS
sudo apt update (Facultative)
sudo apt install nginx -y (Facultative)
sudo systemctl status nginx
sudo systemctl start nginx
Si premiere installation, il faut configurer le pare-feu pour autoriser le trafic HTTP et HTTPS :
sudo mkdir -p /etc/nginx/sites-available
sudo mkdir -p /etc/nginx/sites-enabled

sudo nano /etc/nginx/sites-available/vename.com

server {
    listen 80;
    server_name vename.com www.vename.com;

    location / {
        proxy_pass http://localhost:3000;  # ou la vraie app
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}

Avec page de maintenance

server {
    listen 80;
    server_name vename.com www.vename.com;

    location / {
        proxy_pass http://localhost:3000;  # ou la vraie app
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    
    # Page de maintenance si backend est down
    error_page 502 503 504 /depensio.html;

    location = /depensio.html {
        root /var/www/maintenance;
        internal;
    }
}

ou pour n8n

server {
    listen 80;
    server_name vename.com www.vename.com;

    location / {
        proxy_pass http://localhost:5678;

        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}


Active le site :
sudo ln -s /etc/nginx/sites-available/vename.com /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx

# deinstaller

sudo apt remove nginx nginx-common
sudo apt purge nginx nginx-common
sudo apt autoremove

# Liste des site 
ls -l /etc/nginx/sites-enabled/
ls -l /etc/nginx/sites-available/

# Activer le HTTPS avec Let’s Encrypt
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d vename.com -d www.vename.com


sudo certbot delete --cert-name demo.depensio.com

# Étapes pour corriger

1. Vérifie l’état du service :
sudo systemctl status nginx

2. Si le service est arrêté ou planté, essaie de le démarrer :
sudo systemctl start nginx

3. Si tu veux qu’il démarre automatiquement au boot :
sudo systemctl enable nginx

1. repart de zero
sudo mv /etc/nginx/sites-enabled/vename.com /etc/nginx/sites-enabled/vename.com.bak
sudo rm /etc/nginx/sites-enabled/nom_du_site
sudo rm /etc/nginx/sites-available/nom_du_site

statut des site
ls -l /etc/nginx/sites-enabled/


# Pare-feu recommandé : ufw (Uncomplicated Firewall)
sudo apt install ufw -y

# Autoriser SSH
sudo ufw allow OpenSSH

# Autoriser port 80 (HTTP), 443 (HTTPS), 3000 (si Next.js en dev)
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow OpenSSH
sudo ufw allow 22/tcp

# Activer le pare-feu
sudo ufw enable
# DesActiver le pare-feu
sudo ufw disable
# Vérifier les règles
sudo ufw status

# Supprimer un dossier 
sudo rm -rf depensio
sudo chown -R komanatse:komanatse /home/



# Cloner le projet depensio
Cloner le projet depensio depuis GitHub avec un token d'accès personnel
git clone https://<USERNAME>:<TOKEN>@github.com/KOMANSERVICE/depensio.git


# Github Actions Workflow Ajouter un cle private SSH 

ssh-keygen -t rsa -b 4096 -C "deploy@yourdomain.com"
Ajouter la cle dans ~/.ssh/authorized_keys sur le serveur distant


# Github Actions Workflow Ajouter un cle private SSH  sans passphrase

ssh-keygen -t rsa -b 4096 -C "ci-deploy@yourdomain.com" -N "" -f ~/.ssh/yourdomain.com
Ajouter la cle dans ~/.ssh/authorized_keys sur le serveur distant

 
# Pour utiliser SSH_PRIVATE_KEY dans ton GitHub Actions afin de te connecter à ton VPS via SSH, voici comment faire étape par étape 🔐📦 :

✅ 1. Générer une clé SSH sur ta machine locale (si pas déjà fait)
ssh-keygen -t rsa -b 4096 -C "github-actions"
⬇️ Cela va générer deux fichiers :

Clé privée : ~/.ssh/id_rsa ➡️ à ajouter dans GitHub Secrets

Clé publique : ~/.ssh/id_rsa.pub ➡️ à copier sur ton VPS

✅ 2. Copier la clé publique dans le VPS
ssh-copy-id -i ~/.ssh/id_rsa.pub user@your-vps-ip
Cela va ajouter la clé dans ~/.ssh/authorized_keys sur ton serveur Linux 🛡️

✅ 3. Ajouter SSH_PRIVATE_KEY dans GitHub Secrets
Va dans ton repo GitHub

Settings ➡️ Secrets and variables ➡️ Actions ➡️ New repository secret

Nom : SSH_PRIVATE_KEY

Valeur : colle le contenu de ~/.ssh/id_rsa

cat ~/.ssh/id_rsa


# Probleme d'installation image docker
sudo systemctl daemon-reexec
sudo systemctl restart docker

✅ Structure d’un code EAN-13 :
Partie	Longueur	Description
Préfixe (GS1)	3	Pays ou organisation (ex: 613)
Code entreprise	4–5	Code assigné à l’entreprise
Code produit	5–4	ID unique pour le produit
Clé de contrôle	1	Calculée avec l’algorithme modulo

Le format du fichier est correct pour Linux
file .env          # doit indiquer UTF-8 text
dos2unix .env      # convertit les retours Windows \r\n en Linux \n


# Generation de cle
    openssl rand -base64 32

# Generation de mot de passe 
    openssl rand -hex 32


# Code barre
    https://serratus.github.io/quaggaJS/
    https://github.com/zxing-js/library


# 1. Scanner avec une douchette (USB ou Bluetooth)

    👉 La douchette agit comme un clavier : elle écrit le code-barres dans un <input>.

    Exemple Blazor
    <InputText @bind-Value="ScannedCode"
               @onkeydown="HandleKeyDown"
               placeholder="Scannez un produit..."
               class="border p-2 rounded w-full" />

    @if (!string.IsNullOrEmpty(ProductName))
    {
        <p class="mt-2 text-green-600 font-bold">Produit trouvé : @ProductName</p>
    }

    @code {
        private string ScannedCode { get; set; } = "";
        private string ProductName { get; set; } = "";

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            // La douchette termine souvent par "Enter"
            if (e.Key == "Enter")
            {
                // Ici tu recherches ton produit via le code scanné
                var product = await ProductService.GetByBarcodeAsync(ScannedCode);

                if (product != null)
                    ProductName = product.Name;
                else
                    ProductName = "Produit introuvable";

                // Reset pour le prochain scan
                ScannedCode = "";
            }
        }
    }


    ✅ Avec une douchette USB, ça marche direct sans JavaScript.

# 2. Scanner avec la caméra du navigateur

    👉 Là, il faut utiliser une librairie JS comme QuaggaJS
     ou ZXing-js
    .
    Ensuite, tu fais un interop JS ↔️ Blazor.

    Étape 1 : Ajouter le script (ex. wwwroot/js/barcodeScanner.js)
    window.barcodeScanner = {
        start: function (dotnetHelper) {
            Quagga.init({
                inputStream: {
                    name: "Live",
                    type: "LiveStream",
                    target: document.querySelector('#scanner')
                },
                decoder: {
                    readers: ["ean_reader", "code_128_reader"] // formats supportés
                }
            }, function (err) {
                if (err) {
                    console.log(err);
                    return;
                }
                Quagga.start();
            });

            Quagga.onDetected(function (result) {
                if (result.codeResult && result.codeResult.code) {
                    dotnetHelper.invokeMethodAsync('OnBarcodeScanned', result.codeResult.code);
                }
            });
        }
    };

    Étape 2 : Créer un composant Blazor BarcodeScanner.razor
    <div>
        <div id="scanner" style="width: 100%; height: 300px; border: 2px solid #ccc;"></div>
        <p class="mt-2">Dernier code : <strong>@ScannedCode</strong></p>
    </div>

    @code {
        private string ScannedCode { get; set; } = "";

        [JSInvokable]
        public void OnBarcodeScanned(string code)
        {
            ScannedCode = code;

            // 👉 Cherche ton produit dans la DB
            // var product = await ProductService.GetByBarcodeAsync(code);
            // ...
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("barcodeScanner.start", dotnetRef);
            }
        }
    }

# Résumé

    Douchette USB/Bluetooth → pas besoin de JS, ça marche comme un clavier (input + enter).

    Caméra smartphone / PC → JS interop avec QuaggaJS ou ZXing.


# Graphique
    https://github.com/apexcharts/Blazor-ApexCharts


# 1️⃣ Blazor WebAssembly (WASM)

    Exécution côté client :
    Tout le code C# de l’application Blazor s’exécute dans le navigateur via WebAssembly.

    Caractéristiques :

    L’application est complètement téléchargée sur le client.

    Les composants Razor sont exécutés côté navigateur.

    Tu peux faire des appels API HTTP pour récupérer des données.

    Très bien pour les applications offline ou distribuées.

    Nécessite plus de temps de chargement initial (le client télécharge l’assembly + runtime WASM).

    Authentification :

    Le token (JWT, etc.) est géré côté client.

    Tu dois gérer la redirection et les 401 côté client (pas directement via [Authorize] sur la page).

    Exemple d’usage :

    Progressive Web App (PWA)

    App MAUI Hybrid qui doit fonctionner offline

# 2️⃣ Blazor Server

    Exécution côté serveur :
    Les composants Razor sont exécutés sur le serveur, et le DOM du navigateur est synchronisé via SignalR.

    Caractéristiques :

    Très rapide au chargement initial (juste le JS minimal pour SignalR).

    Toute la logique C# reste côté serveur.

    La communication avec le client est en temps réel via SignalR.

    Les applications sont moins adaptées à l’utilisation offline, car elles dépendent de la connexion serveur.

    Authentification :

    Tu peux utiliser [Authorize] sur les pages et c’est directement pris en compte côté serveur.

    Les 401/403 sont gérés côté serveur → pas de problème de rafraîchissement de page comme en WASM.

    Exemple d’usage :

    App interne d’entreprise avec connexion constante au serveur

    Dashboard temps réel, admin panel

# 3️⃣ Dans un projet MAUI Blazor Hybrid

    Le projet Hybrid lui-même est “hosté” dans MAUI :

    La partie Blazor peut être WebAssembly ou Server.

    Dans la plupart des cas, le code est exécuté localement dans le WebView du MAUI (comme un mini navigateur).

    Différence pratique :

    WASM Hybrid : tout tourne dans le WebView du device (offline possible)

    Server Hybrid : nécessite que le device se connecte à ton serveur Blazor Server (moins courant pour MAUI)


# Configuration mail 
server {
    listen 80;
    server_name mail.easymarket.ci;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

sudo ln -s /etc/nginx/sites-available/roundcube /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx


# MakeFile
makefile
https://gl.developpez.com/tutoriel/outil/makefile/