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
                // Получаем данные из базы данных
                var parentsQuery = Helper.DB.Parents
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
            catch (Exception ex)
            {
                // Ошибки просто игнорируем, как указано в требованиях
            }
        }

        private void ConfigureDataGridView()
        {
            // Настройка внешнего вида DataGridView
            dataGridViewParents.BackgroundColor = Color.FromArgb(238, 245, 245);
            dataGridViewParents.BorderStyle = BorderStyle.None;
            dataGridViewParents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewParents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewParents.RowHeadersVisible = false;
            dataGridViewParents.AllowUserToAddRows = false;
            dataGridViewParents.AllowUserToDeleteRows = false;
            dataGridViewParents.AllowUserToResizeRows = false;
            dataGridViewParents.ReadOnly = true;
            dataGridViewParents.EnableHeadersVisualStyles = false;

            // Настройка стиля заголовков
            dataGridViewParents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 245, 245);
            dataGridViewParents.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(25, 25, 25);
            dataGridViewParents.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 16, FontStyle.Bold);
            dataGridViewParents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewParents.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Настройка стиля строк
            dataGridViewParents.DefaultCellStyle.BackColor = Color.FromArgb(238, 245, 245);
            dataGridViewParents.DefaultCellStyle.ForeColor = Color.FromArgb(25, 25, 25);
            dataGridViewParents.DefaultCellStyle.Font = new Font("Verdana", 14.25f);
            dataGridViewParents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 145, 145);
            dataGridViewParents.DefaultCellStyle.SelectionForeColor = Color.FromArgb(254, 255, 255);
            dataGridViewParents.RowTemplate.Height = 35;
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
    }
}