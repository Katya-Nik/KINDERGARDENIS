﻿using KINDERGARDENIS.DBModel;
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
            this.FormClosing += EmployeesWindow_FormClosing;
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

        private void labelAddEmp_Click(object sender, EventArgs e)
        {
            AddEmployees addEmployees = new AddEmployees();
            addEmployees.ShowDialog();
            this.Show();
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
            // Эта форма
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
    }
}