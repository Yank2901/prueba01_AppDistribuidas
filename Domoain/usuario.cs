 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class usuario
    {
        private int _id;
        private string _nombre;
        private int _telefono;

        public usuario(int _id, string _nombre,int _telefono)
        {
            this._id= _id;
            this._nombre = _nombre;
            this._telefono= _telefono;
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        public int Telefono
        {
            get { return _telefono; }
            set { _telefono = value; }
        }

    }
}
