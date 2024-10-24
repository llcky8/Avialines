using MySql.Data.MySqlClient;
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
    public partial class SelectionForm : Form
    {
        private MySqlConnection con;
        public SelectionForm()
        {
            InitializeComponent();
            //подключение бд
            string connstring = "server=localhost;port=3306;username=root;password=admin;database=avialines";
            con = new MySqlConnection(connstring);
            try
            {
                con.Open();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Открыть новое окно и скрыть текущее
            Airplane nextForm = new Airplane();
            this.Hide();  // Скрыть текущее окно
            nextForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            HelloForm mainForm = new HelloForm();
            mainForm.Show(); // Открывает главную форму
            this.Hide(); // Скрывает текущую форму
        }
    }
}
