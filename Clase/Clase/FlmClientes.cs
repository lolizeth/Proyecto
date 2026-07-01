using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clase
{
    public partial class FlmClientes : Form
    {
        public FlmClientes()
        {
            InitializeComponent();
        }

        private void CargarClientes(string buscar = "")
        {
            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();
                    string query = "@SELECT id, nombre, telefono, correo, direccion FROM " +
                        "Cliente WHERE nombre LIKE @buscar or telefono LIKE @buscar or" +
                        " correo LIKE @buscar or direccion LIKE @buscar";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@buscar", "%" + buscar + "%");
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable tabla = new DataTable();
                            adapter.Fill(tabla);
                            dataGridView1.DataSource = tabla;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
  



        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try 
            {

                Conexion conexion = new Conexion();
                using(MySqlConnection conn = conexion.ObtenerConexion())
                {

                     conn.Open();
                    string query = "INSERT INTO Cliente (Nombre,  Telefono, Correo,Direccion) VALUES (@Nombre, @Telefono, @Correo, @Direccion)";
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                        cmd.ExecuteNonQuery();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cliente guardado correctamente");
                            txtNombre.Clear();

                            txtCorreo.Clear();
                            txtTelefono.Clear();
                            txtCorreo.Clear();
                            txtDireccion.Clear();

                        }
                        else
                        {
                            MessageBox.Show("Error al guardar el cliente");
                        }

                    }
            }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Error: " + ex.Message);
            }
    }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            CargarClientes(txtBuscar.Text);
        }
    }
}
