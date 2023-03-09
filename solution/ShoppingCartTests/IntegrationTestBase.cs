using MongoDB.Driver;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.DataAccess;
using ShoppingCartTests.Fixtures;
using AutoMapper;
using ShoppingCartService.Mapping;

namespace ShoppingCartTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected const string UnknownID = "123456789012345678901234";
        protected IMapper _mapper;

        protected IShoppingCartDatabaseSettings _databaseSettings;

        public IntegrationTestBase(DockerMongoFixture dockerMongoFixture)
        {
            _databaseSettings = dockerMongoFixture.GetDatabaseSettings();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()))
                .CreateMapper();
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
