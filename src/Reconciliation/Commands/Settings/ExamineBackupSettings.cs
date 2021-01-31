using System.ComponentModel;
using Spectre.Console.Cli;

namespace Reconciliation.Commands.Settings
{
#pragma warning disable CA1812
    internal class ExamineBackupSettings : CommandSettings
    {
        [CommandArgument(0, "<Instagram export path>")]
        [Description("Path to export of Instagram files")]
        public string InstagramPath { get; set; } = string.Empty;
    }
#pragma warning restore CA1812
}
