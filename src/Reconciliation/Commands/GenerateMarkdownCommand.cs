using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reconciliation.Commands.Settings;
using Spectre.Console.Cli;

namespace Reconciliation.Commands
{
#pragma warning disable CA1812
    internal class GenerateMarkdownCommand : AsyncCommand<GenerateMarkdownSettings>
    {
        private ILogger Logger { get; }
        private Insta.Backup Backup { get; }
        private Output.Markdown Output { get; }

        public GenerateMarkdownCommand(ILogger<GenerateMarkdownCommand> logger,
            Insta.Backup backup,
            Output.Markdown output)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Backup = backup ?? throw new ArgumentNullException(nameof(backup));
            Output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public override Task<int> ExecuteAsync(CommandContext context,
            GenerateMarkdownSettings settings)
        {
            if (context == null)
            {
                Logger.LogError("No context to execute command.");
                throw new ArgumentNullException(nameof(context));
            }
            if (settings == null)
            {
                Logger.LogError("No settings to execute command.");
                throw new ArgumentNullException(nameof(settings));
            }

            return ExecuteInternalAsync(context, settings);
        }

        private async Task<int> ExecuteInternalAsync(CommandContext context,
            GenerateMarkdownSettings settings)
        {
            var ig
                = await Backup.Parse(context.Name, settings.InstagramPath).ConfigureAwait(false);
            await Output.GeneratePostsAsync(context.Name,
                settings.OutputPath,
                ig);
            return await Task.FromResult(0).ConfigureAwait(false);
        }
    }
#pragma warning restore CA1812
}
