namespace depensio.Infrastructure.Data.Extensions;

public static class DataSeeder
{
    public static void Seed(DepensioDbContext context)
    {
        context.Database.Migrate();

        // 1. Features
        if (!context.Features.Any())
        {
            var features = new List<Feature>
            {
                new Feature { Key = "MAX_USERS", CreatedBy = "seed" },
                new Feature { Key = "MAX_PRODUCTS", CreatedBy = "seed" },
                new Feature { Key = "ADVANCED_REPORTS", CreatedBy = "seed" }
            };
            context.Features.AddRange(features);
            context.SaveChanges();
        }

        // 2. Plans
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

        // 3. Menus
        var menus = new List<Menu>
            {
                new Menu { Name = "Tableau de bord", ApiRoute = "/sale", UrlFront="/dashboard", Icon = "fa-solid fa-chart-pie", Order=1 },
                new Menu { Name = "Produit", ApiRoute = "/product", UrlFront="/produit", Icon = "fa-solid fa-bag-shopping", Order=2 },
                new Menu { Name = "Acheter", ApiRoute = "/purchase", UrlFront="/achatproduit", Icon = "fa-solid fa-money-bills", Order=3 },
                new Menu { Name = "Caisse", ApiRoute = "/sale", UrlFront="/caisse", Icon = "fa-solid fa-cash-register", Order=4 },
                new Menu { Name = "Imprimer code barre", ApiRoute = "/product", UrlFront="/print-barcodes", Icon = "fa-solid fa-barcode", Order=5 },
                new Menu { Name = "Liste utilisateurs", ApiRoute = "/purchase", UrlFront="/liste-user", Icon = "fa-solid fa-address-card", Order=6 },
                new Menu { Name = "Liste des profile", ApiRoute = "/profile", UrlFront="/profile", Icon = "fa-solid fa-chalkboard-user", Order=7 },
                new Menu { Name = "Paramètre", ApiRoute = "/settings", UrlFront="/settings", Icon = "fa-solid fa-gear", Order=8 }
            };

        if (!context.Menus.Any())
        {
            
            context.Menus.AddRange(menus);
            context.SaveChanges();
        }
        else
        {
            var existingMenus = context.Menus.ToList();

            foreach (var menu in menus)
            {
                var existing = existingMenus.FirstOrDefault(m => m.Name == menu.Name);

                if (existing == null)
                {
                    // ➕ Créer un nouveau menu
                    context.Menus.Add(menu);
                }
                else
                {
                    // 🔁 Mettre à jour les infos si nécessaire
                    bool needsUpdate =
                        existing.ApiRoute != menu.ApiRoute ||
                        existing.UrlFront != menu.UrlFront ||
                        existing.Icon != menu.Icon;

                    if (needsUpdate)
                    {
                        existing.ApiRoute = menu.ApiRoute;
                        existing.UrlFront = menu.UrlFront;
                        existing.Icon = menu.Icon;
                        context.Menus.Update(existing);
                    }
                }
            }

            context.SaveChanges();
        }

        // 4. PlanFeatures
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

        // 5. PlanMenus
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
