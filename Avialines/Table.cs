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
    public partial class Table : BaseForm
    {
        private MySqlConnection con;
        private string buttonClicked;
        private string tableName;  // Имя таблицы для SQL-запроса
        private DataTable dt;
        private bool isEditing = false;  // Флаг, чтобы отслеживать режим редактирования


        public Table(string buttonName)
        {
            InitializeComponent();
            buttonClicked = buttonName;
            SetTableName();  // Устанавливаем имя таблицы
            LoadData();

            // Скрываем кнопки "Сохранить" и "Отменить" при загрузке формы
            buttonSave.Visible = false;
            buttonCancel.Visible = false;
        }

        private void SetTableName()
        {
            // Присваиваем tableName в зависимости от buttonClicked
            switch (buttonClicked)
            {
                case "Button1":
                    tableName = "avialines.airplane";
                    break;
                case "Button2":
                    tableName = "avialines.airport";
                    break;
                case "Button3":
                    tableName = "avialines.flight";
                    break;
                case "Button4":
                    tableName = "avialines.model";
                    break;
                case "Button5":
                    tableName = "avialines.passanger";
                    break;
                case "Button6":
                    tableName = "avialines.pilot";
                    break;
                case "Button7":
                    tableName = "avialines.rout";
                    break;
                case "Button8":
                    tableName = "avialines.ticket";
                    break;
                default:
                    MessageBox.Show("Неизвестная таблица.");
                    break;
            }
        }
        private void LoadData()
        {
            //подключение бд
            string connstring = "server=localhost;port=3306;username=root;password=admin;database=avialines";
            con = new MySqlConnection(connstring);

            try
            {
                con.Open();
                if (string.IsNullOrEmpty(tableName))
                {
                    MessageBox.Show("Имя таблицы не определено.");
                    return;
                }
                string sql = "SELECT * FROM " + tableName;  // Используем tableName
                MySqlDataAdapter da = new MySqlDataAdapter(sql, con);

                dt = new DataTable();  // Инициализируем DataTable перед заполнением
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Данные не найдены в таблице: " + tableName);
                }

                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
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

            // Скрываем кнопки "Сохранить" и "Отменить" при загрузке формы
            buttonSave.Visible = false;
            buttonCancel.Visible = false;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            isEditing = true;  // Включаем режим редактирования
            dataGridView1.ReadOnly = false;  // Разрешаем редактирование ячеек
            buttonSave.Visible = true;  // Отображаем кнопку "Сохранить"
            buttonCancel.Visible = true;  // Отображаем кнопку "Отменить"
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                try
                {
                    // Создаем адаптер с использованием tableName вместо buttonClicked
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM " + tableName, con);

                    // Создаем командный билдер
                    MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(da);

                    // Сохраняем изменения
                    da.Update(dt);

                    MessageBox.Show("Изменения сохранены!");
                    dataGridView1.ReadOnly = true;  // Закрываем возможность редактирования
                    buttonSave.Visible = false;  // Скрываем кнопку "Сохранить"
                    buttonCancel.Visible = false;  // Скрываем кнопку "Отменить"
                    isEditing = false;  // Выходим из режима редактирования
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Ошибка сохранения данных: " + ex.Message);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                dt.RejectChanges();  // Отменяем изменения в DataTable
                dataGridView1.ReadOnly = true;  // Закрываем возможность редактирования
                buttonSave.Visible = false;  // Скрываем кнопку "Сохранить"
                buttonCancel.Visible = false;  // Скрываем кнопку "Отменить"
                isEditing = false;  // Выходим из режима редактирования
                MessageBox.Show("Изменения отменены.");
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int nextId = 1; // Начальное значение для порядкового номера

            if (dt.Rows.Count > 0)
            {
                nextId = dt.AsEnumerable().Max(row => row.Field<int>(0)) + 1; // Получаем максимальный порядковый номер
            }

            DataRow newRow = dt.NewRow();
            newRow[0] = nextId; // Устанавливаем порядковый номер

            for (int i = 1; i < dt.Columns.Count; i++)
            {
                DataColumn column = dt.Columns[i];

                // Отладка: Проверьте, выводится ли имя столбца
                MessageBox.Show($"Имя столбца: {column.ColumnName}"); // Временно выводим имя столбца

                string inputValue = Prompt.ShowDialog($"Введите значение для '{column.ColumnName}':", "Добавить запись");

                // Обработка типа данных
                if (column.DataType == typeof(int))
                {
                    if (int.TryParse(inputValue, out int intValue))
                    {
                        newRow[column.ColumnName] = intValue;
                    }
                    else
                    {
                        MessageBox.Show($"Некорректное значение для '{column.ColumnName}'. Ожидалось число.");
                        return;
                    }
                }
                else if (column.DataType == typeof(DateTime))
                {
                    if (DateTime.TryParse(inputValue, out DateTime dateValue))
                    {
                        newRow[column.ColumnName] = dateValue;
                    }
                    else
                    {
                        MessageBox.Show($"Некорректное значение для '{column.ColumnName}'. Ожидалась дата в формате 'дд.мм.гггг' или 'мм/дд/гггг'.");
                        return;
                    }
                }
                else
                {
                    newRow[column.ColumnName] = inputValue; // Для остальных типов данных
                }
            }

            dt.Rows.Add(newRow);

            // Обновляем базу данных
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM " + tableName, con);
                MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(da);
                da.Update(dt);

                MessageBox.Show("Запись добавлена!");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении записи: " + ex.Message);
            }
        }



        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить выделенные строки?",
                                                     "Подтверждение удаления",
                                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    List<DataRow> rowsToDelete = new List<DataRow>();

                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            rowsToDelete.Add(((DataRowView)row.DataBoundItem).Row);
                        }
                    }

                    // Устанавливаем состояние строк как удаленные
                    foreach (DataRow row in rowsToDelete)
                    {
                        row.Delete(); // Устанавливаем состояние строки как удаленное
                    }

                    // Обновляем базу данных
                    try
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM " + tableName, con))
                        {
                            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(da);
                            int rowsAffected = da.Update(dt); // Обновляем базу данных
                            MessageBox.Show($"{rowsAffected} строк(и) удалены из базы данных.");
                        }

                        // Обновляем DataTable из базы данных
                        dt.Clear(); // Очищаем текущий DataTable
                        using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM " + tableName, con))
                        {
                            da.Fill(dt); // Заполняем DataTable новыми данными из базы данных
                        }

                        dataGridView1.DataSource = dt; // Привязываем обновленный DataTable
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Ошибка при удалении записей: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы одну строку для удаления.");
            }
        }

    }
}
