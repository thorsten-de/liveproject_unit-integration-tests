using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using ShoppingCartService.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartTests.Fixtures
{
    public class DockerMongoFixture : IDisposable
    {
        private const string DockerImageName = "mongodb_test";
        private const string MongoPort = "1111";
        public static readonly string ConnectionString = $"mongodb://localhost:{MongoPort}";

        private Process? _process;


        public DockerMongoFixture()
        {
            _process = Process.Start("docker", $"run --name {DockerImageName} -p {MongoPort}:27017 mongo");
            var started = WaitForMongoDbConnection(ConnectionString, "admin");
            if (!started)
                throw new Exception("Could not get MongoDB connection after trying for some time.");
        }

        public IShoppingCartDatabaseSettings GetDatabaseSettings() =>
            new ShoppingCartDatabaseSettings
            {
                CollectionName = "CartTestCollection",
                ConnectionString = ConnectionString,
                DatabaseName = "ShoppingCartTestDB"
            };


        public void Dispose()
        {
            Console.WriteLine("Shutdown docker container");
            if (_process != null)
            {
                _process.Dispose();
                _process = null;
            }

            Process
                .Start("docker", $"stop {DockerImageName}")
                ?.WaitForExit();

            Process
                .Start("docker", $"rm {DockerImageName}")
                ?.WaitForExit();
        }


        private bool WaitForMongoDbConnection(string connectionString, string dbName)
        {
            Console.Out.WriteLine("Waiting for Mongo to respond");
            var probeTask = Task.Run(() =>
            {
                var isAlive = false;
                var client = new MongoClient(connectionString);

                for (var i = 0; i < 3000; i++)
                {
                    client.GetDatabase(dbName);
                    var server = client.Cluster.Description.Servers.FirstOrDefault();
                    isAlive = server != null &&
                        server.HeartbeatException == null &&
                        server.State == ServerState.Connected;

                    if (isAlive)
                        break;

                    Thread.Sleep(100);
                }

                return isAlive;
            });

            probeTask.Wait();
            return probeTask.Result;
        }
    }

    [CollectionDefinition("Dockerized MongoDB collection")]
    public class DockerMongoCollection : ICollectionFixture<DockerMongoFixture>
    { }
}
