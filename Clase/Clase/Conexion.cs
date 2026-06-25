using MySql.Data.MySqlClient;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clase
{
    internal class Conexion
    {

        private string cadenaConexin = "Server=localhost;database=POS;uid=root;password=lolo20;";

    

    public MySqlConnection ObtenerConexion()

        {
            MySqlConnection conexion = new MySqlConnection(cadenaConexin);
            return conexion;

        }
    }
}
