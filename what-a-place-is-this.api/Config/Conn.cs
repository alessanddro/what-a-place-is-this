using Microsoft.Extensions.Options;
using MongoDB.Driver;
using what_a_place_is_this.api.Data;

namespace what_a_place_is_this.api.Config
{
    public class Conn
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        public Conn(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IMongoDatabase getDataBase()
        {

            var mongoClient = new MongoClient(
            _databaseSettings.Value.ConnectionString);

            return mongoClient.GetDatabase(
                _databaseSettings.Value.DatabaseName);
        }
    }
}
