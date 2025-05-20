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
            // Общий цвет фона таблицы
            Color backgroundColor = Color.FromArgb(238, 245, 245);
            Color headerColor = Color.FromArgb(238, 245, 245); // Отдельный цвет для заголовков

            // Настройка стиля заголовков
            dataGridViewChild.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = headerColor, // Фон заголовков
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = headerColor, // Важно: цвет при "выделении" заголовка
                SelectionForeColor = Color.FromArgb(39, 39, 39) // Цвет текста при "выделении"
            };

            // Настройка стиля ячеек
            dataGridViewChild.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = backgroundColor,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Настройка стиля выделенных строк (только для строк данных)
            dataGridViewChild.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewChild.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);

            // Настройка цвета сетки
            dataGridViewChild.GridColor = Color.FromArgb(39, 39, 39);

            // Настройка внешнего вида
            dataGridViewChild.BackgroundColor = backgroundColor;
            dataGridViewChild.BorderStyle = BorderStyle.None;
            dataGridViewChild.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewChild.EnableHeadersVisualStyles = false;
            dataGridViewChild.RowHeadersVisible = false;
            dataGridViewChild.AllowUserToAddRows = false;
            dataGridViewChild.AllowUserToDeleteRows = false;
            dataGridViewChild.ReadOnly = true;
            dataGridViewChild.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Дополнительные настройки
            dataGridViewChild.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewChild.ColumnHeadersHeight = 30;
            dataGridViewChild.RowTemplate.Height = 25;

            // Важно: отключаем выделение заголовков
            dataGridViewChild.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
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
                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    var query = db.Children.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(textBoxSearchSurname.Text))
                    {
                        query = query.Where(c => c.ChildrenSurname.Contains(textBoxSearchSurname.Text));
                    }

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
                        ID = c.ChildrenID, // Добавлен столбец с ID
                        Фамилия = c.ChildrenSurname,
                        Имя = c.ChildrenName,
                        Отчество = c.ChildrenPatronymic,
                        Дата_Рождения = c.ChildrenDateofBirth,
                        Группа = c.Groups.GroupsGroupName,
                        Воспитатель = c.Groups.Educators
                            .Select(e => e.Employees)
                            .Where(e => e.User.Role.RoleID == 7)
                            .Select(e => e.EmployeesSurname + " " + e.EmployeesName)
                            .FirstOrDefault(),
                        Младший_Воспитатель = c.Groups.Educators
                            .Select(e => e.Employees)
                            .Where(e => e.User.Role.RoleID == 8)
                            .Select(e => e.EmployeesSurname + " " + e.EmployeesName)
                            .FirstOrDefault()
                    })
                    .ToList();

                    dataGridViewChild.DataSource = childrenData;

                    // Скрываем столбец ID после привязки данных
                    if (dataGridViewChild.Columns.Contains("ID"))
                    {
                        dataGridViewChild.Columns["ID"].Visible = false;
                    }
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
            UIForms.AddParent addParent = new AddParent();
            addParent.Show();
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

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            InfoUser infoUser = new InfoUser();
            infoUser.ShowDialog();
            this.Show();
        }

        private void buttonInfoChild_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewChild.SelectedRows.Count > 0)
                {
                    // Получаем ID из скрытого столбца
                    var idCell = dataGridViewChild.SelectedRows[0].Cells["ID"];
                    if (idCell != null && idCell.Value != null)
                    {
                        int idChild = Convert.ToInt32(idCell.Value);
                        UIForms.MoreInfoChild moreInfoChild = new UIForms.MoreInfoChild(idChild);
                        moreInfoChild.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show("ID ребенка не указан.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}