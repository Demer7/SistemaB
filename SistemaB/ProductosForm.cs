using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace SistemaB
{
    public partial class ProductosForm : Form
    {
        // Componentes de la Izquierda (Registro / Edición)
        private Guna2Panel panelRegistro;
        private Guna2TextBox txtNombre, txtPrecioVenta, txtCostoEstimado;
        private Guna2Button btnAgregar;

        // Componentes de la Derecha (Lista)
        private Guna2Panel panelLista;
        private Guna2DataGridView dgProductos;

        // Cabecera y Navegación
        private Guna2Button btnVolver;
        private Guna2HtmlLabel lblTitulo, lblSubtitulo;

        // Cadena de conexión de tu BD
        private string cadenaConexion = "Server=.\\SQLEXPRESS;Database=CherriesBloodDB;Trusted_Connection=True;";

        // Variable de control para el estado de edición
        private string idProductoSeleccionado = null;

        public ProductosForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(242, 242, 242);

            ConfigurarInterfaz();

            // Responsividad centrada
            this.Resize += (s, e) => CentrarTodo();
        }

        private void ConfigurarInterfaz()
        {
            btnVolver = new Guna2Button();
            btnVolver.Text = "← Volver al Menú";
            btnVolver.Size = new Size(150, 40);
            btnVolver.Location = new Point(30, 30);
            btnVolver.FillColor = Color.Transparent;
            btnVolver.ForeColor = Color.FromArgb(91, 59, 115);
            btnVolver.Font = new Font("Segoe UI Semibold", 11);
            btnVolver.Cursor = Cursors.Hand;
            btnVolver.Click += (s, e) => {
                DashboardForm dash = new DashboardForm();
                dash.Show();
                this.Close();
            };
            this.Controls.Add(btnVolver);

            // 2. TÍTULOS CENTRADOS
            lblTitulo = new Guna2HtmlLabel();
            lblTitulo.Text = "<span style='font-family:Segoe UI Semibold; font-size:24pt; color:#5B3B73;'>Cherries Blood</span>";
            this.Controls.Add(lblTitulo);

            lblSubtitulo = new Guna2HtmlLabel();
            lblSubtitulo.Text = "<span style='font-family:Segoe UI; font-size:12pt; color:#7A5A95;'>Gestión de Productos</span>";
            this.Controls.Add(lblSubtitulo);

            // 3. PANEL IZQUIERDO (Añadir / Editar Producto)
            panelRegistro = new Guna2Panel();
            panelRegistro.Size = new Size(350, 520);
            panelRegistro.FillColor = Color.White;
            panelRegistro.BorderRadius = 15;
            this.Controls.Add(panelRegistro);

            CrearLabel(panelRegistro, "Añadir Nuevo Producto", 20, 25, true);

            // INPUT: NOMBRE
            CrearLabel(panelRegistro, "Nombre del Producto", 20, 75, false);
            txtNombre = CrearInput(panelRegistro, "Escribe el nombre del producto...", 90);

            // INPUT: PRECIO DE VENTA
            CrearLabel(panelRegistro, "Precio de Venta", 20, 155, false);
            txtPrecioVenta = CrearInput(panelRegistro, "Escribe el precio de venta...", 160);
            txtPrecioVenta.KeyPress += ValidarDecimales;

            // INPUT: COSTO ESTIMADO
            CrearLabel(panelRegistro, "Costo Estimado", 20, 235, false);
            txtCostoEstimado = CrearInput(panelRegistro, "Escribe el costo estimado...", 230);
            txtCostoEstimado.KeyPress += ValidarDecimales;

            // BOTÓN: ACCIÓN (Agregar / Guardar)
            btnAgregar = new Guna2Button();
            btnAgregar.Text = "+ Agregar";
            btnAgregar.Size = new Size(310, 45);
            btnAgregar.Location = new Point(20, 440);
            btnAgregar.BorderRadius = 15;
            btnAgregar.FillColor = Color.FromArgb(91, 59, 115); // Morado original
            btnAgregar.Font = new Font("Segoe UI Semibold", 12);
            btnAgregar.Cursor = Cursors.Hand;
            btnAgregar.Click += BtnAgregar_Click;
            panelRegistro.Controls.Add(btnAgregar);

            // 4. PANEL DERECHO (Lista de Productos)
            panelLista = new Guna2Panel();
            panelLista.Size = new Size(800, 520);
            panelLista.FillColor = Color.White;
            panelLista.BorderRadius = 15;
            this.Controls.Add(panelLista);

            CrearLabel(panelLista, "Lista de Productos", 25, 25, true);

            dgProductos = new Guna2DataGridView();
            dgProductos.AllowUserToAddRows = false;
            dgProductos.Size = new Size(750, 400);
            dgProductos.Location = new Point(25, 80);
            dgProductos.BackgroundColor = Color.White;
            dgProductos.BorderStyle = BorderStyle.None;
            dgProductos.ColumnHeadersHeight = 40;

            // Estilo Figma
            dgProductos.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgProductos.ThemeStyle.HeaderStyle.BackColor = Color.White;
            dgProductos.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgProductos.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            dgProductos.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(242, 242, 242);
            dgProductos.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgProductos.RowHeadersVisible = false;

            // Columnas de datos
            dgProductos.Columns.Add("id", "ID");
            dgProductos.Columns.Add("nombre", "Nombre del Producto");
            dgProductos.Columns.Add("precioVenta", "Precio Venta");
            dgProductos.Columns.Add("costoEstimado", "Costo Estimado");
            dgProductos.Columns.Add("margen", "Margen de Ganancia");

            dgProductos.Columns["id"].Visible = false;
            dgProductos.Columns["nombre"].Width = 200;

            // Botón Editar
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
            colEditar.Name = "editar";
            colEditar.HeaderText = "Acciones";
            colEditar.Text = "✏️";
            colEditar.UseColumnTextForButtonValue = true;
            colEditar.Width = 60;
            colEditar.FlatStyle = FlatStyle.Flat;
            colEditar.DefaultCellStyle.BackColor = Color.FromArgb(91, 59, 115);
            colEditar.DefaultCellStyle.ForeColor = Color.White;
            dgProductos.Columns.Add(colEditar);

            // Botón Eliminar
            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.Name = "eliminar";
            colEliminar.HeaderText = "";
            colEliminar.Text = "🗑️";
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.Width = 60;
            colEliminar.FlatStyle = FlatStyle.Flat;
            colEliminar.DefaultCellStyle.BackColor = Color.FromArgb(227, 110, 110);
            colEliminar.DefaultCellStyle.ForeColor = Color.White;
            dgProductos.Columns.Add(colEliminar);

            dgProductos.CellContentClick += DgProductos_CellContentClick;
            panelLista.Controls.Add(dgProductos);

            dgProductos.CellFormatting += (s, e) => {
                if (e.RowIndex < 0) return;

                if (dgProductos.Columns[e.ColumnIndex].Name == "editar")
                {
                    e.CellStyle.BackColor = Color.FromArgb(91, 59, 115);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(91, 59, 115);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                else if (dgProductos.Columns[e.ColumnIndex].Name == "eliminar")
                {
                    e.CellStyle.BackColor = Color.FromArgb(227, 110, 110);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(227, 110, 110);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            };

            CentrarTodo();
            CargarProductos();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string precioStr = txtPrecioVenta.Text.Trim();
            string costoStr = txtCostoEstimado.Text.Trim();

            // Validación de campos vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(precioStr) || string.IsNullOrEmpty(costoStr))
            {
                MessageBox.Show("Todos los campos (Nombre, Precio de Venta y Costo Estimado) son obligatorios.",
                                "Campos Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sanitizado preventivo de caracteres de formato regional por si se edita directo del DGV
            precioStr = precioStr.Replace("$", "").Replace(",", "").Trim();
            costoStr = costoStr.Replace("$", "").Replace(",", "").Trim();

            if (!decimal.TryParse(precioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioVenta) ||
                !decimal.TryParse(costoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal costoEstimated))
            {
                // Intento fallback con la cultura actual si falla el invariante
                if (!decimal.TryParse(precioStr, out precioVenta) || !decimal.TryParse(costoStr, out costoEstimated))
                {
                    MessageBox.Show("Por favor, ingresa valores numéricos válidos para los precios y costos.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (idProductoSeleccionado != null)
            {
                // --- MODO EDICIÓN (UPDATE) ---
                string queryUpdate = "UPDATE Productos SET Nombre = @nombre, PrecioVenta = @precioVenta, CostoEstimado = @costoEstimado WHERE Id = @id";

                try
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                    {
                        using (SqlCommand comando = new SqlCommand(queryUpdate, conexion))
                        {
                            comando.Parameters.AddWithValue("@id", idProductoSeleccionado);
                            comando.Parameters.AddWithValue("@nombre", nombre);
                            comando.Parameters.AddWithValue("@precioVenta", precioVenta);
                            comando.Parameters.AddWithValue("@costoEstimado", costoEstimated);

                            conexion.Open();
                            comando.ExecuteNonQuery();

                            MessageBox.Show("¡Producto actualizado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Restaurar componentes a su estado original de inserción
                            idProductoSeleccionado = null;
                            btnAgregar.Text = "+ Agregar";
                            btnAgregar.FillColor = Color.FromArgb(91, 59, 115);

                            LimpiarCampos();
                            CargarProductos();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el producto: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // --- MODO REGISTRO (INSERT) ---
                string queryInsert = "INSERT INTO Productos (Nombre, PrecioVenta, CostoEstimado) VALUES (@nombre, @precioVenta, @costoEstimado)";

                try
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                    {
                        using (SqlCommand comando = new SqlCommand(queryInsert, conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", nombre);
                            comando.Parameters.AddWithValue("@precioVenta", precioVenta);
                            comando.Parameters.AddWithValue("@costoEstimado", costoEstimated);

                            conexion.Open();
                            comando.ExecuteNonQuery();

                            MessageBox.Show("¡Producto registrado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LimpiarCampos();
                            CargarProductos();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar el producto: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarProductos()
        {
            dgProductos.Rows.Clear();
            string querySelect = "SELECT Id, Nombre, PrecioVenta, CostoEstimado, MargenGanancia FROM Productos";

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    using (SqlCommand comando = new SqlCommand(querySelect, conexion))
                    {
                        conexion.Open();
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                decimal precioVenta = Convert.ToDecimal(lector["PrecioVenta"]);
                                decimal costoEst = Convert.ToDecimal(lector["CostoEstimado"]);
                                decimal margen = Convert.ToDecimal(lector["MargenGanancia"]);

                                string pVentaFormateado = precioVenta.ToString("C2", CultureInfo.CreateSpecificCulture("es-MX"));
                                string costoFormateado = costoEst.ToString("C2", CultureInfo.CreateSpecificCulture("es-MX"));
                                string margenFormateado = margen.ToString("F1") + "%";

                                dgProductos.Rows.Add(
                                    lector["Id"].ToString(),
                                    lector["Nombre"].ToString(),
                                    pVentaFormateado,
                                    costoFormateado,
                                    margenFormateado
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de productos: " + ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string idProducto = dgProductos.Rows[e.RowIndex].Cells["id"].Value.ToString();
            string nombreProd = dgProductos.Rows[e.RowIndex].Cells["nombre"].Value.ToString();

            // ACCIÓN: ELIMINAR
            if (dgProductos.Columns[e.ColumnIndex].Name == "eliminar")
            {
                // Si intenta eliminar el producto que actualmente se está editando, cancelamos el flujo
                if (idProductoSeleccionado == idProducto)
                {
                    MessageBox.Show("No puedes eliminar un producto mientras se encuentra en modo edición.", "Operación Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirmacion = MessageBox.Show(
                    $"¿Deseas eliminar permanentemente '{nombreProd}' de la lista?",
                    "Confirmar Baja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.Yes)
                {
                    string queryDelete = "DELETE FROM Productos WHERE Id = @id";
                    try
                    {
                        using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                        {
                            using (SqlCommand comando = new SqlCommand(queryDelete, conexion))
                            {
                                comando.Parameters.AddWithValue("@id", idProducto);
                                conexion.Open();
                                comando.ExecuteNonQuery();

                                MessageBox.Show("Producto eliminado exitosamente.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CargarProductos();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            // ACCIÓN: MODIFICAR
            if (dgProductos.Columns[e.ColumnIndex].Name == "editar")
            {
                // Guardamos el ID en la variable global para cambiar al estado "Edición"
                idProductoSeleccionado = idProducto;

                txtNombre.Text = nombreProd;

                // Limpieza absoluta de símbolos del string formateado del DataGridView para parseo plano
                string precioLimpio = dgProductos.Rows[e.RowIndex].Cells["precioVenta"].Value.ToString().Replace("$", "").Replace(",", "").Replace(" ", "").Trim();
                string costoLimpio = dgProductos.Rows[e.RowIndex].Cells["costoEstimado"].Value.ToString().Replace("$", "").Replace(",", "").Replace(" ", "").Trim();

                txtPrecioVenta.Text = precioLimpio;
                txtCostoEstimado.Text = costoLimpio;

                // Cambiar estética del botón a modo confirmación de edición (Azul Figma de Acción)
                btnAgregar.Text = "✔ Guardar Cambios";
                btnAgregar.FillColor = Color.FromArgb(91, 59, 115);

                MessageBox.Show("Los datos del producto se cargaron en el panel izquierdo.\nEfectúa los cambios y haz clic en '✔ Guardar Cambios'.",
                                "Modo Edición Activado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtPrecioVenta.Clear();
            txtCostoEstimado.Clear();
        }

        private void ValidarDecimales(object sender, KeyPressEventArgs e)
        {
            Guna2TextBox textbox = (Guna2TextBox)sender;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.') e.Handled = true;
            if (e.KeyChar == '.' && textbox.Text.Contains(".")) e.Handled = true;
        }

        private void CrearLabel(Guna2Panel p, string texto, int x, int y, bool esTitulo)
        {
            Guna2HtmlLabel lbl = new Guna2HtmlLabel();
            lbl.Text = esTitulo ? $"<span style='font-family:Segoe UI Semibold; font-size:14pt; color:#242424;'>{texto}</span>"
                                : $"<span style='font-family:Segoe UI Semibold; font-size:10pt; color:#242424;'>{texto}</span>";
            lbl.Location = new Point(x, y);
            p.Controls.Add(lbl);
        }

        private Guna2TextBox CrearInput(Guna2Panel p, string placeholder, int y)
        {
            Guna2TextBox txt = new Guna2TextBox();
            txt.Size = new Size(310, 36);
            txt.Location = new Point(20, y);
            txt.BorderRadius = 8;
            txt.PlaceholderText = placeholder;
            txt.FillColor = Color.FromArgb(242, 242, 242);
            txt.BorderThickness = 0;
            txt.Font = new Font("Segoe UI", 10);
            p.Controls.Add(txt);
            return txt;
        }

        private void CentrarTodo()
        {
            int gap = 30;
            int totalWidth = panelRegistro.Width + panelLista.Width + gap;

            lblTitulo.Location = new Point((this.ClientSize.Width - lblTitulo.Width) / 2, 40);
            lblSubtitulo.Location = new Point((this.ClientSize.Width - lblSubtitulo.Width) / 2, 85);

            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int startY = 140;

            panelRegistro.Location = new Point(startX, startY);
            panelLista.Location = new Point(startX + panelRegistro.Width + gap, startY);
        }
    }
}