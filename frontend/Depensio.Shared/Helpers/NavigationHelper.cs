using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Shared.Helpers;

public static class NavigationHelper
{
    /// <summary>
    /// Extrait le premier GUID trouvé dans l’URL actuelle.
    /// </summary>
    public static Guid ExtractGuidFromUri(NavigationManager navigation)
    {
        var uri = new Uri(navigation.Uri);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var index = Array.IndexOf(segments, "dashboard");
        if (segments.Length > 0)
        {
            if (Guid.TryParse(segments[segments.Length - 1], out var id))
            {
                return id;
            }
        }

        return Guid.Empty; // Aucun GUID trouvé
    }
}
