using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemaB
{
    public partial class LoginForm : Form
    {
        private Guna2Panel cardPanel;
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtPassword;
        private Guna2Button btnSignIn;
        private Guna2HtmlLabel lblTitle;
        private Guna2HtmlLabel lblError;
        private Guna2ControlBox btnExit;

        public LoginForm()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            InitializeCustomComponents();

            this.Resize += (s, e) => CentrarDiseno();

            this.FormClosed += (s, e) => Application.Exit();
        }

        private void InitializeCustomComponents()
        {
            // Fondo gris claro
            this.BackColor = Color.FromArgb(242, 242, 242);

            // BOTÓN DE SALIDA DEL SISTEMA 
            btnExit = new Guna2ControlBox();
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.FillColor = Color.Transparent;
            btnExit.IconColor = Color.FromArgb(64, 64, 64); // Gris oscuro neutro en lugar de morado
            btnExit.Location = new Point(this.Width - 45, 0);
            btnExit.Size = new Size(45, 29);
            btnExit.HoverState.FillColor = Color.FromArgb(231, 76, 60);
            btnExit.HoverState.IconColor = Color.White;

            btnExit.Click += (s, e) => {
                Application.Exit();
            };
            this.Controls.Add(btnExit);

            // PANEL CENTRAL
            cardPanel = new Guna2Panel();
            cardPanel.Size = new Size(400, 500);
            cardPanel.FillColor = Color.White;
            cardPanel.BorderRadius = 30;

            cardPanel.ShadowDecoration.Enabled = true;
            cardPanel.ShadowDecoration.BorderRadius = 30;

            // TÍTULO DEL SISTEMA
            lblTitle = new Guna2HtmlLabel();
            // Cambio a "Gestify" y color HEX gris oscuro (#333333)
            lblTitle.Text = "<span style='font-family:Segoe UI Semibold; font-size:28pt; color:#333333;'>Gestify</span>";
            lblTitle.Location = new Point(125, 60); // Ajuste de X (de 70 a 125) para centrar la palabra más corta
            cardPanel.Controls.Add(lblTitle);

            txtUsername = new Guna2TextBox();
            txtUsername.Size = new Size(320, 45);
            txtUsername.Location = new Point(40, 180);
            txtUsername.BorderRadius = 15;
            txtUsername.PlaceholderText = "Nombre de usuario";
            txtUsername.FillColor = Color.FromArgb(242, 242, 242);
            txtUsername.BorderThickness = 0;
            txtUsername.IconLeft = Properties.Resources.Icon_user;
            txtUsername.IconLeftSize = new Size(20, 20);
            txtUsername.IconLeftOffset = new Point(10, 0);
            txtUsername.TabIndex = 0; // Orden para la tecla TAB
            cardPanel.Controls.Add(txtUsername);

            // Evento de teclado para Usuario
            txtUsername.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    e.SuppressKeyPress = true; // Evita el sonido "ding" de Windows
                    txtPassword.Focus();       // Salta a la contraseña
                }
            };

            // INPUT CONTRASEÑA
            txtPassword = new Guna2TextBox();
            txtPassword.Size = new Size(320, 45);
            txtPassword.Location = new Point(40, 260);
            txtPassword.BorderRadius = 15;
            txtPassword.PlaceholderText = "Contraseña";
            txtPassword.PasswordChar = '●';
            txtPassword.FillColor = Color.FromArgb(242, 242, 242);
            txtPassword.BorderThickness = 0;
            txtPassword.IconLeft = Properties.Resources.icon_lock;
            txtPassword.IconLeftSize = new Size(20, 20);
            txtPassword.IconLeftOffset = new Point(10, 0);
            txtPassword.TabIndex = 1; // Orden para la tecla TAB
            cardPanel.Controls.Add(txtPassword);

            // Evento de teclado para Contraseña
            txtPassword.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnSignIn_Click(this, EventArgs.Empty); // Inicia sesión al dar Enter
                }
                else if (e.KeyCode == Keys.Up)
                {
                    e.SuppressKeyPress = true;
                    txtUsername.Focus(); // Regresa al usuario con la flecha arriba
                }
            };

            // BOTÓN DE INICIAR SESIÓN
            btnSignIn = new Guna2Button();
            btnSignIn.Size = new Size(320, 45);
            btnSignIn.Location = new Point(40, 360);
            btnSignIn.BorderRadius = 15;
            btnSignIn.FillColor = Color.FromArgb(35, 35, 35); // Negro/Gris carbón en lugar de morado
            btnSignIn.Text = "Iniciar Sesión";
            btnSignIn.Font = new Font("Segoe UI Semibold", 12);
            btnSignIn.ForeColor = Color.White;
            btnSignIn.Click += BtnSignIn_Click;
            btnSignIn.UseTransparentBackground = true;
            btnSignIn.TabIndex = 2;
            cardPanel.Controls.Add(btnSignIn);

            this.Controls.Add(cardPanel);

            CentrarDiseno();
        }

        private void CentrarDiseno()
        {
            if (cardPanel != null)
            {
                cardPanel.Location = new Point(
                    (this.ClientSize.Width - cardPanel.Width) / 2,
                    (this.ClientSize.Height - cardPanel.Height) / 2
                );
            }
            if (btnExit != null)
            {
                btnExit.Location = new Point(this.ClientSize.Width - 45, 0);
            }
        }

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            string usuarioIngresado = txtUsername.Text.Trim();
            string contrasenaIngresada = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usuarioIngresado) || string.IsNullOrEmpty(contrasenaIngresada))
            {
                MessageBox.Show("Por favor, llena todos los campos.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Actualización del nombre de la base de datos a GestifyDB
            string cadenaConexion = "Server=.\\SQLEXPRESS;Database=GestifyDB;Trusted_Connection=True;";
            string query = "SELECT COUNT(1) FROM Usuarios WHERE Usuario = @usuario AND Contrasena = @contrasena";

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", usuarioIngresado);
                        comando.Parameters.AddWithValue("@contrasena", contrasenaIngresada);

                        conexion.Open();
                        int existe = Convert.ToInt32(comando.ExecuteScalar());

                        if (existe == 1)
                        {
                            MessageBox.Show($"¡Bienvenido al sistema, {usuarioIngresado}!", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            DashboardForm dashboard = new DashboardForm();
                            dashboard.Show();

                            this.FormClosed -= (s, ev) => Application.Exit();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos. Intenta de nuevo.", "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}