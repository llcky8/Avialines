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
    public partial class Table : Form
    {
        private MySqlConnection con;
        private string buttonClicked;
        private DataTable dt;

        public Table(string buttonName)
        {
            InitializeComponent();
            buttonClicked = buttonName;
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
                string sql = string.Empty;

                // В зависимости от нажатой кнопки, задаем разные запросы
                switch (buttonClicked)
                {
                    case "Button1":
                        sql = "SELECT * FROM avialines.airplane";
                        break;
                    case "Button2":
                        sql = "SELECT * FROM avialines.airport";
                        break;
                    case "Button3":
                        sql = "SELECT * FROM avialines.flight";
                        break;
                    case "Button4":
                        sql = "SELECT * FROM avialines.model";
                        break;
                    case "Button5":
                        sql = "SELECT * FROM avialines.passanger5";
                        break;
                    case "Button6":
                        sql = "SELECT * FROM avialines.pilot";
                        break;
                    case "Button7":
                        sql = "SELECT * FROM avialines.rout";
                        break;
                    case "Button8":
                        sql = "SELECT * FROM avialines.ticket";
                        break;
                    default:
                        MessageBox.Show("Неизвестная таблица.");
                        break;
                }
                // Выполняем запрос

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

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SelectionForm mainForm = new SelectionForm();
            mainForm.Show(); // Открывает главную форму
            this.Hide(); // Скрывает текущую форму
        }

        private void Table_Load(object sender, EventArgs e)
        {
            this.Text = "Выбранная таблица";
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
        }
    }
}
