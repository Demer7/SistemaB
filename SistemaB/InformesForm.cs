using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace SistemaB
{
    public partial class InformesForm : Form
    {
        string cadenaConexion = "Server=.\\SQLEXPRESS;Database=GestifyDB;Trusted_Connection=True;";

        // Componentes de Cabecera
        private Guna2Button btnVolver;
        private Guna2HtmlLabel lblTitulo;
        private Guna2HtmlLabel lblSubtitulo;

        // Panel contenedor central
        private Guna2Panel mainPanel;

        // Métricas de Analítica
        private Label lblVentasValor;
        private Label lblGananciaValor;
        private Label lblProductosValor;
        private Label lblClientesValor;

        // Tabla de Datos
        private Guna2DataGridView dgvPedidos;

        // Paleta de colores minimalista neutra
        private Color colorOscuroBase = Color.FromArgb(35, 35, 35); // Gris carbón principal
        private Color colorTextoOscuro = Color.FromArgb(36, 36, 36);
        private Color colorAccionBaja = Color.FromArgb(100, 100, 100); // Gris medio para eliminar

        public InformesForm()
        {
            // Configuración de pantalla completa (Estilo unificado)
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(242, 242, 242);
            this.Text = "Gestify - Informes y Analítica";

            InitializeCustomComponents();
            CargarDatosDesdeBaseDatos();

            // Responsividad dinámica del formulario
            this.Resize += (s, e) => CentrarElementosPantalla();
            CentrarElementosPantalla();
        }

        private void InitializeCustomComponents()
        {
            // ---- 1. BOTÓN VOLVER (Fijo en la esquina superior izquierda) ----
            btnVolver = new Guna2Button();
            btnVolver.Text = "← Volver al Menú";
            btnVolver.Size = new Size(150, 40);
            btnVolver.Location = new Point(30, 30);
            btnVolver.FillColor = Color.Transparent;
            btnVolver.ForeColor = Color.FromArgb(64, 64, 64); // Gris oscuro
            btnVolver.Font = new Font("Segoe UI Semibold", 11);
            btnVolver.Cursor = Cursors.Hand;
            btnVolver.Click += (s, e) => {
                DashboardForm dash = new DashboardForm();
                dash.Show();
                this.Close();
            };
            this.Controls.Add(btnVolver);

            // ---- 2. TÍTULOS DE PANTALLA (Centrados dinámicamente) ----
            lblTitulo = new Guna2HtmlLabel();
            lblTitulo.Text = "<span style='font-family:Segoe UI Semibold; font-size:24pt; color:#333333;'>Gestify</span>";
            this.Controls.Add(lblTitulo);

            lblSubtitulo = new Guna2HtmlLabel();
            lblSubtitulo.Text = "<span style='font-family:Segoe UI; font-size:12pt; color:#555555;'>Gestión de Informes y Análisis</span>";
            this.Controls.Add(lblSubtitulo);

            // ---- 3. PANEL CONTENEDOR CENTRAL ----
            mainPanel = new Guna2Panel();
            mainPanel.Size = new Size(780, 560);
            mainPanel.BackColor = Color.Transparent;
            this.Controls.Add(mainPanel);

            // ---- 4. TARJETAS INDICADORAS ----
            // Fila 1
            CrearTarjeta("Ventas Totales", new Point(0, 0), true, out lblVentasValor);
            CrearTarjeta("Ganancia Total", new Point(400, 0), false, out lblGananciaValor);
            // Fila 2
            CrearTarjeta("Productos Vendidos", new Point(0, 140), true, out lblProductosValor);
            CrearTarjeta("Clientes Activos", new Point(400, 140), false, out lblClientesValor);

            // ---- 5. SECCIÓN INFERIOR: TABLA DE PEDIDOS ACTIVOS ----
            Label lblPedidos = new Label();
            lblPedidos.Text = "Pedidos Activos";
            lblPedidos.Font = new Font("Segoe UI Semibold", 14);
            lblPedidos.ForeColor = colorOscuroBase; // Homologado a gris carbón
            lblPedidos.Location = new Point(0, 300);
            lblPedidos.AutoSize = true;
            mainPanel.Controls.Add(lblPedidos);

            dgvPedidos = new Guna2DataGridView();
            dgvPedidos.Size = new Size(780, 220);
            dgvPedidos.Location = new Point(0, 330);
            dgvPedidos.BackgroundColor = Color.White;
            dgvPedidos.BorderStyle = BorderStyle.None;
            dgvPedidos.RowHeadersVisible = false;
            dgvPedidos.AllowUserToAddRows = false;
            dgvPedidos.ReadOnly = false;

            // Configuración base de columnas
            dgvPedidos.Columns.Add("idVenta", "ID Venta");
            dgvPedidos.Columns.Add("cliente", "Cliente");
            dgvPedidos.Columns.Add("fechaVenta", "Fecha Venta");
            dgvPedidos.Columns.Add("fechaEntrega", "Fecha Entrega");
            dgvPedidos.Columns.Add("total", "Total");

            dgvPedidos.Columns["idVenta"].Visible = false;
            dgvPedidos.Columns["cliente"].ReadOnly = true;
            dgvPedidos.Columns["fechaVenta"].ReadOnly = true;
            dgvPedidos.Columns["fechaEntrega"].ReadOnly = true;
            dgvPedidos.Columns["total"].ReadOnly = true;

            // COLUMNA BOTÓN ELIMINAR
            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.Name = "eliminar";
            colEliminar.HeaderText = "Acciones";
            colEliminar.Text = "🗑️";
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.Width = 70;
            colEliminar.FlatStyle = FlatStyle.Flat;
            colEliminar.DefaultCellStyle.BackColor = colorAccionBaja;
            colEliminar.DefaultCellStyle.ForeColor = Color.White;
            dgvPedidos.Columns.Add(colEliminar);

            dgvPedidos.CellContentClick += DgvPedidos_CellContentClick;

            dgvPedidos.CellFormatting += (s, e) => {
                if (e.RowIndex < 0) return;
                if (dgvPedidos.Columns[e.ColumnIndex].Name == "eliminar")
                {
                    e.CellStyle.BackColor = colorAccionBaja;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = colorAccionBaja;
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            };

            // Estilos visuales neutros del DataGridView
            dgvPedidos.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(242, 242, 242);
            dgvPedidos.ThemeStyle.HeaderStyle.ForeColor = Color.DimGray;
            dgvPedidos.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            dgvPedidos.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvPedidos.ThemeStyle.RowsStyle.ForeColor = colorTextoOscuro;
            dgvPedidos.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvPedidos.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvPedidos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPedidos.GridColor = Color.FromArgb(242, 242, 242);
            dgvPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            mainPanel.Controls.Add(dgvPedidos);
        }

        private void CrearTarjeta(string titulo, Point ubicacion, bool usarFondoOscuro, out Label lblValorFoco)
        {
            Guna2Panel tarjeta = new Guna2Panel();
            tarjeta.Size = new Size(380, 110);
            tarjeta.Location = ubicacion;
            tarjeta.BorderRadius = 15;
            tarjeta.FillColor = usarFondoOscuro ? colorOscuroBase : Color.White;

            Label lblTituloTarjeta = new Label();
            lblTituloTarjeta.Text = titulo;
            lblTituloTarjeta.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblTituloTarjeta.ForeColor = usarFondoOscuro ? Color.FromArgb(200, 255, 255, 255) : Color.Gray;
            lblTituloTarjeta.Location = new Point(20, 20);
            lblTituloTarjeta.AutoSize = true;
            lblTituloTarjeta.BackColor = Color.Transparent;

            lblValorFoco = new Label();
            lblValorFoco.Text = usarFondoOscuro ? "$0.00" : "0";
            lblValorFoco.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblValorFoco.ForeColor = usarFondoOscuro ? Color.White : colorTextoOscuro;
            lblValorFoco.Location = new Point(20, 45);
            lblValorFoco.AutoSize = true;
            lblValorFoco.BackColor = Color.Transparent;

            tarjeta.Controls.Add(lblTituloTarjeta);
            tarjeta.Controls.Add(lblValorFoco);
            mainPanel.Controls.Add(tarjeta);
        }

        // LÓGICA DE DETECCIÓN DE CLIC EN EL BOTÓN ELIMINAR PEDIDO
        private void DgvPedidos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvPedidos.Columns[e.ColumnIndex].Name == "eliminar")
            {
                string idVenta = dgvPedidos.Rows[e.RowIndex].Cells["idVenta"].Value.ToString();
                string cliente = dgvPedidos.Rows[e.RowIndex].Cells["cliente"].Value.ToString();
                string total = dgvPedidos.Rows[e.RowIndex].Cells["total"].Value.ToString();

                DialogResult confirmacion = MessageBox.Show(
                    $"¿Deseas cancelar y eliminar permanentemente el pedido de {cliente} por un total de {total}?\n\nEsta acción removerá el pedido de las estadísticas globales.",
                    "Confirmar Cancelación de Pedido", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.Yes)
                {
                    EliminarPedidoSistema(idVenta);
                }
            }
        }

        // LÓGICA DE PERSISTENCIA: ELIMINACIÓN CON TRANSACCIÓN SQL
        private void EliminarPedidoSistema(string idVenta)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    string queryDetalles = "DELETE FROM DetallesVentas WHERE IdVenta = @idVenta";
                    using (SqlCommand cmdDetalles = new SqlCommand(queryDetalles, conexion, transaccion))
                    {
                        cmdDetalles.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdDetalles.ExecuteNonQuery();
                    }

                    string queryVenta = "DELETE FROM Ventas WHERE IdVenta = @idVenta";
                    using (SqlCommand cmdVenta = new SqlCommand(queryVenta, conexion, transaccion))
                    {
                        cmdVenta.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdVenta.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    MessageBox.Show("El pedido ha sido removido y las métricas actualizadas con éxito.", "Pedido Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatosDesdeBaseDatos();
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    MessageBox.Show("Error crítico al intentar cancelar el pedido: " + ex.Message, "Error de Transacción", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CentrarElementosPantalla()
        {
            if (lblTitulo == null || mainPanel == null) return;

            lblTitulo.Location = new Point((this.ClientSize.Width - lblTitulo.Width) / 2, 40);
            lblSubtitulo.Location = new Point((this.ClientSize.Width - lblSubtitulo.Width) / 2, 85);

            int xCentral = (this.ClientSize.Width - mainPanel.Width) / 2;
            int yCentral = 140;

            mainPanel.Location = new Point(xCentral, yCentral);
        }

        public void CargarDatosDesdeBaseDatos()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string queryVentas = "SELECT ISNULL(SUM(Total), 0) FROM Ventas";
                    using (SqlCommand cmd = new SqlCommand(queryVentas, conexion))
                    {
                        lblVentasValor.Text = Convert.ToDecimal(cmd.ExecuteScalar()).ToString("C2");
                    }

                    string queryGanancias = "SELECT ISNULL(SUM(GananciaEstimada), 0) FROM Ventas";
                    using (SqlCommand cmd = new SqlCommand(queryGanancias, conexion))
                    {
                        lblGananciaValor.Text = Convert.ToDecimal(cmd.ExecuteScalar()).ToString("C2");
                    }

                    string queryProductos = "SELECT ISNULL(SUM(Cantidad), 0) FROM DetallesVentas";
                    using (SqlCommand cmd = new SqlCommand(queryProductos, conexion))
                    {
                        lblProductosValor.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    string queryClientes = "SELECT COUNT(1) FROM Clientes";
                    using (SqlCommand cmd = new SqlCommand(queryClientes, conexion))
                    {
                        lblClientesValor.Text = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    }

                    dgvPedidos.Rows.Clear();

                    string queryGrid = @"
                        SELECT 
                            V.IdVenta, 
                            C.Nombre AS Cliente, 
                            V.FechaVenta, 
                            V.FechaEntrega, 
                            V.Total
                        FROM Ventas V
                        INNER JOIN Clientes C ON V.IdCliente = C.Id
                        WHERE V.FechaEntrega >= CAST(GETDATE() AS DATE)
                        ORDER BY V.FechaEntrega ASC";

                    using (SqlCommand cmdGrid = new SqlCommand(queryGrid, conexion))
                    {
                        using (SqlDataReader lector = cmdGrid.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                DateTime fVenta = Convert.ToDateTime(lector["FechaVenta"]);
                                DateTime fEntrega = Convert.ToDateTime(lector["FechaEntrega"]);
                                decimal totalVenta = Convert.ToDecimal(lector["Total"]);

                                dgvPedidos.Rows.Add(
                                    lector["IdVenta"].ToString(),
                                    lector["Cliente"].ToString(),
                                    fVenta.ToString("dd/MM/yyyy"),
                                    fEntrega.ToString("dd/MM/yyyy"),
                                    totalVenta.ToString("C2")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar las métricas del sistema: " + ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}