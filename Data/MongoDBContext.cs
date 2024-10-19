using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime;
using UserAuthAPI.Models;

namespace UserAuthAPI.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;
        private readonly DataBaseSettings _settings;

        public MongoDBContext(IOptions<DataBaseSettings> options)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(_settings.ConnectionURI))
                throw new ArgumentException("ConnectionURI cannot be null or empty", nameof(_settings.ConnectionURI));
            if (string.IsNullOrEmpty(_settings.DatabaseName))
                throw new ArgumentException("DatabaseName cannot be null or empty", nameof(_settings.DatabaseName));

            var settings = MongoClientSettings.FromConnectionString(_settings.ConnectionURI);
            settings.ApplicationName = _settings.AppName;
            var client = new MongoClient(settings);
            _database = client.GetDatabase(_settings.DatabaseName); // Inicializando _database
        }

        public IMongoCollection<UserModel> Users =>
            _database.GetCollection<UserModel>(_settings.UserCollectionName);

    }

}
