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

vault kv put secret/depensio \
  Secret="secret" \
  DataBase="DataBase" \
  FromMailIdPassword="FromMailIdPassword" \
  KEY='{
  "v1": "",
  "v2": ""
}'

# 4️⃣ Export dans le volume partagé
ROLE_ID=$(vault read -field=role_id auth/approle/role/my-role/role-id)
SECRET_ID=$(vault write -field=secret_id -f auth/approle/role/my-role/secret-id)

JSON_PATH="/vault/shared/vault-depensio-env.json"

# ✅ Création du fichier s'il n'existe pas
if [ ! -f "$JSON_PATH" ]; then
  echo "📄 Création du fichier $JSON_PATH"
  touch "$JSON_PATH"
  chmod +x  "$JSON_PATH"
fi

cat <<EOF > /vault/shared/vault-depensio-env.json
{
  "Vault__Uri": "$VAULT_ADDR",
  "Vault__RoleId": "$ROLE_ID",
  "Vault__SecretId": "$SECRET_ID"
}
EOF

cat /vault/shared/vault-depensio-env.json

# 🔧 Installer curl si manquant (Alpine Linux)
if ! command -v curl >/dev/null 2>&1; then
  echo "📦 curl non trouvé, installation..."
  apk add --no-cache curl
fi

echo "⏳ Attente de Vault..."

until curl -k --silent $VAULT_ADDR/v1/sys/health | grep '"initialized":true' >/dev/null; do
  echo "⏳ Vault pas encore prêt..."
  sleep 1
done

echo "✅ Vault est prêt !"
# 5️⃣ Garde le serveur Vault actif (attente du process)
wait $VAULT_PID

