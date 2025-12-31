// Version: 1.0.0
/*  Name:   SharePointLink
 *  Author: Jhampton
 *  Desc:   Base for connecting to SharePoint with C#.
 * 
 */

using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using PnP.Core.Auth;
using PnP.Core.Services;
using PnP.Core.Services.Builder.Configuration;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var siteUrl = "https://yourtenant.sharepoint.com/sites/yoursite";

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPnPCore(options =>
        {
            options.Sites.Add("Default", new PnPCoreSiteOptions
            {
                SiteUrl = siteUrl,
                AuthenticationProvider = new DefaultAuthenticationProvider(new DefaultAzureCredential())
            });
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var pnpContextFactory = serviceProvider.GetRequiredService<IPnPContextFactory>();

        using (var context = await pnpContextFactory.CreateAsync("Default"))
        {
            // Your code to interact with SharePoint
            Console.WriteLine("Authenticated successfully!");
        }
    }
}