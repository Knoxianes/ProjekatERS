using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Solar_panels_and__wind_generators
{
    public interface IMySqlConnection
    {
        public SqlConnection Connection { get; set; }

        void Open();
        void Close();
    }
}
