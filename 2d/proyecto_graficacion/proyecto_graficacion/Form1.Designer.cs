namespace proyecto_graficacion
{
    partial class frm_principal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_fondo = new System.Windows.Forms.Panel();
            this.pb_servbot = new System.Windows.Forms.PictureBox();
            this.btn_salvar = new System.Windows.Forms.Button();
            this.panel_fondo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_servbot)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_fondo
            // 
            this.panel_fondo.BackColor = System.Drawing.Color.White;
            this.panel_fondo.BackgroundImage = global::proyecto_graficacion.Properties.Resources.fondo_temporal;
            this.panel_fondo.Controls.Add(this.pb_servbot);
            this.panel_fondo.Location = new System.Drawing.Point(12, 12);
            this.panel_fondo.Name = "panel_fondo";
            this.panel_fondo.Size = new System.Drawing.Size(860, 520);
            this.panel_fondo.TabIndex = 0;
            this.panel_fondo.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_fondo_Paint);
            // 
            // pb_servbot
            // 
            this.pb_servbot.BackColor = System.Drawing.Color.Transparent;
            this.pb_servbot.Image = global::proyecto_graficacion.Properties.Resources.servbot_temporal;
            this.pb_servbot.Location = new System.Drawing.Point(360, 32);
            this.pb_servbot.Name = "pb_servbot";
            this.pb_servbot.Size = new System.Drawing.Size(320, 488);
            this.pb_servbot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_servbot.TabIndex = 0;
            this.pb_servbot.TabStop = false;
            this.pb_servbot.Paint += new System.Windows.Forms.PaintEventHandler(this.pb_servbot_Paint);
            // 
            // btn_salvar
            // 
            this.btn_salvar.Location = new System.Drawing.Point(799, 538);
            this.btn_salvar.Name = "btn_salvar";
            this.btn_salvar.Size = new System.Drawing.Size(75, 23);
            this.btn_salvar.TabIndex = 1;
            this.btn_salvar.Text = "&Salvar";
            this.btn_salvar.UseVisualStyleBackColor = true;
            this.btn_salvar.Click += new System.EventHandler(this.btn_salvar_Click);
            // 
            // frm_principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(886, 568);
            this.Controls.Add(this.btn_salvar);
            this.Controls.Add(this.panel_fondo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_principal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Proyecto Graficación";
            this.Load += new System.EventHandler(this.frm_principal_Load);
            this.panel_fondo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_servbot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_fondo;
        private System.Windows.Forms.PictureBox pb_servbot;
        private System.Windows.Forms.Button btn_salvar;
    }
}

