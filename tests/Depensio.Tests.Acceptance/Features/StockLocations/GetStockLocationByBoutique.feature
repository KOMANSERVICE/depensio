Feature: Récupération des magasins d'une boutique
  En tant que gestionnaire
  Je veux obtenir les magasins associés à ma boutique
  Afin de visualiser mes magasins de stocks

  Background:
    Given une boutique "A1" appartenant à un utilisateur "b6e2e2e2-7e2a-4e2a-9e2a-2e2a2e2a2e2a" existe avec les magasins magasins de stocks suivants :
    """
    | Nom     | Adresse     |
    | Central | Abidjan     |
    | Second  | Yamoussoukro |
    """

  Scenario: Récupération des magasins de stocks d'une boutique existante
    When je consulte les magasins de stocks de la boutique "A1" appartenant à l'utilisateur "b6e2e2e2-7e2a-4e2a-9e2a-2e2a2e2a2e2a"
    Then je dois obtenir 2 magasins

  Scenario: Aucune donnée trouvée pour une autre boutique
    When je consulte les magasins de stocks de la boutique "B2" appartenant à l'utilisateur "b6e2e2e2-7e2a-4e2a-9e2a-2e2a2e2a2e2a"
    Then je dois obtenir 0 magasin

  Scenario: Aucune donnée trouvée pour une autre boutique n'appartenant pas a l'utilisateur
    When je consulte les magasins de stocks de la boutique "A1" n'appartenant pas à l'utilisateur "e3c1a7b2-4f8d-4e2a-9b1c-7a2e3b4c5d6f"
    Then je dois obtenir 0 magasin