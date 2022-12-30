using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModdingTools.App.CommandLine;
using ModdingTools.Core.Options;

namespace ModdingTools.App;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("ModdingTools CLI");

        rootCommand.AddGlobalOption(CommandLineArgs.DryRunOption);
        rootCommand.AddGlobalOption(CommandLineArgs.QuietOption);
        var createCommand = CommandNew.Create();
        var infoCommand = CommandInfo.Create();
        var addCommand = CommandAdd.Create();
        var gameInfoCommand = CommandGameInfo.Create();
        rootCommand.AddCommand(createCommand);
        rootCommand.AddCommand(infoCommand);
        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(gameInfoCommand);
        
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
                        });
                    /*.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console());*/
                })
                .UseDefaults()
                // .UseExceptionHandler(
                 //   (ex, _) => Console.WriteLine("Exception, cannot continue! " + ex.Message), -1)
                .Build()
                .InvokeAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine("EXCEPTION: " + e.Message);
            Console.WriteLine(e);

            return 0;
        }
        
        // return 0;
    }
}
