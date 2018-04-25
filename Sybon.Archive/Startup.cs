using System;
using System.Collections.Generic;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bacs.Archive.TestFetcher;
using Bacs.StatementProvider;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sybon.Archive.HostedServices;
using Sybon.Archive.Repositories.CachedInternalProblemsRepository;
using Sybon.Archive.Repositories.CachedTestsRepository;
using Sybon.Archive.Repositories.CacheRevisionRepository;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Archive.Repositories.ProblemsRepository;
using Sybon.Archive.Services.CachedInternalProblemsService;
using Sybon.Archive.Services.CollectionsService;
using Sybon.Archive.Services.CollectionsService.Models;
using Sybon.Archive.Services.ProblemsService;
using Sybon.Archive.Services.ProblemsService.Models;
using Sybon.Auth.Client.Api;
using Sybon.Auth.Client.Client;
using Sybon.Common;
using CachedInternalProblemRevision = Sybon.Archive.Repositories.CachedInternalProblemsRepository.CachedInternalProblemRevision;
using Problem = Sybon.Archive.Repositories.ProblemsRepository.Problem;
using Test = Bacs.Archive.TestFetcher.Test;

namespace Sybon.Archive
{
    public class Startup
    {
        private const string ServiceName = "Sybon.Archive";

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
            services.AddCors();

            var metrics = AppMetrics.CreateDefaultBuilder()
                .Report.ToInfluxDb(options =>
                {
                    options.InfluxDb.Password = SecurityConfiguration.InfluxDb.Password;
                    options.InfluxDb.UserName = SecurityConfiguration.InfluxDb.UserName;
                    options.InfluxDb.BaseUri = new Uri(SecurityConfiguration.InfluxDb.Url);
                    options.InfluxDb.Database = SecurityConfiguration.InfluxDb.Database;
                    options.FlushInterval = TimeSpan.FromSeconds(1);
                })
                .Configuration.ReadFrom(Configuration)
                .Configuration.Configure(
                        options =>
                        {
                            options.AddAppTag(ServiceName);
                            options.AddEnvTag("development");
                        })
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsReportScheduler();
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
            
            services.AddMvc(options => options.AddMetricsResourceFilter());

            services.AddSwagger(ServiceName, "v1");
            
            services.AddDbContext<ArchiveContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddScoped<IRepositoryUnitOfWork, RepositoryUnitOfWork<ArchiveContext>>();
            
            services.AddScoped<IProblemsRepository, ProblemsRepository>();
            services.AddScoped<IProblemsService, ProblemsService>();
            
            services.AddScoped<ICachedInternalProblemsService, CachedInternalProblemsService>();
            services.AddScoped<ICachedInternalProblemsRepository, CachedInternalProblemsRepository>();
            services.AddScoped<ICachedTestsRepository, CachedTestsRepository>();
            services.AddScoped<ICacheRevisionRepository, CacheRevisionRepository>();
            
            services.AddScoped<ICollectionsRepository, CollectionsRepository>();
            services.AddScoped<ICollectionsService, CollectionsService>();

            services.AddSingleton<IAccountApi>(new AccountApi(AuthClientConfiguration));
            services.AddSingleton<IPermissionsApi>(new PermissionsApi(AuthClientConfiguration));
            services.AddSingleton<IArchiveClient, IArchiveClient>(CreateArchiveClient);
            services.AddSingleton<StatementProvider, StatementProvider>(CreateStatementProvider);

            services.AddSingleton<ITestsFetcher, TestsFetcher>();
            
            services.AddSingleton<IMapper, IMapper>(CreateMapper);
            
            services.AddSingleton<IHostedService, ArchiveCacheSynchronizer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseMetricsAllMiddleware();
            app.UseMetricsAllEndpoints();
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
                config.CreateMap<Collection, ProblemCollection>();
                config.CreateMap<CollectionModelWithProblemsCount, ProblemCollection>()
                    .AfterMap((from, to) => to.Problems = new Services.ProblemsService.Models.Problem[0]);
                config.CreateMap<ProblemCollection, Collection>();
                config.CreateMap<Problem, Services.ProblemsService.Models.Problem>()
                    .AfterMap((from, to) =>
                    {
                        to.Name = from.CachedInternalProblem.Name;
                        to.StatementUrl = from.CachedInternalProblem.StatementUrl;
                        to.TestsCount = from.CachedInternalProblem.TestsCount;
                        to.ResourceLimits = new ResourceLimits
                        {
                            MemoryLimitBytes = from.CachedInternalProblem.MemoryLimitBytes,
                            TimeLimitMillis = from.CachedInternalProblem.TimeLimitMillis
                        };
                        to.Pretests = serviceProvider.GetService<IMapper>()
                            .Map<Services.ProblemsService.Models.Test[]>(from.CachedInternalProblem.Pretests);

                        to.InputFileName = from.CachedInternalProblem.InputFileName;
                        to.OutputFileName = from.CachedInternalProblem.OutputFileName;
                    });
                config.CreateMap<Services.ProblemsService.Models.Problem, Problem>();
                config.CreateMap<CollectionForm, Collection>();
                config.CreateMap<CollectionForm.ProblemModel, Problem>();
                config.CreateMap<Test, Services.ProblemsService.Models.Test>();
                config.CreateMap<CachedInternalProblemRevision, Services.CachedInternalProblemsService.CachedInternalProblemRevision>();
                config.CreateMap<CachedTest, Services.ProblemsService.Models.Test>()
                    .AfterMap((from, to) => { to.Id = from.InternalId; });
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