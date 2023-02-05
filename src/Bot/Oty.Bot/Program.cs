const string configurationFileName = "config.json";

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
            .AddEventHandler<IOtyCommandsExtension, CommandHandledEventArgs, CommandFailedEvent>()
            .AddAddonEvents());

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
            .AddSingleton<IAddonServiceFactory, AddonServiceFactory>()
            .AddSingleton<IGuildGetterExpressionCache, GuildGetterExpressionCache>();

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