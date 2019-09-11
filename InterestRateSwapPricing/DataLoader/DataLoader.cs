using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace InterestRateSwapPricing
{
    public class DataLoader
    {
        public static Dictionary<int, double> LoadSwapCurve(string swapCurveTableName)
        {
            var swapCurve = new Dictionary<int, double>();

            var settings = ConfigurationManager.ConnectionStrings["PricingFramework"];

            using (SqlConnection connection = new SqlConnection(settings.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM [PricingFrameworkDb].[dbo].[" + swapCurveTableName + "]", connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Maturity | Swap Rate");

                    while (reader.Read())
                    {
                        Console.WriteLine("{0} | {1}",
                            reader.GetInt32(0), reader.GetDouble(1));
                        swapCurve.Add(reader.GetInt32(0), reader.GetDouble(1));
                    }

                    Console.WriteLine();
                }
            }

            return swapCurve;
        }
    }
}