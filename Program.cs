using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RMS;
using RMS.Background;
using RMS.CustomDtoValidators;
using RMS.CustomDtoValidators.Auths;
using RMS.CustomDtoValidators.Ingredients;
using RMS.CustomDtoValidators.Recipes;
using RMS.CustomDtoValidators.Tags;
using RMS.Infrastructure.Middleware;
using RMS.IService.IIngredients.ICreate;
using RMS.IService.IRecipes;
using RMS.IService.ITags.ICreate;
using RMS.Service.Auths;
using RMS.Service.Ingredients.Create;
using RMS.Service.Recipes;
using RMS.Service.Tags.Create;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

ConfigureDbPeristenceService(builder.Services, builder.Configuration);
ConfigureApplicationServices(builder.Services);
ConfigureApiServices(builder.Services, builder.Configuration);


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

    services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<RMSDbContext>()
    .AddDefaultTokenProviders();

    services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!)
    );

    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = configuration["Redis:ConnectionString"];
        options.InstanceName = "RedisCache";
    });

    services.AddHostedService<ViewFlushJob>();
    services.Configure<HostOptions>(opts =>
    {
        opts.ShutdownTimeout = TimeSpan.FromSeconds(30); 
    });
}

static void ConfigureApplicationServices(IServiceCollection service)
{
    service.AddAutoMapper(typeof(Program).Assembly);
    service.AddScoped<ICreateRecipeService, CreateRecipeService>();
    service.AddScoped<IUpdateRecipeService, UpdateRecipeService>();
    service.AddScoped<IRecipePagingService, RecipePagingService>();
    service.AddScoped<IRecipeDetailService, RecipeDetailService>();
    service.AddScoped<RecipeValidator>();
    service.AddScoped<RecipeBuilder>();
    service.AddScoped<CreateRecipeDtoValidator>();
    service.AddScoped<UpdateRecipeDtoValidator>();

    // ingredient services
    service.AddScoped<IIngredientCreateSingleService, IngredientCreateSingleService>();
    service.AddScoped<IIngredientCreateByFileInputService, IngredientCreateByFileInputService>();
    service.AddScoped<IngredientCreationBuilder>();
    service.AddScoped<IngredientCreateDtoValidator>();

    // tag services
    service.AddScoped<ITagCreateSingleService, TagCreateSingleService>();
    service.AddScoped<TagCreateSingleDtoValidator>();

    // auth services
    service.AddScoped<RegisterUserService>();
    service.AddScoped<LoginUserService>();
    service.AddScoped<LogoutUserService>();
    service.AddScoped<RefreshTokenService>();
    service.AddScoped<RegisterDtoValidator>();
    service.AddScoped<JwtTokenService>();
}

static void ConfigureApiServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers();
    services.AddOpenApi();

    var jwtKey = configuration["Jwt:Key"] ?? "fklsjfnho13-0032d";
    var jwtIssuer = configuration["Jwt:Issuer"] ?? "RMS";
    var jwtAudience = configuration["Jwt:Audience"] ?? "RMS API";

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

    services.AddAuthorization();
}

