Feature: Mise à jour d'un magazin de stock
  En tant que gestionnaire de boutique
  Je veux modifier un emplacement de stock existant
  Afin de corriger ses informations

  Background:
    Given un stock nommé "Ancien" existe avec l'adresse "Cocody"

  Scenario: Mise à jour réussie
    When je renomme le stock "Ancien" en "Nouveau" avec l'adresse "Plateau"
    Then le stock "Nouveau" doit exister dans la base

  Scenario: Échec si le stock n'existe pas
    When je tente de mettre à jour un stock inexistant
    Then une erreur "Magazin introuvable" doit être levée
