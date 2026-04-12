using Microsoft.EntityFrameworkCore;
using RMS;
using RMS.CustomDtoValidators;
using RMS.Infrastructure.Middleware;
using RMS.IService;
using RMS.IService.IRecipes;
using RMS.Service;
using RMS.Service.Recipes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

ConfigureDbPeristenceService(builder.Services, builder.Configuration);
ConfigureApplicationServices(builder.Services);
ConfigureApiServices(builder.Services);


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ApiExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

static void ConfigureDbPeristenceService(IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<RMSDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
}

static void ConfigureApplicationServices(IServiceCollection service)
{
    service.AddAutoMapper(typeof(Program).Assembly);
    service.AddScoped<IRecipeHistoryService, RecipeHistoryService>();
    service.AddScoped<ITagService, TagService>();
    service.AddScoped<IIngredientService, IngredientService>();
    service.AddScoped<ICreateRecipeService, CreateRecipeService>();
    service.AddScoped<ICreateRecipeValidator, CreateRecipeValidator>();
    service.AddScoped<IRecipeBuilder, RecipeBuilder>();
    service.AddScoped<CreateRecipeDtoValidator>();
}

static void ConfigureApiServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddAuthentication();
    services.AddAuthorization();
}

