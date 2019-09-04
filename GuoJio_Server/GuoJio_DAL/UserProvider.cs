using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using MySql.Data;
using GuoJio_DBHelper;
using Sugar.Enties;

namespace GuoJio_DAL
{
    public class UserProvider
    {
        public List<tbl_user> getUsers()
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<tbl_user> users = db.Queryable<tbl_user>().Take(30).ToList();
            return users;
        }
    }
}
