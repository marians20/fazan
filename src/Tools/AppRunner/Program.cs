using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AppRunner
{
    using Microsoft.Extensions.CommandLineUtils;

    public class Program
    {
        private static CommandLineApplication commandLineApplication;

        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ro-RO");

            InitializeCommandLineApplication();
            commandLineApplication.Execute(args);
        }

        private static void InitializeCommandLineApplication()
        {
            commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

            var action = commandLineApplication.Option(
                "-$|-a |--action <action>",
                "Chose play for playing fazan or addwords to add the words into database",
                CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");
            commandLineApplication.OnExecute(() => RunSelectedApplicationAsync(action));
        }

        private static async Task<int> RunSelectedApplicationAsync(CommandOption action)
        {
            if (!action.HasValue())
            {
                return 0;
            }

            var application = ApplicationsFactory.GetApplication(action.Value());
            await application.RunAsync().ConfigureAwait(false);
            return 0;
        }
    }
}
