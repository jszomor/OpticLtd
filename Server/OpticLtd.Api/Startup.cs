using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.BusinessLogic.Product.Queries;
using OpticLtd.BusinessLogic.Services;
using OpticLtd.Data;
using OpticLtd.Domain.Configuration;
using System.Text;

namespace OpticLtd.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Defaultconnection")));

      var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

      var tokenValidationParams = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        RequireExpirationTime = false
      };

      var clonedParams = tokenValidationParams.Clone();
      clonedParams.ValidateLifetime = false;
      services.AddSingleton(clonedParams);
      services.AddSingleton(typeof(TokenServices));

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(jwt =>
      {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParams;
        
      });

      services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<AppDbContext>();


      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
      services.AddAutoMapper(typeof(Startup));
      services.AddMediatR(typeof(GetProducts));
      services.AddControllers();

      services.AddAuthorization(options =>
      {
        options.AddPolicy("ValidRoles", policy => policy.RequireRole("Admin", "Customer"));

      });

      services.AddSwaggerGen(config =>
      {
        config.SwaggerDoc("v1", new OpenApiInfo { Title = "OpticLtd.Api", Version = "v1" });
        config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description = "JWT Authorization",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer"
        });
        config.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type= ReferenceType.SecurityScheme,
                Id = "Bearer"
              }
            },
            new string[] {}
          }
        });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpticLtd.Api v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
