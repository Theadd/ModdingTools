using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModdingTools.App.CommandLine;
using ModdingTools.Core.Options;
// using ModdingTools.Templates.Writers;

namespace ModdingTools.App;

class Program
{
    // S:\Program Files\Steam\steamapps\common\Humankind
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("ModdingTools CLI");

        rootCommand.AddGlobalOption(CommandLineArgs.DryRunOption);
        var createCommand = CommandNew.Create();
        rootCommand.AddCommand(createCommand);


        try
        {
            return await new CommandLineBuilder(rootCommand)
                .UseHost(builder =>
                {
                    builder
                        .UseConsoleLifetime(opts => opts.SuppressStatusMessages = true)
                        // .ConfigureAppConfiguration(configuration => configuration.AddTomlFile("config.toml", true))
                        .ConfigureServices(services =>
                        {
                            services.AddOptions<GameOptions>().BindConfiguration("GameOptions");

                            // services.AddTransient<InitialStructure>();
                            /*services.AddHttpClient("unity", client =>
                            {
                                client.BaseAddress = new Uri("https://unity3d.com/");
                            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5,
                                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));*/
                        });
                    //.UseCommandHandler<MineCommand, MineCommand.Handler>();
                    /*.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console());*/
                })
                .UseDefaults()
                .UseExceptionHandler(
                    (ex, _) => Console.WriteLine("Exception, cannot continue! " + ex.Message), -1)
                .Build()
                .InvokeAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine("EXCEPTION: " + e.Message);
            // await Task.Delay(100);
            return 0;
        }


        // return 0;
    }
}
