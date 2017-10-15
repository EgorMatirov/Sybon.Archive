using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bacs.StatementProvider;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Archive.Repositories.ProblemsRepository;
using Sybon.Archive.Services.CollectionsService;
using Sybon.Archive.Services.CollectionsService.Models;
using Sybon.Archive.Services.InternalProblemsService;
using Sybon.Archive.Services.ProblemsService;
using Sybon.Auth.Client.Api;
using Sybon.Auth.Client.Client;
using Sybon.Common;

namespace Sybon.Archive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SecurityConfiguration = new ArchiveSecurityConfiguration(configuration.GetSection("Security"));
        }

        private IConfiguration Configuration { get; }
        private ArchiveSecurityConfiguration SecurityConfiguration { get; }

        private Configuration AuthClientConfiguration => new Configuration
        {
            BasePath = SecurityConfiguration.SybonAuth.Url,
            ApiKey = new Dictionary<string, string>
            {
                {"api_key", SecurityConfiguration.ApiKey}
            }
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Sybon.Archive", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.AddSecurityDefinition("api_key", new ApiKeyScheme {In = "query", Name = "api_key"});
                c.OperationFilter<SwaggerApiKeySecurityFilter>();
            });
            
            services.AddDbContext<ArchiveContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddScoped<IRepositoryUnitOfWork, RepositoryUnitOfWork<ArchiveContext>>();
            
            services.AddScoped<IProblemsRepository, ProblemsRepository>();
            services.AddScoped<IProblemsService, ProblemsService>();
            
            services.AddScoped<IInternalProblemsService, InternalProblemsService>();
            
            services.AddScoped<ICollectionsRepository, CollectionsRepository>();
            services.AddScoped<ICollectionsService, CollectionsService>();

            services.AddSingleton<IAccountApi>(new AccountApi(AuthClientConfiguration));
            services.AddSingleton<IPermissionsApi>(new PermissionsApi(AuthClientConfiguration));
            services.AddSingleton<IArchiveClient, IArchiveClient>(CreateArchiveClient);
            services.AddSingleton<StatementProvider, StatementProvider>(CreateStatementProvider);
            services.AddSingleton<IMapper, IMapper>(CreateMapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // TODO: Move to Sybon.Common
            // TODO: Filter exceptions and do not log system exceptions (db problems \ etc)
            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/html";

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                await context.Response.WriteAsync($"{{\"error\": \"{error.Error.Message}\"}}").ConfigureAwait(false);
                            }{}
                        });
                });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sybon.Archive V1");
            });
            app.UseMvc();
        }
        
        private static IMapper CreateMapper(IServiceProvider serviceProvider)
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Collection, ProblemCollectionWithProblems>();
                config.CreateMap<CollectionModelWithProblemsCount, ProblemCollectionWithoutProblems>();
                config.CreateMap<ProblemCollectionWithProblems, Collection>();
                config.CreateMap<Problem, Services.ProblemsService.Models.Problem>();
                config.CreateMap<Services.ProblemsService.Models.Problem, Problem>();
                config.CreateMap<CollectionForm, Collection>();
                config.CreateMap<CollectionForm.ProblemModel, Problem>();
            });
            return Mapper.Instance;
        }
        
        private IArchiveClient CreateArchiveClient(IServiceProvider serviceProvider)
        {
            var config = SecurityConfiguration.BacsArchive;
            return ArchiveClientFactory.CreateFromFiles(
                config.Host,
                config.Port,
                config.ClientCertificatePath,
                config.ClientKeyPath,
                config.CAPath
            );
        }
        
        private StatementProvider CreateStatementProvider(IServiceProvider serviceProvider)
        {
            var config = SecurityConfiguration.BacsStatement;
            return new StatementProvider(config.Url, config.Refferer, config.Key);
        }

    }
}