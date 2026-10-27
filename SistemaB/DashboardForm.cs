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
            // Título superior
            lblTitle = new Guna2HtmlLabel();
            lblTitle.Text = "<div style='text-align:center;'><span style='font-family:Segoe UI Semibold; font-size:24pt; color:#5B3B73;'>Cherries Blood</span></div>";
            this.Controls.Add(lblTitle);

            // Contenedor para los 6 botones
            container = new FlowLayoutPanel();
            container.Size = new Size(850, 600);
            container.BackColor = Color.Transparent;
            this.Controls.Add(container);

            // Crear los 6 botones
            CrearBotonMenu("Inventario", Properties.Resources.icon_inventory, true);
            CrearBotonMenu("Productos", Properties.Resources.icon_products, false);
            CrearBotonMenu("Clientes", Properties.Resources.icon_clients, true);
            CrearBotonMenu("Ventas", Properties.Resources.icon_sales, false);
            CrearBotonMenu("Informes", Properties.Resources.icon_reports, true);
            CrearBotonMenu("Salir", Properties.Resources.icon_logout, false);

            CentrarContenedor();
        }

        private void CrearBotonMenu(string texto, Image icono, bool esMorado)
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

            if (esMorado)
            {
                btn.FillColor = Color.FromArgb(122, 90, 149);
                btn.ForeColor = Color.Black;
            }
            else
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
                btn.BorderThickness = 1;
                btn.BorderColor = Color.FromArgb(230, 230, 230);
            }

            btn.Click += (s, e) => {
                if (texto == "Salir")
                {
                    EjecutarCerrarSesion();
                    return;
                }

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
                    // Volver de forma limpia al Login
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
            if (container != null) {
                lblTitle.Location = new Point((this.ClientSize.Width - lblTitle.Width) / 2, 50);
                container.Location = new Point((this.ClientSize.Width - container.Width) / 2, 150);
            }
        }
    }
}