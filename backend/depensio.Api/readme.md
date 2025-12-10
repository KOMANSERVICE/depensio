## Configuration du SEO
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
dotnet ef migrations script  --project backend/depensio.Infrastructure  --startup-project backend/depensio.Api --idempotent --output migrations.sql

# Remove migration
dotnet ef migrations remove --project backend/depensio.Infrastructure --startup-project backend/depensio.Api

docker compose exec depensio.api dotnet ef migrations remove  --project backend/depensio.Infrastructure   --startup-project backend/depensio.Api

## Configuration des secrets

# Voir les logs en direct 
docker logs -f <nom_du_conteneur>
docker compose logs -f <nom_du_conteneur>

# Se connecter a HashiCorp Vault sur docker 

docker compose -f docker-compose.prod.yml down -v || true
docker compose -f docker-compose.prod.yml up -d --build

docker compose down // facultative
docker compose build --no-cache // facultative
docker compose up -d // facultative

vault kv get -format=json secret/depensio

# ✅ Pour aller plus loin : supprimer aussi les images
docker compose down --volumes --rmi all --remove-orphans

docker exec -it secretvault /bin/sh
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

# Configurer le serveur web sur ton VPS NGINX
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

# deinstaller NGINX

sudo apt remove nginx nginx-common
sudo apt purge nginx nginx-common
sudo apt autoremove

# Liste des site 
ls -l /etc/nginx/sites-enabled/
ls -l /etc/nginx/sites-available/

# Activer le HTTPS avec Let’s Encrypt
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d vename.com -d www.vename.com


# Renouvellement automatique (déjà configuré par défaut)
sudo certbot renew --dry-run

sudo ufw allow 'Apache Full'


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
sudo ufw reset
sudo ufw default deny incoming
sudo ufw allow OpenSSH

# Autoriser port 80 (HTTP), 443 (HTTPS), 3000 (si Next.js en dev)
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow OpenSSH
sudo ufw allow 22/tcp

# Activer le pare-feu
sudo ufw enable
sudo ufw status verbose
sudo ss -tulpn | grep LISTEN
# DesActiver le pare-feu
sudo ufw disable
# Vérifier les règles
sudo ufw status

# Supprimer un dossier 
sudo rm -rf depensio_old
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

https://apexcharts.github.io/Blazor-ApexCharts/


▶️ Installation rapide (Ubuntu)
sudo apt update && sudo apt upgrade -y
sudo apt install git -y
git clone https://github.com/mailcow/mailcow-dockerized
cd mailcow-dockerized
./generate_config.sh


On te demande ton nom de domaine → ex: mail.tondomaine.com

Puis :
cat ghcr_token.txt | docker login ghcr.io -u dlte --password-stdin
docker compose pull
docker compose up -d

🧱 6️⃣ Vérification intrusion / Malware
sudo apt install -y lynis
sudo lynis audit system


#### Configuration WireGuard : VPS Contabo ↔ PC Local
Parfait ! Utilisons votre VPS Contabo existant. Impact minimal sur la prod.

Architecture finale :
Internet
    ↓
VPS Contabo (depensio.com)
    ├─ Nginx → depensio.com (prod) [pas touché]
    ├─ Nginx → demo.depensio.com (nouveau)
    └─ WireGuard (tunnel vers votre PC local)
           ↓
       komanserveur (chez vous, derrière CGNAT)
           └─ Application sur localhost:9001

#### PARTIE 1 : Sur le VPS Contabo
Étape 1 : Se connecter au VPS
Depuis Windows (PowerShell) :
powershellssh root@IP_VPS_CONTABO
# ou
ssh votre-user@IP_VPS_CONTABO

Étape 2 : Installer WireGuard
bash# Mettre à jour
sudo apt update

# Installer WireGuard
sudo apt install wireguard -y

# Vérifier
wg --version

