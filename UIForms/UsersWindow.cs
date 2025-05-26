using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class UsersWindow : Form
    {
        public UsersWindow()
        {
            InitializeComponent();
            this.FormClosing += UsersWindow_FormClosing;
        }

        private void UsersWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadUserlMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                pictureBox6, pictureBox7, pictureBox8, pictureBox9,
                label1, label2, label3, label4, label5,
                label6, label7, label8, label9);
            LoadRolesToComboBox();
            LoadUsersToDataGridView();
            ConfigureDataGridViewAppearance();
        }

        private void LoadRolesToComboBox()
        {
            try
            {
                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    var roles = db.Role
                        .OrderBy(r => r.RoleName)
                        .Select(r => new { r.RoleName, r.RoleID })
                        .ToList();

                    comboBoxRoleName.DisplayMember = "RoleName";
                    comboBoxRoleName.ValueMember = "RoleID";

                    // Добавляем элемент "Все роли" с ID = 0
                    var allRolesItem = new { RoleName = "Все роли", RoleID = 0 };
                    var items = new List<object> { allRolesItem };
                    items.AddRange(roles);

                    comboBoxRoleName.DataSource = items;
                }
            }
            catch (Exception ex)
            {
                // В соответствии с требованиями, сообщения об ошибках не выводятся
            }
        }


        private void LoadUsersToDataGridView()
        {
            try
            {
                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    var usersQuery = from user in db.User
                                     join role in db.Role on user.RoleID equals role.RoleID
                                     join emp in db.Employees on user.UserID equals emp.EmployeesUserID into empJoin
                                     from emp in empJoin.DefaultIfEmpty()
                                     join parent in db.Parents on user.UserID equals parent.ParentsUserID into parentJoin
                                     from parent in parentJoin.DefaultIfEmpty()
                                     select new
                                     {
                                         Фамилия = emp != null ? emp.EmployeesSurname : parent.ParentsSurname,
                                         Имя = emp != null ? emp.EmployeesName : parent.ParentsName,
                                         Должность = role.RoleName,
                                         Логин = user.UserLogin,
                                         Номер_Телефона = emp != null ? emp.EmployeesPhoneNumber : parent.ParentsPhoneNumber,
                                         Почта = emp != null ? emp.EmployeesEmail : parent.ParentsEmail,
                                         RoleID = role.RoleID
                                     };

                    // Применяем фильтрацию, если выбрана конкретная роль (не "Все роли")
                    if (comboBoxRoleName.SelectedIndex > 0)
                    {
                        int selectedRoleId = (int)comboBoxRoleName.SelectedValue;
                        usersQuery = usersQuery.Where(u => u.RoleID == selectedRoleId);
                    }

                    // Применяем фильтрацию по фамилии, если введен текст
                    if (!string.IsNullOrWhiteSpace(textBoxSearchSurname.Text))
                    {
                        string searchText = textBoxSearchSurname.Text.ToLower();
                        usersQuery = usersQuery.Where(u => u.Фамилия.ToLower().Contains(searchText));
                    }

                    // Сортируем по фамилии
                    usersQuery = usersQuery.OrderBy(u => u.Фамилия);

                    dataGridViewUsers.DataSource = usersQuery.ToList();
                }
            }
            catch (Exception ex)
            {
                // В соответствии с требованиями, сообщения об ошибках не выводятся
            }
        }

        private void ConfigureDataGridViewAppearance()
        {
            // Общий цвет фона таблицы
            Color backgroundColor = Color.FromArgb(238, 245, 245);
            Color headerColor = Color.FromArgb(238, 245, 245); // Отдельный цвет для заголовков

            // Настройка стиля заголовков
            dataGridViewUsers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            { 
                Font = new Font("Verdana", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = headerColor, // Фон заголовков
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = headerColor, // Важно: цвет при "выделении" заголовка
                SelectionForeColor = Color.FromArgb(39, 39, 39) // Цвет текста при "выделении"
            };

            // Настройка стиля ячеек
            dataGridViewUsers.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = backgroundColor,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Настройка стиля выделенных строк (только для строк данных)
            dataGridViewUsers.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewUsers.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);

            // Настройка цвета сетки
            dataGridViewUsers.GridColor = Color.FromArgb(39, 39, 39);

            // Настройка внешнего вида
            dataGridViewUsers.BackgroundColor = backgroundColor;
            dataGridViewUsers.BorderStyle = BorderStyle.None;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.EnableHeadersVisualStyles = false;
            dataGridViewUsers.RowHeadersVisible = false;
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Дополнительные настройки
            dataGridViewUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewUsers.ColumnHeadersHeight = 30;
            dataGridViewUsers.RowTemplate.Height = 25;

            // Отключаем выделение заголовков
            dataGridViewUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewUsers.EnableHeadersVisualStyles = false;

            // Переименовываем заголовки столбцов
            if (dataGridViewUsers.Columns.Count >= 6)
            {
                dataGridViewUsers.Columns[0].HeaderText = "Фамилия";
                dataGridViewUsers.Columns[1].HeaderText = "Имя";
                dataGridViewUsers.Columns[2].HeaderText = "Роль";
                dataGridViewUsers.Columns[3].HeaderText = "Логин";
                dataGridViewUsers.Columns[4].HeaderText = "Телефон";
                dataGridViewUsers.Columns[5].HeaderText = "Email";
            }

            // Скрываем столбец RoleID
            if (dataGridViewUsers.Columns.Contains("RoleID"))
            {
                dataGridViewUsers.Columns["RoleID"].Visible = false;
            }
        }

        private void textBoxSearchSurname_TextChanged(object sender, EventArgs e)
        {
            LoadUsersToDataGridView();
        }

        private void comboBoxRoleName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsersToDataGridView();
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
            // Эта форма
        }

        private void UsersWindow_FormClosing(object sender, FormClosingEventArgs e)
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