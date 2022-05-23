using Microsoft.VisualBasic.FileIO;

namespace Console_App {
    public static class Program {
        
        //private
        static Command[] _commands = new Command[] {
            
            new Command("help", "Shows this help menu", new string[]{}, Help),

            new Command("listConnectedScales", "Lists all connected scales", new string[]{}, ListConnectedScales),

            new Command("listAllScales", "Lists all scales in database", new string[]{}, ListAllScales),

            new Command("connectScales", "Tries to connect to all scales in the scale database", new string[]{}, ConnectScales),

            new Command("disconnectScales", "Disconnects all scales", new string[]{}, DisconnectScales),

            new Command("showDailyStats", "Shows daily statistics of selected connected scales, or all if no argument is given",
                new string[]{"string[] scaleNames = allScales"}, ShowDailyStatistics),

            new Command("showSummaryStats", "Shows summary statistics of selected connected scales, or all if no argument is given",
                new string[]{"string[] scaleNames = allScales"}, ShowSummaryStatistics),

            new Command("exit", "Shuts down Scaler™", new string[]{}, Exit),

            new Command("ez", "Connects to all scales if not already connected, gets stat summaries and all daily stats, " +
                "and disconnects if it was disconnected before", new string[]{}, GetEzStats)
        };

        static Scale[] _scales = new Scale[] { };

        static bool _exit = false;

        static string _scalePath = ".\\"; // Path where to look for scales.csv, individual scales are in _scalePath\scales

        // public
        public static string ScalePath { get { return _scalePath; } }
        public static void Main(string[] args) {
            if (args.Length > 0) {
                _scalePath = args[0]; // Made possible to pass a custom path to scale database
            }
            Console.WriteLine("Welcome to Scaler™ - Scale management console");
            Console.WriteLine("Discovering all scales in the scale database");

            try {
                string[][] csvScales = CsvReader.ReadCsv($"{ScalePath}\\scales.csv");
                List<Scale> scales = new List<Scale>();
                foreach (string scale in csvScales[0]) {
                    scales.Add(new Scale(scale));
                }
                _scales = scales.ToArray();
            } catch (FileNotFoundException ex) {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Start by typing 'help' to list all available commads");

            while (!_exit) {
                Console.Write(">_");
                string input = Console.ReadLine();
                if (input == string.Empty) {
                    continue;
                }
                ParseInput(input);
            }

        }
        public static void Exit(string[] args) {
            _exit = true;
        }
        static void ParseInput(string input) {
            input = input.Trim();  
            string cmd = input.Split(' ')[0];
            string[] args = input.Split(' ').Skip(1).ToArray();

            Command[] possibleCommands = _commands.Where(c => c.Name == cmd).ToArray();
            if (possibleCommands.Length == 0) {
                Console.WriteLine("Invalid command, please type 'help' to list all available commands");
                return;
            } else if (possibleCommands.Length > 1) {
                Console.WriteLine("I'm impressed if you managed to get this message");
                return;
            }
            possibleCommands[0].Execute(args);

        }
        public static void Help(string[] args) {
            Console.WriteLine("Syntax: Command arg1 arg2 arg3 arg4 ...\n");
            
            foreach (Command cmd in _commands) {
                string argString = string.Join(", ", cmd.Args);
                string cmdString = $"{cmd.Name }({argString})";
                
                string separator = string.Join("", Enumerable.Repeat('.', cmdString.Length <= 69 ? 69-cmdString.Length : 0).ToArray());
                Console.WriteLine($"{cmdString} {separator} {cmd.Description}");
            }
        }

        public static void ListConnectedScales(string[] args) {
            Console.WriteLine($"Number of connected scales: {_scales.Where(s => s.Connected).Count()}");
            foreach (Scale scale in _scales.Where(s => s.Connected)) {
                Console.WriteLine($"{scale.Name}");
            }
        }
        public static void ListAllScales(string[] args) {
            Console.WriteLine($"Number of scales: {_scales.Length}");
            foreach (Scale scale in _scales) {
                Console.WriteLine($"{scale.Name}");
            }
        }

        public static void ConnectScales(string[] args) {

            foreach (Scale scale in _scales) {
                try {
                    scale.Connect();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static Scale[] GetScales(string[] args) {
            Scale[] possibleScales;
            if (args.Length == 0) {
                possibleScales = _scales;
            } else {
                possibleScales = _scales.Where(s => args.Contains(s.Name)).ToArray();
            }
            Console.WriteLine($"Scales found ({possibleScales.Length}): {string.Join(", ", possibleScales.Select(s => s.Name))}");
            return possibleScales;
        }

        public static void ShowDailyStatistics(string[] args) {
            Scale[] scales = GetScales(args);

            foreach (Scale scale in scales) {
                try {
                    Console.WriteLine($"Scale {scale.Name}:");
                    scale.ReadDailyStatistics();
                } catch (Exception ex) {
                    Console.WriteLine($"\t{ex.Message}");
                }
            }
        }
        public static void ShowSummaryStatistics(string[] args) {

            Scale[] scales = GetScales(args);

            foreach (Scale scale in scales) {
                try {
                    Console.WriteLine($"Scale {scale.Name}:");
                    scale.ReadStatisticsSummary();
                } catch (Exception ex) {
                    Console.WriteLine($"\t{ex.Message}");
                }
            }
        }

        public static void DisconnectScales(string[] args) {
            foreach (Scale scale in _scales) {
                scale.Disconnect();
            }
        }

        public static void GetEzStats(string[] args) {
            bool connectAndDisconnect = false; // If user has not connected,
                                               // it will connect and then disconnect,
                                               // to preserve the state before the command was executed
            if (_scales.Where(s => s.Connected).Count() == 0) {
                ConnectScales(new string[] { });
                connectAndDisconnect = true;
            }

            foreach (Scale scale in _scales) {
                try {
                    Console.WriteLine($"Scale {scale.Name}:");
                    scale.ReadStatisticsSummary();
                    scale.ReadDailyStatistics();
                } catch (Exception ex) {
                    Console.WriteLine($"\t{ex.Message}");
                }
            }
            if (connectAndDisconnect) {
                DisconnectScales(new string[] { });
            }
        }

    }
}