Étape 3 : Générer les clés du serveur (VPS)
bash# Créer le répertoire
sudo mkdir -p /etc/wireguard
cd /etc/wireguard

# Générer la clé privée du serveur
wg genkey | sudo tee server_private.key | wg pubkey | sudo tee server_public.key

# Sécuriser la clé privée
sudo chmod 600 server_private.key

# Afficher les clés (notez-les)
echo "=== CLÉ PRIVÉE SERVEUR ==="
sudo cat server_private.key
echo ""
echo "=== CLÉ PUBLIQUE SERVEUR ==="
cat server_public.key
⚠️ NOTEZ CES DEUX CLÉS !

Étape 4 : Configuration WireGuard sur le VPS
bashsudo nano /etc/wireguard/wg0.conf
Contenu (remplacez SERVER_PRIVATE_KEY) :
ini[Interface]
PrivateKey = SERVER_PRIVATE_KEY
Address = 10.99.0.1/24
ListenPort = 51820
PostUp = iptables -A FORWARD -i wg0 -j ACCEPT; iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE
PostDown = iptables -D FORWARD -i wg0 -j ACCEPT; iptables -t nat -D POSTROUTING -o eth0 -j MASQUERADE

# Cette section sera remplie après avoir configuré le client
[Peer]
# PublicKey du client (à ajouter après l'étape 8)
# PublicKey = CLIENT_PUBLIC_KEY
# AllowedIPs = 10.99.0.2/32
Remplacez :

SERVER_PRIVATE_KEY : La clé privée de l'étape 3
eth0 : Votre interface réseau (vérifiez avec ip addr - peut être ens3, enp0s3, etc.)

Sauvegarder : Ctrl+O, Enter, Ctrl+X

Étape 5 : Activer le forwarding IP
bash# Activer temporairement
sudo sysctl -w net.ipv4.ip_forward=1

# Activer de façon permanente
echo "net.ipv4.ip_forward=1" | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

Étape 6 : Ouvrir le port WireGuard dans le firewall
bash# Si vous utilisez UFW
sudo ufw allow 51820/udp

# Si vous utilisez iptables directement
sudo iptables -A INPUT -p udp --dport 51820 -j ACCEPT
sudo netfilter-persistent save

PARTIE 2 : Sur komanserveur (PC local Ubuntu)
Étape 7 : Installer WireGuard
Via SSH depuis Windows vers komanserveur :
powershellssh komanatse@IP_LOCAL_KOMANSERVEUR
Sur komanserveur :
bash# Installer
sudo apt update
sudo apt install wireguard -y

Étape 8 : Générer les clés du client
bashcd ~
wg genkey | sudo tee client_private.key | wg pubkey | sudo tee client_public.key
sudo chmod 600 client_private.key

# Afficher les clés
echo "=== CLÉ PRIVÉE CLIENT ==="
cat client_private.key
echo ""
echo "=== CLÉ PUBLIQUE CLIENT ==="
cat client_public.key
⚠️ NOTEZ CES DEUX CLÉS !

Étape 9 : Configuration WireGuard sur komanserveur
bashsudo nano /etc/wireguard/wg0.conf
Contenu (remplacez les valeurs) :
ini[Interface]
PrivateKey = CLIENT_PRIVATE_KEY
Address = 10.99.0.2/24

[Peer]
PublicKey = SERVER_PUBLIC_KEY
Endpoint = IP_PUBLIC_VPS_CONTABO:51820
AllowedIPs = 10.99.0.0/24
PersistentKeepalive = 25
Remplacez :

CLIENT_PRIVATE_KEY : Clé privée client de l'étape 8
SERVER_PUBLIC_KEY : Clé publique serveur de l'étape 3
IP_PUBLIC_VPS_CONTABO : L'IP publique de votre VPS Contabo

Sauvegarder : Ctrl+O, Enter, Ctrl+X

