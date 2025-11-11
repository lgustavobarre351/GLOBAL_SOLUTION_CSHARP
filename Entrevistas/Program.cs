using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProjetoEntrevistas.Data;
using ProjetoEntrevistas.Repositories;
using ProjetoEntrevistas.Services;
using ProjetoEntrevistas.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- Services Configuration ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Resolver problema de referência circular (Entrevista -> Empregador -> Entrevistas -> Empregador...)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // ✅ Customizar mensagens de erro de validação
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => 
                    string.IsNullOrEmpty(err.ErrorMessage) 
                        ? $"Campo '{e.Key}' inválido" 
                        : err.ErrorMessage
                ))
                .ToList();

            return new BadRequestObjectResult(new
            {
                message = "Dados inválidos",
                errors = errors
            });
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Entrevistas API",
        Version = "v1",
        Description = @"🎯 API RESTful para Agendamento de Entrevistas

**Funcionalidades:**
- ✅ CRUD completo para Empregadores, Candidatos e Entrevistas
- 🔍 8+ consultas LINQ (Where, OrderBy, GroupBy, Include, etc)
- ✅ Validações de regras de negócio (conflitos, campos obrigatórios)
- 📊 Dashboard com estatísticas agregadas
- 🔄 Versionamento da API (v1)
- 🗄️ PostgreSQL + Entity Framework Core
- ☁️ Deploy-ready (Railway, Render, Fly.io)

**Tipos de Entrevista:** Online, Presencial, Telefone
**Status:** Agendada, Cancelada, Concluída",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipe Challenge FIAP 2025",
            Email = "contato@entrevistas.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar exemplos e anotações
    c.EnableAnnotations();
    
    // Customizar exemplos exibidos no Swagger
    c.SchemaFilter<ProjetoEntrevistas.Swagger.ExampleSchemaFilter>();
    
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Incluir apenas endpoints versionados (v1)
        return apiDesc.RelativePath?.StartsWith("api/v1/") == true;
    });
    
    // ✅ Desabilitar a seção de Schemas (modelos) no Swagger
    c.UseInlineDefinitionsForEnums();
    c.CustomSchemaIds(type => type.FullName);
});

// --- Entity Framework ---
// Railway/Render específico - verificar DATABASE_URL primeiro
string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found");

Console.WriteLine($"🔗 Using connection: {(Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "DATABASE_URL (Cloud)" : "appsettings")}");

// Configurar Npgsql para usar comportamento legacy de timestamp (mais tolerante)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Configurações específicas para produção
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(30);
    });
    
    // Log detalhado em desenvolvimento
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
        options.EnableSensitiveDataLogging(true);
        options.EnableDetailedErrors(true);
    }
});

// --- Repositories ---
builder.Services.AddScoped<IEmpregadorRepository, EmpregadorRepository>();
builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<IEntrevistaRepository, EntrevistaRepository>();

// --- Serviços ---
builder.Services.AddScoped<IEntrevistaValidationService, EntrevistaValidationService>();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- Teste de Conexão com Banco ---
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.CanConnectAsync();
    Console.WriteLine("✅ Conexão com banco de dados estabelecida com sucesso!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao conectar com banco: {ex.Message}");
}

// --- Middleware Pipeline ---
// Swagger habilitado também em produção para demonstração
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Entrevistas API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Entrevistas API - Documentação Interativa";
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // ✅ Padrão: mostra tags, endpoints fechados
    c.DefaultModelsExpandDepth(-1); // ✅ OCULTAR seção de Schemas/Models
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableValidator();
    c.EnableTryItOutByDefault();
});

// ✅ Habilitar arquivos estáticos para o CSS customizado
app.UseStaticFiles();

app.UseCors();
app.UseRouting();

// Redirecionar raiz para Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Endpoint de saúde/debug
app.MapGet("/api/v1/health", () => 
{
    return Results.Ok(new
    {
        Status = "healthy",
        Service = "Entrevistas API",
        Version = "v1",
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
        Port = Environment.GetEnvironmentVariable("PORT"),
        Database = "Connected",
        Timestamp = DateTime.UtcNow
    });
});

app.MapControllers();

// --- Configuração Multi-Ambiente (Local + Cloud) ---
var environment = app.Environment.EnvironmentName;
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

if (environment == "Development")
{
    // Desenvolvimento local
    Console.WriteLine("🚀 API rodando em ambiente de DESENVOLVIMENTO");
    Console.WriteLine($"📋 Swagger Local: http://localhost:{port}/swagger");
    Console.WriteLine($"🔍 Health Check: http://localhost:{port}/api/v1/health");
    
    app.Run($"http://localhost:{port}");
}
else
{
    // Produção (Railway, Render, etc.)
    Console.WriteLine("🌍 API rodando em ambiente de PRODUÇÃO");
    Console.WriteLine($"🚀 Porta: {port}");
    Console.WriteLine($"📋 Swagger: /swagger");
    Console.WriteLine($"🔍 Health: /api/v1/health");
    
    var urls = $"http://0.0.0.0:{port}";
    app.Run(urls);
}