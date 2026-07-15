using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clase
{
    public partial class FrmProveedores : Form
    {
        public FrmProveedores()
        {
            InitializeComponent();
            this.Load += FrmProveedores_Load;

        }
        private void FrmProveedores_Load(object sender, EventArgs e)
        {
            CargarProveedor();

        }



        private void CargarProveedor(string buscar = "")
        {
            try
            {
                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = @"SELECT id_proveedor, nombre, direccion, telefono, 
                                            contacto, productos_suministra, correo
                                     FROM proveedores
                                     WHERE nombre LIKE @buscar
                                        OR direccion LIKE @buscar
                                        OR telefono LIKE @buscar
                                        OR contacto LIKE @buscar
                                        OR productos_suministra LIKE @buscar
                                        OR correo LIKE @buscar";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@buscar", "%" + buscar + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable tabla = new DataTable();

                        adapter.Fill(tabla);

                        dataGridView1.DataSource = tabla;
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
                List<string> errores = Validar();

                if (errores.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errores), "Errores");
                    return;
                }

                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    if (!string.IsNullOrWhiteSpace(txtId.Text))
                    {

                        string queryUpdate = @"UPDATE proveedores SET
                                                nombre = @Nombre,
                                                direccion = @Direccion,
                                                telefono = @Telefono,
                                                contacto = @Contacto,
                                                productos_suministra = @Productos,
                                                correo = @Correo
                                                WHERE id_proveedor = @Id";

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                            cmd.Parameters.AddWithValue("@Contacto", txtContacto.Text.Trim());
                            cmd.Parameters.AddWithValue("@Productos", txtProductos.Text.Trim());
                            cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@Id", txtId.Text);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Proveedor actualizado correctamente.");
                                LimpiarCampos();
                                CargarProveedor();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar.");
                            }
                        }
                    }
                    else
                    {

                        string queryInsert = @"INSERT INTO proveedores
                                        (nombre, direccion, telefono, contacto, productos_suministra, correo)
                                        VALUES
                                        (@Nombre, @Direccion, @Telefono, @Contacto, @Productos, @Correo)";

                        using (MySqlCommand cmd = new MySqlCommand(queryInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                            cmd.Parameters.AddWithValue("@Contacto", txtContacto.Text.Trim());
                            cmd.Parameters.AddWithValue("@Productos", txtProductos.Text.Trim());
                            cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Proveedor registrado correctamente.");
                                LimpiarCampos();
                                CargarProveedor();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo registrar.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarProveedor(txtBuscar.Text.Trim());
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarProveedor(txtBuscar.Text.Trim());
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtContacto.Clear();
            txtProductos.Clear();
            txtCorreo.Clear();
            txtBuscar.Clear();
            txtNombre.Focus();
        }

        private List<string> Validar()
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("El nombre es obligatorio");
            else if (txtNombre.Text.Trim().Length < 6)
                errores.Add("El nombre debe tener al menos 6 caracteres");

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
                errores.Add("La dirección es obligatoria");

            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
                errores.Add("El teléfono es obligatorio");
            else if (!Regex.IsMatch(txtTelefono.Text.Trim(), @"^[0-9+\-\s]{7,15}$"))
                errores.Add("El teléfono no tiene un formato válido");

            if (string.IsNullOrWhiteSpace(txtContacto.Text))
                errores.Add("El contacto (persona) es obligatorio");

            if (string.IsNullOrWhiteSpace(txtProductos.Text))
                errores.Add("Debe indicar los productos que suministra");

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
                errores.Add("El correo es obligatorio");
            else if (!Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$"))
                errores.Add("El correo no tiene un formato válido");

            return errores;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtId.Text = row.Cells["id_proveedor"].Value.ToString();
                txtNombre.Text = row.Cells["nombre"].Value.ToString();
                txtDireccion.Text = row.Cells["direccion"].Value.ToString();
                txtTelefono.Text = row.Cells["telefono"].Value.ToString();
                txtContacto.Text = row.Cells["contacto"].Value.ToString();
                txtProductos.Text = row.Cells["productos_suministra"].Value.ToString();
                txtCorreo.Text = row.Cells["correo"].Value.ToString();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un proveedor para eliminar.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Desea eliminar este proveedor?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.No)
                    return;

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id_proveedor"].Value);

                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = "DELETE FROM proveedores WHERE id_proveedor=@Id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Proveedor eliminado correctamente.");

                            LimpiarCampos();
                            CargarProveedor();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExportarEx_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
            guardar.FileName = "Proveedores.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    
                    Conexion conexion = new Conexion();
                    DataTable tabla = new DataTable();

                    using (MySqlConnection conn = conexion.ObtenerConexion())
                    {
                        conn.Open();

                        string query = @"SELECT id_proveedor, nombre, direccion, telefono, 
                                                contacto, productos_suministra, correo
                                         FROM proveedores";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                            adapter.Fill(tabla);
                        }
                    }

                    XLWorkbook libro = new XLWorkbook();
                    var hoja = libro.Worksheets.Add("Proveedores");
                    hoja.Cell(1, 1).InsertTable(tabla);
                    hoja.Columns().AdjustToContents();

                    libro.SaveAs(guardar.FileName);
                    MessageBox.Show("Datos exportados correctamente a " + guardar.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }


            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un proveedor para editar.");
                    return;
                }
                DataGridViewRow row = dataGridView1.CurrentRow;
                txtId.Text = row.Cells["id_proveedor"].Value.ToString();
                txtNombre.Text = row.Cells["nombre"].Value.ToString();
                txtDireccion.Text = row.Cells["direccion"].Value.ToString();
                txtTelefono.Text = row.Cells["telefono"].Value.ToString();
                txtContacto.Text = row.Cells["contacto"].Value.ToString();
                txtProductos.Text = row.Cells["productos_suministra"].Value.ToString();
                txtCorreo.Text = row.Cells["correo"].Value.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos para editar: " + ex.Message);
        }   }
    }
}

