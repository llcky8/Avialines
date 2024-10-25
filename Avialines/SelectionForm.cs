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
    public partial class SelectionForm : BaseForm
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
            Table nextForm = new Table("Button1");
            this.Hide();  // Скрыть текущее окно
            nextForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button2");
            this.Hide();
            nextForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button4");
            this.Hide();
            nextForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button3");
            this.Hide();
            nextForm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button5");
            this.Hide();
            nextForm.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button6");
            this.Hide();
            nextForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button7");
            this.Hide();
            nextForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Table nextForm = new Table("Button8");
            this.Hide();
            nextForm.Show();
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            HelloForm mainForm = new HelloForm();
            mainForm.Show();
            this.Hide();
        }
        private void SelectionForm_Load(object sender, EventArgs e)
        {
            this.Text = "Выбери таблицу данных";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
