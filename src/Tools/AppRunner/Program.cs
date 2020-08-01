namespace AppRunner
{
    using System.Threading.Tasks;
    using PlayGame;

    class Program
    {
        static async Task Main(string[] args)
        {
            var application = new Application();
            application.ConfigureDi();
            await application.Run();
        }
    }
}