PARTIE 3 : Finaliser la configuration
Étape 10 : Ajouter le peer client sur le VPS
Retournez sur le VPS Contabo :
bashsudo nano /etc/wireguard/wg0.conf
Complétez la section [Peer] :
ini[Peer]
PublicKey = CLIENT_PUBLIC_KEY
AllowedIPs = 10.99.0.2/32
Remplacez CLIENT_PUBLIC_KEY par la clé publique client de l'étape 8.
Sauvegarder.

Étape 11 : Démarrer WireGuard
Sur le VPS Contabo :
bashsudo systemctl enable wg-quick@wg0
sudo systemctl start wg-quick@wg0
sudo systemctl status wg-quick@wg0
Sur komanserveur :
bashsudo systemctl enable wg-quick@wg0
sudo systemctl start wg-quick@wg0
sudo systemctl status wg-quick@wg0

Étape 12 : Tester le tunnel
Depuis komanserveur, pinguer le VPS via le tunnel :
bashping 10.99.0.1
Depuis le VPS, pinguer komanserveur via le tunnel :
bashping 10.99.0.2
✅ Si les deux fonctionnent, le tunnel est UP !

PARTIE 4 : Configurer Nginx sur le VPS
Étape 13 : Créer la configuration pour demo.depensio.com
Sur le VPS Contabo :
bashsudo nano /etc/nginx/sites-available/demo.depensio.com
Contenu :
nginxserver {
    listen 80;
    server_name demo.depensio.com;

    location / {
        proxy_pass http://10.99.0.2:9001;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        
        # Pour WebSocket (Blazor SignalR)
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_exchange;
        proxy_set_header Connection "upgrade";
    }
}
Activer le site :
bashsudo ln -s /etc/nginx/sites-available/demo.depensio.com /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx

Étape 14 : HTTPS avec Certbot
bashsudo apt install certbot python3-certbot-nginx -y
sudo certbot --nginx -d demo.depensio.com
```

Suivez les instructions.

---

## **PARTIE 5 : Configuration DNS chez LWS**

### **Étape 15 : Ajouter l'enregistrement A**

**Dans votre panneau LWS :**
```
Type: A
Nom: demo
Valeur: IP_PUBLIC_VPS_CONTABO
TTL: 3600
```

**Attendez 5-10 minutes pour la propagation.**

---

## **PARTIE 6 : Test final**

### **Étape 16 : Tester**

**Depuis n'importe où sur Internet :**
```
https://demo.depensio.com
✅ Devrait afficher votre application locale !

Gestion et monitoring
Voir le statut du tunnel :
Sur le VPS ou komanserveur :
bashsudo wg show
Redémarrer WireGuard :
bashsudo systemctl restart wg-quick@wg0
Voir les logs :
bashsudo journalctl -u wg-quick@wg0 -f

Dépannage
Le tunnel ne fonctionne pas :
bash# Vérifier que le port 51820 est ouvert sur le VPS
sudo netstat -tulpn | grep 51820

# Vérifier les interfaces
sudo wg show

# Vérifier les routes
ip route
demo.depensio.com ne répond pas :
bash# Sur le VPS, tester le tunnel
curl http://10.99.0.2:9001

# Vérifier Nginx
sudo nginx -t
sudo systemctl status nginx
```

---

## **Récapitulatif des IPs :**
```
VPS Contabo : IP publique (ex: 142.x.x.x)
    └─ WireGuard : 10.99.0.1

komanserveur (local) : IP privée locale (ex: 192.168.0.x)
    └─ WireGuard : 10.99.0.2
    └─ Application : localhost:9001

Commencez par l'Étape 1 sur le VPS Contabo et avancez étape par étape. Dites-moi si vous bloquez ! 🚀

# Redirection SSH avec NGINX

sudo nano /etc/nginx/nginx.conf

stream {
    upstream ssh_komanserveur {
        server 10.99.0.2:22;
    }
    
    server {
        listen 2222;
        proxy_pass ssh_komanserveur;
    }
}