var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL: tek sunucu, her servis için ayrı veritabanı
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var catalogDb = postgres.AddDatabase("catalogdb");
var identityDb = postgres.AddDatabase("identitydb");
var orderingDb = postgres.AddDatabase("orderingdb");

// Sepet için Redis
var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisInsight();


// Mesajlaşma için RabbitMQ
var rabbit = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithManagementPlugin();

// Servisler eklendikçe buraya bağlanacak, örneğin:
// builder.AddProject<Projects.ECommerce_Catalog_Api>("catalog-api")
//     .WithReference(catalogDb).WaitFor(catalogDb)
//     .WithReference(rabbit).WaitFor(rabbit);

var catalogApi = builder.AddProject<Projects.ECommerce_Catalog_Api>("catalog-api")
    .WithReference(catalogDb).WaitFor(catalogDb);


builder.AddProject<Projects.ECommerce_Gateway>("gateway")
    .WithReference(catalogApi).WaitFor(catalogApi)
    .WithExternalHttpEndpoints();


builder.Build().Run();
