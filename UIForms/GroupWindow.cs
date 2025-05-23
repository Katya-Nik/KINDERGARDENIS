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
using System.Data.Entity;
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
            using (var db = new DBModel.KindergartenInformationSystemEntities())
            {
                var query = from groupItem in db.Groups
                            let educator = db.Educators
                                .Where(e => e.EducatorsGroupsID == groupItem.GroupsID &&
                                       e.Employees.User.Role.RoleID == 7)
                                .Select(e => e.Employees.EmployeesSurname)
                                .FirstOrDefault()
                            let juniorEducator = db.Educators
                                .Where(e => e.EducatorsGroupsID == groupItem.GroupsID &&
                                        e.Employees.User.Role.RoleID == 8)
                                .Select(e => e.Employees.EmployeesSurname)
                                .FirstOrDefault()
                            let modeType = db.ModeType
                                .Where(m => m.ModTypeGroupID == groupItem.GroupsID)
                                .Select(m => m.ModeTypeIDName)
                                .FirstOrDefault()
                            let childrenCount = db.Children.Count(c => c.ChildrenGroupsID == groupItem.GroupsID)
                            select new
                            {
                                Название = groupItem.GroupsGroupName,
                                Номер = groupItem.GroupsGroupNumber,
                                Тип = groupItem.GroupsGroupType,
                                Режим = modeType,
                                Количество = childrenCount,
                                Воспитатель = educator,
                                МладшийВоспитатель = juniorEducator
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
            Excel.Application excelApp = null;
            Excel.Workbook excelBook = null;
            Excel.Worksheet excelSheet = null;

            try
            {
                excelApp = new Excel.Application { Visible = false };
                string templatePath = Path.Combine(Environment.CurrentDirectory, @"Templates\ExcelGroupTemplate.xls");

                if (!File.Exists(templatePath))
                {
                    MessageBox.Show("Шаблон не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                excelBook = excelApp.Workbooks.Open(templatePath);
                excelSheet = (Excel.Worksheet)excelBook.Worksheets[1];

                // Создаем новый контекст базы данных
                using (var db = new KindergartenInformationSystemEntities())
                {
                    // Получаем все данные о группах, воспитателях и детях одним запросом
                    var groupsData = db.Groups
                        .Select(g => new
                        {
                            GroupName = g.GroupsGroupName,
                            GroupNumber = g.GroupsGroupNumber,
                            GroupType = g.GroupsGroupType,
                            Educator = g.Educators
                                .Where(ed => ed.Employees.User.Role.RoleID == 7)
                                .Select(ed => ed.Employees.EmployeesSurname)
                                .FirstOrDefault(),
                            JuniorEducator = g.Educators
                                .Where(ed => ed.Employees.User.Role.RoleID == 8)
                                .Select(ed => ed.Employees.EmployeesSurname)
                                .FirstOrDefault(),
                            ChildrenCount = g.Children.Count,
                            BoysCount = g.Children.Count(c => c.ChildrenGender == "мужской"),
                            GirlsCount = g.Children.Count(c => c.ChildrenGender == "женский"),
                            ModeType = g.ModeType.FirstOrDefault().ModeTypeIDName
                        })
                        .OrderBy(g => g.GroupName)
                        .ToList();

                    // Заполняем Excel
                    int startRow = 5;
                    foreach (var group in groupsData)
                    {
                        excelSheet.Cells[startRow, 1] = group.GroupName;
                        excelSheet.Cells[startRow, 2] = group.Educator;
                        excelSheet.Cells[startRow, 3] = group.JuniorEducator;
                        excelSheet.Cells[startRow, 4] = group.ChildrenCount;
                        excelSheet.Cells[startRow, 5] = group.BoysCount;
                        excelSheet.Cells[startRow, 6] = group.GirlsCount;
                        startRow++;
                        Excel.Range dataExcel = excelSheet.Range[$"A{startRow}:$F{startRow - 1}"];
                        dataExcel.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        dataExcel.Borders.Weight = Excel.XlBorderWeight.xlThin;
                    }

                    // Создаем листы для каждой группы с детьми
                    var groupsWithChildren = db.Groups
                        .Include(g => g.Children.Select(c => c.Parents))
                        .OrderBy(g => g.GroupsGroupName)
                        .ToList();

                    foreach (var group in groupsWithChildren)
                    {
                        // Создаем новый лист для группы
                        Excel.Worksheet groupSheet = (Excel.Worksheet)excelBook.Worksheets.Add();
                        groupSheet.Name = group.GroupsGroupName.Length > 31 ?
                            group.GroupsGroupName.Substring(0, 31) : group.GroupsGroupName;

                        // Настройка стилей и заголовков
                        // 1) Объединяем первую строку и вставляем название группы
                        Excel.Range headerRange = groupSheet.Range["A1:E1"];
                        headerRange.Merge();
                        headerRange.Value = group.GroupsGroupName;
                        headerRange.Font.Name = "Verdana";
                        headerRange.Font.Size = 13;
                        headerRange.Font.Bold = true;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        groupSheet.Rows[1].RowHeight = 35;

                        // 2) Заголовки столбцов
                        groupSheet.Cells[2, 1] = "ФИО ребенка";
                        groupSheet.Cells[2, 2] = "Дата рождения";
                        groupSheet.Cells[2, 3] = "ФИО родителя";
                        groupSheet.Cells[2, 4] = "Телефон родителя";
                        groupSheet.Cells[2, 5] = "Email родителя";

                        // Настройка стиля заголовков (2 строка)
                        Excel.Range headerRow = groupSheet.Range["A2:E2"];
                        headerRow.Font.Name = "Verdana";
                        headerRow.Font.Size = 11;
                        headerRow.Font.Bold = true;
                        headerRow.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        headerRow.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        groupSheet.Rows[2].RowHeight = 28.5;

                        // Заполняем данными о детях
                        int childRow = 3;
                        foreach (var child in group.Children.OrderBy(c => c.ChildrenSurname))
                        {
                            groupSheet.Cells[childRow, 1] = $"{child.ChildrenSurname} {child.ChildrenName} {child.ChildrenPatronymic}";
                            groupSheet.Cells[childRow, 2] = child.ChildrenDateofBirth?.ToString("dd.MM.yyyy");
                            groupSheet.Cells[childRow, 3] = $"{child.Parents.ParentsSurname} {child.Parents.ParentsName} {child.Parents.ParentsPatronymic}";
                            groupSheet.Cells[childRow, 4] = child.Parents.ParentsPhoneNumber;
                            groupSheet.Cells[childRow, 5] = child.Parents.ParentsEmail;

                            // Настройка стиля данных (начиная с 3 строки)
                            Excel.Range dataRow = groupSheet.Range[$"A{childRow}:E{childRow}"];
                            dataRow.Font.Name = "Verdana";
                            dataRow.Font.Size = 10;
                            groupSheet.Rows[childRow].RowHeight = 15.5;

                            childRow++;
                        }

                        // Устанавливаем границы для заполненных ячеек (начиная со 2 строки)
                        Excel.Range dataRange = groupSheet.Range[$"A2:E{childRow - 1}"];
                        dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                        // Автоподбор ширины столбцов
                        Excel.Range usedRange = groupSheet.UsedRange;
                        usedRange.Columns.AutoFit();
                    }

                    // Удаляем лишние пустые листы (если они есть)
                    // Сохраняем первый лист и листы с группами
                    int sheetsToKeep = groupsWithChildren.Count + 1; // +1 для основного листа
                    while (excelBook.Worksheets.Count > sheetsToKeep)
                    {
                        Excel.Worksheet lastSheet = (Excel.Worksheet)excelBook.Worksheets[excelBook.Worksheets.Count];
                        lastSheet.Delete();
                    }
                }

                // Активируем первый лист
                ((Excel.Worksheet)excelBook.Worksheets[1]).Activate();

                // Сохранение в "Загрузки"
                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
                Directory.CreateDirectory(downloadsPath);
                string fileName = $"Группы_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string fullPath = Path.Combine(downloadsPath, fileName);

                excelApp.DisplayAlerts = false;
                excelBook.SaveAs(fullPath, Excel.XlFileFormat.xlOpenXMLWorkbook);

                MessageBox.Show($"Файл сохранён: {fullPath}", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Runtime.InteropServices.COMException ex) when (ex.ErrorCode == -2146827286)
            {
                MessageBox.Show("Ошибка: файл занят или недоступен. Закройте Excel и попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}