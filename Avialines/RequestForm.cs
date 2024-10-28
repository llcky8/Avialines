using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avialines
{
    public partial class RequestForm : BaseForm
    {
        public RequestForm()
        {
            InitializeComponent();
        }

        private void Request_Load(object sender, EventArgs e)
        {
            this.Text = "Сделайте запрос";
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SelectionForm mainForm = new SelectionForm();
            mainForm.Show();
            this.Hide();
        }
    }
}
