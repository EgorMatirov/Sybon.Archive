using System.Collections.Generic;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bacs.StatementProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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

            var authApiConfiguration = new Configuration
            {
                BasePath = Configuration.GetConnectionString("Sybon.AuthUrl"),
                ApiKey = new Dictionary<string, string>
                {
                    {"api_key", Configuration.GetConnectionString("ApiKey")}
                }
            };
            services.AddSingleton<IAccountApi>(new AccountApi(authApiConfiguration));
            services.AddSingleton<IPermissionsApi>(new PermissionsApi(authApiConfiguration));

            var clientCert = Configuration.GetConnectionString("Bacs.ArchiveClientCertPath");
            var clientKey =  Configuration.GetConnectionString("Bacs.ArchiveClientKeyPath");
            var ca =  Configuration.GetConnectionString("Bacs.ArchiveCAPath");
            var host =  Configuration.GetConnectionString("Bacs.ArchiveHost");
            var port = Configuration.GetSection("ConnectionStrings").GetValue<int>("Bacs.ArchivePort");
            services.AddSingleton<IArchiveClient, IArchiveClient>(provider => ArchiveClientFactory.CreateFromFiles(host, port, clientCert, clientKey, ca));

            var baseStatementUrl = Configuration.GetConnectionString("Bacs.StatementBaseUrl");
            var statementRefferer = Configuration.GetConnectionString("Bacs.StatementRefferer");
            var statementKey =Configuration.GetConnectionString("Bacs.StatemenKey");
            services.AddSingleton(new StatementProvider(baseStatementUrl, statementRefferer, statementKey));
            
            ConfigureMapper();
            services.AddSingleton(Mapper.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sybon.Archive V1");
            });
            app.UseMvc();
        }
        
        private static void ConfigureMapper()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Collection, ProblemCollection>();
                config.CreateMap<ProblemCollection, Collection>();
                config.CreateMap<Problem, Services.ProblemsService.Models.Problem>();
                config.CreateMap<Services.ProblemsService.Models.Problem, Problem>();
            });
        }
    }
}