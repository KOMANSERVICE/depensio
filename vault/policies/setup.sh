#!/bin/sh
set -euxo pipefail

# 1️⃣ Démarre Vault en arrière-plan
vault server -dev -dev-root-token-id=root-token &
VAULT_PID=$!

# 2️⃣ Attend que l’API Vault soit disponible
export VAULT_ADDR=http://127.0.0.1:8201
export VAULT_TOKEN=root-token

echo "⏳ Attente de Vault..."

until vault status >/dev/null 2>&1; do
  echo "⏳ Vault non prêt, attente..."
  sleep 1
done

echo "✅ Vault est prêt !"

# 3️⃣ Configuration
vault login "root-token"
vault auth enable approle
vault policy write depensio-policy /vault/policies/depensio-policy.hcl
vault write auth/approle/role/my-role token_policies="depensio-policy" token_ttl=1h token_max_ttl=4h
vault write -f auth/approle/role/my-role/secret-id
vault read auth/approle/role/my-role/role-id

vault kv put secret/depensio DataBase=""
vault kv put secret/depensio FromMailIdPassword=""
vault kv put secret/depensio Secret=""

# 4️⃣ Export dans le volume partagé
ROLE_ID=$(vault read -field=role_id auth/approle/role/my-role/role-id)
SECRET_ID=$(vault write -field=secret_id -f auth/approle/role/my-role/secret-id)

echo "Vault__Uri=$VAULT_ADDR" > /vault/shared/vault-env.export
echo "Vault__RoleId=$ROLE_ID" >> /vault/shared/vault-env.export
echo "Vault__SecretId=$SECRET_ID" >> /vault/shared/vault-env.export

cat /vault/shared/vault-env.export

# 5️⃣ Garde le serveur Vault actif (attente du process)
wait $VAULT_PID


# #!/bin/sh
# set -euxo pipefail

# # 1️⃣ Démarre Vault en background
# # vault server -dev -dev-root-token-id=root-token &

# # # 2️⃣ Patiente le temps que l'API Vault soit prête
# # until vault status >/dev/null 2>&1; do
# #   echo "Attente du démarrage de Vault…" 
# #   sleep 1
# # done
# export VAULT_ADDR=http://127.0.0.1:8200
# export VAULT_TOKEN=root-token

# # 3️⃣ Initialise Vault : auth, policy, AppRole, secret
# # vault login "root-token"
# vault auth enable approle
# vault policy write depensio-policy /vault/policies/depensio-policy.hcl

# vault write auth/approle/role/my-role token_policies="depensio-policy" token_ttl=1h token_max_ttl=4h

# vault write -f auth/approle/role/my-role/secret-id
# vault read auth/approle/role/my-role/role-id

# vault kv put secret/depensio DataBase="Server=localhost;user=root;password=;database=DepenseDB"

# # Récupère la valeur du secret
# ROLE_ID=$(vault read -field=role_id auth/approle/role/my-role/role-id)
# SECRET_ID=$(vault write -field=secret_id -f auth/approle/role/my-role/secret-id)

# # 🧠 Rends la variable persistante dans le conteneur
# # Correction ici : écriture du premier echo avec >, puis >> pour append
# echo "Vault__Uri=http://127.0.0.1:8200" > /vault/shared/vault-env.export
# echo "Vault__RoleId=$ROLE_ID" >> /vault/shared/vault-env.export
# echo "Vault__SecretId=$SECRET_ID" >> /vault/shared/vault-env.export

# # ✅ Export immédiat aussi pour ce shell
# export Vault__Uri=http://127.0.0.1:8200
# export Vault__RoleId=$ROLE_ID
# export Vault__SecretId=$SECRET_ID

# # Optionnel : affichage des valeurs pour debug
# cat /vault/shared/vault-env.export

# # 4️⃣ Garde Vault en fonctionnement (foreground)
# wait
