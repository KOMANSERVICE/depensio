# Depensio

Depensio is a retail and point of sale platform built with a modular .NET stack. The backend follows a clean architecture (Domain, Application, Infrastructure, API) and the frontend ships as a .NET MAUI + Blazor hybrid app alongside supporting web projects. This repository also bundles automation, infrastructure scripts, and test suites that exercise the application end to end.

## Repository layout

- `backend/depensio.Domain` – business entities, value objects, enums, and domain exceptions.
- `backend/depensio.Application` – use cases, DTOs, services, and abstractions (MediatR, EF Core, helpers).
- `backend/depensio.Infrastructure` – EF Core DbContext, data configurations, external services, security.
- `backend/depensio.Api` – ASP.NET Core entry point and HTTP endpoints.
- `frontend/depensio` – .NET MAUI app with embedded Blazor UI targeting Android, iOS, macOS, and Windows.
- `frontend/depensio.Web*` – supporting web host and shared Razor components.
- `tests/Depensio.Tests.*` – acceptance (SpecFlow), API, and unit test projects.
- `vault`, `maintenance`, `.github` – deployment scripts, policies, and GitHub workflows.

## Prerequisites

- .NET 8 SDK for the backend, domain, infrastructure, and automated tests.
- .NET 9 SDK and the .NET MAUI workload for the cross-platform client (`dotnet workload install maui`).
- Node.js 18+ (optional) if you extend any frontend tooling.
- Docker (optional) for containerized development via the supplied compose files.

## Getting started

1. Restore dependencies:
   ```bash
   dotnet restore depensio.sln
   ```
2. Apply database migrations (if you have a configured database connection):
   ```bash
   dotnet ef database update --project backend/depensio.Infrastructure --startup-project backend/depensio.Api
   ```
   The solution ships with an in-memory database option for quick testing, so this step is optional in local scenarios.
3. Run the API:
   ```bash
   dotnet run --project backend/depensio.Api
   ```
4. Launch the MAUI client (pick the target framework that matches your platform):
   ```bash
   dotnet build frontend/depensio/depensio.csproj -f net9.0-windows10.0.19041.0
   dotnet run --project frontend/depensio/depensio.csproj -f net9.0-windows10.0.19041.0
   ```
5. Alternatively, start the composed services:
   ```bash
   docker compose -f docker-compose.yml up --build
   ```

## Testing

- **Unit tests:**  
  ```bash
  dotnet test tests/Depensio.Tests.Unit/Depensio.Tests.Unit.csproj
  ```
  This suite now covers helpers (boolean and enum parsing), barcode generation services, and core value objects.
- **API and acceptance tests:**  
  ```bash
  dotnet test tests/Depensio.Tests.Api/Depensio.Tests.Api.csproj
  dotnet test tests/Depensio.Tests.Acceptance/Depensio.Tests.Acceptance.csproj
  ```
  The acceptance suite uses Gherkin scenarios (Xunit.Gherkin.Quick) and the API suite is ready for integration tests with `Microsoft.AspNetCore.Mvc.Testing`.

## Environment configuration

- Copy `.env` or the `appsettings*.json` files to align with your local secrets management tooling.
- Backend configuration is handled through `DependencyInjection.cs` files and feature toggles stored in boutique settings.
- MAUI client secrets live in `appsettings.json` and the vault scripts automate Vault policy setup.

## Continuous delivery

- GitHub Actions workflows live under `.github/workflows` (see `deploy-on-merge.yml` for the main pipeline).
- `vault/policies/setup.deploy.sh` configures policy and secret engines required during deployments.

## Conventions

- Follow the clean architecture boundaries: Domain (pure), Application (business rules), Infrastructure (implementation), and API (presentation).
- Use the existing helpers (`BoolHelper`, `EnumHelper`) and services when adding new features to ensure consistent behaviour.
- Prefer adding unit tests in `Depensio.Tests.Unit` for business logic, and wire up integration or acceptance tests when touching endpoints or workflows.

## Useful commands

```bash
# Build everything
dotnet build depensio.sln

# Run linting/formatting (example using dotnet format)
dotnet format depensio.sln

# Update dependencies
dotnet tool restore
dotnet outdated
```

## Support

