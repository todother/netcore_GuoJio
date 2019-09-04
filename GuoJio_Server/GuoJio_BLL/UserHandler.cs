using GuoJio_Converter;
using GuoJio_DAL;
using GuoJio_Model;
using Sugar.Enties;
using System;
using System.Collections.Generic;
using System.Text;

namespace GuoJio_BLL
{
    public class UserHandler
    {
        public List<UserModel> getUsers()
        {
            List<tbl_user> users = new UserProvider().getUsers();
            List<UserModel> result = new List<UserModel>();
            foreach(var item in users)
            {
                result.Add(UserConverter.userEntityToModel(item));
            }
            return result;
        }
    }
}
