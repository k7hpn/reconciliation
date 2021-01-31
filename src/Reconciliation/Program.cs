using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reconciliation.Commands;
using Spectre.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli;

[assembly: System.CLSCompliant(true)]

var serviceCollection = new ServiceCollection()
    .AddLogging(_ => _.AddSimpleConsole(opts => opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss "))
    .AddScoped(typeof(Reconciliation.Insta.Backup))
    .AddScoped(typeof(Reconciliation.Output.Markdown));

using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.ValidateExamples();

        config.AddCommand<ExamineBackupCommand>("examinebackup")
            .WithDescription("Examine Instagram backup directories")
            .WithExample(new[] { "examinebackup", "<Instagram export path>" });

        config.AddCommand<GenerateMarkdownCommand>("generatemarkdown")
            .WithDescription("Generate Markdown posts from Instagram backups")
            .WithExample(new[] { "generatemarkdown", "<Instagram export path> <Markdown blog base path>" });
    });

return await app.RunAsync(args).ConfigureAwait(false);