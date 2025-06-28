
# Installation de ef
dotnet tool install --global dotnet-ef

# commande de migration
dotnet ef migrations add InitialCreate --project depensio.Infrastructure  --startup-project depensio.Api --output-dir Data/Migrations

dotnet ef database update  --project depensio.Infrastructure  --startup-project depensio.Api

# Generer un script SQL pour la migration
dotnet ef migrations script  --project depensio.Infrasturcture  --startup-project depensio.Api  --output ./deploiementsql/deploy-20260606.sql  --idempotent
