using BoDi;
using Catalogo.API.Spec.Repositories;
using Catalogo.Application.Interfaces;
using Catalogo.Application.Services;
using Catalogo.Domain.Interfaces;
using Catalogo.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Catalogo.API.Spec.Hooks
{
    [Binding]
    public class CategoriasHooks
    {
        private readonly IObjectContainer _objectContainer;
        private const string AppSettingsFile = "appsettings.json";

        public CategoriasHooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public async Task RegisterServices()
        {
            var factory = GetWebApplicationFactory();
            _objectContainer.RegisterInstanceAs(factory);
            var jsonFilesRepo = new JsonFilesRepository();
            _objectContainer.RegisterInstanceAs(jsonFilesRepo);

            IServiceScope scope = ObterServiceScope(factory);

            ApplicationDbContext dbconext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _objectContainer.RegisterInstanceAs(dbconext);

            var repository = (ICategoriaRepository)scope.ServiceProvider.GetService(typeof(ICategoriaRepository));
            _objectContainer.RegisterInstanceAs(repository);

            var service = (CategoriaService)scope.ServiceProvider.GetService(typeof(ICategoriaService));
            _objectContainer.RegisterInstanceAs(service);

            await ClearData(factory, repository);
        }

        private static IServiceScope ObterServiceScope(WebApplicationFactory<Startup> factory)
        {
            return factory.Services.CreateScope();
        }

        private WebApplicationFactory<Startup> GetWebApplicationFactory() =>
            new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    IConfigurationSection? configSection = null;
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), AppSettingsFile));
                        //configSection = context.Configuration.GetSection(nameof(WeatherDatabaseSettings));
                    });
                    //builder.ConfigureTestServices(services =>
                    //    services.Configure<WeatherDatabaseSettings>(configSection));
                });

        private async Task ClearData(WebApplicationFactory<Startup> factory, ICategoriaRepository repository)
        {
            var entities = await repository.GetCategoriasAsync();
            foreach (var entity in entities)
                await repository.RemoveAsync(entity);
        }
    }
}
