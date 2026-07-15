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
    public partial class FrmProducto : Form
    {
        public FrmProducto()
        {
            InitializeComponent();
            this.Load += FrmProducto_Load;
        }

        private void FrmProducto_Load(object sender, EventArgs e)
        {
            CargarProducto();
        }

        private void CargarProducto(string buscar = "")
        {
            try
            {
                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = @"SELECT id_producto, nombre, descripcion, precio, stock
                                     FROM producto 
                                     WHERE nombre LIKE @buscar
                                        OR descripcion LIKE @buscar
                                        OR precio LIKE @buscar
                                        OR stock LIKE @buscar ";

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

        private void Limpiar()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtBuscar.Clear();
            txtNombre.Focus();
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

                if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
                    errores.Add("La descripción es obligatoria");

                if (string.IsNullOrWhiteSpace(txtPrecio.Text))
                    errores.Add("El precio es obligatorio");
                else if (!decimal.TryParse(txtPrecio.Text, out _))
                    errores.Add("El precio debe ser un número válido");

                if (string.IsNullOrWhiteSpace(txtStock.Text))
                    errores.Add("El stock es obligatorio");
                else if (!int.TryParse(txtStock.Text, out _))
                    errores.Add("El stock debe ser un número entero válido");

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
                        string queryUpdate = @"UPDATE producto SET
                                                nombre = @Nombre,
                                                descripcion = @Descripcion,
                                                precio = @Precio,
                                                stock = @Stock
                                                WHERE id_producto = @Id";

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                            cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text);
                            cmd.Parameters.AddWithValue("@Stock", txtStock.Text);
                            cmd.Parameters.AddWithValue("@Id", txtId.Text);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Producto actualizado correctamente.");
                                Limpiar();
                                CargarProducto();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar.");
                            }
                        }
                    }
                    else
                    {
                        string queryInsert = @"INSERT INTO producto
                                        (nombre, descripcion, precio, stock)
                                        VALUES
                                        (@Nombre, @Descripcion, @Precio, @Stock)";

                        using (MySqlCommand cmd = new MySqlCommand(queryInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                            cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text);
                            cmd.Parameters.AddWithValue("@Stock", txtStock.Text);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Producto guardado correctamente.");
                                Limpiar();
                                CargarProducto();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo guardar.");
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtId.Text = row.Cells["id_producto"].Value.ToString();
                txtNombre.Text = row.Cells["nombre"].Value.ToString();
                txtDescripcion.Text = row.Cells["descripcion"].Value.ToString();
                txtPrecio.Text = row.Cells["precio"].Value.ToString();
                txtStock.Text = row.Cells["stock"].Value.ToString();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Seleccione un producto para editar.");
                return;
            }

            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
                errores.Add("El precio es obligatorio");
            else if (!decimal.TryParse(txtPrecio.Text, out _))
                errores.Add("El precio debe ser un número válido");

            if (string.IsNullOrWhiteSpace(txtStock.Text))
                errores.Add("El stock es obligatorio");
            else if (!int.TryParse(txtStock.Text, out _))
                errores.Add("El stock debe ser un número entero válido");

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Errores");
                return;
            }

            try
            {
                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    
                    string sql = @"UPDATE producto SET
                                    nombre = @Nombre,
                                    descripcion = @Descripcion,
                                    precio = @Precio,
                                    stock = @Stock
                                    WHERE id_producto = @Id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                        cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text);
                        cmd.Parameters.AddWithValue("@Stock", txtStock.Text);
                        cmd.Parameters.AddWithValue("@Id", txtId.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto editado correctamente.");
                Limpiar();
                CargarProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un producto para eliminar.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Desea eliminar este producto?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.No)
                    return;

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id_producto"].Value);

                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = "DELETE FROM producto WHERE id_producto=@Id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Producto eliminado correctamente.");

                            Limpiar();
                            CargarProducto();
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
            CargarProducto(txtBuscar.Text.Trim());
        }

        private void lblErrorNombre_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Length < 5)
            {
                MessageBox.Show("El nombre debe tener al menos 5 caracteres.");
            }
            else
            {
                lblErrorNombre.Text = "";
            }
        }

        private void lblErrorPrecio_Click(object sender, EventArgs e)
        {
            if (txtPrecio.Text.Length < 10)
            {
                MessageBox.Show("El precio debe tener al menos 10 caracteres.");
            }
            else
            {
                lblErroPrecio.Text = "";
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
            guardar.FileName = "Producto.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XLWorkbook libro = new XLWorkbook();

                    var hoja = libro.Worksheets.Add("Producto");

                    DataTable tabla = (DataTable)dataGridView1.DataSource;

                    hoja.Cell(1, 1).InsertTable(tabla);

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