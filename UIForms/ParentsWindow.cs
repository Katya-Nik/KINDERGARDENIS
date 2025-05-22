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
    public partial class ParentsWindow : Form
    {
        public ParentsWindow()
        {
            InitializeComponent();
            this.FormClosing += ParentsWindow_FormClosing;
        }

        private void ParentsWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadParentlMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                pictureBox6, pictureBox7, pictureBox8, pictureBox9,
                label1, label2, label3, label4, label5,
                label6, label7, label8, label9);

            LoadParentsData();
        }

        private void LoadParentsData()
        {
            try
            {
                using (var db = new DBModel.KindergartenInformationSystemEntities()) // Замените YourDbContextClass на реальный тип вашего контекста
                {
                    // Получаем данные из базы данных
                    var parentsQuery = db.Parents
                    .OrderBy(p => p.ParentsSurname)
                    .Select(p => new
                    {
                        Фамилия = p.ParentsSurname,
                        Имя = p.ParentsName,
                        Отчество = p.ParentsPatronymic,
                        Телефон = p.ParentsPhoneNumber,
                        Почта = p.ParentsEmail,
                        Логин = p.User.UserLogin
                    })
                    .ToList();

                    // Настраиваем DataGridView перед загрузкой данных
                    ConfigureDataGridView();

                    // Привязываем данные к DataGridView
                    dataGridViewParents.DataSource = parentsQuery;
                }
            }
            catch (Exception ex)
            {
                // Ошибки просто игнорируем, как указано в требованиях
            }
        }

        private void ConfigureDataGridView()
        {
            // Общий цвет фона таблицы
            Color backgroundColor = Color.FromArgb(238, 245, 245);
            Color headerColor = Color.FromArgb(238, 245, 245); // Отдельный цвет для заголовков

            // Настройка стиля заголовков
            dataGridViewParents.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = headerColor, // Фон заголовков
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = headerColor, // Важно: цвет при "выделении" заголовка
                SelectionForeColor = Color.FromArgb(39, 39, 39) // Цвет текста при "выделении"
            };

            // Настройка стиля ячеек
            dataGridViewParents.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Verdana", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(39, 39, 39),
                BackColor = backgroundColor,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Настройка стиля выделенных строк (только для строк данных)
            dataGridViewParents.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewParents.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);
            
            // Настройка цвета сетки
            dataGridViewParents.GridColor = Color.FromArgb(39, 39, 39);

            // Настройка внешнего вида
            dataGridViewParents.BackgroundColor = backgroundColor;
            dataGridViewParents.BorderStyle = BorderStyle.None;
            dataGridViewParents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewParents.EnableHeadersVisualStyles = false;
            dataGridViewParents.RowHeadersVisible = false;
            dataGridViewParents.AllowUserToAddRows = false;
            dataGridViewParents.AllowUserToDeleteRows = false;
            dataGridViewParents.ReadOnly = true;
            dataGridViewParents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Дополнительные настройки
            dataGridViewParents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewParents.ColumnHeadersHeight = 30;
            dataGridViewParents.RowTemplate.Height = 25;

            // Важно: отключаем выделение заголовков
            dataGridViewParents.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        private void textBoxSearchSurname_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var searchText = textBoxSearchSurname.Text.Trim().ToLower();

                var filteredParents = Helper.DB.Parents
                    .Where(p => p.ParentsSurname.ToLower().Contains(searchText))
                    .OrderBy(p => p.ParentsSurname)
                    .Select(p => new
                    {
                        Фамилия = p.ParentsSurname,
                        Имя = p.ParentsName,
                        Отчество = p.ParentsPatronymic,
                        Телефон = p.ParentsPhoneNumber,
                        Почта = p.ParentsEmail,
                        Логин = p.User.UserLogin
                    })
                    .ToList();

                dataGridViewParents.DataSource = filteredParents;
            }
            catch (Exception ex)
            {
                
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
            // Эта форма
        }

        private void label7_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new UsersWindow(), this);
        }

        private void ParentsWindow_FormClosing(object sender, FormClosingEventArgs e)
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
            if (dataGridViewParents.SelectedRows.Count > 0)
            {
                // Получаем логин пользователя из последней колонки
                string userLogin = dataGridViewParents.CurrentRow.Cells["Логин"].Value.ToString();

                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    // Находим сотрудника по логину пользователя
                    var par = db.Parents.FirstOrDefault(pa => pa.User.UserLogin == userLogin);
                    if (par != null)
                    { 
                        UIForms.MoreInfoParent moreInfoParent = new UIForms.MoreInfoParent(par);
                        moreInfoParent.ShowDialog();
                        this.Show();
                    }
                }
            }
        }
    }
}