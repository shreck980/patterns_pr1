using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace noslq_pr.DAO
{
    public class LastIdGetter  
    {
        protected string GetLastID;
        protected long GetLastId(MySqlConnection connection, MySqlTransaction transaction)
        {
            long id = 0;
            using (MySqlCommand command = new MySqlCommand(GetLastID, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToInt64(result);
                }
            }

            return id;
        }

        protected long GetLastId(MySqlConnection connection, MySqlTransaction transaction, string GetLastIdQuery)
        {
            long id = 0;
            using (MySqlCommand command = new MySqlCommand(GetLastIdQuery, connection))
            {
                command.Transaction = transaction;
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    id = Convert.ToInt64(result);
                }
            }

            return id;
        }

    }
}

