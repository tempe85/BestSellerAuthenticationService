using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.HostedServices;
using BestSeller.Authentication.Service.Models;
using BestSeller.Authentication.Service.MongoDB;
using BestSeller.Authentication.Service.Processors;
using BestSeller.Authentication.Service.Repositories;
using BestSeller.Authentication.Service.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace BestSeller.Authentication.Service
{
    //TODO: Eventually we want to split this out into multiple projects
    //WebApi
    //Database
    //Common
    //Authentication
    //Scheduler

    public class Startup
    {
        private const string AllowedClientOriginSetting = "AllowedOrigin";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });
            ConfigureMongoDb(services);
            services.AddSingleton<IWorkStationProcessor, WorkStationProcessor>();

            var identityServerSettings = Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
            services.AddIdentityServer(options =>
            {
                //Helps you see console logs for identity server
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
            })
                    .AddAspNetIdentity<FactorySchedulerUser>()
                    .AddInMemoryClients(identityServerSettings.Clients)
                    .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
                    .AddInMemoryApiResources(identityServerSettings.ApiResources)
                    .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
                    .AddProfileService<ProfileService>()
                    .AddDeveloperSigningCredential();
            services.AddLocalApiAuthentication();

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddHostedService<IdentityConfigureHostedService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BestSeller.Authentication.Service", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            if (true)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BestSeller.Authentication.Service v1"));
                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration.GetSection(AllowedClientOriginSetting).Value)
                           .AllowAnyMethod()
                           .AllowAnyHeader();

                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
        private void ConfigureMongoDb(IServiceCollection services)
        {
            var serviceSettings = GetMongoDbSettings();
            services.AddMongoDB(serviceSettings);
            services.AddMongoDbRepository<WorkBuildingRepository, WorkBuilding>(serviceSettings.WorkBuildingCollectionName);
            services.AddMongoDbRepository<WorkAreaRepository, WorkArea>(serviceSettings.WorkAreaCollectionName);
            services.AddMongoDbRepository<WorkStationRepository, WorkStation>(serviceSettings.WorkStationCollectionName);
            services
                .Configure<IdentitySettings>(Configuration.GetSection(nameof(IdentitySettings)))
                .AddDefaultIdentity<FactorySchedulerUser>()
                .AddRoles<FactorySchedulerRole>()
                .AddMongoDbStores<FactorySchedulerUser, FactorySchedulerRole, Guid>
                (
                    serviceSettings.ConnectionString,
                    serviceSettings.DatabaseName //Update this later to auth specific database
                );

        }
        private FactorySchedulerSettings GetMongoDbSettings() =>
            Configuration.GetSection(nameof(FactorySchedulerSettings)).Get<FactorySchedulerSettings>();

    }
}
