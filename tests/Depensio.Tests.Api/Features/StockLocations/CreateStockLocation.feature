Feature: Création d'un magasin de stock
  En tant qu'administrateur de boutique
  Je veux créer un magasin de stock
  Afin de pouvoir organiser mes produits


  Scenario: Échec de création quand le nom est vide
    Given une base de données vide
    When je crée un magasin de stock nommé "" avec l'adresse "Abidjan"
    Then une erreur "Le nom du stock est obligatoire." doit être levée
