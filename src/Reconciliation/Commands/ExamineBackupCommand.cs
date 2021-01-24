using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reconciliation.Commands.Setting;
using Spectre.Console.Cli;

namespace Reconciliation.Commands
{
#pragma warning disable CA1812
    internal class ExamineBackupCommand : AsyncCommand<ConsoleSettings>
    {
        private ILogger Logger { get; }
        private Insta.Backup Backup { get; }
        public ExamineBackupCommand(ILogger<ExamineBackupCommand> logger,
            Insta.Backup backup)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Backup = backup ?? throw new ArgumentNullException(nameof(backup));
        }

        public override Task<int> ExecuteAsync(CommandContext context,
            ConsoleSettings settings)
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
            ConsoleSettings settings)
        {
            await Backup.Parse(context.Name, settings.InstagramPath).ConfigureAwait(false);
            return await Task.FromResult(0).ConfigureAwait(false);
        }
    }
#pragma warning restore CA1812
}
