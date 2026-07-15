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
using ClosedXML.Excel;

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

        private void Limpiar()
        {
            if (txtId != null) txtId.Clear();
            txtNombre.Clear();
            txtCorreo.Clear();
            txtTelefono.Clear();
            txtArea.Clear();
            txtAños.Clear();
            txtDireccion.Clear();
            txtNombre.Focus();
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
                    telefono, area, anos_trabajando, direccion From empleados 
                     WHERE nombre LIKE @buscar OR correo 
                    LIKE @buscar OR telefono LIKE @buscar
                    OR area LIKE @buscar OR direccion LIKE @buscar";
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

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtArea.Text) ||
                string.IsNullOrWhiteSpace(txtAños.Text))
            {
                MessageBox.Show("Complete todos los campos antes de guardar.");
                return false;
            }

            if (txtNombre.Text.Trim().Length < 5)
            {
                MessageBox.Show("Nombre del empleado muy corto, debe contener al menos 5 caracteres.");
                return false;
            }

            if (txtTelefono.Text.Trim().Length < 8)
            {
                MessageBox.Show("Teléfono del empleado muy corto, debe contener al menos 8 caracteres.");
                return false;
            }

            if (!int.TryParse(txtAños.Text.Trim(), out int anos) || anos < 0)
            {
                MessageBox.Show("Años trabajando debe ser un número entero válido.");
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

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
                            Limpiar();
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

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (txtId == null || string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione un empleado para editar.");
                return;
            }

            if (!ValidarCampos())
                return;

            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();
                    string query = @"UPDATE empleados SET
                                      nombre = @nombre,
                                      direccion = @direccion,
                                      correo = @correo,
                                      telefono = @telefono,
                                      area = @area,
                                      anos_trabajando = @anos_trabajando
                                      WHERE id_empleado = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@area", txtArea.Text);
                        cmd.Parameters.AddWithValue("@anos_trabajando", txtAños.Text);
                        cmd.Parameters.AddWithValue("@id", txtId.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Empleado editado correctamente.");
                            Limpiar();
                            CargarEmpleados();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo editar el empleado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (txtId == null || string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione un empleado para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este empleado?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.No)
                return;

            try
            {
                Conexion conexion = new Conexion();
                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();
                    string query = "DELETE FROM empleados WHERE id_empleado = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Empleado eliminado correctamente.");
                            Limpiar();
                            CargarEmpleados();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el empleado.");
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
            if (e.RowIndex >= 0)
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

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dtEmpleado.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
            guardar.FileName = "Empleados.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XLWorkbook libro = new XLWorkbook();


                    var hoja = libro.Worksheets.Add("Empleados");

                    DataTable tabla = (DataTable)dtEmpleado.DataSource;

                    hoja.Cell(1, 1).InsertTable(tabla);


                    libro.SaveAs(guardar.FileName);
                    MessageBox.Show("Datos exportados correctamente a " + guardar.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }

        }    }
    }
}