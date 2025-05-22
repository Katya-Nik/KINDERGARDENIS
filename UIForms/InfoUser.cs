using KINDERGARDENIS.DBModel;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class InfoUser : Form
    {
        private static string pathPhotoUsers = Environment.CurrentDirectory + @"\PhotoUsers\";
        private static string pathChevron = Environment.CurrentDirectory + @"\Resources\";
        private bool isPasswordVisible = false;
        private DBModel.User _user;

        public InfoUser()
        {
            InitializeComponent();
        }

        public InfoUser(DBModel.User user) : this()  
        {
            _user = user;
            LoadUserData();  // Загружаем данные пользователя в форму
        }

        private void LoadUserData()
        {
            if (_user != null)
            {
                
            }
        }

        private void InfoUser_Load(object sender, EventArgs e)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка фото пользователя
            string filePhotoUsers = pathPhotoUsers + Helper.users.UserID + ".jpg";
            if (!File.Exists(filePhotoUsers))
            {
                filePhotoUsers = pathPhotoUsers + "Stub.png";
            }
            pictureBoxPhotoUsers.Load(filePhotoUsers);

            // Загрузка данных пользователя
            textBoxLogin.Text = Helper.users.UserLogin;
            textBoxLogin.ReadOnly = true;
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.Text = Helper.users.UserPassword;

            // Загрузка иконки глаза
            UpdateEyeIcon();

            // Роль, ФИО, почта работника и телефон
            if (Helper.users != null && Helper.users.RoleID >= 1 && Helper.users.RoleID <= 15)
            {
                if (Helper.users != null && Helper.users.Employees != null)
                {
                    var employee = Helper.users.Employees.FirstOrDefault();

                    if (employee != null)
                    {
                        labelRole.Text = employee.User.Role.RoleName;
                        labelFIO.Text = employee.EmployeesSurname + " " + employee.EmployeesName + " " + employee.EmployeesPatronymic;
                        labelUserEmaile.Text = employee.EmployeesEmail;
                        labelPhoneNumber.Text = employee.EmployeesPhoneNumber;
                    }
                    else
                    {
                        labelName.Text = "Нет данных о сотруднике";
                    }
                }
            }
            else if (Helper.users != null && Helper.users.RoleID == 16) // ФИО и почта родителя
            {
                if (Helper.users != null && Helper.users.Parents != null)
                {
                    var parents = Helper.users.Parents.FirstOrDefault();
                    if (parents != null)
                    {
                        labelRole.Text = parents.User.Role.RoleName;
                        labelFIO.Text = parents.ParentsSurname + " " + parents.ParentsName + " " + parents.ParentsPatronymic;
                        labelUserEmaile.Text = parents.ParentsEmail;
                        labelPhoneNumber.Text = parents.ParentsPhoneNumber;
                    }
                    else
                    {
                        labelName.Text = "Нет данных о родителе";
                    }
                }
            }
        }

        private void textBoxLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("У вас нет прав на редактирование. Обратитесь к администратору.");
        }

        private void pictureBoxEye_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            textBoxPassword.UseSystemPasswordChar = !isPasswordVisible;
            UpdateEyeIcon();
        }

        private void UpdateEyeIcon()
        {
            string eyeIconPath = isPasswordVisible
                ? pathChevron + "eyeon.png"
                : pathChevron + "eyeoff.png";

            if (File.Exists(eyeIconPath))
            {
                pictureBoxeye.Image = Image.FromFile(eyeIconPath);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Пароль не может быть пустым!");
                return;
            }

            try
            {
                using (var db = new KindergartenInformationSystemEntities())
                {
                    var user = db.User.FirstOrDefault(u => u.UserID == Helper.users.UserID);
                    if (user != null)
                    {
                        // В реальном приложении здесь должно быть хеширование пароля!
                        user.UserPassword = textBoxPassword.Text;
                        db.SaveChanges();
                        MessageBox.Show("Пароль успешно обновлен!");
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден в базе данных!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пароля: {ex.Message}");
            }
        }
    }
}