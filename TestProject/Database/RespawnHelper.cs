using System.Threading.Tasks;
using Respawn;
using Microsoft.Data.SqlClient;

namespace TestProject.Database
{
    public class RespawnHelper
    {
        private readonly Respawner _respawner;
        private readonly string _connectionString;

        public RespawnHelper(string connectionString)
        {
            _connectionString = connectionString;
            _respawner = Respawner.CreateAsync(_connectionString, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer
            }).GetAwaiter().GetResult();
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_connectionString);
        }
    }
}
