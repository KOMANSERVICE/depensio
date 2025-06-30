
## Configuration de la migration

# Installation de ef
dotnet tool install --global dotnet-ef

# commande de migration
dotnet ef migrations add InitialCreate --project backend/depensio.Infrastructure  --startup-project backend/depensio.Api --output-dir Data/Migrations

dotnet ef database update  --project depensio.Infrastructure  --startup-project depensio.Api

# Generer un script SQL pour la migration
dotnet ef migrations script  --project depensio.Infrasturcture  --startup-project depensio.Api  --output ./deploiementsql/deploy-20260606.sql  --idempotent


## Configuration des secrets

# Se connecter a HashiCorp Vault sur docker 

docker compose down // facultative
docker compose up -d // facultative

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

vault kv put secret/depensio DataBase="Server=localhost;user=root;password=;database=DepenseDB"


# Definir les variable d'environement sur powershell
[System.Environment]::SetEnvironmentVariable("Vault__Uri", "http://127.0.0.1:8200", "User")

vault read auth/approle/role/my-role/role-id // reccuperer la valeur de role-id
[System.Environment]::SetEnvironmentVariable("Vault__RoleId", "xxx", "User")

vault write -f auth/approle/role/my-role/secret-id // reccuperer la valeur de secret-id
[System.Environment]::SetEnvironmentVariable("Vault__SecretId", "yyy", "User")

# Se connecter au container de bd
docker exec -it {Container ID} bash

# Connexion a la BD line de command
mysql -h <h�te> -u <utilisateur> -p <base_de_donn�es>
mysql -h depensioDB -u root -p

USE depensioDB
SHOW TABLES;
SHOW DATABASES; 