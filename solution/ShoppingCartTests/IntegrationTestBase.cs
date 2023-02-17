using MongoDB.Driver;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.DataAccess;
using ShoppingCartTests.Fixtures;

namespace ShoppingCartTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected IShoppingCartDatabaseSettings _databaseSettings;

        public IntegrationTestBase(DockerMongoFixture dockerMongoFixture)
        {
            _databaseSettings = dockerMongoFixture.GetDatabaseSettings();
        }

        protected ShoppingCartRepository InitializeRepository(params Cart[] carts)
        {
            var repo = new ShoppingCartRepository(_databaseSettings);
            foreach (var cart in carts)
            {
                repo.Create(cart);
            }
            return repo;
        }

        public void Dispose()
        {
            new MongoClient(_databaseSettings.ConnectionString)
                .DropDatabase(_databaseSettings.DatabaseName);
        }
    }
}
