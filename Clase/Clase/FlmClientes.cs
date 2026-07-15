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
    public partial class FlmClientes : Form
    {
        public FlmClientes()
        {
            InitializeComponent();
            this.Load += FlmClientes_Load;
        }

        private void FlmClientes_Load(object sender, EventArgs e)
        {
            CargarClientes();
        }

        private void Limpiar()
        {
            txtNombre.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtDireccion.Clear();
            txtBuscar.Clear();
            txtNombre.Focus();
        }

        private void CargarClientes(string buscar = "")
        {
            try
            {
                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = @"SELECT id_cliente, nombre, telefono, correo, direccion
                                     FROM clientes
                                     WHERE nombre LIKE @buscar
                                        OR telefono LIKE @buscar
                                        OR correo LIKE @buscar
                                        OR direccion LIKE @buscar";

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
                List<string> errores = new List<string>();

                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                    errores.Add("El nombre es obligatorio");

                if (txtNombre.Text.Trim().Length < 6)
                    errores.Add("El nombre debe tener al menos 6 caracteres");

                if (string.IsNullOrWhiteSpace(txtTelefono.Text))
                    errores.Add("El teléfono es obligatorio");

                if (string.IsNullOrWhiteSpace(txtCorreo.Text))
                    errores.Add("El correo es obligatorio");

                if (string.IsNullOrWhiteSpace(txtDireccion.Text))
                    errores.Add("La dirección es obligatoria");

                if (errores.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errores), "Errores");
                    return;
                }

                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = @"INSERT INTO clientes
                                    (nombre, telefono, correo, direccion)
                                    VALUES
                                    (@Nombre,@Telefono,@Correo,@Direccion)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                        cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Cliente guardado correctamente.");

                            Limpiar();
                            CargarClientes();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo guardar.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                txtId.Text = fila.Cells["id_cliente"].Value.ToString();
                txtNombre.Text = fila.Cells["nombre"].Value.ToString();
                txtTelefono.Text = fila.Cells["telefono"].Value.ToString();
                txtCorreo.Text = fila.Cells["correo"].Value.ToString();
                txtDireccion.Text = fila.Cells["direccion"].Value.ToString();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        { if (txtId.Text == "")
            {
                MessageBox.Show("Seleccione un cliente para editar.");
                return;
            }
            Conexion conexion = new Conexion();
            MySqlConnection conn = conexion.ObtenerConexion();
            conn.Open();
            string sql = "UPDATE clientes SET nombre ='" + txtNombre.Text + 
                "', telefono = '" + txtTelefono.Text + 
                "', correo = '" + txtCorreo.Text +
                "', direccion = '" + txtDireccion.Text + 
                "' WHERE id_cliente = " + txtId.Text;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Cliente editado correctamente.");
            Limpiar();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un cliente.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Desea eliminar este cliente?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.No)
                    return;

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id_cliente"].Value);

                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = "DELETE FROM clientes WHERE id_cliente=@Id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Cliente eliminado correctamente.");

                            Limpiar();
                            CargarClientes();
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

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarClientes(txtBuscar.Text.Trim());
        }

        private void lblErrorNombre_Click(object sender, EventArgs e)
        {
            if(txtNombre.Text.Length < 5)
            {
                MessageBox.Show("El nombre debe tener al menos 5 caracteres.");
            }
            else
            {
                lblErrorNombre.Text = "";

            }
        }

        private void lblErrotTelefono_Click(object sender, EventArgs e)
        {
            if(txtTelefono.Text.Length < 8)
            {
                MessageBox.Show("El teléfono debe tener al menos 8 caracteres.");
            } else {
                lblErrotTelefono.Text = "";
            }
         }

        private void btnExportarEx_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count == 0) 
            { 
                MessageBox.Show("No hay datos para exportar.");
                return;
            }
           SaveFileDialog guardar = new SaveFileDialog();
           guardar.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
           guardar.FileName = "Clientes.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XLWorkbook libro = new XLWorkbook();
                        
                    //Craer Hoja de excel
                    var hoja = libro.Worksheets.Add("Clientes");
                
                    DataTable tabla = (DataTable)dataGridView1.DataSource;
                    
                    hoja.Cell(1, 1).InsertTable(tabla);

                    //Guardar un Archivo
                    libro.SaveAs(guardar.FileName);
                    MessageBox.Show("Datos exportados correctamente a " + guardar.FileName);

                }
                catch (Exception ex)
                {
                         MessageBox.Show("Error al exportar: " + ex.Message);

                }

        }
    }
}
}