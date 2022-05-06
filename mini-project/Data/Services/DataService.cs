using Microsoft.Extensions.Configuration;
using mini_project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace mini_project.Data.Services
{
    public class DataService
    {
        string connectionString;
        string hourlyTable, dailyTable;
        public DataService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("VerticaConnectionString");
            hourlyTable = configuration.GetValue<string>("HourlyTable");
            dailyTable = configuration.GetValue<string>("DailyTable");
        }
        
        public List<DataObject> GetData(string granularity)
        {
            string tableName = hourlyTable;
            if (granularity == "day")
            {
                tableName = dailyTable;
            }
            else if (granularity == "hour")
            {
                tableName = hourlyTable;
            }
            List<DataObject> list = new List<DataObject>();
            DataObject obj;
            VerticaConnection connection = new VerticaConnection(connectionString);
            connection.Open();
            VerticaCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT TIME, LINK, NEALIAS, NETYPE, MAX(MAX_RX_LEVEL), MAX(MAX_TX_LEVEL), MAX(RSL_DEVIATION) FROM {tableName} GROUP BY 1,2,3,4";
            VerticaDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    obj = new DataObject();
                    obj.Time = (DateTime)reader[0];
                    obj.Link = (string)reader[1];
                    obj.NeAlias = (string)reader[2];
                    obj.NeType = (string)reader[3];
                    obj.Max_RX_Level = Convert.ToDouble(reader[4]);
                    obj.Max_TX_Level = Convert.ToDouble(reader[5]);
                    obj.RSL_Deviation = Convert.ToDouble(reader[6]);
                    list.Add(obj);
                }
            }
            reader.Close();
            connection.Close();
            return list;
        }

        public List<DataObject> GetDataBetween(string granularity, DateTime from_date, DateTime to_date)
        {
            string tableName = hourlyTable;
            if (granularity == "day")
            {
                tableName = dailyTable;
            }
            else if (granularity == "hour")
            {
                tableName = hourlyTable;
            }
            List<DataObject> list = new List<DataObject>();
            DataObject obj;
            VerticaConnection connection = new VerticaConnection(connectionString);
            connection.Open();
            VerticaCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT TIME, LINK, NEALIAS, NETYPE, MAX(MAX_RX_LEVEL), MAX(MAX_TX_LEVEL), MAX(RSL_DEVIATION) FROM {tableName} where Time between '{from_date}' and '{to_date}' group by 1,2,3,4;";
            VerticaDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    obj = new DataObject();
                    obj.Time = (DateTime)reader[0];
                    obj.Link = (string)reader[1];
                    obj.NeAlias = (string)reader[2];
                    obj.NeType = (string)reader[3];
                    obj.Max_RX_Level = Convert.ToDouble(reader[4]);
                    obj.Max_TX_Level = Convert.ToDouble(reader[5]);
                    obj.RSL_Deviation = Convert.ToDouble(reader[6]);
                    list.Add(obj);
                }
            }
            reader.Close();
            connection.Close();
            return list;
        }

    }
}
