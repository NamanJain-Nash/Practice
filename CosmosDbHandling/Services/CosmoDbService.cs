using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.IServices;
using DatabaseModel;
namespace DataBaseHandling.Services
{
    public class CosmoDbService: ICosmoDbService
    {
        private readonly string EndpointUrl;
        private readonly string AuthorizationKey;
        private readonly string DatabaseName;
        private readonly string CollectionName;
        private readonly IConfiguration _config;
        private DocumentClient _client;
        private readonly ILogger _logger;
        public CosmoDbService(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            EndpointUrl = _config.GetSection("CosmoDbEndpoint").Value;
            AuthorizationKey = _config.GetSection("CosmoDbAuthKey").Value;
            DatabaseName = _config.GetSection("CosmoDbName").Value;
            CollectionName = _config.GetSection("CosmoDbCollectionName").Value;
            _client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            InitializeDatabase().Wait();
        }
        private async Task InitializeDatabase()
        {
            //OptionalStepForsafety
            await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            //Making of the DBconnect
            await _client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseName),
                new DocumentCollection { Id = CollectionName });
        }
        public async Task<string> SaveToCosmosDB(MessageingModel data)
        {
            try
            {
                var document = new
                {
                    id = Guid.NewGuid().ToString(),
                    data.message // Access data to be saved
                                           // Add more properties as needed
                };

                await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), document);
                return "Added Succesfully";
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"Error saving to Cosmos DB: {ex.Message}");
                return ex.Message;
            }
        }


    }
}
