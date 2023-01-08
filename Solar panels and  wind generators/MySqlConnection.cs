using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Solar_panels_and__wind_generators
{
    public class MySqlConnection : IMySqlConnection
    {
        public SqlConnection Connection { get; set; }

        public MySqlConnection() 
        {
        
        }

        public void Close()
        {
            Connection.Close();
        }

        public void Open()
        {
            Connection.Open();
        }
    }
}
