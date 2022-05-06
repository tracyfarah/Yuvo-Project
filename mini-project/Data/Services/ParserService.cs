
using Jitbit.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace mini_project.Data.Services
{
    public class ParserService
    {
        private string loaderDirectory;
        string connectionString;
        public ParserService(IConfiguration configuration)
        {
            loaderDirectory = configuration.GetValue<string>("LoaderDirectory");
            connectionString = configuration.GetConnectionString("VerticaConnectionString");
        }
        public void ParseData(string filePath)
        {
            string file_name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            VerticaConnection connection = new VerticaConnection(connectionString);
            connection.Open();
            VerticaCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(1) FROM parsed_files WHERE fileName = '{file_name}'";
            VerticaDataReader reader = command.ExecuteReader();
            //check if file already parsed
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                    if (Convert.ToInt64(reader[0]) != 0)
                        return;
                }
            }
            reader.Close();

            var myExport = new CsvExport();
            int count = 0;
            filePath = filePath.Replace(@"\", @"/");
            string newFile = loaderDirectory + "/" + System.IO.Path.GetFileNameWithoutExtension(filePath) + ".csv";
            string[] lines = System.IO.File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] data = line.Split(",");
                string[] obj = data[2].Split("_");
                string[] obj_split;
                int row = 1;
                string link, slot, port;
                string[] slots = new string[1];
                if (count != 0)
                {
                    if (!data[2].Equals("Unreachable Bulk FC") || data[17] == "-")
                    {
                        if (obj[0].Contains("."))
                        {
                            obj_split = obj[0].Split(".");
                            slot = obj_split[0].Split("/")[1];
                            port = obj_split[1].Split("/")[0];
                            link = slot + "/" + port;
                        }
                        else
                        {
                            link = obj[0].Substring(obj[0].IndexOf("/") + 1);
                            slot = link.Split("/")[0];
                            port = link.Split("/")[1];
                        }
                        slots[0] = slot;
                        if (link.Contains("+"))
                        {
                            row = link.Count((f => (f == '+'))) + 1;
                            slots = slot.Split("+");

                        }
                        for (int i = 0; i < row; i++)
                        {
                            myExport.AddRow();
                            myExport["Network_sid"] = hash(data[2], data[6]);
                            myExport["Datetime_key"] = parseDate(filePath);
                            myExport["NeId"] = data[1];
                            myExport["Object"] = data[2];
                            myExport["Time"] = data[3];
                            myExport["Interval"] = data[4];
                            myExport["Direction"] = data[5];
                            myExport["NeAlias"] = data[6];
                            myExport["NeType"] = data[7];
                            myExport["RxLevelBelowTS1"] = data[9];
                            myExport["RxLevelBelowTS2"] = data[10];
                            myExport["MinRxLevel"] = data[11];
                            myExport["MaxRxLevel"] = data[12];
                            myExport["TxLevelAboveTS1"] = data[13];
                            myExport["MinTxLevel"] = data[14];
                            myExport["MaxTxLevel"] = data[15];
                            myExport["IdLogNum"] = data[16];
                            myExport["FailureDescription"] = data[17];
                            myExport["Link"] = link;
                            myExport["TId"] = obj[2];
                            myExport["FarEndTid"] = obj[4];
                            myExport["Slot"] = slots[i];
                            myExport["Port"] = port;
                        }
                    }
                }
                count = 1;
            }

            command.CommandText = $"INSERT INTO parsed_files VALUES ('{file_name}')";
            command.ExecuteNonQuery();
            connection.Close();
            myExport.ExportToFile(newFile);
        }

        public int hash(string a, string b)
        {
            using (var md5Hash = MD5.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(a + b);
                var hashBytes = md5Hash.ComputeHash(sourceBytes);
                var hash = BitConverter.ToString(hashBytes);
                return Math.Abs(string.GetHashCode(hash));
            }
        }

        public DateTime parseDate(string filename)
        {
            string pattern = @".*_(\d{4})(\d{2})(\d{2})_(\d{2})(\d{2})(\d{2})";
            Regex re = new Regex(pattern);
            Match m = re.Match(filename);
            DateTime datetime = new DateTime();
            if (m.Success)
            {
                int year = Int32.Parse(m.Groups[1].Value);
                int month = Int32.Parse(m.Groups[2].Value);
                int day = Int32.Parse(m.Groups[3].Value);
                int hour = Int32.Parse(m.Groups[4].Value);
                int min = Int32.Parse(m.Groups[5].Value);
                int sec = Int32.Parse(m.Groups[6].Value);
                datetime = new DateTime(year, month, day, hour, min, sec);
            }
            return datetime;

        }
    }
}
