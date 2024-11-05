using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using Projeto.Fintec.Repositorio;
using Projeto.Fintec.Repositorio.Interface;
using Projeto.Fintec.Servico;
using Projeto.Fintec.Servico.Interface;
using System.Data;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IDbConnection>(db =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

#region Servico
builder.Services.AddScoped<IFinanceiroServico, FinanceiroServico>();
builder.Services.AddScoped<IEmpresaServico, EmpresaServico>();
#endregion

#region Repositorio
builder.Services.AddScoped<IFinanceiroRepositorio, FinanceiroRepositorio>();
builder.Services.AddScoped<IEmpresaRepositorio, EmpresaRepositorio>();
#endregion

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API de Antecipação de Recebíveis",
        Description = "API para cálculo de antecipação de recebíveis de empresas",
        Contact = new OpenApiContact
        {
            Name = "Nathaly Moura",
            Email = "nathalyfmoura@msn.com",
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' [espaço] e o token JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Antecipação de Recebíveis v1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
