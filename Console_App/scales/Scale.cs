using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_App {
    public partial class Scale {
        // private
        bool _connected;
        string _name;

        // public
        public bool Connected { get { return _connected; } }
        public string Name { get { return _name; } }

        public Scale(string name) {
            _connected = false;
            _name = name;   
        }
        public void Connect() {
            if (!File.Exists($"{Program.ScalePath}\\scales\\{Name}.csv")) {
                throw new Exception($"Scale {Name} is turned off");
            }
            
            _connected = true;
            
        }
        ~Scale() {
            Disconnect();
        }
        public void Disconnect() {
            _connected = false;
        }

        ScaleDayData[] ReadStatistics() {
            if (!_connected) {
                // Gets triggered when the connection attempt has failed or hasn't been made yet
                throw new Exception("Scale is disconnected");
            }
            if (!File.Exists($"{Program.ScalePath}\\scales\\{Name}.csv")) {
                // Gets triggered when the scale has been connected, but the file has been deleted since then
                _connected = false;
                throw new FileNotFoundException("Scale has been disconnected");
            }
            string[][] data = CsvReader.ReadCsv($"{Program.ScalePath}\\scales\\{Name}.csv");
            List<ScaleDayData> statistics = new List<ScaleDayData>();
            
            foreach (string[] day in data) {
                try {
                    statistics.Add(new ScaleDayData(day));
                } catch (ArgumentException e) {
                    Console.WriteLine($"Scale {Name}: {e.Message}");
                }
            }
            return statistics.ToArray();
        }

        public void ReadDailyStatistics() {
            
            foreach (ScaleDayData data in ReadStatistics()) {
                Console.WriteLine(data);
            }
        }
        public void ReadStatisticsSummary() {
            ScaleDayData[] statistics = ReadStatistics();
            int days = statistics.Count();
            int countSum = statistics.Select(c => (int)c.Count).ToArray().Sum();
            int weightAvg = (int)Math.Round(statistics.Select(w => (int)w.Weight).ToArray().Average());
            int deviationAvg = (int)Math.Round(statistics.Select(d => (int)d.Deviation).ToArray().Average());
            Console.WriteLine($"\tDays: {days}\n" +
                $"\tCount: {countSum}\n" +
                $"\tAverage Weight: {weightAvg}g\n" +
                $"\tAverage Deviation: {deviationAvg}g");
        }
        
    }
}
