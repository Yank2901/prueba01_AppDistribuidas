﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace Domain
{
    public class UserModel
    {
        UserDao _userDao=new UserDao();

        public bool validLogin(int _id, int _telefono)
        {
            return _userDao.validLogin(_id, _telefono);
        }

        public void insertIP(string _ip)
        {
            _userDao.insertIP(_ip);
        }

        public List<string> getIPs()
        {
            return _userDao.getIPs();
        }

        public int insertUser(string _nombre, int _telefono)
        {
            return _userDao.insertUser(_nombre,_telefono);
        }

        public bool validNumber(int _telefono)
        {
            return _userDao.validNumber(_telefono);
        }
    }
}
