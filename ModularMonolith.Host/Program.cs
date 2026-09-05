using ModularMonolith.Host.Seeding;
using Modules.Common.API.Extensions;
using Modules.Common.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddWebHostDependencies();

builder.AddCoreHostLogging();

builder.Services.AddCoreWebApiInfrastructure();

builder.Services.AddCoreInfrastructure(builder.Configuration, new[]
{
    ShipmentsModuleRegistration.ActivityModuleName,
    CarriersModuleRegistration.ActivityModuleName,
    StocksModuleRegistration.ActivityModuleName,
    CatalogModuleRegistration.ActivityModuleName,
    BasketModuleRegistration.ActivityModuleName,
    CheckoutModuleRegistration.ActivityModuleName
});

builder.Services
    .AddUsersModule(builder.Configuration)
    .AddShipmentsModule(builder.Configuration)
    .AddCarriersModule(builder.Configuration)
    .AddStocksModule(builder.Configuration)
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddCheckoutModule(builder.Configuration);

// Seed entities in DEVELOPMENT mode
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<SeedService>();
}

var app = builder.Build();

app.MapDefaultEndpoints();

// Run migrations in DEVELOPMENT mode
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.MigrateModuleDatabasesAsync();

    var userSeedService = scope.ServiceProvider.GetRequiredService<UserSeedService>();
    await userSeedService.SeedUsersAsync();

    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.SeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseModuleMiddlewares();

app.MapApiEndpoints();

await app.RunAsync();
