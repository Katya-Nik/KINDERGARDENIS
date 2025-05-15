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
    public partial class ChildrenWindow : Form
    {
        public ChildrenWindow()
        {
            InitializeComponent();
            this.FormClosing += ChildrenWindow_FormClosing;
        }

        private void ChildrenWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadChidrenlMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9,
            label1, label2, label3, label4, label5,
            label6, label7, label8, label9);
            // Настройка внешнего вида dataGridViewChild
            ConfigureDataGridView();

            // Загрузка данных в comboBoxGroupName
            LoadGroupNames();

            // Загрузка данных в dataGridViewChild
            LoadChildrenData();
        }

        private void ConfigureDataGridView()
        {
            // Настройка стиля заголовков
            dataGridViewChild.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.FromArgb(238, 245, 245),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Настройка стиля ячеек
            dataGridViewChild.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 14.25f),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.FromArgb(238, 245, 245),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Настройка стиля выделенных строк
            dataGridViewChild.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewChild.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);

            // Настройка внешнего вида
            dataGridViewChild.BackgroundColor = Color.FromArgb(238, 245, 245);
            dataGridViewChild.BorderStyle = BorderStyle.None;
            dataGridViewChild.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewChild.RowHeadersVisible = false;
            dataGridViewChild.AllowUserToAddRows = false;
            dataGridViewChild.AllowUserToDeleteRows = false;
            dataGridViewChild.ReadOnly = true;
            dataGridViewChild.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadGroupNames()
        {
            try
            {
                var groups = Helper.DB.Groups
                    .OrderBy(g => g.GroupsGroupName)
                    .Select(g => g.GroupsGroupName)
                    .Distinct()
                    .ToList();

                comboBoxGroupName.Items.Add("Все группы");
                foreach (var group in groups)
                {
                    comboBoxGroupName.Items.Add(group);
                }
                comboBoxGroupName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Cообщения об ошибках не выводятся
            }
        }

        private void LoadChildrenData()
        {
            try
            {
                var query = Helper.DB.Children.AsQueryable();

                // Фильтрация по фамилии, если введена
                if (!string.IsNullOrWhiteSpace(textBoxSearchSurname.Text))
                {
                    query = query.Where(c => c.ChildrenSurname.Contains(textBoxSearchSurname.Text));
                }

                // Фильтрация по группе, если выбрана
                if (comboBoxGroupName.SelectedIndex > 0)
                {
                    string selectedGroup = comboBoxGroupName.SelectedItem.ToString();
                    query = query.Where(c => c.Groups.GroupsGroupName == selectedGroup);
                }

                // Получение данных с сортировкой по фамилии и названию группы
                var childrenData = query
                    .OrderBy(c => c.ChildrenSurname)
                    .ThenBy(c => c.Groups.GroupsGroupName)
                    .Select(c => new
                    {
                        Фамилия = c.ChildrenSurname,
                        Имя = c.ChildrenName,
                        Отчество = c.ChildrenPatronymic,
                        ДатаРождения = c.ChildrenDateofBirth,
                        Группа = c.Groups.GroupsGroupName,
                        Воспитатель = c.Groups.Educators
                            .Where(e => e.Employees.User.Role.RoleName == "Воспитатель")
                            .Select(e => e.Employees.EmployeesSurname)
                            .FirstOrDefault(),
                        МладшийВоспитатель = c.Groups.Educators
                            .Where(e => e.Employees.User.Role.RoleName == "Младший воспитатель")
                            .Select(e => e.Employees.EmployeesSurname)
                            .FirstOrDefault()
                    })
                    .ToList();

                dataGridViewChild.DataSource = childrenData;

                // Настройка ширины колонок
                foreach (DataGridViewColumn column in dataGridViewChild.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                // В соответствии с требованиями, сообщения об ошибках не выводятся
            }
        }

        private void textBoxSearchSurname_TextChanged(object sender, EventArgs e)
        {
            LoadChildrenData();
        }

        private void comboBoxGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChildrenData();
        }

        private void buttonAddChild_Click(object sender, EventArgs e)
        {
            UIForms.AddChild addChild = new AddChild();
            addChild.Show();
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
            // Эта форма
        }

        private void label6_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ParentsWindow(), this);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new UsersWindow(), this);
        }

        private void ChildrenWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // Если окно закрывается пользователем
            {
                Authorization auth = new Authorization();
                auth.Show(); // Открываем Authorization
            }
        }
    }
}