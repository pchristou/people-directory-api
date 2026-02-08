using PeopleDirectory.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add controllers
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Swagger
builder.Services.AddSwaggerGen();
// Repositories
builder.Services.AddSingleton<PeopleRepository>();
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
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();