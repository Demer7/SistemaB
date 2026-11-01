using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaB
{
    public partial class ConfiguracionForm : Form
    {
        // Cabecera y Navegación
        private Guna2Button btnVolver;
        private Guna2HtmlLabel lblTitulo, lblSubtitulo;

        // Contenedor principal y Tabs
        private Guna2Panel panelContenedor;
        private Guna2TabControl tabConfiguracion;

        // Paleta de colores neutra
        private Color colorOscuroBase = Color.FromArgb(35, 35, 35); // Gris carbón
        private Color colorFondo = Color.FromArgb(242, 242, 242);   // Gris claro
        private Color colorTexto = Color.FromArgb(80, 80, 80);

        public ConfiguracionForm()
        { 
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = colorFondo;

            ConfigurarInterfaz();

            this.Resize += (s, e) => CentrarTodo();
        }

        private void ConfigurarInterfaz()
        {
            // ---- BOTÓN VOLVER ----
            btnVolver = new Guna2Button();
            btnVolver.Text = "← Volver al Menú";
            btnVolver.Size = new Size(150, 40);
            btnVolver.Location = new Point(30, 30);
            btnVolver.FillColor = Color.Transparent;
            btnVolver.ForeColor = Color.FromArgb(64, 64, 64);
            btnVolver.Font = new Font("Segoe UI Semibold", 11);
            btnVolver.Cursor = Cursors.Hand;
            btnVolver.Click += (s, e) => {
                DashboardForm dash = new DashboardForm();
                dash.Show();
                this.Close();
            };
            this.Controls.Add(btnVolver);

            // ---- TÍTULOS ----
            lblTitulo = new Guna2HtmlLabel();
            lblTitulo.Text = "<span style='font-family:Segoe UI Semibold; font-size:24pt; color:#333333;'>Gestify</span>";
            this.Controls.Add(lblTitulo);

            lblSubtitulo = new Guna2HtmlLabel();
            lblSubtitulo.Text = "<span style='font-family:Segoe UI; font-size:12pt; color:#555555;'>Configuración del Sistema</span>";
            this.Controls.Add(lblSubtitulo);

            // ---- CONTENEDOR PRINCIPAL ----
            panelContenedor = new Guna2Panel();
            panelContenedor.Size = new Size(850, 580);
            panelContenedor.FillColor = Color.White;
            panelContenedor.BorderRadius = 15;
            this.Controls.Add(panelContenedor);

            // ---- TAB CONTROL ----
            tabConfiguracion = new Guna2TabControl();
            tabConfiguracion.Size = new Size(810, 540);
            tabConfiguracion.Location = new Point(20, 20);
            tabConfiguracion.TabMenuBackColor = Color.White;
            tabConfiguracion.TabButtonHoverState.FillColor = colorFondo;
            tabConfiguracion.TabButtonIdleState.FillColor = Color.White;
            tabConfiguracion.TabButtonIdleState.ForeColor = colorTexto;
            tabConfiguracion.TabButtonSelectedState.FillColor = colorOscuroBase;
            tabConfiguracion.TabButtonSelectedState.ForeColor = Color.White;
            tabConfiguracion.Font = new Font("Segoe UI Semibold", 10);
            panelContenedor.Controls.Add(tabConfiguracion);

            // Crear las 4 pestañas
            CrearTabDatosNegocio();
            CrearTabGenerico("Categorías de Productos", "Nueva Categoría", "Ej. Pasteles, Bebidas...");
            CrearTabGenerico("Unidades de Medida", "Nueva Unidad", "Ej. Kg, Litros, Piezas...");
            CrearTabGenerico("Estatus de Pedido", "Nuevo Estatus", "Ej. Pendiente, Entregado...");

            CentrarTodo();
        }

        private void CrearTabDatosNegocio()
        {
            TabPage tab = new TabPage("Datos del Negocio");
            tab.BackColor = Color.White;

            CrearLabel(tab, "Configuración General del Negocio", 30, 30, true);

            CrearLabel(tab, "Nombre del Negocio", 30, 80, false);
            Guna2TextBox txtNombre = CrearInput(tab, "Ej. Mi Tienda...", 30, 105, 400);

            CrearLabel(tab, "Teléfono de Contacto", 30, 160, false);
            Guna2TextBox txtTelefono = CrearInput(tab, "Ej. 555 123 4567", 30, 185, 400);

            CrearLabel(tab, "Dirección Física", 30, 240, false);
            Guna2TextBox txtDireccion = CrearInput(tab, "Calle, Colonia, Ciudad...", 30, 265, 400);

            Guna2Button btnGuardar = CrearBotonGuardar(30, 340, "Guardar Cambios");
            tab.Controls.Add(btnGuardar);

            tabConfiguracion.TabPages.Add(tab);
        }

        // Método reutilizable para las pestañas de tablas (Categorías, Unidades, Estatus)
        private void CrearTabGenerico(string tituloTab, string labelInput, string placeholder)
        {
            TabPage tab = new TabPage(tituloTab);
            tab.BackColor = Color.White;

            CrearLabel(tab, $"Gestión de {tituloTab}", 30, 30, true);

            // Panel Izquierdo (Formulario)
            CrearLabel(tab, labelInput, 30, 80, false);
            Guna2TextBox txtInput = CrearInput(tab, placeholder, 30, 105, 300);

            Guna2Button btnAgregar = CrearBotonGuardar(30, 160, "+ Agregar");
            tab.Controls.Add(btnAgregar);

            // Panel Derecho (Tabla de previsualización)
            CrearLabel(tab, "Registros Actuales", 380, 80, false);
            Guna2DataGridView dgvMock = new Guna2DataGridView();
            dgvMock.Size = new Size(390, 350);
            dgvMock.Location = new Point(380, 105);
            dgvMock.BackgroundColor = colorFondo;
            dgvMock.BorderStyle = BorderStyle.None;
            dgvMock.AllowUserToAddRows = false;
            dgvMock.RowHeadersVisible = false;

            // Estilo neutro de tabla
            dgvMock.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(230, 230, 230);
            dgvMock.ThemeStyle.HeaderStyle.ForeColor = Color.DimGray;
            dgvMock.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10);
            dgvMock.ThemeStyle.RowsStyle.SelectionBackColor = colorFondo;
            dgvMock.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;

            dgvMock.Columns.Add("id", "ID");
            dgvMock.Columns.Add("nombre", "Descripción");
            dgvMock.Columns["id"].Width = 50;

            tab.Controls.Add(dgvMock);
            tabConfiguracion.TabPages.Add(tab);
        }

        // ---- MÉTODOS AUXILIARES (Diseño UI) ----
        private void CrearLabel(Control padre, string texto, int x, int y, bool esTitulo)
        {
            Guna2HtmlLabel lbl = new Guna2HtmlLabel();
            lbl.Text = esTitulo ? $"<span style='font-family:Segoe UI Semibold; font-size:16pt; color:#333333;'>{texto}</span>"
                                : $"<span style='font-family:Segoe UI Semibold; font-size:10pt; color:#555555;'>{texto}</span>";
            lbl.Location = new Point(x, y);
            lbl.BackColor = Color.Transparent;
            padre.Controls.Add(lbl);
        }

        private Guna2TextBox CrearInput(Control padre, string placeholder, int x, int y, int ancho)
        {
            Guna2TextBox txt = new Guna2TextBox();
            txt.Size = new Size(ancho, 40);
            txt.Location = new Point(x, y);
            txt.BorderRadius = 8;
            txt.PlaceholderText = placeholder;
            txt.FillColor = colorFondo;
            txt.BorderThickness = 0;
            padre.Controls.Add(txt);
            return txt;
        }

        private Guna2Button CrearBotonGuardar(int x, int y, string texto)
        {
            Guna2Button btn = new Guna2Button();
            btn.Text = texto;
            btn.Size = new Size(300, 45);
            btn.Location = new Point(x, y);
            btn.BorderRadius = 10;
            btn.FillColor = colorOscuroBase;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 11);
            btn.Cursor = Cursors.Hand;

            // Evento simulado sin funcionalidad real
            btn.Click += (s, e) => {
                MessageBox.Show("Esta función de configuración está en construcción para futuras actualizaciones.", "Gestify - Módulo Inactivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            return btn;
        }

        private void CentrarTodo()
        {
            if (lblTitulo == null || panelContenedor == null) return;

            lblTitulo.Location = new Point((this.ClientSize.Width - lblTitulo.Width) / 2, 40);
            lblSubtitulo.Location = new Point((this.ClientSize.Width - lblSubtitulo.Width) / 2, 85);

            panelContenedor.Location = new Point(
                (this.ClientSize.Width - panelContenedor.Width) / 2,
                140
            );
        }
    }
}