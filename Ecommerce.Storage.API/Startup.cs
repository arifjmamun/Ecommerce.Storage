using System.Text.Json;
using Ecommerce.Storage.Common.Configurations;
using Ecommerce.Storage.Services.Abstractions;
using Ecommerce.Storage.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Ecommerce.Storage.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<DriveConfig>(Configuration.GetSection(nameof(DriveConfig)));
      services.AddSingleton<IDriveConfig>(s => s.GetRequiredService<IOptions<DriveConfig>>().Value);

      services.AddScoped<IStorageService, StorageService>();
      services.AddScoped<ICloudDriveService, CloudDriveService>();
      services.AddCors();
      services.AddControllers();
      services
        .AddMvc()
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
          options.JsonSerializerOptions.DictionaryKeyPolicy = null;
          options.JsonSerializerOptions.IgnoreNullValues = true;
        });
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce Storage API Documentations", Version = "v1" });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseForwardedHeaders(new ForwardedHeadersOptions //needed for nginx reverse proxy
        {
          ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

      app.UseRouting();

      app.UseSwagger();
      app.UseSwaggerUI(config =>
      {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce Storage API Documentations v1");
        config.RoutePrefix = string.Empty;
      });
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}