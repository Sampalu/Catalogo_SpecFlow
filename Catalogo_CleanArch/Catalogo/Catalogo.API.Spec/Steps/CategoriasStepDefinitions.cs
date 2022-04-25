using Catalogo.API.Spec.Helpers;
using Catalogo.API.Spec.Repositories;
using Catalogo.Domain.Entities;
using Catalogo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

//https://blog.tonysneed.com/2022/01/28/using-specflow-for-bdd-with-net-6-web-api/
//https://engineering.deltax.com/articles/2019-05/dotnetcore-api-specflow
//https://renatogroffe.medium.com/asp-net-core-specflow-implementando-testes-a-partir-de-uma-user-story-8a60f6335d70

namespace Catalogo.API.Spec.Steps
{
    [Binding]
    public sealed class CategoriasStepDefinitions
    {

        private const string BaseAddress = "https://localhost:5001/api/v1/";
        public WebApplicationFactory<Startup> Factory { get; }
        public ICategoriaRepository Repository { get; }
       
        public HttpClient Client { get; set; } = null!;
        private HttpResponseMessage Response { get; set; } = null!;
        public JsonFilesRepository JsonFilesRepo { get; }
        private Categoria? Entity { get; set; }

        private JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        public CategoriasStepDefinitions(
            WebApplicationFactory<Startup> factory,
            ICategoriaRepository repository,
            JsonFilesRepository jsonFilesRepo)
        {
            Factory = factory;
            Repository = repository;
            JsonFilesRepo = jsonFilesRepo;
        }

        [Given(@"eu sou um httpClient")]
        public void GivenIAmAClient()
        {
            Client = Factory.CreateDefaultClient(new Uri(BaseAddress));
        }

        [Given(@"o repositório tem dados")]
        public async Task GivenTheRepositoryHasWeatherData()
        {
            var weathersJson = JsonFilesRepo.Files["categorias.json"];
            var weathers = JsonSerializer.Deserialize<IList<Categoria>>(weathersJson, JsonSerializerOptions);
            if (weathers != null)
                foreach (var weather in weathers)
                    await Repository.CreateAsync(weather);
        }

        [When(@"eu fizer uma requisição GET em '(.*)'")]
        public async Task WhenIMakeAgetRequestTo(string endpoint)
        {
            Response = await Client.GetAsync(endpoint);
        }

        [When(@"eu fizer uma requisição GET com id '(.*)' em '(.*)'")]
        public async Task WhenIMakeAgetRequestWithIdTo(int id, string endpoint)
        {
            Response = await Client.GetAsync($"{endpoint}/{id}");
        }

        [When(@"eu fizer uma requisição POST com '(.*)' em '(.*)'")]
        public async Task WhenIMakeApostRequestWithTo(string file, string endpoint)
        {
            var json = JsonFilesRepo.Files[file];
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            Response = await Client.PostAsync(endpoint, content);
        }

        [When(@"eu fizer uma requisição PUT com '(.*)' com id '(.*)' em '(.*)'")]
        public async Task WhenIMakeAputRequestWithTo(string file, string id, string endpoint)
        {
            var json = JsonFilesRepo.Files[file];
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            Response = await Client.PutAsync($"{endpoint}/{id}", content);
        }

        [When(@"eu fizer uma requisição DELETE com id '(.*)' em '(.*)'")]
        public async Task WhenIMakeAdeleteRequestWithIdTo(int id, string endpoint)
        {
            Response = await Client.DeleteAsync($"{endpoint}/{id}");
        }

        [Then(@"o código de resposta deve ser '(.*)'")]
        public void ThenTheResponseStatusCodeIs(int statusCode)
        {
            var expected = (HttpStatusCode)statusCode;
            Assert.Equal(expected, Response.StatusCode);
        }

        [Then(@"o location no header é '(.*)'")]
        public void ThenTheLocationHeaderIs(Uri location)
        {
            Assert.Equal(location, Response.Headers.Location);
        }

        [Then(@"o json deve ser '(.*)'")]
        public async Task ThenTheResponseDataShouldBe(string file)
        {
            var json = JsonFilesRepo.Files[file];
            var expected = JsonSerializer.Deserialize<Categoria>(json, JsonSerializerOptions);
            var actual = await Response.Content.ReadFromJsonAsync<Categoria>();
            Entity = actual;
            Assert.Equal(expected, actual, new CategoriaComparer()!);
        }

        [Then(@"o json deve ser lista de '(.*)'")]
        public async Task ThenTheResponseEntityShouldBe(string file)
        {
            var json = JsonFilesRepo.Files[file];
            var expectedList = JsonSerializer.Deserialize<IList<Categoria>>(json, JsonSerializerOptions);
            var actualList = await Response.Content.ReadFromJsonAsync<IEnumerable<Categoria>>();

            foreach (var actual in actualList)
            {
                var expected = expectedList.FirstOrDefault(e => e.CategoriaId == actual.CategoriaId);
                Assert.Equal(expected, actual, new CategoriaComparer()!);
            }            
        }

        ////[Then(@"it should have a new ETag")]
        ////public void ThenItShouldHaveANewETag()
        ////{
        ////    Guid etag = Guid.Empty;
        ////    if (Entity?.ETag != null)
        ////        etag = Guid.Parse(Entity.ETag);
        ////    Assert.NotEqual(Guid.Empty, etag);
        ////}
    }
}
