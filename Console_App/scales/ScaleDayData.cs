using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_App {
    public partial class Scale {
        public record ScaleDayData {
            // private
            ushort _day;
            ushort _count;
            ushort _weight;
            ushort _deviation;
            // public
            public ushort Day { get { return _day; } }
            public ushort Count { get { return _count; } }
            public ushort Weight { get { return _weight; } }
            public ushort Deviation { get { return _deviation; } }

            public ScaleDayData(string[] data) {
                // Parse data
                if(data.Length != 4) {
                    throw new ArgumentException("Invalid number of values or corrupted data");
                } else if (!ushort.TryParse(data[0], out _day)) {
                    throw new ArgumentException("Invalid Day value or corrupted data");
                } else if (!ushort.TryParse(data[1], out _count)) {
                    throw new ArgumentException("Invalid Count value or corrupted data");
                } else if (!ushort.TryParse(data[2], out _weight)) {
                    throw new ArgumentException("Invalid Weight value or corrupted data");
                } else if (!ushort.TryParse(data[3], out _deviation)) {
                    throw new ArgumentException("Invalid Deviation value or corrupted data");
                }

                // Verify Day
                if(Day < 1 || Day > 365) {
                    throw new ArgumentException("Invalid Day value or corrupted data");
                }
            }
            public override string ToString() {
                return $"\tDay {Day}:\n" +
                    $"\t\tCount: {Count}\n" +
                    $"\t\tAverage Weight: {Weight}g\n" +
                    $"\t\tAverage Deviation: {Deviation}g";
            }
        }
    }
}
