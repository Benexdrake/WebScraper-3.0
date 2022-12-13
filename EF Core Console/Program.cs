using EF_Core_Console.Data;
using EF_Core_Console.Interfaces;
using Webscraper_API.Interfaces;
using Webscraper_API.Scraper.Crunchyroll.Controllers;
using Webscraper_API.Scraper.Honda.Controllers;
using Webscraper_API.Scraper.IMDB.Controllers;
using Webscraper_API.Scraper.Pokemons.Controller;
using Webscraper_API.Scraper.TCG_Pokemon.Controller;

IConfiguration conf;

var builder = new ConfigurationBuilder();
BuildConfig(builder);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

conf = builder.Build();

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, service) =>
    {
        service.AddSingleton<Browser>();
        service.AddTransient<ICR_API, CR_API>();
        service.AddTransient<IIMDb_API, IMDb_API>();
        service.AddTransient<IPokemon_API, Pokemon_API>();
        service.AddTransient<ITCG_API, TCG_API>();
        service.AddTransient<IHonda_Api, Honda_Api>();

        service.AddTransient<ICrunchyrollController, CrunchyrollController>();
        service.AddTransient<IIMDbController, IMDbController>();
        service.AddTransient<IPokemonController, PokemonController>();
        service.AddTransient<IPokemonCardController, PokemonCardController>();
        service.AddTransient <IHondaPartsController, HondaPartsController>();

        service.AddDbContext<CrunchyrollDBContext>(options => options.UseSqlServer(conf.GetConnectionString("Crunchyroll")));
        service.AddDbContext<ImdbDBContext>(options => options.UseSqlServer(conf.GetConnectionString("IMDB")));
        service.AddDbContext<PokemonDBContext>(options => options.UseSqlServer(conf.GetConnectionString("Pokemon")));
        service.AddDbContext<PokemonCardDbContext>(options => options.UseSqlServer(conf.GetConnectionString("PokemonCards")));
        service.AddDbContext<HondaPartsDbContext>(options => options.UseSqlServer(conf.GetConnectionString("HondaParts")));

        service.AddHostedService<Main>();
    })
    .Build();

host.StartAsync();


static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
}