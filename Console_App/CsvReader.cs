using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace Console_App {
    public static class CsvReader {
        public static string[][] ReadCsv(string path) {
            
            if (!File.Exists(path)) {
                throw new FileNotFoundException("ScalerTM is not connected to the scale database");
            }

            using (TextFieldParser csvReader = new TextFieldParser(path)) {
                csvReader.Delimiters = new string[] { "," };
                List<string[]> rows = new List<string[]>();
                while (!csvReader.EndOfData) { 
                    rows.Add(csvReader.ReadFields());

                }
                return rows.ToArray();
            }
        }
    }
}
