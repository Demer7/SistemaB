using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace SistemaB
{
    public partial class VentasForm : Form
    {
        // Cadena de conexión corregida y validada para tu SQLEXPRESS
        string cadenaConexion = "Server=.\\SQLEXPRESS;Database=GestifyDB;Trusted_Connection=True;";

        // Tabla en memoria para el carrito de compras
        private DataTable dtCarrito;

        #region Declaración de Componentes de Interfaz (Guna.UI2)
        // Cabecera y Navegación
        private Guna2Button btnVolver;
        private Guna2HtmlLabel lblTitulo;
        private Guna2HtmlLabel lblSubtitulo;

        // Paneles y Controles principales
        private Guna2Panel panelControles;
        private Guna2Panel panelTotales;

        private Label lblLabelCliente;
        private Guna2ComboBox cmbCliente;

        private Label lblLabelFechaVenta;
        private Guna2DateTimePicker dtpFechaVenta;

        private Label lblLabelFechaEntrega;
        private Guna2DateTimePicker dtpFechaEntrega;

        private Label lblLabelProducto;
        private Guna2ComboBox cmbProducto;

        private Label lblLabelCantidad;
        private Guna2TextBox txtCantidad;
        private Guna2Button btnAgregar;

        private Guna2DataGridView dgvDetalles;

        private Label lblTextoTotal;
        private Label lblTotal;

        private Label lblTextoGanancia;
        private Label lblGananciaEstimada;

        private Guna2Button btnCompletarVenta;
        #endregion

        public VentasForm()
        {
            // CONFIGURACIÓN DE PANTALLA COMPLETA
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(242, 242, 242); // Gris claro unificado
            this.Text = "Gestify - Módulo de Ventas";

            // 1. Construir la interfaz gráficamente desde código
            ConstruirInterfazGrafica();

            // 2. Inicializar la lógica y datos
            ConfigurarCarrito();
            CargarClientes();
            CargarProductos();

            // 3. Responsividad centrada dinámica
            this.Resize += (s, e) => CentrarTodo();
            CentrarTodo(); // Ejecución inicial de centrado
        }

        private void ConstruirInterfazGrafica()
        {
            // Colores de la paleta minimalista (Neutralizada)
            Color colorPrincipal = Color.FromArgb(35, 35, 35); // Gris carbón en reemplazo del morado
            Color verdeGanancia = Color.FromArgb(78, 159, 109);   // Se mantiene para el indicador de dinero
            Color textoOscuro = Color.FromArgb(36, 36, 36);

            // ---- BOTÓN VOLVER (Top Izquierda) ----
            btnVolver = new Guna2Button();
            btnVolver.Text = "← Volver al Menú";
            btnVolver.Size = new Size(150, 40);
            btnVolver.Location = new Point(30, 30);
            btnVolver.FillColor = Color.Transparent;
            btnVolver.ForeColor = Color.FromArgb(64, 64, 64); // Gris oscuro neutro
            btnVolver.Font = new Font("Segoe UI Semibold", 11);
            btnVolver.Cursor = Cursors.Hand;
            btnVolver.Click += (s, e) => {
                DashboardForm dash = new DashboardForm();
                dash.Show();
                this.Close();
            };
            this.Controls.Add(btnVolver);

            // ---- TÍTULOS EN HTML (Se centran dinámicamente en CentrarTodo) ----
            lblTitulo = new Guna2HtmlLabel();
            lblTitulo.Text = "<span style='font-family:Segoe UI Semibold; font-size:24pt; color:#333333;'>Gestify</span>";
            this.Controls.Add(lblTitulo);

            lblSubtitulo = new Guna2HtmlLabel();
            lblSubtitulo.Text = "<span style='font-family:Segoe UI; font-size:12pt; color:#555555;'>Gestión de Ventas y Pedidos</span>";
            this.Controls.Add(lblSubtitulo);

            // ---- PANEL IZQUIERDO: CONTROLES DE ENTRADA ----
            panelControles = new Guna2Panel { Size = new Size(340, 540), FillColor = Color.White, BorderRadius = 15 };
            this.Controls.Add(panelControles);

            lblLabelCliente = new Label { Text = "Seleccionar Cliente", Location = new Point(20, 15), Size = new Size(200, 18), Font = new Font("Segoe UI Semibold", 10), ForeColor = textoOscuro, BackColor = Color.White };
            cmbCliente = new Guna2ComboBox { Location = new Point(20, 38), Size = new Size(300, 36), BorderRadius = 8, FillColor = Color.FromArgb(242, 242, 242), BorderThickness = 0 };

            lblLabelFechaVenta = new Label { Text = "Fecha de Venta", Location = new Point(20, 95), Size = new Size(200, 18), Font = new Font("Segoe UI Semibold", 10), ForeColor = textoOscuro, BackColor = Color.White };
            dtpFechaVenta = new Guna2DateTimePicker { Location = new Point(20, 118), Size = new Size(300, 36), BorderRadius = 8, FillColor = Color.FromArgb(242, 242, 242), BorderThickness = 0, Format = DateTimePickerFormat.Short, Enabled = false };

            lblLabelFechaEntrega = new Label { Text = "Fecha de Entrega (Pedido)", Location = new Point(20, 175), Size = new Size(200, 18), Font = new Font("Segoe UI Semibold", 10), ForeColor = textoOscuro, BackColor = Color.White };
            dtpFechaEntrega = new Guna2DateTimePicker { Location = new Point(20, 198), Size = new Size(300, 36), BorderRadius = 8, FillColor = Color.FromArgb(242, 242, 242), BorderThickness = 0, Format = DateTimePickerFormat.Short };

            lblLabelProducto = new Label { Text = "Seleccionar Producto", Location = new Point(20, 275), Size = new Size(200, 18), Font = new Font("Segoe UI Semibold", 10), ForeColor = textoOscuro, BackColor = Color.White };
            cmbProducto = new Guna2ComboBox { Location = new Point(20, 298), Size = new Size(300, 36), BorderRadius = 8, FillColor = Color.FromArgb(242, 242, 242), BorderThickness = 0 };

            lblLabelCantidad = new Label { Text = "Cant.", Location = new Point(20, 355), Size = new Size(70, 18), Font = new Font("Segoe UI Semibold", 10), ForeColor = textoOscuro, BackColor = Color.White };
            txtCantidad = new Guna2TextBox { Location = new Point(20, 378), Size = new Size(110, 36), BorderRadius = 8, FillColor = Color.FromArgb(242, 242, 242), BorderThickness = 0, PlaceholderText = "0" };

            btnAgregar = new Guna2Button { Text = "+ Agregar", Location = new Point(145, 378), Size = new Size(175, 36), BorderRadius = 8, FillColor = colorPrincipal, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 11), Cursor = Cursors.Hand };
            btnAgregar.Click += new EventHandler(btnAgregar_Click);

            panelControles.Controls.AddRange(new Control[] { lblLabelCliente, cmbCliente, lblLabelFechaVenta, dtpFechaVenta, lblLabelFechaEntrega, dtpFechaEntrega, lblLabelProducto, cmbProducto, lblLabelCantidad, txtCantidad, btnAgregar });

            // ---- SECCIÓN DERECHA: DATAGRIDVIEW ----
            dgvDetalles = new Guna2DataGridView
            {
                Size = new Size(660, 380),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            // Estilo neutro de tabla homologado con otras pantallas
            dgvDetalles.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(242, 242, 242);
            dgvDetalles.ThemeStyle.HeaderStyle.ForeColor = Color.DimGray;
            dgvDetalles.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            dgvDetalles.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10);
            dgvDetalles.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvDetalles.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            this.Controls.Add(dgvDetalles);

            // ---- TARJETA INFERIOR DERECHA: TOTALES Y CONFIRMACIÓN ----
            panelTotales = new Guna2Panel { Size = new Size(660, 140), FillColor = Color.White, BorderRadius = 15 };
            this.Controls.Add(panelTotales);

            lblTextoTotal = new Label { Text = "TOTAL VENTA", Location = new Point(25, 25), Size = new Size(120, 20), Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold), ForeColor = Color.Gray, BackColor = Color.White };
            lblTotal = new Label { Text = "$0.00", Location = new Point(25, 55), Size = new Size(180, 45), Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = textoOscuro, BackColor = Color.White };

            lblTextoGanancia = new Label { Text = "GANANCIA ESTIMADA", Location = new Point(230, 25), Size = new Size(150, 20), Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold), ForeColor = Color.Gray, BackColor = Color.White };
            lblGananciaEstimada = new Label { Text = "$0.00", Location = new Point(230, 55), Size = new Size(180, 45), Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = verdeGanancia, BackColor = Color.White };

            btnCompletarVenta = new Guna2Button { Text = "Completar Venta", Location = new Point(440, 45), Size = new Size(190, 50), BorderRadius = 10, FillColor = colorPrincipal, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCompletarVenta.Click += new EventHandler(btnCompletarVenta_Click);

            panelTotales.Controls.AddRange(new Control[] { lblTextoTotal, lblTotal, lblTextoGanancia, lblGananciaEstimada, btnCompletarVenta });
        }

        // ---- RESPONSIVIDAD Y CENTRADO DINÁMICO ----
        private void CentrarTodo()
        {
            if (lblTitulo == null || panelControles == null || dgvDetalles == null || panelTotales == null) return;

            lblTitulo.Location = new Point((this.ClientSize.Width - lblTitulo.Width) / 2, 40);
            lblSubtitulo.Location = new Point((this.ClientSize.Width - lblSubtitulo.Width) / 2, 85);

            int gap = 30;
            int totalWidth = panelControles.Width + dgvDetalles.Width + gap;

            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int startY = 140;

            panelControles.Location = new Point(startX, startY);
            dgvDetalles.Location = new Point(startX + panelControles.Width + gap, startY);
            panelTotales.Location = new Point(startX + panelControles.Width + gap, startY + dgvDetalles.Height + 20);
        }

        private void ConfigurarCarrito()
        {
            dtCarrito = new DataTable();
            dtCarrito.Columns.Add("IdProducto", typeof(int));
            dtCarrito.Columns.Add("Producto", typeof(string));
            dtCarrito.Columns.Add("Cantidad", typeof(int));
            dtCarrito.Columns.Add("Precio Unitario", typeof(decimal));
            dtCarrito.Columns.Add("Subtotal", typeof(decimal));
            dtCarrito.Columns.Add("Ganancia", typeof(decimal));

            dgvDetalles.DataSource = dtCarrito;

            if (dgvDetalles.Columns["IdProducto"] != null) dgvDetalles.Columns["IdProducto"].Visible = false;
            if (dgvDetalles.Columns["Ganancia"] != null) dgvDetalles.Columns["Ganancia"].Visible = false;
        }

        private void CargarClientes()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cadenaConexion))
                {
                    string query = "SELECT Id, Nombre FROM Clientes";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbCliente.DataSource = dt;
                    cmbCliente.DisplayMember = "Nombre";
                    cmbCliente.ValueMember = "Id";
                    cmbCliente.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductos()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cadenaConexion))
                {
                    string query = "SELECT Id, Nombre, PrecioVenta, CostoEstimado FROM Productos";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbProducto.DataSource = dt;
                    cmbProducto.DisplayMember = "Nombre";
                    cmbProducto.ValueMember = "Id";
                    cmbProducto.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedIndex == -1 || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Por favor, selecciona un producto e introduce una cantidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView producto = (DataRowView)cmbProducto.SelectedItem;
            int idProducto = Convert.ToInt32(producto["Id"]);
            string nombreProducto = producto["Nombre"].ToString();
            decimal precioVenta = Convert.ToDecimal(producto["PrecioVenta"]);
            decimal costoEstimado = Convert.ToDecimal(producto["CostoEstimado"]);

            decimal subtotal = precioVenta * cantidad;
            decimal gananciaUnitaria = precioVenta - costoEstimado;
            decimal gananciaTotalRenglon = gananciaUnitaria * cantidad;

            bool existe = false;
            foreach (DataRow fila in dtCarrito.Rows)
            {
                if (Convert.ToInt32(fila["IdProducto"]) == idProducto)
                {
                    int nuevaCantidad = Convert.ToInt32(fila["Cantidad"]) + cantidad;
                    fila["Cantidad"] = nuevaCantidad;
                    fila["Subtotal"] = nuevaCantidad * precioVenta;
                    fila["Ganancia"] = nuevaCantidad * gananciaUnitaria;
                    existe = true;
                    break;
                }
            }

            if (!existe)
            {
                dtCarrito.Rows.Add(idProducto, nombreProducto, cantidad, precioVenta, subtotal, gananciaTotalRenglon);
            }

            CalcularTotales();

            cmbProducto.SelectedIndex = -1;
            txtCantidad.Clear();
        }

        private void CalcularTotales()
        {
            decimal totalGeneral = 0;
            decimal gananciaGeneral = 0;

            foreach (DataRow fila in dtCarrito.Rows)
            {
                totalGeneral += Convert.ToDecimal(fila["Subtotal"]);
                gananciaGeneral += Convert.ToDecimal(fila["Ganancia"]);
            }

            lblTotal.Text = $"${totalGeneral:N2}";
            lblGananciaEstimada.Text = $"${gananciaGeneral:N2}";
        }

        private void btnCompletarVenta_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un cliente antes de procesar la venta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtCarrito.Rows.Count == 0)
            {
                MessageBox.Show("El carrito de compras está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idCliente = Convert.ToInt32(cmbCliente.SelectedValue);
            DateTime fechaVenta = dtpFechaVenta.Value;
            DateTime fechaEntrega = dtpFechaEntrega.Value;

            decimal total = 0;
            decimal gananciaTotal = 0;
            foreach (DataRow r in dtCarrito.Rows)
            {
                total += Convert.ToDecimal(r["Subtotal"]);
                gananciaTotal += Convert.ToDecimal(r["Ganancia"]);
            }

            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                SqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string queryVenta = @"INSERT INTO Ventas (IdCliente, FechaVenta, FechaEntrega, Total, GananciaEstimada) 
                                          VALUES (@IdCliente, @FechaVenta, @FechaEntrega, @Total, @GananciaEstimada);
                                          SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaccion);
                    cmdVenta.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmdVenta.Parameters.AddWithValue("@FechaVenta", fechaVenta);
                    cmdVenta.Parameters.AddWithValue("@FechaEntrega", fechaEntrega);
                    cmdVenta.Parameters.AddWithValue("@Total", total);
                    cmdVenta.Parameters.AddWithValue("@GananciaEstimada", gananciaTotal);

                    int idVentaGenerada = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    string queryDetalle = @"INSERT INTO DetallesVentas (IdVenta, IdProducto, Cantidad, PrecioUnitario, Subtotal, GananciaArticulo) 
                                            VALUES (@IdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal, @GananciaArticulo);";

                    foreach (DataRow fila in dtCarrito.Rows)
                    {
                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@IdVenta", idVentaGenerada);
                        cmdDetalle.Parameters.AddWithValue("@IdProducto", fila["IdProducto"]);
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", fila["Cantidad"]);
                        cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", fila["Precio Unitario"]);
                        cmdDetalle.Parameters.AddWithValue("@Subtotal", fila["Subtotal"]);
                        cmdDetalle.Parameters.AddWithValue("@GananciaArticulo", fila["Ganancia"]);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    MessageBox.Show("¡Pedido y venta registrados con éxito con su fecha de entrega!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    MessageBox.Show($"Error crítico en la transacción: {ex.Message}", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LimpiarFormulario()
        {
            cmbCliente.SelectedIndex = -1;
            cmbProducto.SelectedIndex = -1;
            txtCantidad.Clear();
            dtCarrito.Rows.Clear();
            lblTotal.Text = "$0.00";
            lblGananciaEstimada.Text = "$0.00";
            dtpFechaEntrega.Value = DateTime.Now;
        }
    }
}