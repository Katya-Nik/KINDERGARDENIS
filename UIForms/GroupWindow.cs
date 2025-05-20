using KINDERGARDENIS.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace KINDERGARDENIS.UIForms
{
    public partial class GroupWindow : Form
    {
        //Глобальные объекты Excel
        Excel.Application excelApp;     //Сервер Excel
        Excel.Workbook excelBook;       //Книга (документ)
        Excel.Worksheet excelSheet;     //Один лист (закладка)
        Excel.Range excelCells;			//Ячейки


        public GroupWindow()
        {
            InitializeComponent();
            this.FormClosing += GroupWindow_FormClosing;
            LoadGroupsData();
            ConfigureDataGridView();
        }

        private void LoadGroupsData()
        {
            // Создаем новый контекст для этой операции
            using (var db = new DBModel.KindergartenInformationSystemEntities()) // Замените YourDbContextClass на реальный тип вашего контекста
            {
                var query = from groupItem in db.Groups
                            join educator in db.Educators on groupItem.GroupsID equals educator.EducatorsGroupsID into educators
                            from educator in educators.DefaultIfEmpty()
                            join employee in db.Employees on educator.EducatorsEmployeesID equals employee.EmployeesID into employees
                            from employee in employees.DefaultIfEmpty()
                            join modeType in db.ModeType on groupItem.GroupsID equals modeType.ModTypeGroupID into modeTypes
                            from modeType in modeTypes.DefaultIfEmpty()
                            let childrenCount = db.Children.Count(c => c.ChildrenGroupsID == groupItem.GroupsID)
                            select new
                            {
                                Название = groupItem.GroupsGroupName,
                                Номер = groupItem.GroupsGroupNumber,
                                Тип = groupItem.GroupsGroupType,
                                Режим = modeType != null ? modeType.ModeTypeIDName : null,
                                Количество = childrenCount,
                                Воспитатель = employee != null ? employee.EmployeesSurname : null,
                                МладшийВоспитатель = (string)null
                            };

                if (!string.IsNullOrEmpty(textBoxSearchGroupName.Text))
                {
                    query = query.Where(g => g.Название.Contains(textBoxSearchGroupName.Text));
                }

                query = query.OrderBy(g => g.Название);

                dataGridViewGroups.DataSource = query.ToList();
            }
        }

        private void ConfigureDataGridView()
        {
            // Общий цвет фона таблицы
            Color backgroundColor = Color.FromArgb(238, 245, 245);
            Color headerColor = Color.FromArgb(238, 245, 245); // Отдельный цвет для заголовков

            // Настройка стиля заголовков
            dataGridViewGroups.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            { 
                Font = new Font("Verdana", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = headerColor, // Фон заголовков
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = headerColor, // Важно: цвет при "выделении" заголовка
                SelectionForeColor = Color.FromArgb(39, 39, 39) // Цвет текста при "выделении"
            };

            // Настройка стиля ячеек
            dataGridViewGroups.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = backgroundColor,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Настройка стиля выделенных строк (только для строк данных)
            dataGridViewGroups.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewGroups.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);

            // Настройка цвета сетки
            dataGridViewGroups.GridColor = Color.FromArgb(39, 39, 39);

            // Настройка внешнего вида
            dataGridViewGroups.BackgroundColor = backgroundColor;
            dataGridViewGroups.BorderStyle = BorderStyle.None;
            dataGridViewGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewGroups.EnableHeadersVisualStyles = false;
            dataGridViewGroups.RowHeadersVisible = false;
            dataGridViewGroups.AllowUserToAddRows = false;
            dataGridViewGroups.AllowUserToDeleteRows = false;
            dataGridViewGroups.ReadOnly = true;
            dataGridViewGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Дополнительные настройки
            dataGridViewGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewGroups.ColumnHeadersHeight = 30;
            dataGridViewGroups.RowTemplate.Height = 25;

            // Важно: отключаем выделение заголовков
            dataGridViewGroups.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        private void textBoxSearchGroupName_TextChanged(object sender, EventArgs e)
        {
            LoadGroupsData();
        }

        private void GroupWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadGroupMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9,
            label1, label2, label3, label4, label5,
            label6, label7, label8, label9);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new MainWindow(), this);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ScheduleWindow(), this);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new EmployeesWindow(), this);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Эта форма
        }

        private void label5_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ChildrenWindow(), this);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ParentsWindow(), this);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new UsersWindow(), this);
        }

        private void GroupWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // Если окно закрывается пользователем
            {
                Authorization auth = new Authorization();
                auth.Show(); // Открываем Authorization
            }
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            InfoUser infoUser = new InfoUser();
            infoUser.ShowDialog();
            this.Show();
        }

        private void labelExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Инициализация Excel
                excelApp = new Excel.Application();
                excelApp.Visible = false;

                // Путь к шаблону
                string pathTemplates = Environment.CurrentDirectory + @"\Templates\ExcelGroupTemplate.xlsx";

                if (!File.Exists(pathTemplates))
                {
                    MessageBox.Show("Шаблон ExcelGroupTemplate.xlsx не найден!");
                    return;
                }

                // Открытие шаблона
                excelBook = excelApp.Workbooks.Open(pathTemplates);
                excelSheet = (Excel.Worksheet)excelBook.Worksheets[1];

                // Заполнение данных на первом листе (A5:F5)
                excelSheet.Cells[5, 1] = "Название";
                excelSheet.Cells[5, 2] = "Воспитатель";
                excelSheet.Cells[5, 3] = "Младший воспитатель";
                excelSheet.Cells[5, 4] = "Количество детей";
                excelSheet.Cells[5, 5] = "Количество мальчиков";
                excelSheet.Cells[5, 6] = "Количество девочек";

                // Установка границ
                Excel.Range range = excelSheet.Range["A5:F5"];
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                // Получение данных из базы
                using (var db = new KindergartenInformationSystemEntities())
                {
                    // Исправленный запрос с явной загрузкой связанных данных
                    var groups = db.Groups
                                .Include("Children") // Используем строку для указания навигационного свойства
                                .ToList();

                    foreach (var group in groups)
                    {
                        // Создаем новый лист
                        Excel.Worksheet groupSheet = (Excel.Worksheet)excelBook.Worksheets.Add();
                        groupSheet.Name = group.GroupsGroupName;

                        // Заголовок листа
                        Excel.Range headerRange = groupSheet.Range["A1:G1"];
                        headerRange.Merge();
                        headerRange.Value = group.GroupsGroupName;
                        headerRange.Font.Name = "Verdana";
                        headerRange.Font.Size = 13;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        // Заголовки таблицы
                        groupSheet.Cells[2, 1] = "ФИО ребенка";
                        groupSheet.Cells[2, 2] = "Дата рождения";
                        groupSheet.Cells[2, 3] = "Пол";
                        groupSheet.Cells[2, 4] = "СНИЛС";
                        groupSheet.Cells[2, 5] = "ФИО родителя";
                        groupSheet.Cells[2, 6] = "Телефон родителя";
                        groupSheet.Cells[2, 7] = "Email родителя";

                        // Форматирование заголовков
                        Excel.Range headersRange = groupSheet.Range["A2:G2"];
                        headersRange.Font.Name = "Verdana";
                        headersRange.Font.Size = 11;
                        headersRange.Font.Bold = true;
                        headersRange.RowHeight = 28.5;

                        // Получаем детей для текущей группы
                        var children = db.Children
                                      .Include("Parents") // Явно загружаем данные родителей
                                      .Where(c => c.ChildrenGroupsID == group.GroupsID)
                                      .ToList();

                        // Заполнение данными детей
                        int row = 3;
                        foreach (var child in children)
                        {
                            groupSheet.Cells[row, 1] = $"{child.ChildrenSurname} {child.ChildrenName} {child.ChildrenPatronymic}";
                            groupSheet.Cells[row, 2] = child.ChildrenDateofBirth?.ToShortDateString();
                            groupSheet.Cells[row, 3] = child.ChildrenGender;
                            groupSheet.Cells[row, 4] = child.ChildrenSNILS;
                            groupSheet.Cells[row, 5] = $"{child.Parents.ParentsSurname} {child.Parents.ParentsName} {child.Parents.ParentsPatronymic}";
                            groupSheet.Cells[row, 6] = child.Parents.ParentsPhoneNumber;
                            groupSheet.Cells[row, 7] = child.Parents.ParentsEmail;

                            // Форматирование данных
                            Excel.Range dataRange = groupSheet.Range[$"A{row}:G{row}"];
                            dataRange.Font.Name = "Verdana";
                            dataRange.Font.Size = 10;
                            dataRange.RowHeight = 15.5;

                            row++;
                        }

                        // Установка границ
                        Excel.Range allDataRange = groupSheet.Range[$"A2:G{row - 1}"];
                        allDataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        // Автоподбор ширины столбцов
                        groupSheet.Columns.AutoFit();
                    }
                }

                // Сохранение
                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
                string fileName = $"GroupsReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string fullPath = Path.Combine(downloadsPath, fileName);

                excelBook.SaveAs(fullPath);
                excelBook.Close();
                excelApp.Quit();


                MessageBox.Show($"Отчет успешно сохранен: {fullPath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (excelBook != null)
                {
                    excelBook.Close(false);
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelSheet);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelBook);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}