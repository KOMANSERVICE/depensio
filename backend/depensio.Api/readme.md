
## Configuration de la migration

# Installation de ef
dotnet tool install --global dotnet-ef

# commande de migration
dotnet ef migrations add InitialCreate --project backend/depensio.Infrastructure  --startup-project backend/depensio.Api --output-dir Data/Migrations

dotnet ef database update  --project backend/depensio.Infrastructure  --startup-project backend/depensio.Api

# Generer un script SQL pour la migration
dotnet ef migrations script  --project backend/depensio.Infrasturcture  --startup-project backend/depensio.Api  --output ./deploiementsql/deploy-20260606.sql  --idempotent


## Configuration des secrets

# Voir les logs en direct 
docker logs -f <nom_du_conteneur>

# Se connecter a HashiCorp Vault sur docker 

docker compose down // facultative
docker compose build --no-cache // facultative
docker compose up -d // facultative
vault kv get -format=json secret/depensio

docker exec -it depensioVault /bin/sh
cat /etc/environment | grep Vault__
printenv | grep Vault__
export VAULT_ADDR="http://127.0.0.1:8200"
vault status
vault login "root-token"



# Ajout de secret
vault auth enable approle

creer le fichier vault-env.export dans le repertoire /vault/shared/
creer le fichier depensio-policy.hcl dans le repertoire /vault/policies/
creer le fichier setup.sh dans le repertoire /vault/policies/

vault policy write depensio-policy /vault/policies/depensio-policy.hcl
vault write auth/approle/role/my-role token_policies="depensio-policy" token_ttl=1h token_max_ttl=4h

vault write -f auth/approle/role/my-role/secret-id
vault read auth/approle/role/my-role/role-id

vault kv put secret/depensio DataBase="Host=localhost;Port=5436;Database=Database;Username=Username;Password=Password"


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

Active le site :
sudo ln -s /etc/nginx/sites-available/vename.com /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx


Activer le HTTPS avec Let’s Encrypt
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d vename.com -d www.vename.com


# Étapes pour corriger

1. Vérifie l’état du service :
sudo systemctl status nginx

2. Si le service est arrêté ou planté, essaie de le démarrer :
sudo systemctl start nginx

3. Si tu veux qu’il démarre automatiquement au boot :
sudo systemctl enable nginx

1. repart de zero
sudo mv /etc/nginx/sites-enabled/vename.com /etc/nginx/sites-enabled/vename.com.bak

statut des site
ls -l /etc/nginx/sites-enabled/


# Pare-feu recommandé : ufw (Uncomplicated Firewall)
sudo apt install ufw -y

# Autoriser SSH
sudo ufw allow OpenSSH

# Autoriser port 80 (HTTP), 443 (HTTPS), 3000 (si Next.js en dev)
sudo ufw allow 80
sudo ufw allow 443
sudo ufw allow OpenSSH
sudo ufw allow 22

# Activer le pare-feu
sudo ufw enable
# DesActiver le pare-feu
sudo ufw disable
# Vérifier les règles
sudo ufw status

# Supprimer un dossier 
sudo rm -rf depensio


# Cloner le projet depensio
Cloner le projet depensio depuis GitHub avec un token d'accès personnel
git clone https://<USERNAME>:<TOKEN>@github.com/KOMANSERVICE/depensio.git