- Issues and feature requests: open a ticket in this repository.
- For secrets management and deployments, review the scripts in `vault/` and `maintenance/` before running in production.


# Configuration Apache2
    sudo apt update (Facultative)
    sudo apt install apache2 -y
    sudo a2enmod proxy
    sudo a2enmod proxy_http
    sudo a2enmod headers
    sudo a2enmod rewrite
    sudo systemctl restart apache2

    sudo nano /etc/apache2/sites-available/vename.com.conf

    <VirtualHost *:80>
        ServerName vename.com
        ServerAlias www.vename.com

        # Proxy vers votre application locale (port 3000)
        ProxyPreserveHost On
        ProxyPass / http://localhost:3000/
        ProxyPassReverse / http://localhost:3000/

        # Headers forwarding
        RequestHeader set X-Forwarded-Proto "http"
        RequestHeader set X-Forwarded-Port "80"

        # Logs
        ErrorLog ${APACHE_LOG_DIR}/vename.com-error.log
        CustomLog ${APACHE_LOG_DIR}/vename.com-access.log combined
    </VirtualHost>
    
    # Activer le virtual host
        sudo a2ensite vename.com.conf

    # Recharger Apache
        sudo systemctl reload apache2

    # Tester la configuration
        sudo apache2ctl configtest

    # SSL avec Let's Encrypt
        sudo apt install certbot python3-certbot-apache
        sudo certbot --apache -d vename.com -d www.vename.com


# Installation de logiciel de virtualisation
    KVM + Cockpit

    Étape 1 : Installation de KVM et Cockpit
    sudo apt update
    sudo apt upgrade -y
    sudo apt install qemu-kvm libvirt-daemon-system libvirt-clients bridge-utils virtinst cockpit cockpit-machines -y

    # Remplacez "votre_nom_utilisateur" par votre nom d'utilisateur SSH
    sudo usermod -aG libvirt votre_nom_utilisateur

    sudo ufw status
    sudo ufw allow 9090/tcp

    2. Désactiver la liaison IPv6 pour SPICE (Solution recommandée)
    sudo nano /etc/libvirt/qemu.conf
    Trouvez et modifiez la ligne spice_listen :
    Recherchez la ligne suivante (elle est peut-être commentée avec un #).
    #spice_listen = "127.0.0.1"
    Décommentez-la et assurez-vous qu'elle utilise l'adresse IPv4 de localhost :
    spice_listen = "127.0.0.1"
    sudo systemctl restart libvirtd

    mettre image dans /var/cache/libvirt

    sudo nano /etc/NetworkManager/conf.d/disable-wifi-powersave.conf
    [connection]
    wifi.powersave = 2

    [device]
    wifi.powersave = 2

    Dans votre contexte KVM/VM
    Vous avez posé des questions sur la vérification de la connexion de votre VM Win10 au réseau. NetworkManager gère 
    l'interface physique de votre serveur. L'outil libvirt crée un pont réseau (virbr0) qui se connecte à cette interface gérée 
    par NetworkManager. 
    
    Pour vérifier le pont : 
    Utilisez "nmcli device status" sur votre serveur pour voir si virbr0 est actif et a une adresse 
    IP (généralement 192.168.122.1).
    
    Pour que la VM ait accès au réseau : Dans Cockpit, la VM doit être configurée pour utiliser ce pont 
    (le réglage par défaut "NAT" ou "pont par défaut" fait généralement cela).

    sudo virsh domifaddr vm-diallo
    virsh list --state-running
    virsh list --all
    nmcli device status

## Configuration DNS pour serveur mail avec Poste.io
    smtp.ivoirecasa.com
    mail.ivoirecasa.com
    
    Type	Nom (Host)	Valeur (Cible)	Description
    A	mail	IP_publique_du_serveur	ton VPS
    MX	@	mail.ivoirecasa.com	serveur mail
    TXT	@	v=spf1 mx ~all	autorise ton serveur à envoyer
    TXT	_dmarc	v=DMARC1; p=none; rua=mailto:admin@ivoirecasa.com	DMARC
    TXT	default._domainkey	(valeur fournie par Poste.io)	DKIM

    DKIM
    Nom (Host)
    default._domainkey
    Valeur (TXT Record)
    v=DKIM1; k=rsa; p=MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A...
