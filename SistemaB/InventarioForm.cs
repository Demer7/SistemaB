using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace SistemaB
{
    public partial class InventarioForm : Form
    {
        // Componentes de la Izquierda (Registro)
        private Guna2Panel panelRegistro;
        private Guna2TextBox txtNombre, txtCantidad, txtPrecioCompra;
        private Guna2Button btnAgregar;

        // Componentes de la Derecha (Lista)
        private Guna2Panel panelLista;
        private Guna2DataGridView dgInventario;

        // Cabecera y Navegación
        private Guna2Button btnVolver;
        private Guna2HtmlLabel lblTitulo, lblSubtitulo;

        private string cadenaConexion = "Server=.\\SQLEXPRESS;Database=CherriesBloodDB;Trusted_Connection=True;";
        private int idEditando = -1; // -1 = Nuevo registro, cualquier otro número = Editando ID

        public InventarioForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(242, 242, 242);

            ConfigurarInterfaz();

            this.Resize += (s, e) => CentrarTodo();
        }

        private void ConfigurarInterfaz()
        {
            //BOTÓN VOLVER
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

            //  TÍTULOS
            lblTitulo = new Guna2HtmlLabel();
            lblTitulo.Text = "<span style='font-family:Segoe UI Semibold; font-size:24pt; color:#5B3B73;'>Cherries Blood</span>";
            this.Controls.Add(lblTitulo);

            lblSubtitulo = new Guna2HtmlLabel();
            lblSubtitulo.Text = "<span style='font-family:Segoe UI; font-size:12pt; color:#7A5A95;'>Gestión de Inventarios</span>";
            this.Controls.Add(lblSubtitulo);

            //  PANEL IZQUIERDO (Añadir Artículo)
            panelRegistro = new Guna2Panel();
            panelRegistro.Size = new Size(350, 500);
            panelRegistro.FillColor = Color.White;
            panelRegistro.BorderRadius = 20;
            this.Controls.Add(panelRegistro);

            CrearLabel(panelRegistro, "Añadir Nuevo Artículo", 20, 25, true);

            // SECCIÓN: NOMBRE
            Guna2HtmlLabel lblNombre = new Guna2HtmlLabel();
            lblNombre.Text = "<span style='font-family:Segoe UI Semibold; font-size:10pt; color:#555;'>Nombre</span>";
            lblNombre.Location = new Point(20, 75);
            panelRegistro.Controls.Add(lblNombre);

            txtNombre = CrearInput(panelRegistro, "Escribe el nombre del artículo...", 90);

            // SECCIÓN: CANTIDAD
            Guna2HtmlLabel lblCantidad = new Guna2HtmlLabel();
            lblCantidad.Text = "<span style='font-family:Segoe UI Semibold; font-size:10pt; color:#555;'>Cantidad</span>";
            lblCantidad.Location = new Point(20, 155);
            panelRegistro.Controls.Add(lblCantidad);

            txtCantidad = CrearInput(panelRegistro, "Ej. 50", 160);
            txtCantidad.KeyPress += (s, e) => {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;
            };

            // SECCIÓN: PRECIO
            Guna2HtmlLabel lblPrecio = new Guna2HtmlLabel();
            lblPrecio.Text = "<span style='font-family:Segoe UI Semibold; font-size:10pt; color:#555;'>Precio de Compra ($ MXN)</span>";
            lblPrecio.Location = new Point(20, 235);
            panelRegistro.Controls.Add(lblPrecio);

            txtPrecioCompra = CrearInput(panelRegistro, "Ej. 18.50", 230);
            txtPrecioCompra.KeyPress += (s, e) => {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.') e.Handled = true;
                if (e.KeyChar == '.' && txtPrecioCompra.Text.Contains(".")) e.Handled = true;
            };

            // BOTÓN: AGREGAR / GUARDAR
            btnAgregar = new Guna2Button();
            btnAgregar.Text = "+ Agregar";
            btnAgregar.Size = new Size(310, 45);
            btnAgregar.Location = new Point(20, 420);
            btnAgregar.BorderRadius = 15;
            btnAgregar.FillColor = Color.FromArgb(91, 59, 115);
            btnAgregar.Font = new Font("Segoe UI Semibold", 12);
            btnAgregar.ShadowDecoration.Enabled = false;
            btnAgregar.Click += BtnAgregar_Click;
            panelRegistro.Controls.Add(btnAgregar);

            // PANEL DERECHO (Lista de Inventario)
            panelLista = new Guna2Panel();
            panelLista.Size = new Size(750, 500);
            panelLista.FillColor = Color.White;
            panelLista.BorderRadius = 20;
            this.Controls.Add(panelLista);

            CrearLabel(panelLista, "Lista de Inventario", 25, 25, true);

            dgInventario = new Guna2DataGridView();
            dgInventario.AllowUserToAddRows = false;
            dgInventario.Size = new Size(700, 380);
            dgInventario.Location = new Point(25, 80);
            dgInventario.BackgroundColor = Color.White;
            dgInventario.BorderStyle = BorderStyle.None;
            dgInventario.ColumnHeadersHeight = 40;

            // Paleta de colores de la cabecera
            dgInventario.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(242, 242, 242);
            dgInventario.ThemeStyle.HeaderStyle.ForeColor = Color.DimGray;
            dgInventario.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10);
            dgInventario.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgInventario.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(238, 235, 241);
            dgInventario.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgInventario.RowHeadersVisible = false;

            // Configurar columnas de datos
            dgInventario.Columns.Add("id", "ID");
            dgInventario.Columns.Add("nombre", "Nombre");
            dgInventario.Columns.Add("cantidad", "Cantidad");
            dgInventario.Columns.Add("precio", "Precio de Compra");

            dgInventario.Columns["id"].Visible = false;

            // Ajustar anchos automáticos proporcionales
            dgInventario.Columns["nombre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgInventario.Columns["cantidad"].Width = 100;
            dgInventario.Columns["precio"].Width = 150;

            // Columnas de Acción
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
            colEditar.Name = "editar";
            colEditar.HeaderText = "Acciones";
            colEditar.Text = "✏️";
            colEditar.UseColumnTextForButtonValue = true;
            colEditar.Width = 80;
            colEditar.FlatStyle = FlatStyle.Flat;
            dgInventario.Columns.Add(colEditar);

            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.Name = "eliminar";
            colEliminar.HeaderText = "";
            colEliminar.Text = "🗑️";
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.Width = 80;
            colEliminar.FlatStyle = FlatStyle.Flat;
            dgInventario.Columns.Add(colEliminar);

            // Eventos de la Tabla
            dgInventario.CellContentClick += DgInventario_CellContentClick;

            dgInventario.CellFormatting += (s, e) => {
                if (e.RowIndex < 0) return;

                if (dgInventario.Columns[e.ColumnIndex].Name == "editar")
                {
                    e.CellStyle.BackColor = Color.FromArgb(91, 59, 115);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(111, 79, 135);
                    e.CellStyle.SelectionForeColor = Color.White;
                }

                else if (dgInventario.Columns[e.ColumnIndex].Name == "eliminar")
                {
                    e.CellStyle.BackColor = Color.FromArgb(227, 110, 110);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(242, 130, 130);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            };

            panelLista.Controls.Add(dgInventario);

            CentrarTodo();
            CargarInventario();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string cantidadStr = txtCantidad.Text.Trim();
            string precioStr = txtPrecioCompra.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(cantidadStr) || string.IsNullOrEmpty(precioStr))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Campos Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(cantidadStr, out int cantidad) || !decimal.TryParse(precioStr, out decimal precio))
            {
                MessageBox.Show("Por favor, ingresa valores numéricos válidos.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query;
            if (idEditando == -1)
            {
                query = "INSERT INTO Inventario (Nombre, Cantidad, PrecioCompra) VALUES (@nombre, @cantidad, @precio)";
            }
            else
            {
                query = "UPDATE Inventario SET Nombre = @nombre, Cantidad = @cantidad, PrecioCompra = @precio WHERE Id = @id";
            }

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@cantidad", cantidad);
                        comando.Parameters.AddWithValue("@precio", precio);

                        if (idEditando != -1)
                        {
                            comando.Parameters.AddWithValue("@id", idEditando);
                        }

                        conexion.Open();
                        comando.ExecuteNonQuery();

                        if (idEditando == -1)
                        {
                            MessageBox.Show("¡Artículo registrado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("¡Artículo actualizado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        txtNombre.Clear();
                        txtCantidad.Clear();
                        txtPrecioCompra.Clear();

                        idEditando = -1;
                        btnAgregar.Text = "+ Agregar";

                        CargarInventario();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar en la base de datos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarInventario()
        {
            dgInventario.Rows.Clear();
            string querySelect = "SELECT Id, Nombre, Cantidad, PrecioCompra FROM Inventario";

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
                                decimal precio = Convert.ToDecimal(lector["PrecioCompra"]);
                                string precioFormateado = precio.ToString("C2", CultureInfo.CreateSpecificCulture("es-MX"));

                                dgInventario.Rows.Add(
                                    lector["Id"].ToString(),
                                    lector["Nombre"].ToString(),
                                    lector["Cantidad"].ToString(),
                                    precioFormateado
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar el inventario: " + ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgInventario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgInventario.Rows[e.RowIndex];

            // SI PRESIONA EDITAR (Carga los datos al panel izquierdo)
            if (dgInventario.Columns[e.ColumnIndex].Name == "editar")
            {
                idEditando = Convert.ToInt32(fila.Cells["id"].Value);

                txtNombre.Text = fila.Cells["nombre"].Value.ToString();
                txtCantidad.Text = fila.Cells["cantidad"].Value.ToString();

                // Limpia el símbolo de moneda para volver a cargarlo numéricamente en el campo
                string precioLimpio = fila.Cells["precio"].Value.ToString()
                                              .Replace("$", "").Replace(" ", "").Trim();
                txtPrecioCompra.Text = precioLimpio;

                btnAgregar.Text = "✔ Guardar Cambios";
            }
            // SI PRESIONA ELIMINAR
            else if (dgInventario.Columns[e.ColumnIndex].Name == "eliminar")
            {
                int idEliminar = Convert.ToInt32(fila.Cells["id"].Value);
                string nombreArticulo = fila.Cells["nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show($"¿Estás seguro de eliminar '{nombreArticulo}' del inventario?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (resultado == DialogResult.Yes)
                {
                    string queryDelete = "DELETE FROM Inventario WHERE Id = @id";
                    try
                    {
                        using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                        {
                            using (SqlCommand comando = new SqlCommand(queryDelete, conexion))
                            {
                                comando.Parameters.AddWithValue("@id", idEliminar);
                                conexion.Open();
                                comando.ExecuteNonQuery();
                                MessageBox.Show("Artículo eliminado perfectamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CargarInventario();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CrearLabel(Guna2Panel p, string texto, int x, int y, bool esTitulo)
        {
            Guna2HtmlLabel lbl = new Guna2HtmlLabel();
            lbl.Text = esTitulo ? $"<span style='font-family:Segoe UI Semibold; font-size:14pt; color:#333;'>{texto}</span>"
                                : $"<span style='font-family:Segoe UI Semibold; font-size:10pt; color:#555;'>{texto}</span>";
            lbl.Location = new Point(x, y);
            p.Controls.Add(lbl);
        }

        private Guna2TextBox CrearInput(Guna2Panel p, string placeholder, int y)
        {
            Guna2TextBox txt = new Guna2TextBox();
            txt.Size = new Size(310, 36);
            txt.Location = new Point(20, y);
            txt.BorderRadius = 10;
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