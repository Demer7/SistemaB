using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaB
{
    public partial class DashboardForm : Form
    {
        private FlowLayoutPanel container;
        private Guna2HtmlLabel lblTitle;
        private Guna2Button btnSalirTop; // Nuevo botón superior

        public DashboardForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(242, 242, 242);

            ConfigurarDashboard();
            this.Resize += (s, e) => CentrarContenedor();
        }

        private void ConfigurarDashboard()
        {
            // Título superior - Cambiado a Gestify y gris oscuro
            lblTitle = new Guna2HtmlLabel();
            lblTitle.Text = "<div style='text-align:center;'><span style='font-family:Segoe UI Semibold; font-size:24pt; color:#333333;'>Gestify</span></div>";
            this.Controls.Add(lblTitle);

            // Nuevo botón de Salir (Superior Derecha)
            btnSalirTop = new Guna2Button();
            btnSalirTop.Text = "Salir";
            btnSalirTop.Size = new Size(120, 40); // Más corto de altura, un poco más largo
            btnSalirTop.BorderRadius = 10;
            btnSalirTop.FillColor = Color.White;
            btnSalirTop.ForeColor = Color.FromArgb(35, 35, 35);
            btnSalirTop.BorderThickness = 1;
            btnSalirTop.BorderColor = Color.FromArgb(200, 200, 200);
            btnSalirTop.Font = new Font("Segoe UI Semibold", 11);
            btnSalirTop.Cursor = Cursors.Hand;
            btnSalirTop.Click += (s, e) => EjecutarCerrarSesion();
            this.Controls.Add(btnSalirTop);

            // Contenedor para los 6 botones
            container = new FlowLayoutPanel();
            container.Size = new Size(850, 600);
            container.BackColor = Color.Transparent;
            this.Controls.Add(container);

            // Crear los 6 botones (cambiando Salir por Configuración)
            // Nota: Asegúrate de tener una imagen 'icon_settings' en tus Properties.Resources
            CrearBotonMenu("Inventario", Properties.Resources.icon_inventory, true);
            CrearBotonMenu("Productos", Properties.Resources.icon_products, false);
            CrearBotonMenu("Clientes", Properties.Resources.icon_clients, true);
            CrearBotonMenu("Ventas", Properties.Resources.icon_sales, false);
            CrearBotonMenu("Informes", Properties.Resources.icon_reports, true);
            CrearBotonMenu("Configuración", Properties.Resources.icon_settings, false);

            CentrarContenedor();
        }

        // Se cambió 'esMorado' por 'esOscuro' para reflejar la nueva paleta
        private void CrearBotonMenu(string texto, Image icono, bool esOscuro)
        {
            Guna2TileButton btn = new Guna2TileButton();
            btn.Text = texto;
            btn.Image = icono;
            btn.ImageSize = new Size(50, 50);
            btn.Size = new Size(400, 180);
            btn.Margin = new Padding(10);
            btn.BorderRadius = 25;
            btn.Font = new Font("Segoe UI Semibold", 14);
            btn.ShadowDecoration.Enabled = false;

            if (esOscuro)
            {
                // Paneles oscuros (Gris carbón/Negro)
                btn.FillColor = Color.FromArgb(35, 35, 35);
                btn.ForeColor = Color.White; // Texto blanco para contraste
            }
            else
            {
                // Paneles claros (Blanco)
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
                btn.BorderThickness = 1;
                btn.BorderColor = Color.FromArgb(230, 230, 230);
            }

            btn.Click += (s, e) => {
                Form siguienteForm = null;

                switch (texto)
                {
                    case "Inventario":
                        siguienteForm = new InventarioForm();
                        break;
                    case "Productos":
                        siguienteForm = new ProductosForm();
                        break;
                    case "Clientes":
                        siguienteForm = new ClientesForm();
                        break;
                    case "Ventas":
                        siguienteForm = new VentasForm();
                        break;
                    case "Informes":
                        siguienteForm = new InformesForm();
                        break;
                    case "Configuración":
                        siguienteForm = new ConfiguracionForm();
                        break;
                }

                if (siguienteForm != null)
                {
                    siguienteForm.Show();
                    this.Hide();
                }
            };

            container.Controls.Add(btn);
        }

        private void EjecutarCerrarSesion()
        {
            using (Cerrar_Ses dialogo = new Cerrar_Ses())
            {
                dialogo.ShowDialog(this);

                if (dialogo.Resultado == "Sesion")
                {
                    LoginForm login = new LoginForm();
                    login.Show();
                    this.Dispose();
                }
                else if (dialogo.Resultado == "Programa")
                {
                    Application.Exit();
                }
            }
        }

        private void CentrarContenedor()
        {
            if (container != null && lblTitle != null)
            {
                lblTitle.Location = new Point((this.ClientSize.Width - lblTitle.Width) / 2, 50);
                container.Location = new Point((this.ClientSize.Width - container.Width) / 2, 150);
            }

            // Mantiene el botón de salir siempre en la esquina superior derecha
            if (btnSalirTop != null)
            {
                btnSalirTop.Location = new Point(this.ClientSize.Width - 160, 30);
            }
        }
    }
}