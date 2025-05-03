using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;

namespace KINDERGARDENIS.UIForms
{
    public partial class MoreInfoEmp : Form
    {
        DBModel.Employees employees;
        string pathPhotoUsers = Environment.CurrentDirectory + @"\PhotoUsers\";
        string pathResources = Environment.CurrentDirectory + @"\Resources\";
        private bool isPasswordVisible = false;

        public MoreInfoEmp(DBModel.Employees emp)
        {
            InitializeComponent();
            employees = emp;
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            // Загрузка данных сотрудника в текстовые поля
            labelIDEmp.Text = "Должность: " + employees.User.Role.RoleName;
            textBoxSurname.Text = employees.EmployeesSurname;
            textBoxName.Text = employees.EmployeesName;
            textBoxPatronymic.Text = employees.EmployeesPatronymic;
            textBoxDateofBirth.Text = employees.EmployeesDateofBirth?.ToString("dd.MM.yyyy");
            textBoxPassportSeries.Text = employees.EmployeesPassportSeries;
            textBoxPassportNumber.Text = employees.EmployeesPassportNumber;
            textBoxSNILS.Text = employees.EmployeesSNILS;
            textBoxPhoneNumber.Text = employees.EmployeesPhoneNumber;
            textBoxEmail.Text = employees.EmployeesEmail;
            textBoxSalary.Text = employees.EmployeesSalary?.ToString("N2");

            // Загрузка данных пользователя (логин и пароль)
            using (var db = new DBModel.KindergartenInformationSystemEntities())
            {
                var user = db.User.Find(employees.EmployeesUserID);
                if (user != null)
                {
                    textBoxLogin.Text = user.UserLogin;
                    textBoxPassword.Text = user.UserPassword;
                }
            }

            // Загрузка фото сотрудника
            LoadEmployeePhoto();

            // Настройка отображения пароля
            textBoxPassword.UseSystemPasswordChar = true;
            pictureBoxeye.Image = Image.FromFile(pathResources + "eyeon.png");
        }

        private void LoadEmployeePhoto()
        {
            string photoPath = pathPhotoUsers + employees.EmployeesID + ".jpg";
            if (File.Exists(photoPath))
            {
                pictureBoxEmployees.Image = Image.FromFile(photoPath);
            }
            else
            {
                pictureBoxEmployees.Image = Image.FromFile(pathPhotoUsers + "images.png");
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Обновление данных сотрудника
                    employees.EmployeesSurname = textBoxSurname.Text;
                    employees.EmployeesName = textBoxName.Text;
                    employees.EmployeesPatronymic = textBoxPatronymic.Text;

                    if (DateTime.TryParse(textBoxDateofBirth.Text, out DateTime date))
                        employees.EmployeesDateofBirth = date;

                    employees.EmployeesPassportSeries = textBoxPassportSeries.Text;
                    employees.EmployeesPassportNumber = textBoxPassportNumber.Text;
                    employees.EmployeesSNILS = textBoxSNILS.Text;
                    employees.EmployeesPhoneNumber = textBoxPhoneNumber.Text;
                    employees.EmployeesEmail = textBoxEmail.Text;

                    if (decimal.TryParse(textBoxSalary.Text, out decimal salary))
                        employees.EmployeesSalary = salary;

                    // Обновление данных пользователя
                    using (var db = new DBModel.KindergartenInformationSystemEntities())
                    {
                        var user = db.User.Find(employees.EmployeesUserID);
                        if (user != null)
                        {
                            user.UserLogin = textBoxLogin.Text;
                            user.UserPassword = textBoxPassword.Text;
                            db.SaveChanges();
                        }

                        db.Entry(employees).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonCancellation_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxEye_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                textBoxPassword.UseSystemPasswordChar = false;
                pictureBoxeye.Image = Image.FromFile(pathResources + "eyeoff.png");
            }
            else
            {
                textBoxPassword.UseSystemPasswordChar = true;
                pictureBoxeye.Image = Image.FromFile(pathResources + "eyeon.png");
            }
        }
    }
}