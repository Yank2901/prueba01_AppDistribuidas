using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UserDao:Connection
    {
        public bool validLogin(int _id, int _telefono)
        {
            using (var conexion = geConnection())
            {
                conexion.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conexion;
                    command.CommandText = "SELECT * FROM usuario WHERE _id=@id AND _telefono=@telefono";
                    command.Parameters.AddWithValue("@id", _id);
                    command.Parameters.AddWithValue("@telefono", _telefono);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return true;
                    }
                }
            }
            return false;
        }

        public void insertUser(string _nombre,string _direccion ,int _telefono)
        {
            using (var conexion = geConnection())
            {
                conexion.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conexion;
                    command.CommandText = "INSERT INTO usuario VALUES (@nombre, @direccion,@telefono);";
                    command.Parameters.AddWithValue("@nombre", _nombre);
                    command.Parameters.AddWithValue("@direccion", _direccion);
                    command.Parameters.AddWithValue("@telefono", _telefono);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertIP(string _ip)
        {
            using (var conexion = geConnection())
            {
                conexion.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conexion;
                    command.CommandText = "INSERT INTO listaIP(_ip) VALUES (@ip);";
                    command.Parameters.AddWithValue("@ip", _ip);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<string> getIPs()
        {
            List<string> IPs=new List<string>();
            using (var conexion = geConnection())
            {
                conexion.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conexion;
                    command.CommandText = "SELECT * FROM listaIP";
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        IPs.Add(reader[0].ToString());
                    }
                }
            }
            return IPs;
        }

        public bool validNumber(int _telefono)
        {
            using (var conexion = geConnection())
            {
                conexion.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conexion;
                    command.CommandText = "SELECT * FROM usuario WHERE _telefono=@telefono";
                    command.Parameters.AddWithValue("@telefono", _telefono);
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                        return false;
                }
            }
            return true;
        }
    }
}
