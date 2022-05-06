using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace mini_project.Data.Services
{
    public class AggregatorService
    {
        string connectionString;
        string tableName, hourlyTable, dailyTable;
        public AggregatorService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("VerticaConnectionString");
            tableName = configuration.GetValue<string>("TableName");
            hourlyTable = configuration.GetValue<string>("HourlyTable");
            dailyTable = configuration.GetValue<string>("DailyTable");
        }
        public void AggregateData()
        {
            VerticaConnection connection = new VerticaConnection(connectionString);
            connection.Open();
            AggregateHourly(connection);
            AggregateDaily(connection);
            connection.Close();
        }

        public void AggregateHourly(VerticaConnection connection)
        {
            VerticaCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO {hourlyTable} " +
                $"(TIME, LINK, SLOT, NEALIAS, NETYPE, MAX_RX_LEVEL, MAX_TX_LEVEL, RSL_DEVIATION) " +
                $"SELECT DATE_TRUNC('HOUR', TIME), LINK, SLOT, NEALIAS, NETYPE, MAX(MAXRXLEVEL), MAX(MAXTXLEVEL), ABS(MAX(MAXRXLEVEL)) - ABS(MAX(MAXTXLEVEL))" +
                $"FROM {tableName} GROUP BY 1, 2, 3, 4, 5";
            command.ExecuteNonQuery();

        }
        public void AggregateDaily(VerticaConnection connection)
        {
            VerticaCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO {dailyTable} " +
                $"(TIME, LINK, SLOT, NEALIAS, NETYPE, MAX_RX_LEVEL, MAX_TX_LEVEL, RSL_DEVIATION) " +
                $"SELECT DATE_TRUNC('DAY', TIME), LINK, SLOT, NEALIAS, NETYPE, MAX(MAX_RX_LEVEL), MAX(MAX_TX_LEVEL), ABS(MAX(MAX_RX_LEVEL)) - ABS(MAX(MAX_TX_LEVEL))" +
                $"FROM {hourlyTable} GROUP BY 1, 2, 3, 4, 5";
            command.ExecuteNonQuery();
        }
    }
}
