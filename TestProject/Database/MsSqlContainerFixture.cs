using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using lithuanian_language_learning_tool.Data;
using Testcontainers.MsSql;
using Respawn;
using lithuanian_language_learning_tool.Models;
using lithuanian_language_learning_tool.Services;

namespace TestProject.Database
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public string ConnectionString { get; private set; }
        private readonly MsSqlContainer _container;
        public IServiceProvider ServiceProvider { get; private set; }
        private Respawner _respawner;

        public DatabaseFixture()
        {
            _container = new MsSqlBuilder()
                .WithPassword("YourStrongPassword123") // Set 'sa' password
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

          
            var masterConnectionString = $"{_container.GetConnectionString()};Database=master;TrustServerCertificate=True;";

            await CreateDatabaseAsync("TestDatabase", masterConnectionString);

            ConnectionString = $"{_container.GetConnectionString()};Database=TestDatabase;TrustServerCertificate=True;";

            await ApplyMigrationsAsync();

            

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString));
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService<PunctuationTask>, TaskService<PunctuationTask>>();
            services.AddScoped<ITaskService<SpellingTask>, TaskService<SpellingTask>>();

            ServiceProvider = services.BuildServiceProvider();

            _respawner = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer
            });
        }

        private async Task CreateDatabaseAsync(string databaseName, string masterConnectionString)
        {
            using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $"IF DB_ID('{databaseName}') IS NULL CREATE DATABASE [{databaseName}];";
            await command.ExecuteNonQueryAsync();
        }

        private async Task ApplyMigrationsAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            using var context = new AppDbContext(options);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(ConnectionString);
        }
    }
}
