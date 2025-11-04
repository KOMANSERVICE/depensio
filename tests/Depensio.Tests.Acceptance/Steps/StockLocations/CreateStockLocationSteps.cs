using depensio.Application.Interfaces;
using depensio.Application.UseCases.StockLocations.Commands.CreateStockLocation;
using depensio.Application.UseCases.StockLocations.DTOs;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using FluentAssertions;
using IDR.Library.BuildingBlocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace Depensio.Tests.Acceptance.Steps.StockLocations;

[FeatureFile("./Features/StockLocations/CreateStockLocation.feature")]
public sealed class CreateStockLocationSteps : Feature
{
    private readonly DepensioDbContext _db;
    private CreateStockLocationHandler _handler;
    private new IGenericRepository<StockLocation> _repository;
    private IUnitOfWork _unitOfWork;
    private StockLocation _createdEntity;
    private Exception _caughtException;
    public CreateStockLocationSteps()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new DepensioDbContext(options);
        _db.Database.EnsureCreated();
        _repository = new GenericRepository<StockLocation>(_db);
        _unitOfWork = new UnitOfWork<DepensioDbContext>(_db);
        _handler = new CreateStockLocationHandler(
            _repository, 
            _unitOfWork
        );
    }


    [Given(@"une base de données vide")]
    public void GivenUneBaseDeDonneesVide() => _db.Database.EnsureDeleted();

    [When(@"je crée un magasin de stock nommé ""(.*)"" avec l'adresse ""(.*)""")]
    public async Task WhenJeCreeUnStock(string name, string address)
    {
        try
        {
            var cmd = new CreateStockLocationCommand(
                BoutiqueId: Guid.NewGuid(),
                StockLocation: new StockLocationDTO
                {
                    Name = name,
                    Address = address
                }
            );

            var result = await _handler.Handle(cmd, CancellationToken.None);
            _createdEntity = await _db.StockLocations.FirstOrDefaultAsync(s => s.Id == StockLocationId.Of(result.Id));
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [Then(@"le stock ""(.*)"" doit exister dans la base")]
    public void ThenStockShouldExist(string name)
    {
        _createdEntity.Should().NotBeNull();
        _createdEntity.Name.Should().Be(name);
    }

}
