var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres")
	.WithDataVolume("postgres-data");

var redis = builder.AddRedis("RedisConnection")
	.WithDataVolume("redis-cache");

var nats = builder.AddNats("Nats")
	.WithJetStream();

var monolith = builder.AddProject<Projects.Rpx_ModularMonolith_Host>("rpx-modular-monolith-host")
	.WithHttpEndpoint(name: "http")
	.WithUrls(c => c.Urls.ForEach(u => u.DisplayText = $"Swagger Api ({u.Endpoint?.EndpointName})"))
	.WithReference(redis)
	.WithReference(postgres)
	.WithReference(nats)
	.WaitFor(postgres)
	.WaitFor(redis)
	.WaitFor(nats);

builder.AddProject<Projects.Rpx_ApiGateway>("rpx-api-gateway")
	.WithReference(monolith)
	.WithEnvironment(
		"ReverseProxy__Clusters__modularMonolithCluster__Destinations__monolith__Address",
		monolith.GetEndpoint("http"))
	.WaitFor(monolith);


await builder.Build().RunAsync();
