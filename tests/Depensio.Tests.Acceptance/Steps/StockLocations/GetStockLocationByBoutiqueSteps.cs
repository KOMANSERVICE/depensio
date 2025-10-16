using depensio.Application.Interfaces;
using depensio.Application.UseCases.StockLocations.DTOs;
using depensio.Application.UseCases.StockLocations.Queries.GetStockLocationByBoutique;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;
using FluentAssertions;

namespace Depensio.Tests.Acceptance.Steps.StockLocations;

[FeatureFile("./Features/StockLocations/GetStockLocationByBoutique.feature")]
public sealed class GetStockLocationByBoutiqueSteps : Feature
{
    private readonly DepensioDbContext _db;
    private GetStockLocationByBoutiqueHandler _handler;
    private Mock<IUserContextService> _userContextService;
    private IEnumerable<StockLocationDTO> _result;
    public GetStockLocationByBoutiqueSteps()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new DepensioDbContext(options);
        _db.Database.EnsureCreated();
        _userContextService = new Mock<IUserContextService>();
        //userId = Guid.NewGuid().ToString();
        //_userContextService.Setup(u => u.GetUserId()).Returns(userId);

        //_handler = new GetStockLocationByBoutiqueHandler(_db, _userContextService.Object);

    }

    [Given(@"une boutique ""(.*)"" appartenant à un utilisateur ""(.*)"" existe avec les magasins magasins de stocks suivants :")]
    public async Task GivenUneBoutiqueExisteAvecLesMagazinsStocksSuivants(string boutiqueName, string userId)
    {
        _db.Database.EnsureDeleted();  
        var boutiqueId = BoutiqueId.Of(System.Guid.NewGuid());
        var boutique = new Boutique
        {
            Id = boutiqueId,
            Name = boutiqueName,
            OwnerId = userId
        };
        _db.Boutiques.Add(boutique);

        var userBoutiqueId = UsersBoutiqueId.Of(System.Guid.NewGuid());
        var userBoutique = new UsersBoutique
        {
            Id = userBoutiqueId,
            UserId = userId,
            BoutiqueId = boutiqueId
        };
        _db.UsersBoutiques.Add(userBoutique);

        if (boutiqueName == "A1")
        {
            await _db.StockLocations.AddAsync(new StockLocation
            {
                Id = StockLocationId.Of(Guid.NewGuid()),
                Name = "Central",
                Address = "Abidjan",
                BoutiqueId = boutiqueId
            });

            await _db.StockLocations.AddAsync(new StockLocation
            {
                Id = StockLocationId.Of(Guid.NewGuid()),
                Name = "Second",
                Address = "Yamoussoukro",
                BoutiqueId = boutiqueId
            });
        }
       
        _db.SaveChanges();
    }

    [When(@"je consulte les magasins de stocks de la boutique ""(.*)"" appartenant à l'utilisateur ""(.*)""")]
    public async Task WhenJeConsulteLesStocksDeLaBoutique(string boutiqueName, string userId)
    {
        var boutique = await _db.Boutiques.FirstOrDefaultAsync(b => b.Name == boutiqueName);
        if (boutique == null)
        {
            _result = new List<StockLocationDTO>();
            return;
        }

        var query = new GetStockLocationByBoutiqueQuery(boutique.Id.Value);

        _userContextService.Setup(u => u.GetUserId()).Returns(userId);
        var handler = new GetStockLocationByBoutiqueHandler(_db, _userContextService.Object);
        var result = await handler.Handle(query, default);
        _result = result.StockLocations;
    }

    [When(@"je consulte les magasins de stocks de la boutique ""(.*)"" n'appartenant pas à l'utilisateur ""(.*)""")]
    public async Task WhenJeConsulteLesStocksDeLaBoutiqueAppartientPasAUtilisateur(string boutiqueName, string userId)
    {
        var boutique = await _db.Boutiques.FirstOrDefaultAsync(b => b.Name == boutiqueName);
        if (boutique == null)
        {
            _result = new List<StockLocationDTO>();
            return;
        }

        var query = new GetStockLocationByBoutiqueQuery(boutique.Id.Value);

        _userContextService.Setup(u => u.GetUserId()).Returns(userId);
        var handler = new GetStockLocationByBoutiqueHandler(_db, _userContextService.Object);
        var result = await handler.Handle(query, default);
        _result = result.StockLocations;
    }

    [Then(@"je dois obtenir (.*) magasins")]
    public void ThenJeDoisObtenirNMagazins(int expectedCount)
    {
        _result.Should().NotBeNull();
        _result.ToList().Count.Should().Be(expectedCount);
    }

    [Then(@"je dois obtenir (.*) magasin")]
    public void ThenJeDoisObtenir0Magazin(int expectedCount)
    {
        _result.Should().NotBeNull();
        _result.ToList().Count.Should().Be(expectedCount);
    }

}
