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
            this.Text = "Сделайте запрос";
            InitializeTableSelection();
        }

        private void LoadTableNames()
        {
            string connString = "server=localhost;port=3306;username=root;password=admin;database=avialines";
            using (var connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    var query = "SHOW TABLES"; // Запрос для получения имен таблиц
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Добавляем каждую таблицу в ComboBox
                                comboBox1.Items.Add(reader[0].ToString());
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Ошибка при загрузке таблиц: " + ex.Message);
                }
            }
        }


        private void InitializeTableSelection()
        {
            // Заполняем comboBox1 доступными таблицами
            comboBox1.Items.AddRange(new string[]
            {
            "airplane", "airport", "flight", "model", "passenger", "pilot", "route", "ticket"
            });

            // Подключаем обработчик изменения выбора таблицы
            comboBox1.SelectedIndexChanged += ComboBoxTables_SelectedIndexChanged;

            // Заполняем comboBox2 значениями для сортировки
            comboBox2.Items.Add("По возрастанию");
            comboBox2.Items.Add("По убыванию");

            // Устанавливаем значение по умолчанию
            comboBox2.SelectedIndex = 0; // Установить по умолчанию "По возрастанию"
        }

        private void ComboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = comboBox1.SelectedItem.ToString();
            LoadColumns(selectedTable);
        }

        private void LoadColumns(string tableName)
        {
            // Очищаем список столбцов перед загрузкой новых
            checkedListBox1.Items.Clear();

            // Получаем имена столбцов для выбранной таблицы
            using (MySqlConnection con = new MySqlConnection("server=localhost;port=3306;username=root;password=admin;database=avialines"))
            {
                con.Open();
                string sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{tableName}' AND TABLE_SCHEMA='avialines'";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkedListBox1.Items.Add(reader["COLUMN_NAME"].ToString());
                        }
                    }
                }
            }
        }

        private void ExecuteQuery()
        {
            if (comboBox1.SelectedItem == null || checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Выберите таблицу и столбцы для запроса.");
                return;
            }

            string tableName = comboBox1.SelectedItem.ToString();
            string selectedColumns = string.Join(", ", checkedListBox1.CheckedItems.Cast<string>());

            string whereClause = textBox1.Text.Trim();

            // Получаем порядок сортировки из comboBox2
            string orderBy = comboBox2.SelectedItem?.ToString() == "По возрастанию" ? "ASC" : "DESC";
            string orderByClause = $"ORDER BY {selectedColumns} {orderBy}";

            // Формирование запроса
            string sql = $"SELECT {selectedColumns} FROM avialines.{tableName}";

            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += " WHERE " + whereClause;
            }

            // Добавление порядка сортировки
            sql += " " + orderByClause;

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
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOrder = comboBox2.SelectedItem.ToString();
        }
    }
}