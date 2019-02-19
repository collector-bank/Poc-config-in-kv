using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace poc_config_in_kv
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config)=>{
                    config.AddJsonFile("appsettings.json", true, false)
                            .AddEnvironmentVariables();
                    var buildConfig = config.Build();

                    var azureServiceTokenProvider = new AzureServiceTokenProvider();

                    var keyVaultClient = new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            azureServiceTokenProvider.KeyVaultTokenCallback));

                    config.AddAzureKeyVault(buildConfig["keyVaultAddress"],
                        keyVaultClient,
                        new PrefixKeyVaultSecretManager(buildConfig["WEBAPP_NAME"]));
                })
                .UseStartup<Startup>();
    }
}
