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
    public partial class RequestForm : BaseForm
    {
        private MySqlConnection con;
        private DataTable dt;
        public RequestForm()
        {
            InitializeComponent();
        }
        private void RequestForm_Load(object sender, EventArgs e)
        {
            this.Text = "Запрос вылетов на выбранную дату";
            InitializeFlightRequestForm();
        }

        private void InitializeFlightRequestForm()
        {
            // Настраиваем интерфейс
            dateTimePicker1.Format = DateTimePickerFormat.Short;

            dataGridView1.DataSource = null;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ExecuteQuery()
        {
            // Получаем выбранную дату
            DateTime selectedDate = dateTimePicker1.Value.Date;
            string formattedDate = selectedDate.ToString("yyyy-MM-dd");

            // Формируем SQL-запрос
            string sql = $@"
            SELECT 
                flight.datetime_flight AS 'Дата и время вылета',
                departure.air_name AS 'Аэропорт вылета',
                departure.air_city AS 'Город вылета',
                arrival.air_name AS 'Аэропорт прибытия',
                arrival.air_city AS 'Город прибытия',
                rout.duration AS 'Длительность маршрута (мин)',
                CASE 
                    WHEN flight.cancel = 'Yes' THEN 'Отменен' 
                    ELSE 'Активен' 
                END AS 'Статус рейса'
            FROM flight
            JOIN rout ON flight.rout_id = rout.rout_id
            JOIN airport AS departure ON rout.departure_id = departure.air_id
            JOIN airport AS arrival ON rout.arrival_id = arrival.air_id
            WHERE DATE(flight.datetime_flight) = '{formattedDate}'";

            ExecuteSqlQuery(sql);
        }

        private void ExecuteSqlQuery(string sql)
        {
            string connString = "server=localhost;port=3306;username=root;password=admin;database=avialines";
            con = new MySqlConnection(connString);

            try
            {
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, con);

                dt = new DataTable();
                da.Fill(dt);

                // Отображение данных
                dataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для выбранной даты.");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void ButtonExecuteQuery_Click(object sender, EventArgs e)
        {
            ExecuteQuery();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SelectionForm mainForm = new SelectionForm();
            mainForm.Show();
            this.Hide();
        }
    }
}