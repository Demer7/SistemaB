using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemaB
{
    public partial class ClientesForm : Form
    {
        // Componentes de la Izquierda (Registro)
        private Guna2Panel panelRegistro;
        private Guna2TextBox txtNombre, txtTelefono, txtDireccion, txtNotas;
        private Guna2Button btnAgregar;

        // Componentes de la Derecha (Lista y Buscador)
        private Guna2Panel panelLista;
        private Guna2DataGridView dgClientes;
        private Guna2TextBox txtBuscar;

        // Cabecera y Navegación
        private Guna2Button btnVolver;
        private Guna2HtmlLabel lblTitulo, lblSubtitulo;

        private string cadenaConexion = "Server=.\\SQLEXPRESS;Database=CherriesBloodDB;Trusted_Connection=True;";

        // Variable para controlar el estado de Edición
        private string idClienteSeleccionado = null;

        public ClientesForm()
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
            // BOTÓN VOLVER
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

            // TÍTULOS CENTRADOS
            lblTitulo = new Guna2HtmlLabel();
            lblTitulo.Text = "<span style='font-family:Segoe UI Semibold; font-size:24pt; color:#5B3B73;'>Cherries Blood</span>";
            this.Controls.Add(lblTitulo);

            lblSubtitulo = new Guna2HtmlLabel();
            lblSubtitulo.Text = "<span style='font-family:Segoe UI; font-size:12pt; color:#7A5A95;'>Gestión de Clientes</span>";
            this.Controls.Add(lblSubtitulo);

            //PANEL IZQUIERDO (Añadir/Editar Cliente)
            panelRegistro = new Guna2Panel();
            panelRegistro.Size = new Size(350, 550);
            panelRegistro.FillColor = Color.White;
            panelRegistro.BorderRadius = 20;
            this.Controls.Add(panelRegistro);

            CrearLabel(panelRegistro, "Añadir Nuevo Cliente", 20, 25, true);

            CrearLabel(panelRegistro, "Nombre", 20, 70, false);
            txtNombre = CrearInput(panelRegistro, "Escribe el nombre...", 100);

            CrearLabel(panelRegistro, "Teléfono", 20, 160, false);
            txtTelefono = CrearInput(panelRegistro, "Escribe el teléfono...", 190);
            txtTelefono.MaxLength = 10;
            txtTelefono.KeyPress += (s, e) => {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            };

            CrearLabel(panelRegistro, "Dirección", 20, 250, false);
            txtDireccion = CrearInput(panelRegistro, "Escribe la dirección...", 280);

            CrearLabel(panelRegistro, "Notas", 20, 340, false);
            txtNotas = CrearInput(panelRegistro, "Notas adicionales...", 370);
            txtNotas.Multiline = true;
            txtNotas.Height = 80;

            btnAgregar = new Guna2Button();
            btnAgregar.Text = "+ Agregar";
            btnAgregar.Size = new Size(310, 45);
            btnAgregar.Location = new Point(20, 480);
            btnAgregar.BorderRadius = 15;
            btnAgregar.FillColor = Color.FromArgb(91, 59, 115);
            btnAgregar.Font = new Font("Segoe UI Semibold", 12);
            btnAgregar.ShadowDecoration.Enabled = false;
            btnAgregar.Cursor = Cursors.Hand;

            btnAgregar.Click += (s, e) => {
                string nombre = txtNombre.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string direccion = txtDireccion.Text.Trim();
                string notas = txtNotas.Text.Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("El campo 'Nombre' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(telefono))
                {
                    MessageBox.Show("El campo 'Teléfono' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (telefono.Length != 10)
                {
                    MessageBox.Show("El número de teléfono debe tener exactamente 10 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query;
                if (idClienteSeleccionado == null)
                {
                    query = "INSERT INTO Clientes (Nombre, Telefono, Direccion, Notas) VALUES (@nombre, @telefono, @direccion, @notas)";
                }
                else
                {
                    query = "UPDATE Clientes SET Nombre = @nombre, Telefono = @telefono, Direccion = @direccion, Notas = @notas WHERE Id = @id";
                }

                try
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                    {
                        using (SqlCommand comando = new SqlCommand(query, conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", nombre);
                            comando.Parameters.AddWithValue("@telefono", telefono);
                            comando.Parameters.AddWithValue("@direccion", direccion);
                            comando.Parameters.AddWithValue("@notas", notas);

                            if (idClienteSeleccionado != null)
                            {
                                comando.Parameters.AddWithValue("@id", idClienteSeleccionado);
                            }

                            conexion.Open();
                            comando.ExecuteNonQuery();

                            if (idClienteSeleccionado == null)
                            {
                                MessageBox.Show("¡Cliente registrado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("¡Cambios guardados correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            txtNombre.Clear();
                            txtTelefono.Clear();
                            txtDireccion.Clear();
                            txtNotas.Clear();

                            idClienteSeleccionado = null;
                            btnAgregar.Text = "+ Agregar";
                            btnAgregar.FillColor = Color.FromArgb(91, 59, 115);

                            CargarClientes();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al procesar en la base de datos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            panelRegistro.Controls.Add(btnAgregar);

            //  PANEL DERECHO
            panelLista = new Guna2Panel();
            panelLista.Size = new Size(750, 550);
            panelLista.FillColor = Color.White;
            panelLista.BorderRadius = 20;
            this.Controls.Add(panelLista);

            CrearLabel(panelLista, "Lista de Clientes", 25, 25, true);

            // CONFIGURACIÓN DEL BUSCADOR
            txtBuscar = new Guna2TextBox();
            txtBuscar.Size = new Size(350, 36);
            txtBuscar.Location = new Point(25, 70);
            txtBuscar.BorderRadius = 10;
            txtBuscar.PlaceholderText = "🔍 Buscar cliente por nombre...";
            txtBuscar.FillColor = Color.FromArgb(242, 242, 242);
            txtBuscar.BorderThickness = 0;
            txtBuscar.Font = new Font("Segoe UI", 10);
            txtBuscar.TextChanged += (s, e) => {
                CargarClientes(txtBuscar.Text.Trim());
            };
            panelLista.Controls.Add(txtBuscar);

            dgClientes = new Guna2DataGridView();
            dgClientes.AllowUserToAddRows = false;
            dgClientes.Size = new Size(700, 405);
            dgClientes.Location = new Point(25, 120);
            dgClientes.BackgroundColor = Color.White;
            dgClientes.BorderStyle = BorderStyle.None;
            dgClientes.ColumnHeadersHeight = 40;

            dgClientes.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgClientes.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgClientes.ThemeStyle.HeaderStyle.BackColor = Color.White;
            dgClientes.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgClientes.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            dgClientes.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(242, 242, 242);
            dgClientes.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgClientes.RowHeadersVisible = false;

            // Columnas
            dgClientes.Columns.Add("id", "ID");
            dgClientes.Columns.Add("nombre", "Nombre");
            dgClientes.Columns.Add("tel", "Teléfono");
            dgClientes.Columns.Add("dir", "Dirección");
            dgClientes.Columns.Add("notas", "Notas");

            dgClientes.Columns["id"].Visible = false;
            dgClientes.Columns["nombre"].Width = 160;
            dgClientes.Columns["tel"].Width = 100;

            // Botón Editar Fila
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
            colEditar.Name = "editar";
            colEditar.HeaderText = "Acciones";
            colEditar.Text = "✏️";
            colEditar.UseColumnTextForButtonValue = true;
            colEditar.Width = 60;
            colEditar.FlatStyle = FlatStyle.Flat;
            colEditar.DefaultCellStyle.BackColor = Color.FromArgb(91, 59, 115);
            colEditar.DefaultCellStyle.ForeColor = Color.White;
            colEditar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(91, 59, 115);
            colEditar.DefaultCellStyle.SelectionForeColor = Color.White;
            dgClientes.Columns.Add(colEditar);

            // Botón Eliminar Fila
            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.Name = "eliminar";
            colEliminar.HeaderText = "";
            colEliminar.Text = "🗑️";
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.Width = 60;
            colEliminar.FlatStyle = FlatStyle.Flat;
            colEliminar.DefaultCellStyle.BackColor = Color.FromArgb(227, 110, 110);
            colEliminar.DefaultCellStyle.ForeColor = Color.White;
            colEliminar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(227, 110, 110);
            colEliminar.DefaultCellStyle.SelectionForeColor = Color.White;
            dgClientes.Columns.Add(colEliminar);

            dgClientes.CellContentClick += DgClientes_CellContentClick;
            dgClientes.CellFormatting += (s, e) => {
                if (e.RowIndex < 0) return;

                if (dgClientes.Columns[e.ColumnIndex].Name == "editar")
                {
                    e.CellStyle.BackColor = Color.FromArgb(91, 59, 115);
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (dgClientes.Columns[e.ColumnIndex].Name == "eliminar")
                {
                    e.CellStyle.BackColor = Color.FromArgb(227, 110, 110);
                    e.CellStyle.ForeColor = Color.White;
                }
            };

            panelLista.Controls.Add(dgClientes);

            CentrarTodo();
            CargarClientes();
        }

        private void CargarClientes(string filtroNombre = "")
        {
            dgClientes.Rows.Clear();
            string querySelect = "SELECT Id, Nombre, Telefono, Direccion, Notas FROM Clientes";

            if (!string.IsNullOrEmpty(filtroNombre))
            {
                querySelect += " WHERE Nombre LIKE @filtro";
            }

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    using (SqlCommand comando = new SqlCommand(querySelect, conexion))
                    {
                        if (!string.IsNullOrEmpty(filtroNombre))
                        {
                            comando.Parameters.AddWithValue("@filtro", "%" + filtroNombre + "%");
                        }

                        conexion.Open();
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                dgClientes.Rows.Add(
                                    lector["Id"].ToString(),
                                    lector["Nombre"].ToString(),
                                    lector["Telefono"].ToString(),
                                    lector["Direccion"].ToString(),
                                    lector["Notas"].ToString()
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar la lista de clientes: " + ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string idCliente = dgClientes.Rows[e.RowIndex].Cells["id"].Value.ToString();
            string nombreCliente = dgClientes.Rows[e.RowIndex].Cells["nombre"].Value.ToString();

            // ACCIÓN: ELIMINAR
            if (dgClientes.Columns[e.ColumnIndex].Name == "eliminar")
            {
                DialogResult confirmacion = MessageBox.Show(
                    $"¿Estás seguro de que deseas eliminar permanentemente al cliente '{nombreCliente}'?",
                    "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.Yes)
                {
                    string queryDelete = "DELETE FROM Clientes WHERE Id = @id";
                    try
                    {
                        using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                        {
                            using (SqlCommand comando = new SqlCommand(queryDelete, conexion))
                            {
                                comando.Parameters.AddWithValue("@id", idCliente);
                                conexion.Open();
                                comando.ExecuteNonQuery();

                                MessageBox.Show("Cliente eliminado exitosamente.", "Operación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtBuscar.Clear();
                                CargarClientes();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al intentar eliminar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            // ACCIÓN: MODO EDICIÓN RÁPIDA
            if (dgClientes.Columns[e.ColumnIndex].Name == "editar")
            {
                // Guardar el ID seleccionado globalmente
                idClienteSeleccionado = idCliente;

                // Cargar los cuadros de texto
                txtNombre.Text = nombreCliente;
                txtTelefono.Text = dgClientes.Rows[e.RowIndex].Cells["tel"].Value.ToString();
                txtDireccion.Text = dgClientes.Rows[e.RowIndex].Cells["dir"].Value.ToString();
                txtNotas.Text = dgClientes.Rows[e.RowIndex].Cells["notas"].Value.ToString();

                btnAgregar.Text = "✔ Guardar Cambios";
                btnAgregar.FillColor = Color.FromArgb(74, 44, 96); 

                MessageBox.Show("Los datos del cliente se cargaron en el panel izquierdo.\nModifica los campos y presiona 'Guardar Cambios'.",
                                "Modo Edición Activo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            txt.Size = new Size(310, 40);
            txt.Location = new Point(20, y);
            txt.BorderRadius = 10;
            txt.PlaceholderText = placeholder;
            txt.FillColor = Color.FromArgb(242, 242, 242);
            txt.BorderThickness = 0;
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