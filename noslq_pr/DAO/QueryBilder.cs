using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySqlConnector;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace noslq_pr.DAO
{
    public class QueryBilder
    {

        public static List<MySqlParameter> Update(string tableName, Dictionary<string, object> updatedValues,string whereClause, out string query)
        {
            if (updatedValues == null || updatedValues.Count == 0)
                throw new ArgumentException("No columns to update provided.");

          
            var queryBuilder = new StringBuilder($"UPDATE {tableName} SET ");
            var parameters = new List<MySqlParameter>();

            foreach (var column in updatedValues)
            {
                queryBuilder.Append($"{column.Key} = @{column.Key}, ");
                parameters.Add(new MySqlParameter($"@{column.Key}", column.Value ?? DBNull.Value));
            }
          
            queryBuilder.Length -= 2;

            queryBuilder.Append(" WHERE " + whereClause);
            query= queryBuilder.ToString();

            return parameters;


        }
    }
}
