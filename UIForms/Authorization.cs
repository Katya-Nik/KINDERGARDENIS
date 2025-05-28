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
    public partial class Authorization : Form
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void buttonEntry_Click(object sender, EventArgs e)
        {
            // Получить логин и пароль от пользователя
            string login = this.textBoxLogin.Text;
            string password = this.textBoxPassword.Text;

            // Получить всех пользователей из БД
            List<DBModel.User> users = Helper.DB.User.ToList();

            // Найти пользователя с таким логином и паролем
            DBModel.User currentUser = users.FirstOrDefault(u => u.UserLogin == login && u.UserPassword == password);

            if (currentUser != null) // Если пользователь найден
            {
                // Проверяем, что RoleID >= 3 (иначе запрещаем вход)
                if (currentUser.RoleID < 3)
                {
                    MessageBox.Show(
                        "Для вашей роли функционал не разработан",
                        "Ограничение доступа",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return; // Выходим из метода, не пускаем дальше
                }

                // Сохраняем пользователя в Helper (если RoleID >= 3)
                Helper.users = currentUser;

                MessageBox.Show(
                    $"Пользователь найден. Вы вошли с ролью {currentUser.Role.RoleName}",
                    "Валидация пользователя",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Открываем главное окно только если RoleID >= 3
                FormHelper.ShowFormAndHideCurrent(this, new MainWindow());
            }
            else // Пользователь не найден
            {
                MessageBox.Show(
                    "Пользователь не найден",
                    "Валидация пользователя",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.FormClosed += (s, args) => ReturnToAuthorization();
            mainWindow.Show();
            this.Hide();
        }

        private void ReturnToAuthorization()
        {
            bool authFormExists = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form is Authorization)
                {
                    authFormExists = true;
                    form.Show();
                    break;
                }
            }
            if (!authFormExists)
            {
                this.Show();
            }
        }

        private void Login_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Helper.DB = new DBModel.KindergartenInformationSystemEntities();	//Создание объекта подключения к БД
            }
            catch
            {
                MessageBox.Show("Ошибка при подключении к БД", "Валидация подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }
        }

        private void Authorization_Load(object sender, EventArgs e)
        {

        }
    }
}
