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
    public partial class FrmEmpleados : Form
    {
        public FrmEmpleados()
        {

            InitializeComponent();
            this.Load += FrmEmpleados_Load;
        }

        private void FrmEmpleados_Load(object sender, EventArgs e)
        {
            CargarEmpleados();
        }


        private void CargarEmpleados()
        {
            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();
                    string query = @"SELECT id_empleado, nombre, correo, 
                    telefono,area, anos_trabajando, direccion From empleados 
                     WHERE NOMBRE LIKE @buscar OR correo 
                    LIKE @buscar OR telefono LIKE @buscar";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@buscar", "%" + txtBuscar.Text + "%");
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dtEmpleado.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los empleados: " + ex.Message);
            }
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarEmpleados();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
     string.IsNullOrWhiteSpace(txtDireccion.Text) ||
     string.IsNullOrWhiteSpace(txtTelefono.Text) ||
     string.IsNullOrWhiteSpace(txtCorreo.Text)||
     string.IsNullOrWhiteSpace(txtArea.Text)
     ||string.IsNullOrWhiteSpace(txtAños.Text))

            {
                MessageBox.Show("Complete todos los campos antes de guardar.");
                return;
            }
            if (txtNombre.Text.Trim().Length < 5)
            {
                MessageBox.Show("Nombre del cliente muy corto, debe contener mas de 3 caracteres");
                return;
            }
            if (txtTelefono.Text.Trim().Length < 8)
            {
                MessageBox.Show("Telefono del empleado muy corto, debe contener mas de 10 caracteres");
                return;
            }

            try
            {

                Conexion conexion = new Conexion();
                using (MySqlConnection conn = conexion.ObtenerConexion())
                {

                    conn.Open();
                    string query = "INSERT INTO empleados" +
                        "(nombre, direccion, correo, telefono, area, anos_trabajando) " +
                        "VALUES (@nombre, @direccion, @correo, @telefono, @area, @anos_trabajando)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@area", txtArea.Text);
                        cmd.Parameters.AddWithValue("@anos_trabajando", txtAños.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Empleado guardado correctamente");
                            txtNombre.Clear();
                            txtCorreo.Clear();
                            txtTelefono.Clear();
                            txtArea.Clear();
                            txtAños.Clear();
                            txtDireccion.Clear();
                            if (txtId != null) txtId.Clear();
                            CargarEmpleados();

                        }
                        else
                        {
                            MessageBox.Show("Error al guardar el Empleado");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dtEmpleado_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                DataGridViewRow row = dtEmpleado.Rows[e.RowIndex];
                txtId.Text = row.Cells["id_empleado"].Value.ToString();
                txtNombre.Text = row.Cells["nombre"].Value.ToString();
                txtCorreo.Text = row.Cells["correo"].Value.ToString();
                txtTelefono.Text = row.Cells["telefono"].Value.ToString();
                txtArea.Text = row.Cells["area"].Value.ToString();
                txtAños.Text = row.Cells["anos_trabajando"].Value.ToString();
                txtDireccion.Text = row.Cells["direccion"].Value.ToString();
            }
    }
}
}
