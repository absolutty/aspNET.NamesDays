namespace ViewerConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleApp app = new ConsoleApp();
            if (args.Length > 0)
            {
                //relative path: "res\\csv\\toBeLoaded.csv"
                string csvFilePath = args[0];
                app.LoadCalendar(csvFilePath);
            }
            
            app.Run();
        }
    }
}