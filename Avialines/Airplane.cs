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
    public partial class Airplane : Form
    {
        private MySqlConnection con;
        public Airplane()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            //подключение бд
            string connstring = "server=localhost;port=3306;username=root;password=admin;database=avialines";
            con = new MySqlConnection(connstring);
            try
            {
                con.Open();
                string sql = "SELECT * FROM avialines.airplane"; // Запрос для получения данных из таблицы airplane

                MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt; // Заполнение DataGridView данными
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SelectionForm mainForm = new SelectionForm();
            mainForm.Show(); // Открывает главную форму
            this.Hide(); // Скрывает текущую форму
        }
    }
}
