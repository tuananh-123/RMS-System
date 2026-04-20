using Microsoft.EntityFrameworkCore;
using RMS;
using RMS.CustomDtoValidators;
using RMS.CustomDtoValidators.Recipes;
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
// app.UseAuthentication();
// app.UseAuthorization();

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
    service.AddScoped<IUpdateRecipeService, UpdateRecipeService>();
    service.AddScoped<IRecipePagingService, RecipePagingService>();
    service.AddScoped<IRecipeDetailService, RecipeDetailService>();
    service.AddScoped<RecipeValidator>();
    service.AddScoped<RecipeBuilder>();
    service.AddScoped<CreateRecipeDtoValidator>();
    service.AddScoped<UpdateRecipeDtoValidator>();
}

static void ConfigureApiServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddOpenApi();

    services.AddAuthentication();
    services.AddAuthorization();
}

