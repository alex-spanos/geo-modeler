using System;
using System.ServiceProcess;
using BoxController;

namespace BoxHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments for the service initialization.</param>
        /// <returns>Tool error code.</returns>
        public static int Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                switch (args[0])
                {
                    case "/is":
                        return Utilities.InstallService(SubArray(args));
                    case "/su":
                        return Utilities.StopService(true);
                    case "/exec":
                    case "/e": return ProcessSingleJob(SubArray(args));
                    case "/self":
                    case "/s": new BoxServiceHost().ConsoleRun(SubArray(args));
                        break;
                    case "/run":
                    case "/r": RunService(SubArray(args));
                        break;
                    case "/install":
                    case "/i": return Utilities.InstallService();
                    case "/start":
                    case "/a": return Utilities.StartService(SubArray(args));
                    case "/pause":
                    case "/p": return Utilities.PauseService();
                    case "/continue":
                    case "/c": return Utilities.ContinueService();
                    case "/stop":
                    case "/o": return Utilities.StopService();
                    case "/unistall":
                    case "/u": return Utilities.UninstallService();
                    case "/status":
                    case "/t":
                        Console.Write("Service is ");
                        if (Utilities.IsInstalled()) Console.WriteLine("installed " +
                            (Utilities.IsRunning() ? "and" : "but not") + " running.");
                        else Console.WriteLine("not installed.");
                        break;
                    case "/help":
                    case "/h":
                        Console.WriteLine("Give one of the following commands as the first argument:");
                        Console.WriteLine("/is .......... : Installs and starts the service.");
                        Console.WriteLine("/su .......... : Stops and uninstalls the service.");
                        Console.WriteLine("The following commands (except as noted) are valid using /[first letter of command]:");
                        Console.WriteLine("/exec ........ : Perfoms single calculation.");
                        Console.WriteLine("/self ........ : Runs the service hosted inside the console application.");
                        Console.WriteLine("/run ......... : Runs the service hosted as a window service.");
                        Console.WriteLine("/install ..... : Installs the service.");
                        Console.WriteLine("/start (or /a) : Starts the service if installed.");
                        Console.WriteLine("/pause ....... : Pauses the service if running.");
                        Console.WriteLine("/continue .... : Continues the service if paused.");
                        Console.WriteLine("/stop (or /o). : Stops the service if running.");
                        Console.WriteLine("/unistall .... : Uninstalls the service.");
                        Console.WriteLine("/status (or /t): Displays the current status of the service.");
                        break;
                    default:
                        Console.WriteLine("Enter '/help' or '/h' to view parameterization information.");
                        return (int)ToolError.UnsupportedCommand;
                }
            }
            else RunService();

            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Removes the first argument from the array.
        /// </summary>
        /// <param name="args">Argument's array.</param>
        /// <returns>The original array without the first element.</returns>
        private static string[] SubArray(string[] args)
        {
            int len = args.Length - 1;
            if (len == 0) return null;
            var res = new string[len];
            Array.Copy(args, 1, res, 0, len);
            return res;
        }

        /// <summary>
        /// Runs the service.
        /// </summary>
        /// <param name="args">Command line arguments for the service initialization.</param>
        private static void RunService(string[] args = null)
        {
            ServiceBase.Run(new ServiceBase[] {new BoxServiceHost(args)});
        }

        /// <summary>
        /// Executes a single calculation on a calculation box.
        /// </summary>
        /// <param name="args">Arguments for the calculation box.</param>
        /// <returns>Box error code.</returns>
        private static int ProcessSingleJob(string[] args)
        {
            int startingStage, endingStage, numberOfStages;

            int i = ParseBasicArgs(args, out startingStage, out endingStage, out numberOfStages);
            if (i != (int)ToolError.NoError) return i;
            var numbersOfWorkers = new int[numberOfStages];
            for (i = 0; i < numberOfStages; i++) numbersOfWorkers[i] = 1;
            return Controller.I.PerformSingleCalculation(startingStage, endingStage, numbersOfWorkers, args[0], args[1]);
        }

        /// <summary>
        /// Parses the basic arguments' list to setup the fiber block.
        /// </summary>
        /// <param name="args">The argument list.</param>
        /// <param name="startingStage">The starting stage of calculation.</param>
        /// <param name="endingStage">The ending stage of calculation.</param>
        /// <param name="numbersOfWorkers">The numbers of workers for each stage of calculation.</param>
        /// <returns>The error code.</returns>
        public static int ParseBlockArgs(string[] args, out int startingStage, out int endingStage, out int[] numbersOfWorkers)
        {
            int c, numberOfStages;
            numbersOfWorkers = null;

            int err = ParseBasicArgs(args, out startingStage, out endingStage, out numberOfStages);
            if (err != (int)ToolError.NoError) return err;

            if (args.Length < 2 + numberOfStages) return (int)ToolError.TooFewArguments;
            numbersOfWorkers = new int[numberOfStages];
            for (c = 0; c < numberOfStages; c++)
                if (!(Int32.TryParse(args[2 + c], out numbersOfWorkers[c]) && numbersOfWorkers[c] >= 0))
                    return (int)ToolError.InvalidWorkersNumber;

            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Parses the basic arguments for the initialization of the fiber block.
        /// </summary>
        /// <param name="args">The argument list.</param>
        /// <param name="startingStage">The starting stage of calculation.</param>
        /// <param name="endingStage">The ending stage of calculation.</param>
        /// <param name="numberOfStages">The number of stages.</param>
        /// <returns>The error code.</returns>
        private static int ParseBasicArgs(string[] args, out int startingStage, out int endingStage, out int numberOfStages)
        {
            int maxStage = 2;
            startingStage = 0;
            endingStage = 0;
            numberOfStages = 0;

            if (args == null) return (int)ToolError.TooFewArguments;
            int i = args.Length;
            if (i < 2) return (int)ToolError.TooFewArguments;

            if (!(Int32.TryParse(args[0], out startingStage) && startingStage > 0))
                return (int)ToolError.InvalidStartingStage;

            if (!(Int32.TryParse(args[1], out endingStage) && endingStage > 0))
                return (int)ToolError.InvalidEndingStage;

            numberOfStages = endingStage - startingStage + 1;
            if (numberOfStages <= 0 || startingStage > maxStage || endingStage > maxStage)
                return (int)ToolError.InvalidStages;

            return (int)ToolError.NoError;
        }
    }

    enum ToolError
    {
        NoError = 0,
        TooFewArguments = 100,
        InvalidEndingStage = 101,
        InvalidStartingStage = 102,
        InvalidStages = 103,
        InvalidWorkersNumber = 104,
        UnsupportedCommand = 201,
        InstallService = 202,
        InstallServiceNotRollBacked = 203,
        ServiceNotInstalled = 204,
        StartService = 205,
        StartServiceTimeOut = 206,
        PauseService = 207,
        PauseServiceTimeOut = 208,
        ContinueService = 209,
        ContinueServiceTimeOut = 210,
        StopService = 211,
        StopServiceTimeOut = 212,
        UnistallService = 213
    }
}
