﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace MAS_Sustainability.Controllers
{
    public class DB
    {
        private String connectionString = @"server=localhost;port=3306;user id=root;database=mas_isscs;password=ThirtyFirst9731@;";

        public MySqlConnection DBConnection()
        {
            MySqlConnection mySqlConn = new MySqlConnection(connectionString);
            return mySqlConn;
        }


    }
}