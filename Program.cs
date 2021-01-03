using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadTLE
{
    class Program
    {
        private static double UTCToJulianDate(int year, int mounth, int day, int hour = 0, int min = 0, int sec = 0)
        {
            return 367 * year - (int)(7 * (year + (int)((mounth + 9) / 12.0)) / 4) + (int)(275 * mounth / 9.0) +
                day + 1721013.5 + ((sec / 60.0 + min) / 60.0 + hour) / 24.0;
        }

        /// <summary>
        /// Этот метод копирует из переданной строки подстроку начиная со startIndex и по endIndex
        /// </summary>
        /// <returns>Подстрока из переданной строки</returns>
        private static string copyAtIndex(string str, int startIndex, int endIndex)
        {
            string temp = "";
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (str[i] == '.')
                {
                    temp += ',';
                    continue;
                }
                temp += str[i];
            }
            return temp;
        }

        private static double date(string line)
        {
            int.TryParse(copyAtIndex(line, 18, 19), out int year);
            year += 2000;

            int mounth = 1;
            double.TryParse(copyAtIndex(line, 20, 31), out double fractionalDay);
            int day = (int)fractionalDay;
            double reminderDay = fractionalDay - day;

            double fractionalHours = reminderDay * 24;
            int hours = (int)fractionalHours;
            double reminderHours = fractionalHours - hours;

            double fractionalMinutes = reminderHours * 60;
            int minutes = (int)fractionalMinutes;
            double reminderMinutes = fractionalMinutes - minutes;

            double fractionalSeconds = reminderMinutes * 60;
            int seconds = (int)fractionalSeconds;

            return UTCToJulianDate(year, mounth, day, hours, minutes, seconds);
        }

        public static void SaveData<T>(List<T> array, string title)
        {
            string path = $"{Environment.CurrentDirectory}\\{title}.txt";
            using (StreamWriter writer = File.CreateText(path))
            {
                string output = $"-----------------------{title}----------------------\n";

                foreach (var item in array)
                {
                    output += item + "\n";
                }
                writer.Write(output);
            }
        }

        static void Main(string[] args)
        {
            string path = $"{Environment.CurrentDirectory}\\data.txt";

            List<double> jd = new List<double>();
            List<double> omega = new List<double>();
            List<double> M = new List<double>();
            List<double> w = new List<double>();

            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {                        
                        if(line[0] == '1')
                            jd.Add(date(line));
                        else if (line[0] == '2')
                        {
                            double.TryParse(copyAtIndex(line, 34, 41), out double wTemp);
                            if (jd.Last() < 2458503)
                                w.Add(wTemp + 360);
                            else 
                                w.Add(wTemp);

                            double.TryParse(copyAtIndex(line, 17, 24), out double omegaTemp);
                            if (jd.Last() < 2458458.7) 
                                omega.Add(omegaTemp);
                            else
                                omega.Add(omegaTemp + 360);

                            double.TryParse(copyAtIndex(line, 43, 50), out double MTemp);
                            if (jd.Last() < 2458495 && MTemp > 120)
                                M.Add(MTemp);
                            else
                                M.Add(MTemp + 360);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            SaveData(jd, "jd");
            SaveData(omega, "omega");
            SaveData(M, "M");
            SaveData(w, "w");
        }
    }
}
