using NDesk.Options;
using TCatSysManagerLib;

namespace TcAutomationInterface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string visualStudioFilePath = "";
            string amsNetId = "";
            bool showHelp = false;

            OptionSet options = new OptionSet()
                .Add("h|help", delegate (string v) { showHelp = v != null; })
                .Add("f|vsFilePath=", delegate (string v) { visualStudioFilePath = v; })
                .Add("a|amsNetId=", delegate (string v) { amsNetId = v; });

            try
            {
                options.Parse(args);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            if(showHelp )
            {
                Console.WriteLine("Usage: TcAutomationInterface [OPTIONS]");
                options.WriteOptionDescriptions(Console.Out);
                Environment.Exit(0);
            }

            if(!File.Exists(visualStudioFilePath))
            {
                Console.WriteLine("Visual studio solution does not exist!");
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(amsNetId))
            {
                Console.WriteLine("No AmsNetId provided, assuming local AmsNetId");
                amsNetId = "127.0.0.1.1.1";
            }

            Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE.15.0");
            EnvDTE.DTE dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
            dte.SuppressUI = false;
            dte.MainWindow.Visible = true;

            EnvDTE.Solution sol = dte.Solution;
            sol.Open(@visualStudioFilePath);

            EnvDTE.Project pro = sol.Projects.Item(1);

            ITcSysManager sysMan = (ITcSysManager) pro.Object;

            //sysMan.SetTargetNetId(amsNetId);

            sysMan.ActivateConfiguration();
            sysMan.StartRestartTwinCAT();

            //sol.Close();
        }

        // usage from cmd
        // source\repos\TcAutomationInterface\TcAutomationInterface\bin\Debug\net6.0\TcAutomationInterface.exe -f "C:\Users\PaulBarron\source\repos\TcCounter\TcCounter.sln"
    }
}