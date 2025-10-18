Feature: Mise a jour d'un produit avec un code barre manuel

  Scenario: Un utilisateur autorise met a jour un produit avec un code barre manuel
    Given un produit existant accessible par l'utilisateur
    Given la configuration de la boutique exige un code barre manuel
    When je mets a jour le produit avec le code barre "6131234567895"
    Then le produit est mis a jour avec ce code barre

