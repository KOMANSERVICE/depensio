namespace depensio.Infrastructure.Data.Extensions;

public static class DataSeeder
{
    public static void Seed(DepensioDbContext context)
    {
        // ✅ Vérifier la base avant de semer
        context.Database.Migrate();

        // === 1. Features ===
        if (!context.Features.Any())
        {
            var features = new List<Feature>
                {
                    new Feature { Key = "MAX_USERS", CreatedBy = "seed" },
                    new Feature { Key = "MAX_PRODUCTS", CreatedBy = "seed"  },
                    new Feature { Key = "ADVANCED_REPORTS", CreatedBy = "seed" }
                };
            context.Features.AddRange(features);
            context.SaveChanges();
        }

        // === 2. Plans ===
        if (!context.Plans.Any())
        {
            var plans = new List<Plan>
                {
                    new Plan { Name = "Gratuit", Description = "Plan gratuit", Price = 0, RequiresPayment = false, IsDisplay = true },
                    new Plan { Name = "Débutant", Description = "Plan premium", Price = 10000, RequiresPayment = true, IsPopular = true, IsDisplay = true },
                    new Plan { Name = "Standard", Description = "Plan entreprise", Price = 15000, RequiresPayment = true, IsDisplay = true },
                    new Plan { Name = "Professionnel", Description = "Plan entreprise", Price = 20000, RequiresPayment = true, IsDisplay = true }
                };
            context.Plans.AddRange(plans);
            context.SaveChanges();
        }

        // === 3. Menus ===
        if (!context.Menus.Any())
        {
            var menus = new List<Menu>
            {
                new Menu { Name = "Tableau de bord", ApiRoute = "/sale", UrlFront="/dashboard/{boutiqueId}", Icon = "dashboard" },
                new Menu { Name = "Produit", ApiRoute = "/product", UrlFront="/produit/{boutiqueId}", Icon = "box" },
                new Menu { Name = "Acheter", ApiRoute = "/purchase", UrlFront="/achatproduit/{boutiqueId}", Icon = "cart-plus" },
                new Menu { Name = "Caisse", ApiRoute = "/sale", UrlFront="/caisse/{boutiqueId}", Icon = "shopping-cart" },
                new Menu { Name = "Imprimer code barre", ApiRoute = "/product/{boutiqueId}", UrlFront="/print-barcodes/{boutiqueId}", Icon = "cart-plus" },
                new Menu { Name = "Liste utilisateurs", ApiRoute = "/purchase/{boutiqueId}", UrlFront="/liste-user/{boutiqueId}", Icon = "cart-plus" },
                new Menu { Name = "Liste des profile", ApiRoute = "/profile/{boutiqueId}", UrlFront="/profile/{boutiqueId}", Icon = "cart-plus" }
                //new Menu { Name = "Paramètre", ApiRoute = "/settings/{boutiqueId}", UrlFront="/settings/{boutiqueId}", Icon = "cart-plus" }
            };
            context.Menus.AddRange(menus);
            context.SaveChanges();
        }

        //TODO : Ajouter Les sou menus

        // === 4. PlanFeatures (exemple limité) ===
        if (!context.PlanFeatures.Any())
        {
            var freePlan = context.Plans.First(p => p.Name == "Gratuit");
            var premiumPlan = context.Plans.First(p => p.Name == "Débutant");

            var maxUsersFeature = context.Features.First(f => f.Key == "MAX_USERS");
            var maxProductsFeature = context.Features.First(f => f.Key == "MAX_PRODUCTS");

            var features = new List<PlanFeature>
                {
                    new PlanFeature { PlanId = freePlan.Id, FeatureId = maxUsersFeature.Id, Value = "1", Description = "1 utilisateur maximum" },
                    new PlanFeature { PlanId = premiumPlan.Id, FeatureId = maxUsersFeature.Id, Value = "10", Description = "10 utilisateurs maximum" },
                    new PlanFeature { PlanId = premiumPlan.Id, FeatureId = maxProductsFeature.Id, Value = "1000", Description = "1000 produits maximum" }
                };

            context.PlanFeatures.AddRange(features);
            context.SaveChanges();
        }

        // === 5. PlanMenus (chaque plan a des menus différents) ===
        if (!context.PlanMenus.Any())
        {
            var freePlan = context.Plans.First(p => p.Name == "Gratuit");
            var premiumPlan = context.Plans.First(p => p.Name == "Débutant");

            var dashboardMenu = context.Menus.First(m => m.Name == "Tableau de bord");
            var productsMenu = context.Menus.First(m => m.Name == "Produit");
            var salesMenu = context.Menus.First(m => m.Name == "Caisse");
            var purchaseMenu = context.Menus.First(m => m.Name == "Acheter");
            var barcodeMenu = context.Menus.First(m => m.Name == "Imprimer code barre");
            var userMenu = context.Menus.First(m => m.Name == "Liste utilisateurs");

            var planMenus = new List<PlanMenu>
                {
                    new PlanMenu { PlanId = freePlan.Id, MenuId = dashboardMenu.Id },
                    new PlanMenu { PlanId = freePlan.Id, MenuId = productsMenu.Id },
                    new PlanMenu { PlanId = freePlan.Id, MenuId = salesMenu.Id },
                    new PlanMenu { PlanId = freePlan.Id, MenuId = purchaseMenu.Id },
                    new PlanMenu { PlanId = freePlan.Id, MenuId = barcodeMenu.Id },
                    new PlanMenu { PlanId = freePlan.Id, MenuId = userMenu.Id },
                    new PlanMenu { PlanId = premiumPlan.Id, MenuId = dashboardMenu.Id },
                    new PlanMenu { PlanId = premiumPlan.Id, MenuId = productsMenu.Id },
                    new PlanMenu { PlanId = premiumPlan.Id, MenuId = salesMenu.Id }
                };

            context.PlanMenus.AddRange(planMenus);
            context.SaveChanges();
        }
    }
}
