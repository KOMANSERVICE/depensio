Feature: Recuperation des codes barres disponibles par boutique

  Scenario: Un utilisateur recupere les codes barres disponibles pour sa boutique
    Given une boutique avec des items disponibles et non disponibles pour l'utilisateur
    When je recupere les items de produit par boutique
    Then seuls les items disponibles sont retournes avec leur produit
