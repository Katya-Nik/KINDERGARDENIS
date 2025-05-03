using KINDERGARDENIS.DBModel;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class EmployeesWindow : Form
    {
        public EmployeesWindow()
        {
            InitializeComponent();
            Load += EmployeesWindow_Load;
        }

        private void EmployeesWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadEducatorsMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9,
            label1, label2, label3, label4, label5,
            label6, label7, label8, label9);
            ConfigureDataGridView();
            LoadEmployeesData();
        }

        private void ConfigureDataGridView()
        {
            // Настройка внешнего вида DataGridView
            dataGridViewEmployees.BackgroundColor = Color.FromArgb(238, 245, 245);
            dataGridViewEmployees.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewEmployees.GridColor = Color.FromArgb(25, 25, 25);
            dataGridViewEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewEmployees.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewEmployees.DefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);
            dataGridViewEmployees.RowHeadersVisible = false;
            dataGridViewEmployees.AllowUserToAddRows = false;
            dataGridViewEmployees.AllowUserToDeleteRows = false;
            dataGridViewEmployees.AllowUserToResizeRows = false;
            dataGridViewEmployees.ReadOnly = true;

            // Настройка шрифтов и цветов
            var headerStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.FromArgb(238, 245, 245),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            var cellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 12f),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.FromArgb(238, 245, 245),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dataGridViewEmployees.ColumnHeadersDefaultCellStyle = headerStyle;
            dataGridViewEmployees.DefaultCellStyle = cellStyle;
            dataGridViewEmployees.EnableHeadersVisualStyles = false;
        }

        private void LoadEmployeesData(string filter = "")
        {
            try
            {
                using (var context = Helper.DB)
                {
                    var query = from emp in context.Employees
                                join user in context.User on emp.EmployeesUserID equals user.UserID
                                join role in context.Role on user.RoleID equals role.RoleID
                                where string.IsNullOrEmpty(filter) || emp.EmployeesPatronymic.Contains(filter)
                                orderby emp.EmployeesSurname // Сортировка по фамилии
                                select new
                                {
                                    Фамилия = emp.EmployeesSurname,
                                    Имя = emp.EmployeesName,
                                    Отчество = emp.EmployeesPatronymic,
                                    Должность = role.RoleName,
                                    ДатаРождения = emp.EmployeesDateofBirth,
                                    Телефон = emp.EmployeesPhoneNumber,
                                    Email = emp.EmployeesEmail,
                                    Логин = user.UserLogin
                                };

                    dataGridViewEmployees.DataSource = query.ToList();

                    // Форматирование колонки с датой
                    if (dataGridViewEmployees.Columns.Contains("ДатаРождения"))
                    {
                        dataGridViewEmployees.Columns["ДатаРождения"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }

                    // Переименование колонок для корректного отображения
                    dataGridViewEmployees.Columns["ДатаРождения"].HeaderText = "Дата рождения";
                    dataGridViewEmployees.Columns["Логин"].HeaderText = "Логин";
                }
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь должна быть обработка ошибок
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            UIForms.MainWindow mainWindow = new UIForms.MainWindow();
            mainWindow.Hide();
            this.Close();
            UIForms.ScheduleWindow scheduleWindow = new UIForms.ScheduleWindow();
            scheduleWindow.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxSearchPatronymic_TextChanged(object sender, EventArgs e)
        {
            LoadEmployeesData(textBoxSearchPatronymic.Text);
        }

        private void buttonInfoEmp_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployees.SelectedRows.Count > 0)
            {
                // Получаем логин пользователя из последней колонки
                string userLogin = dataGridViewEmployees.CurrentRow.Cells["Логин"].Value.ToString();

                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    // Находим сотрудника по логину пользователя
                    var emp = db.Employees.FirstOrDefault(em => em.User.UserLogin == userLogin);
                    if (emp != null)
                    {
                        UIForms.MoreInfoEmp moreInfoEmp = new UIForms.MoreInfoEmp(emp);
                        moreInfoEmp.ShowDialog();
                        this.Show();
                    }
                }
            }
        }
    }
}