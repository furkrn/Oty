const string configurationFileName = "config.json";

if (!File.Exists(configurationFileName))
{
    var config = new
    {
        BotConfiguration = new BotConfiguration(),
    };

    var jsonSettings = new JsonSerializerOptions()
    {
        WriteIndented = true,
    };

    string content = JsonSerializer.Serialize(config, jsonSettings);
    File.WriteAllText(configurationFileName, content, new UTF8Encoding(false));

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("New configuration file created successfully");
    Console.ReadKey();

    return;
}

#if DEBUG
var level = LogLevel.Debug;
#else
var level = LogLevel.Information;
#endif

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(d => d.AddConsole()
        .SetMinimumLevel(level))
    .ConfigureAppConfiguration(c => c.AddJsonFile(configurationFileName, false, true)
        .SetBasePath(Directory.GetCurrentDirectory()))
    .UseConsoleLifetime()
    .ConfigureServices((hostBuilder, services) =>
    {
        services.AddHostedService<OtyHostedService>()
            .Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.FromSeconds(30))
            .AddSingleton<IClientCollection, OtyBotClientStack>();

        services.ConfigureAsyncEventHandlers(c => c.AddEventHandler<DiscordClient, GuildCreateEventArgs, GuildCreatedEvent>()
            .AddEventHandler<DiscordClient, GuildCreateEventArgs, GuildAvaliableEvent>()
            .AddEventHandler<IOtyCommandsExtension, CommandHandledEventArgs, CommandFailedEvent>());

        services.AddOptions<BotConfiguration>()
            .Bind(hostBuilder.Configuration.GetSection("BotConfiguration"))
            .ValidateDataAnnotations();

        services.ConfigureAsyncMonitors()
            .AddPropertyMonitor<BotConfiguration, int, OtyClientMonitor>();

        services.AddDbContext<OtyDbContext>()
            .AddScoped<IGuildRepository, GuildRepository>()
            .AddScoped<IInternalCommandRepository, InternalCommandRepository>()
            .AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IAddonPublisher, InteractionCommandAddonPublisher>()
            .AddSingleton<IAddonServiceFactory, AddonServiceFactory>();

        services.AddCommands(builder => builder
            .AddModule<HelpModule>(LimitedCommandMetadataProvider.DefaultFactory)
            .AddModule<PingModule>(LimitedCommandMetadataProvider.DefaultFactory)
            .AddModule<OwnerEvalModule>()
            .AddModule<SupportModule>(LimitedCommandMetadataProvider.DefaultFactory)
            .AddModule<AddonManager>(LimitedCommandMetadataProvider.DefaultFactory)
            .AddModule<ReportModule>(LimitedCommandMetadataProvider.DefaultFactory)
            .AddModule<PrivacyModule>(LimitedCommandMetadataProvider.DefaultFactory)
            .AddModule<ModuleFinderAutoCompleteModule>());

        services.AddPoLocalization(options => options.ResourcesPath = "./Po");
    });

await hostBuilder.Build().RunAsync();