Feature: Securite sur la generation de codes barres

  Scenario: Un utilisateur non autorise tente de generer des codes barres
    Given un produit existant et un utilisateur non autorise
    When je tente de generer 1 code barre pour ce produit
    Then une erreur d'autorisation est renvoyee
