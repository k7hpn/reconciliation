using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reconciliation.Commands;
using Spectre.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli;

[assembly: System.CLSCompliant(true)]

var serviceCollection = new ServiceCollection()
    .AddLogging(_ => _.AddSimpleConsole(opts => opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss "))
    .AddScoped(typeof(Reconciliation.Insta.Backup));

using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.ValidateExamples();

        config.AddCommand<ExamineBackupCommand>("examinebackup")
            .WithDescription("Examine Instagram backup directories")
            .WithExample(new[] { "examinebackup", "<Instagram export path>" });
    });

return await app.RunAsync(args).ConfigureAwait(false);