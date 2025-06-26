using CNego;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CView
{
    public partial class frm2OrdDet : Form
    {
        private C_Producto producto = new C_Producto();
        private C_Servicio servicio = new C_Servicio();
        private C_Orden orden = new C_Orden();
        private C_OrdDet orddet = new C_OrdDet();
        public frm2OrdDet()
        {
            InitializeComponent();
        }

        private void frm2OrdDet_Load(object sender, EventArgs e)
        {
            try
            {
                CargaOrdenes();
                CargaProductos();
                CargaServicios();
                //dtfecing.Value = DateTime.Now;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error: " + ex.Message);
            }
        }

        private void CargaOrdenes()
        {
            dgOrd.DataSource = orden.GetOrdenes("A");
        }
        private void CargaProductos()
        {
            try
            {
                DataTable dtPro = producto.GetProductos();
                cbPro.DataSource = dtPro;
                cbPro.DisplayMember = "descri";
                cbPro.ValueMember = "id";
                if (cbPro.Items.Count > 0)
                {
                    cbPro.SelectedIndex = 0;
                    cbPro_SelectedIndexChanged(cbPro, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbPro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPro.SelectedValue != null && int.TryParse(cbPro.SelectedValue.ToString(), out int id))
            {
                var pro = producto.GetProducto(id);
                txtprocod.Text = Convert.ToString(pro.Id);
                txtproref.Text = pro.Refere;
                txtpromar.Text = pro.Marca;
                txtproprc.Text = pro.Prcvta.ToString("C", new System.Globalization.CultureInfo("es-EC"));
                txtproprc.Tag = pro.Prcvta;
                if (pro.Cantid > 0)
                {
                    nmprocan.Maximum = pro.Cantid;
                    nmprocan.Minimum = 1;
                    nmprocan.Enabled = true;
                    nmprocan.Value = 1;
                    nmprocan_ValueChanged(nmprocan, EventArgs.Empty);
                }
                else
                {
                    nmprocan.Maximum = 0;
                    nmprocan.Minimum = 0;
                    nmprocan.Value = 0;
                    nmprocan.Enabled = false;
                    MessageBox.Show($"Item sin stock", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
        private void CargaServicios()
        {
            try
            {
                DataTable dtSer = servicio.GetServicios();
                cbSer.DataSource = dtSer;
                cbSer.DisplayMember = "descri";
                cbSer.ValueMember = "id";
                if (cbSer.Items.Count > 0)
                {
                    cbSer.SelectedIndex = 0;
                    cbSer_SelectedIndexChanged(cbSer, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbSer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSer.SelectedValue != null && int.TryParse(cbSer.SelectedValue.ToString(), out int id))
            {
                var ser = servicio.GetServicio(id);
                txtsercod.Text = Convert.ToString(ser.Id);
                txtserref.Text = ser.Refere;
                txtserprc.Text = ser.Prcvta.ToString("C", new System.Globalization.CultureInfo("es-EC"));
                txtserprc.Tag = ser.Prcvta;
            }
        }

        private void txtproobs_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite solo letras, números, tildes, la ñ y la tecla de retroceso
            if (!Char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != (char)8 &&
               !("áéíóúÁÉÍÓÚñÑ".Contains(e.KeyChar.ToString())) && e.KeyChar != ' ')
            {
                e.Handled = true; // Cancela la entrada si no es un carácter permitido
            }
        }
        private void txtserobs_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite solo letras, números, tildes, la ñ y la tecla de retroceso
            if (!Char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != (char)8 &&
               !("áéíóúÁÉÍÓÚñÑ".Contains(e.KeyChar.ToString())) && e.KeyChar != ' ')
            {
                e.Handled = true; // Cancela la entrada si no es un carácter permitido
            }
        }
        private void txtobs_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite solo letras, números, tildes, la ñ y la tecla de retroceso
            if (!Char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != (char)8 &&
               !("áéíóúÁÉÍÓÚñÑ".Contains(e.KeyChar.ToString())) && e.KeyChar != ' ')
            {
                e.Handled = true; // Cancela la entrada si no es un carácter permitido
            }
        }

        private void nmprocan_ValueChanged(object sender, EventArgs e)
        {
            decimal valor = Convert.ToDecimal(txtproprc.Tag);
            valor = valor * nmprocan.Value;
            txtprotot.Text = valor.ToString("C", new System.Globalization.CultureInfo("es-EC"));
            txtprotot.Tag = valor;
        }

        private void btnsave_Click(object sender, EventArgs e)
        {

            try
            {
                TabPage tabActiva = tabc.SelectedTab;

                if (!ValidarDatos(tabActiva.Name))
                {
                    MessageBox.Show("Debe llenar todos los campos para poder Grabar");
                    return;
                }

                DataGridViewRow selectedRow = dgOrd.CurrentRow;

                // Obtener valores de las celdas
                int idord = Convert.ToInt32(selectedRow.Cells[0].Value);

                C_OrdDet odet = new C_OrdDet();
                odet.Id_ord = idord;

                switch (tabActiva.Name)
                {
                    case "tbpro":
                        // Productos                        
                        odet.Tipdet = 1;
                        odet.Id_ite = Convert.ToInt32(txtprocod.Text);
                        odet.Prcvta = Convert.ToDecimal(txtproprc.Tag);
                        odet.Cantid = Convert.ToInt32(nmprocan.Value);
                        odet.Observ = txtproobs.Text;
                        break;
                    case "tbser":
                        // servicios
                        odet.Tipdet = 2;
                        odet.Id_ite = Convert.ToInt32(txtsercod.Text);
                        odet.Prcvta = Convert.ToDecimal(txtserprc.Tag);
                        odet.Cantid = 1;
                        odet.Observ = txtserobs.Text;
                        break;
                    case "tbobs":
                        // otros
                        odet.Tipdet = 3;
                        odet.Observ = txtobs.Text;
                        break;
                }
                orddet.GrabaDetalle(odet);
                MessageBox.Show("Detalle grabado con éxito");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error (save) : " + ex.Message);
            }

        }

        private bool ValidarDatos(string name)
        {

            switch (name)
            {
                case "tbpro":
                    if (nmprocan.Value == 0) return false;
                    if (txtproobs.Text.Length == 0) return false;
                    break;
                case "tbser":
                    if (txtserobs.Text.Length == 0) return false;
                    break;
                case "tbobs":
                    if (txtobs.Text.Length == 0) return false;
                    break;
            }
            return true;
        }

        private void frm2OrdDet_Shown(object sender, EventArgs e)
        {
            if (dgOrd.RowCount == 0)
            {
                MessageBox.Show("No hay ordenes activas");
                this.Close();
            }
        }
    }
}
