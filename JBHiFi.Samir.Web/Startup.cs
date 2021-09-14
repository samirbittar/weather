using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AspNetCoreRateLimit;
using JBHiFi.Samir.Core.Queries;
using JBHiFi.Samir.Core.Services;
using JBHiFi.Samir.Web.Docs;
using JBHiFi.Samir.Web.HttpRequestMiddleware;
using JBHiFi.Samir.Web.Infrastructure.Middleware;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Polly;
using Refit;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JBHiFi.Samir.Web
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
            ConfigureOptions();
            ConfigureCache();
            ConfigureRateLimiting();
            ConfigureControllers();
            ConfigureSpa();
            ConfigureApiVersioning();
            ConfigureApiDocumentation();
            ConfigureMediatR();
            ConfigureOpenWeatherApiHttpClient();

            // Local functions
            void ConfigureOptions()
            {
                services.AddOptions();
            }

            void ConfigureCache()
            {
                services.AddMemoryCache();
            }

            void ConfigureRateLimiting()
            {
                services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
                services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

                services.AddInMemoryRateLimiting();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            }

            void ConfigureControllers()
            {
                services.AddControllersWithViews();
            }

            void ConfigureSpa()
            {
                services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
            }

            void ConfigureApiVersioning()
            {
                services.AddApiVersioning(options => options.ReportApiVersions = true);
                services.AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV"; // This format results in a version such as "v1.0"
                    options.SubstituteApiVersionInUrl = true; // Required for versioning by URL segment, e.g. api/v1.0/weatherforecast
                });
            }

            void ConfigureApiDocumentation()
            {
                var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlCommentsFileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                var xmlCommentsFilePath = Path.Combine(applicationBasePath, xmlCommentsFileName);

                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
                services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerDefaultValues>();
                    options.IncludeXmlComments(xmlCommentsFilePath);
                    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Name = "X-ApiKey",
                        Description = "Authorization by request header X-ApiKey",
                        Scheme = "ApiKeyScheme"
                    });

                    var key = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        },
                        In = ParameterLocation.Header
                    };
                    var requirement = new OpenApiSecurityRequirement
                    {
                        { key, new List<string>() }
                    };
                    options.AddSecurityRequirement(requirement);
                });
            }

            void ConfigureMediatR()
            {
                services.AddMediatR(typeof(CurrentWeather));
            }

            void ConfigureOpenWeatherApiHttpClient()
            {
                services.Configure<OpenWeatherMapApiOptions>(Configuration.GetSection("OpenWeatherMapApi"));

                services.AddTransient<IOpenWeatherMapApiKeyProvider, RandomOpenWeatherMapApiKeyProvider>();

                services.AddTransient<OpenWeatherApiAuthHandler>();

                services.AddRefitClient<IOpenWeatherMapApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.openweathermap.org"))
                    .AddHttpMessageHandler<OpenWeatherApiAuthHandler>()
                    .AddTransientHttpErrorPolicy(
                        p => p.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                    )
                    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseApiKeyVerification();
            app.UseClientRateLimiting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    // spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
