using PeopleDirectory.Infrastructure;
using PeopleDirectory.Repositories;
using PhysicalFileWrapper = PeopleDirectory.Infrastructure.PhysicalFileWrapper;

var builder = WebApplication.CreateBuilder(args);

var frontEndPolicy = "_frontEndPolicy";

// 2. Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontEndPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Your Angular URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
// Add controllers
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Swagger
builder.Services.AddSwaggerGen();
// Register the wrapper for the real file system, swapped for a mock when unit testing
builder.Services.AddScoped<IFileWrapper, PhysicalFileWrapper>();
// Register the repository
builder.Services.AddScoped<UserRepository>();
// Enable lowercase urls to promote consistency
builder.Services.Configure<RouteOptions>(options => 
{
    options.LowercaseUrls = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    
    SeedUsersIfDevelopment(app);

}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(frontEndPolicy);

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

// Provide a clean slate for our json upon startup
static void SeedUsersIfDevelopment(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
        return;

    var env = app.Services.GetRequiredService<IWebHostEnvironment>();
    var dataDir = Path.Combine(env.ContentRootPath, "Data");

    var seedPath = Path.Combine(dataDir, "users.seed.json");
    var runtimePath = Path.Combine(dataDir, "users.json");

    File.Copy(seedPath, runtimePath, overwrite: true);
}