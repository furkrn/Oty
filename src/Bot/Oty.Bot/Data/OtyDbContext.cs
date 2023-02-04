namespace Oty.Bot.Data;

[PublicAPI]
public class OtyDbContext : DbContext, IUnitOfWork
{
    private readonly ILoggerFactory _factory;

    #nullable disable

    public DbSet<InternalCommand> Commands { get; set; }

    public DbSet<Guild> Guilds { get; set; }

    public DbSet<User> Users { get; set; }

    public string DbFile { get; }

    #nullable enable

    public OtyDbContext(IOptions<BotConfiguration> configuration, ILoggerFactory factory)
    {
        this._factory = factory;

        DbFile = configuration.Value
            .ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbFile}");
        optionsBuilder.UseLoggerFactory(this._factory);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InternalCommand>(e =>
        {
            e.Property(p => p.CommandType)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ApplicationCommandType>(v));

            e.HasKey(nameof(InternalCommand.CommandType), nameof(InternalCommand.Name));
        });
    }
}