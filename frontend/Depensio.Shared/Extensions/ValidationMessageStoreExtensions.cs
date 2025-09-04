using depensio.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Shared.Extensions;

public static class ValidationMessageStoreExtensions
{
    /// <summary>
    /// Ajoute les erreurs de validation provenant d'un backend (ErrorMessage.ValidationErrors)
    /// au <see cref="ValidationMessageStore"/> pour le modèle donné.
    /// </summary>
    public static void AddValidationErrors<TModel>(
        this ValidationMessageStore messageStore,
        EditContext editContext,
        TModel model,
        ErrorMessage errorResult)
    {
        if (errorResult == null) return;
        if (errorResult?.ValidationErrors == null || !errorResult.ValidationErrors.Any())
            return;

        // Efface les anciennes erreurs
        messageStore.Clear();

        if (errorResult.ValidationErrors?.Any() == true)
        {
            foreach (var validationError in errorResult.ValidationErrors)
            {
                // Nettoie le préfixe éventuel "Product." / "User." etc.
                var propertyName = validationError.PropertyName;
                if (propertyName.Contains('.'))
                    propertyName = propertyName.Split('.').Last();

                // Recherche de la propriété dans le modèle
                var propInfo = typeof(TModel).GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null)
                {
                    // ✅ Associe l'erreur à la bonne propriété
                    var fieldIdentifier = new FieldIdentifier(model!, propInfo.Name);
                    messageStore.Add(fieldIdentifier, validationError.ErrorMessage);
                }
                else
                {
                    // ✅ fallback : message global
                    messageStore.Add(new FieldIdentifier(model!, string.Empty), validationError.ErrorMessage);
                }
            }
        }
        else if (!string.IsNullOrWhiteSpace(errorResult.Detail))
        {
            messageStore.Add(new FieldIdentifier(model!, string.Empty), errorResult.Detail);
        }

        // Rafraîchit l’affichage
        editContext.NotifyValidationStateChanged();
    }
}
