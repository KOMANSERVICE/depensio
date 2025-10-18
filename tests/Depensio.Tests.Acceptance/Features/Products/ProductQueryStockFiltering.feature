Feature: Filtrage des produits selon la configuration de stock

  Scenario: La boutique n'autorise pas la vente avec stock zero
    Given une boutique qui interdit la vente avec un stock nul
    Given plusieurs produits dont certains ont un stock nul
    When je recupere les produits avec la configuration de stock
    Then seuls les produits avec du stock sont retournes

