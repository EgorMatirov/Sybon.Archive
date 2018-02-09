using Microsoft.Extensions.Configuration;

namespace Sybon.Archive
{
    public class ArchiveSecurityConfiguration
    {
        public class BacsArchiveConfiguration
        {
            public BacsArchiveConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            private IConfiguration Configuration { get; }
            
            public string Host => Configuration.GetValue<string>("Host");
            public int Port => Configuration.GetValue<int>("Port");
            public string CAPath => Configuration.GetValue<string>("CAPath");
            public string ClientKeyPath => Configuration.GetValue<string>("ClientKeyPath");
            public string ClientCertificatePath => Configuration.GetValue<string>("ClientCertificatePath");
        }
        
        public class BacsStatementConfiguration
        {
            public BacsStatementConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            private IConfiguration Configuration { get; }
            
            public string Url => Configuration.GetValue<string>("Url");
            public string Refferer => Configuration.GetValue<string>("Refferer");
            public string Key => Configuration.GetValue<string>("Key");
        }
        
        public class SybonAuthConfiguration
        {
            public SybonAuthConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            private IConfiguration Configuration { get; }
            
            public string Url => Configuration.GetValue<string>("Url");
        }
        
        public ArchiveSecurityConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public BacsArchiveConfiguration BacsArchive => new BacsArchiveConfiguration(Configuration.GetSection("BacsArchive"));
        public BacsStatementConfiguration BacsStatement => new BacsStatementConfiguration(Configuration.GetSection("BacsStatement"));
        public SybonAuthConfiguration SybonAuth => new SybonAuthConfiguration(Configuration.GetSection("SybonAuth"));
        public InfluxDbConfiguration InfluxDb => new InfluxDbConfiguration(Configuration.GetSection("InfluxDb"));
        public string ApiKey => Configuration.GetValue<string>("ApiKey");
    }

    public class InfluxDbConfiguration
    {
        public InfluxDbConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        private IConfiguration Configuration { get; }
            
        public string Url => Configuration.GetValue<string>("Url");
        public string Password => Configuration.GetValue<string>("Password");
        public string UserName => Configuration.GetValue<string>("UserName");
        public string Database => Configuration.GetValue<string>("Database");
    }
}