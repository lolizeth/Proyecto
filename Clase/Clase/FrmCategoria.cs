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
    public partial class FrmCategoria : Form
    {
        public FrmCategoria()
        {
            InitializeComponent();
            this.Load += FrmCategoria_Load;
        }

        private void FrmCategoria_Load(object sender, EventArgs e)
        {
            CargarCategoria();
        }

        private void CargarCategoria(string buscar = "")
        {
            try
            {
                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = @"SELECT id_categoria, descripcion
                                     FROM categoria
                                     WHERE descripcion LIKE @buscar";

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




        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarCategoria(txtBuscar.Text.Trim());
        }

        private void Limpiar()
        {
            txtId.Clear();
            txtDescripcion.Clear();
            txtBuscar.Clear();
            txtDescripcion.Focus();
        }

        private List<string> Validar()
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
                errores.Add("La descripción es obligatoria");

            return errores;
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

                        string queryUpdate = @"UPDATE categoria SET
                                                descripcion = @Descripcion
                                                WHERE id_categoria = @Id";

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());
                            cmd.Parameters.AddWithValue("@Id", txtId.Text);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Categoría actualizada correctamente.");
                                Limpiar();
                                CargarCategoria();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar.");
                            }
                        }
                    }
                    else
                    {

                        string queryInsert = @"INSERT INTO categoria
                                        (descripcion)
                                        VALUES
                                        (@Descripcion)";

                        using (MySqlCommand cmd = new MySqlCommand(queryInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Categoría registrada correctamente.");
                                Limpiar();
                                CargarCategoria();
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

        private void btnEditar_Click(object sender, EventArgs e)
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

                        string queryUpdate = @"UPDATE categoria SET
                                                descripcion = @Descripcion
                                                WHERE id_categoria = @Id";

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());
                            cmd.Parameters.AddWithValue("@Id", txtId.Text);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Categoría actualizada correctamente.");
                                Limpiar();
                                CargarCategoria();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar.");
                            }
                        }
                    }
                    else
                    {

                        string queryInsert = @"INSERT INTO categoria
                                        (descripcion)
                                        VALUES
                                        (@Descripcion)";

                        using (MySqlCommand cmd = new MySqlCommand(queryInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Categoría registrada correctamente.");
                                Limpiar();
                                CargarCategoria();
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
            CargarCategoria(txtBuscar.Text.Trim());
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione una categoría para eliminar.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Desea eliminar esta categoría?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.No)
                    return;

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id_categoria"].Value);

                Conexion conexion = new Conexion();

                using (MySqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    string query = "DELETE FROM categoria WHERE id_categoria=@Id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Categoría eliminada correctamente.");

                            Limpiar();
                            CargarCategoria();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtId.Text = row.Cells["id_categoria"].Value.ToString();
                txtDescripcion.Text = row.Cells["descripcion"].Value.ToString();
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
            guardar.FileName = "Categorias.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Se exportan TODOS los registros de la base de datos, no solo
                    // lo que esté filtrado/visible en el grid en ese momento.
                    Conexion conexion = new Conexion();
                    DataTable tabla = new DataTable();

                    using (MySqlConnection conn = conexion.ObtenerConexion())
                    {
                        conn.Open();

                        string query = @"SELECT id_categoria, descripcion FROM categoria";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                            adapter.Fill(tabla);
                        }
                    }

                    XLWorkbook libro = new XLWorkbook();
                    var hoja = libro.Worksheets.Add("Categorias");
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
    }
}