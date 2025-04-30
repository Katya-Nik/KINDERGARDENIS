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
            //Получить логин и пароль от пользователя
            string login = this.textBoxLogin.Text;
            string password = this.textBoxPassword.Text;
            //Получить всех пользователей из БД
            List<DBModel.User> users = Helper.DB.User.ToList();
            //Получить список отфильтрованных пользователей по логину и паролю
            List<DBModel.User> usersLoginPassword = users.Where(u => u.UserLogin == login && u.UserPassword == password).ToList();
            //Получить единственного пользователя или его отсутствие
            Helper.users = usersLoginPassword.FirstOrDefault();
            //Проверка наличия единственного пользователя
            if (Helper.users != null) //Есть такой пользователь
            {
                MessageBox.Show("Пользователь найден. Вы вошли с ролью " + Helper.users.Role.RoleName, "Валидация пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UIForms.MainWindow mainWindow = new UIForms.MainWindow();
                this.Hide();
                mainWindow.ShowDialog();
                this.Show();
            }
            else //Отсутствует пользователь
            {
                MessageBox.Show("Пользователь не найден", "Валидация пользователя", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
