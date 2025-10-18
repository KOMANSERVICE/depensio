Feature: Generer des codes barres pour un produit

  Scenario: Un utilisateur autorise genere de nouveaux codes barres sans depasser le stock disponible
    Given un produit existant avec un stock de 5 et un utilisateur autorise
    Given des codes barres existants pour ce produit
    When je genere 2 nouveaux codes barres pour ce produit
    Then 2 nouveaux codes barres uniques sont persistes
    Then la reponse contient les memes codes barres

