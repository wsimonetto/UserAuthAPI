using DotNetEnv;
using UserAuthAPI.Data;
using UserAuthAPI.Repositories;
using UserAuthAPI.Services;
using Serilog;
using Serilog.Sinks.File;

var builder = WebApplication.CreateBuilder(args);

// Configura o Serilog para gravar logs em um arquivo
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: Serilog.RollingInterval.Day)
    .CreateLogger();
Log.Information("Aplica��o iniciada com sucesso!");

builder.Host.UseSerilog();

// Carregar vari�veis de ambiente
Env.Load();

// Configura��es do MongoDB usando vari�veis de ambiente
builder.Services.Configure<DataBaseSettings>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
    var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
    var userCollectionName = Environment.GetEnvironmentVariable("USER_COLLECTION_NAME");

    if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(userCollectionName))
    {
        throw new ArgumentException("As vari�veis de ambiente do MongoDB n�o est�o configuradas corretamente.");
    }

    options.ConnectionURI = connectionString!;
    options.DatabaseName = databaseName!;
    options.UserCollectionName = userCollectionName!;
});

// Adiciona o contexto do MongoDB
builder.Services.AddScoped<MongoDBContext>();

// Add services to the container.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
