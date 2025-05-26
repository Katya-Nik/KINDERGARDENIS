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
        private KindergartenInformationSystemEntities _dbContext;

        public EmployeesWindow()
        {
            InitializeComponent();
            this.FormClosing += EmployeesWindow_FormClosing;
            Load += EmployeesWindow_Load;
            textBoxSearchPatronymic.TextChanged += textBoxSearchPatronymic_TextChanged;
        }

        private void EmployeesWindow_Load(object sender, EventArgs e)
        {
            _dbContext = new KindergartenInformationSystemEntities();
            RefreshData();
        }

        private void RefreshData()
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
            // Общий цвет фона таблицы
            Color backgroundColor = Color.FromArgb(238, 245, 245);
            Color headerColor = Color.FromArgb(238, 245, 245); // Отдельный цвет для заголовков

            // Настройка стиля заголовков
            dataGridViewEmployees.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = headerColor, // Фон заголовков
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = headerColor, // Важно: цвет при "выделении" заголовка
                SelectionForeColor = Color.FromArgb(39, 39, 39) // Цвет текста при "выделении"
            };

            // Настройка стиля ячеек
            dataGridViewEmployees.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = backgroundColor,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Настройка стиля выделенных строк (только для строк данных)
            dataGridViewEmployees.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewEmployees.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);

            // Настройка цвета сетки
            dataGridViewEmployees.GridColor = Color.FromArgb(39, 39, 39);

            // Настройка внешнего вида
            dataGridViewEmployees.BackgroundColor = backgroundColor;
            dataGridViewEmployees.BorderStyle = BorderStyle.None;
            dataGridViewEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewEmployees.EnableHeadersVisualStyles = false;
            dataGridViewEmployees.RowHeadersVisible = false;
            dataGridViewEmployees.AllowUserToAddRows = false;
            dataGridViewEmployees.AllowUserToDeleteRows = false;
            dataGridViewEmployees.ReadOnly = true;
            dataGridViewEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Дополнительные настройки
            dataGridViewEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewEmployees.ColumnHeadersHeight = 30;
            dataGridViewEmployees.RowTemplate.Height = 25;

            // Важно: отключаем выделение заголовков
            dataGridViewEmployees.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        private void LoadEmployeesData(string filter = "")
        {
            try
            {
                if (_dbContext == null)
                {
                    _dbContext = new KindergartenInformationSystemEntities();
                }

                var query = from emp in _dbContext.Employees
                            join user in _dbContext.User on emp.EmployeesUserID equals user.UserID
                            join role in _dbContext.Role on user.RoleID equals role.RoleID
                            where string.IsNullOrEmpty(filter) ||
                                  emp.EmployeesPatronymic.Contains(filter) ||
                                  emp.EmployeesSurname.Contains(filter) ||
                                  emp.EmployeesName.Contains(filter)
                            orderby emp.EmployeesSurname
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

                // Очищаем текущий источник данных перед установкой нового
                dataGridViewEmployees.DataSource = null;
                dataGridViewEmployees.DataSource = query.ToList();

                // Форматирование колонки с датой
                if (dataGridViewEmployees.Columns.Contains("ДатаРождения"))
                {
                    dataGridViewEmployees.Columns["ДатаРождения"].DefaultCellStyle.Format = "dd.MM.yyyy";
                }

                // Переименование колонок
                if (dataGridViewEmployees.Columns.Contains("ДатаРождения"))
                    dataGridViewEmployees.Columns["ДатаРождения"].HeaderText = "Дата рождения";

                if (dataGridViewEmployees.Columns.Contains("Логин"))
                    dataGridViewEmployees.Columns["Логин"].HeaderText = "Логин";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonInfoEmp_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployees.SelectedRows.Count > 0)
            {
                string userLogin = dataGridViewEmployees.CurrentRow.Cells["Логин"].Value?.ToString();

                if (!string.IsNullOrEmpty(userLogin))
                {
                    using (var db = new KindergartenInformationSystemEntities())
                    {
                        var emp = db.Employees.FirstOrDefault(em => em.User.UserLogin == userLogin);
                        if (emp != null)
                        {
                            UIForms.MoreInfoEmp moreInfoEmp = new UIForms.MoreInfoEmp(emp);
                            moreInfoEmp.ShowDialog();
                            RefreshData();
                        }
                    }
                }
            }
        }

        private void labelAddEmp_Click(object sender, EventArgs e)
        {
            AddEmployees addEmployees = new AddEmployees();
            if (addEmployees.ShowDialog() == DialogResult.OK)
            {
                RefreshData(); // Обновляем данные после добавления сотрудника
            }
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
            // Текущая форма, ничего не делаем
        }

        private void label4_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new GroupWindow(), this);
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

        private void EmployeesWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Authorization auth = new Authorization();
                auth.Show();
            }
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            InfoUser infoUser = new InfoUser();
            infoUser.ShowDialog();
            RefreshData(); // Обновляем данные после закрытия диалога
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _dbContext?.Dispose();
        }

        private void textBoxSearchPatronymic_TextChanged(object sender, EventArgs e)
        {
            // Получаем текст из текстового поля
            string searchText = textBoxSearchPatronymic.Text.Trim();

            // Если текст пустой, загружаем все данные
            if (string.IsNullOrEmpty(searchText))
            {
                LoadEmployeesData();
            }
            else
            {
                // Фильтруем данные по фамилии (и другим полям, если нужно)
                LoadEmployeesData(searchText);
            }
        }
    }
}