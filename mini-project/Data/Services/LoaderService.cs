using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace mini_project.Data.Services
{
    public class LoaderService
    {
        string connectionString;
        string folderPath;
        string tableName;

        public LoaderService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("VerticaConnectionString");
            folderPath = configuration.GetValue<string>("LoaderDirectory");
            tableName = configuration.GetValue<string>("TableName");
        }

        public void CopyToDB(string filePath)
        {
            VerticaConnection connection = new VerticaConnection(connectionString);
            connection.Open();
            VerticaCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(1) FROM loaded_files WHERE fileName = '{filePath}'";
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

            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                filePath = file.Replace(@"\", @"/");
                //check!!!!
                command.CommandText = $"COPY {tableName} FROM LOCAL '{filePath}' DELIMITER ',' DIRECT SKIP 1; INSERT INTO loaded_files values('{filePath}');";
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
}
