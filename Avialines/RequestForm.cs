using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace Avialines
{
    public partial class RequestForm : BaseForm
    {
        private MySqlConnection con;
        private DataTable dt;
        private double totalProfit; 

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
            // Настраиваем DateTimePicker для выбора только года
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy";
            dateTimePicker1.ShowUpDown = true; // Убирает выпадающий календарь
        }

        private DataTable ExecuteQueryAndCalculateProfit(int selectedYear, out double totalProfit)
        {
            string connString = "server=localhost;port=3306;username=root;password=admin;database=avialines";

            DataTable dt = new DataTable();
            totalProfit = 0;

            try
            {
                using (var con = new MySqlConnection(connString))
                {
                    con.Open();
                    string sql = @"
                    SELECT 
                        flight.flight_id AS 'Номер рейса',
                        COUNT(ticket.ticket_id) AS 'Число пассажиров рейса',
                        SUM(rout.price) AS 'Прибыль'
                    FROM Flight flight
                    JOIN Ticket ticket ON flight.flight_id = ticket.flight_id
                    JOIN Rout rout ON ticket.rout_id = rout.rout_id
                    WHERE YEAR(flight.datetime_flight) = @SelectedYear
                    GROUP BY flight.flight_id
                    ORDER BY flight.flight_id";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@SelectedYear", selectedYear);
                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                
                foreach (DataRow row in dt.Rows) // Подсчитываем общую прибыль
                {
                    totalProfit += Convert.ToDouble(row["Прибыль"]);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
            }
            return dt;
        }

        private DataTable ExecuteRouteData(out double totalRouteProfit)
        {
            string connString = "server=localhost;port=3306;username=root;password=admin;database=avialines";
            DataTable dt = new DataTable();
            totalRouteProfit = 0;

            try
            {
                using (var con = new MySqlConnection(connString))
                {
                    con.Open();
                    string sql = @"
                SELECT 
                    rout.rout_id AS 'Номер маршрута',
                    rout.price AS 'Цена билета',
                    (rout.price * COUNT(ticket.ticket_id)) AS 'Прибыль по маршруту'
                FROM Ticket ticket
                JOIN Rout rout ON ticket.rout_id = rout.rout_id
                JOIN Flight flight ON ticket.flight_id = flight.flight_id
                WHERE flight.cancel = 'no'
                GROUP BY rout.rout_id, rout.price
                ORDER BY rout.rout_id";

                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    totalRouteProfit += Convert.ToDouble(row["Прибыль по маршруту"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
            }

            return dt;
        }


        private DataTable ExecuteWord(int selectedYear, out double totalProfit)
        {
            string connString = "server=localhost;port=3306;username=root;password=admin;database=avialines";

            DataTable dt = new DataTable();
            totalProfit = 0;

            try
            {
                using (var con = new MySqlConnection(connString))
                {
                    con.Open();
                    string sql = @"
            SELECT 
                flight.flight_id AS 'Номер рейса',
                COUNT(ticket.ticket_id) AS 'Число пассажиров рейса',
                (rout.price * COUNT(ticket.ticket_id)) AS 'Прибыль от полета',
                rout.rout_id AS 'Номер маршрута',
                rout.price AS 'Цена билета'
            FROM Flight flight
            JOIN Ticket ticket ON flight.flight_id = ticket.flight_id
            JOIN Rout rout ON ticket.rout_id = rout.rout_id
            WHERE flight.cancel = 'no'  -- Учитываем только неотмененные рейсы
                AND YEAR(flight.datetime_flight) = @SelectedYear
            GROUP BY flight.flight_id, rout.rout_id, rout.price
            ORDER BY flight.flight_id, rout.rout_id";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@SelectedYear", selectedYear);
                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                foreach (DataRow row in dt.Rows) // Подсчитываем общую прибыль
                {
                    totalProfit += Convert.ToDouble(row["Прибыль от полета"]);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
            }
            return dt;
        }


        private void ButtonExecuteQuery_Click(object sender, EventArgs e)
        {
            int selectedYear = dateTimePicker1.Value.Year; // Получаем выбранный год
            double totalProfit;

            DataTable dt = ExecuteWord(selectedYear, out totalProfit);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для выбранного года.");
                return;
            }


            try
            {
                // Создаем Word приложение
                Type wordType = Type.GetTypeFromProgID("Word.Application");
                dynamic wordApp = Activator.CreateInstance(wordType);

                wordApp.Visible = true;
                dynamic doc = wordApp.Documents.Add();

                // Заголовок документа
                dynamic titleParagraph = doc.Content.Paragraphs.Add();
                titleParagraph.Range.Text = $"Прибыль от маршрутов авиакомпании 'Полет' за {selectedYear} год";
                titleParagraph.Range.Font.Name = "Times New Roman";
                titleParagraph.Range.Font.Size = 16;
                titleParagraph.Range.Font.Bold = 1;
                titleParagraph.Alignment = 1; // Центрирование текста
                titleParagraph.Range.InsertParagraphAfter();

                // Создаем таблицу для рейсов
                dynamic dataParagraph = doc.Content.Paragraphs.Add();
                dynamic flightTable = doc.Tables.Add(dataParagraph.Range, dt.Rows.Count + 1, 3);
                flightTable.Borders.Enable = 1;

                // Заполняем заголовки таблицы (жирные)
                string[] columnNames = new string[] { "Номер рейса", "Число пассажиров рейса", "Прибыль от полета" };
                for (int i = 0; i < columnNames.Length; i++)
                {
                    flightTable.Cell(1, i + 1).Range.Text = columnNames[i];
                    flightTable.Cell(1, i + 1).Range.Font.Name = "Times New Roman";
                    flightTable.Cell(1, i + 1).Range.Font.Bold = 0; // Заголовки таблицы жирные
                    flightTable.Cell(1, i + 1).Range.ParagraphFormat.Alignment = 1; // Выравнивание по центру
                }

                // Заполняем данные рейсов (обычный текст, не жирный)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < 3; j++)  // Заполняем три столбца
                    {
                        flightTable.Cell(i + 2, j + 1).Range.Text = dt.Rows[i][j].ToString();
                        flightTable.Cell(i + 2, j + 1).Range.Font.Name = "Times New Roman";
                        flightTable.Cell(i + 2, j + 1).Range.Font.Bold = 0; // Делаем текст обычным
                    }
                }

                // Добавляем информацию о маршрутах
                var groupedRoutes = dt.AsEnumerable()
                    .GroupBy(row => row["Номер маршрута"].ToString())
                    .Select(g => new
                    {
                        RouteId = g.Key,
                        TicketPrice = g.First()["Цена билета"],
                        TotalProfit = g.Sum(row => Convert.ToDouble(row["Прибыль от полета"]))
                    });

                foreach (var route in groupedRoutes)
                {
                    dynamic routeParagraph = doc.Content.Paragraphs.Add();
                    routeParagraph.Range.Text = $"Номер маршрута: {route.RouteId}, Цена билета: {route.TicketPrice} руб.";
                    routeParagraph.Range.Font.Name = "Times New Roman";
                    routeParagraph.Range.Font.Size = 14;
                    routeParagraph.Range.Font.Bold = 0; // Не жирный
                    routeParagraph.Range.Font.Italic = 1; // Курсив
                    routeParagraph.Alignment = 0; // Выровнять по левому краю
                    routeParagraph.Range.InsertParagraphAfter();

                    dynamic profitParagraph = doc.Content.Paragraphs.Add();
                    profitParagraph.Range.Text = $"Итого по маршруту: {route.TotalProfit:F2} руб.";
                    profitParagraph.Range.Font.Name = "Times New Roman";
                    profitParagraph.Range.Font.Size = 14;
                    routeParagraph.Range.Font.Bold = 0; // Не жирный
                    routeParagraph.Range.Font.Italic = 1; // Курсив
                    profitParagraph.Alignment = 0; // Выровнять по левому краю
                    profitParagraph.Range.InsertParagraphAfter();
                }
                // Добавляем линию разделения из точек
                dynamic separatorParagraph = doc.Content.Paragraphs.Add();
                separatorParagraph.Range.Text = new string('.', 130); // Строка из 50 точек
                separatorParagraph.Range.Font.Name = "Times New Roman";
                separatorParagraph.Range.InsertParagraphAfter();

                // Добавляем общий итог
                dynamic totalProfitParagraph = doc.Content.Paragraphs.Add();
                totalProfitParagraph.Range.Text = $"Общая прибыль за {selectedYear}: {totalProfit:F2} руб.";
                totalProfitParagraph.Range.Font.Name = "Times New Roman";
                totalProfitParagraph.Range.Font.Size = 16;
                totalProfitParagraph.Range.Font.Bold = 1; // Не жирный
                totalProfitParagraph.Range.Font.Italic = 0; // Курсив
                totalProfitParagraph.Alignment = 0; // Выровнять по левому краю
                totalProfitParagraph.Range.InsertParagraphAfter();

                // Сохраняем документ
                string filePath = $@"D:\Рабочий стол\Отчет_{selectedYear}.docx";
                doc.SaveAs2(filePath);
                doc.Close();
                wordApp.Quit();

                MessageBox.Show($"Отчет успешно сохранен: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Word: {ex.Message}");
            }
        }
        // Метод для добавления итогов по маршруту
        private void AddRouteSummary(dynamic doc, string routeId, double routeProfit)
        {
            dynamic routeParagraph = doc.Content.Paragraphs.Add();
            routeParagraph.Range.Text = $"Номер маршрута: {routeId}";
            routeParagraph.Range.Font.Name = "Times New Roman";
            routeParagraph.Range.Font.Size = 14;
            routeParagraph.Alignment = 0; // Выровнять по левому краю
            routeParagraph.Range.InsertParagraphAfter();

            routeParagraph = doc.Content.Paragraphs.Add();
            routeParagraph.Range.Text = $"Итого по маршруту: {routeProfit:F2} руб.";
            routeParagraph.Range.Font.Name = "Times New Roman";
            routeParagraph.Range.Font.Size = 14;
            routeParagraph.Alignment = 0; // Выровнять по левому краю
            routeParagraph.Range.InsertParagraphAfter();
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

        private void CustomizeDataGridView(DataGridView gridView)
        {
            // Увеличиваем шрифт для содержимого ячеек
            gridView.DefaultCellStyle.Font = new Font("Times New Roman", 12);

            // Настраиваем шрифт и стиль для заголовков столбцов
            gridView.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Italic);
            gridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Выравниваем содержимое ячеек по центру
            gridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Устанавливаем стиль границ ячеек
            gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            gridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Настраиваем автоматическое изменение ширины столбцов
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            int selectedYear = dateTimePicker1.Value.Year; // Получаем выбранный год

            // Получаем данные по рейсам
            DataTable dtFlights = ExecuteQueryAndCalculateProfit(selectedYear, out double totalFlightProfit);

            // Получаем данные по маршрутам
            DataTable dtRoutes = ExecuteRouteData(out double totalRouteProfit);

            // Общая прибыль
            double totalProfit = totalFlightProfit;
            label1.Text = $"Общая прибыль: {totalProfit:F2} ₽";

            // Обработка данных по рейсам
            if (dtFlights.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных о рейсах для выбранного года.");
                dataGridView1.DataSource = null; // Очищаем таблицу рейсов
            }
            else
            {
                dataGridView1.DataSource = dtFlights;
                CustomizeDataGridView(dataGridView1);
            }

            // Обработка данных по маршрутам
            if (dtRoutes.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных о маршрутах.");
                dataGridView2.DataSource = null; // Очищаем таблицу маршрутов
            }
            else
            {
                dataGridView2.DataSource = dtRoutes;
                CustomizeDataGridView(dataGridView2);
            }
        }
    }
}