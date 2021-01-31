using System.ComponentModel;
using Spectre.Console.Cli;

namespace Reconciliation.Commands.Settings
{
#pragma warning disable CA1812
    internal class GenerateMarkdownSettings : ExamineBackupSettings
    {
        [CommandArgument(1, "<blog file output path>")]
        [Description("Path to output blog files")]
        public string OutputPath { get; set; } = string.Empty;
    }
#pragma warning restore CA1812
}
