using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaB
{
    public partial class Cerrar_Ses : Form
    {
        public string Resultado { get; private set; } = "Cancelar";

        public Cerrar_Ses()
        {
            InitializeComponent();

            this.Size = new Size(400, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            Guna2Elipse elipse = new Guna2Elipse();
            elipse.BorderRadius = 20;
            elipse.TargetControl = this;

            ConfigurarComponentes();
        }

        private void ConfigurarComponentes()
        {
            // Texto de pregunta
            Guna2HtmlLabel lblMensaje = new Guna2HtmlLabel();
            lblMensaje.Text = "<div style='text-align:center;'><span style='font-family:Segoe UI Semibold; font-size:14pt; color:#333333;'>¿Qué acción deseas realizar?</span></div>";
            lblMensaje.Location = new Point(60, 40);
            this.Controls.Add(lblMensaje);

            // Botón: Cerrar Sesión (Color actualizado a gris carbón)
            Guna2Button btnCerrarSesion = new Guna2Button();
            btnCerrarSesion.Text = "Cerrar Sesión";
            btnCerrarSesion.Size = new Size(160, 45);
            btnCerrarSesion.Location = new Point(30, 100);
            btnCerrarSesion.BorderRadius = 12;
            btnCerrarSesion.FillColor = Color.FromArgb(35, 35, 35);
            btnCerrarSesion.Font = new Font("Segoe UI Semibold", 11);
            btnCerrarSesion.ForeColor = Color.White;
            btnCerrarSesion.Cursor = Cursors.Hand;
            btnCerrarSesion.Click += (s, e) => { Resultado = "Sesion"; this.Close(); };
            this.Controls.Add(btnCerrarSesion);

            // Botón: Cerrar Programa (Mantiene el estilo de contorno blanco/gris)
            Guna2Button btnCerrarPrograma = new Guna2Button();
            btnCerrarPrograma.Text = "Cerrar Programa";
            btnCerrarPrograma.Size = new Size(160, 45);
            btnCerrarPrograma.Location = new Point(210, 100);
            btnCerrarPrograma.BorderRadius = 12;
            btnCerrarPrograma.FillColor = Color.White;
            btnCerrarPrograma.ForeColor = Color.FromArgb(35, 35, 35);
            btnCerrarPrograma.BorderThickness = 1;
            btnCerrarPrograma.BorderColor = Color.FromArgb(200, 200, 200);
            btnCerrarPrograma.Font = new Font("Segoe UI Semibold", 11);
            btnCerrarPrograma.Cursor = Cursors.Hand;
            btnCerrarPrograma.Click += (s, e) => { Resultado = "Programa"; this.Close(); };
            this.Controls.Add(btnCerrarPrograma);

            // Botón para Cancelar abajo
            Guna2Button btnCancelar = new Guna2Button();
            btnCancelar.Text = "Volver al menú";
            btnCancelar.Size = new Size(140, 30);
            btnCancelar.Location = new Point(130, 145); // Ajusté ligeramente Y de 145 a 160 para mejor espaciado
            btnCancelar.FillColor = Color.Transparent;
            btnCancelar.ForeColor = Color.Gray;
            btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Underline);
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.Click += (s, e) => { Resultado = "Cancelar"; this.Close(); };
            this.Controls.Add(btnCancelar);
        }
    }
}