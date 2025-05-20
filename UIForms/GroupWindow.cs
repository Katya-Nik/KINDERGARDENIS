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
    public partial class GroupWindow : Form
    {
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
    }
}