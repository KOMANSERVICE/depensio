using System.Net;
using System.Net.Http.Json;
using depensio.Application.UseCases.StockLocations.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit.Gherkin.Quick;

namespace Depensio.Tests.Api.Steps.StockLocations;

[FeatureFile("./Features/StockLocations/CreateStockLocation.feature")]
public sealed class CreateStockLocationSteps /// : Feature
{
    //private readonly WebApplicationFactory<Program> _factory;
    //private HttpClient _client;
    //private HttpResponseMessage _response;
    //private Guid _boutiqueId = Guid.NewGuid();
    //private string _name;
    //private string _address;

    //public CreateStockLocationSteps()
    //{
    //    _factory = new WebApplicationFactory<Program>();
    //    _client = _factory.CreateClient();
    //}

    //[Given(@"une base de données vide")]
    //public void GivenUneBaseDeDonneesVide()
    //{
    //    // Si besoin, reset la base ici (selon ta config de test)
    //    // Pour InMemory, chaque test a une base isolée
    //}

    //[When(@"je crée un magasin de stock nommé ""(.*)"" avec l'adresse ""(.*)""")]
    //public async Task WhenJeCreeUnStock(string name, string address)
    //{
    //    _name = name;
    //    _address = address;

    //    var request = new
    //    {
    //        stockLocation = new StockLocationDTO
    //        {
    //            Name = name,
    //            Address = address
    //        }
    //    };

    //    ///_response = await _client.PostAsJsonAsync($"/stocklocation/{_boutiqueId}", request);
    //}


    //[Then(@"une erreur ""(.*)"" doit être levée")]
    //public async Task ThenErrorShouldBeThrown(string message)
    //{
    //    _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    //    var content = await _response.Content.ReadAsStringAsync();
    //    content.Should().Contain(message);
    //}
}
