Feature: Création d'un magasin de stock
  En tant qu'administrateur de boutique
  Je veux créer un magasin de stock
  Afin de pouvoir organiser mes produits

  Scenario: Création réussie d'un nouveau magasin
    Given une base de données vide
    When je crée un magasin de stock nommé "Central" avec l'adresse "Abidjan"
    Then le stock "Central" doit exister dans la base