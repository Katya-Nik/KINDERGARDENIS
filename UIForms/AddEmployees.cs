using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class AddEmployees : Form
    {
        private static string pathImage = Environment.CurrentDirectory + @"\Resources\";
        private static string pathPhotoUsers = Environment.CurrentDirectory + @"\PhotoUsers\";
        private bool isPasswordVisible = false;
        private Image uploadedPhoto = null;

        public AddEmployees()
        {
            InitializeComponent();
        }

        private void AddEmployees_Load(object sender, EventArgs e)
        {
            // Установка начальных изображений
            pictureBoxEmployees.Image = Image.FromFile(pathImage + "AddImage.png");
            pictureBoxeye.Image = Image.FromFile(pathImage + "eyeoff.png");

            // Настройка масок
            maskedTextBoxDateofBirth.Mask = "00/00/0000";
            maskedTextBoxPassportSeries.Mask = "0000";
            maskedTextBoxPassportNumber.Mask = "000000";
            maskedTextBoxSNILS.Mask = "000-000-000 00";
            maskedTextBoxPhoneNumber.Mask = "+7 (000) 000-00-00";

            // Настройка цвета полей
            SetFieldsDefaultColors();

            // Скрытие пароля
            textBoxPassword.UseSystemPasswordChar = true;

            // Загрузка ролей в comboBoxRole
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                // Получаем список ролей из базы данных
                var roles = Helper.DB.Role.ToList();

                // Очищаем comboBox перед загрузкой
                comboBoxRole.Items.Clear();

                // Добавляем роли в comboBox
                foreach (var role in roles)
                {
                    comboBoxRole.Items.Add(new KeyValuePair<int, string>(role.RoleID, role.RoleName));
                }

                // Настраиваем отображение
                comboBoxRole.DisplayMember = "Value";
                comboBoxRole.ValueMember = "Key";

                // Устанавливаем значение по умолчанию (например, первую роль)
                if (comboBoxRole.Items.Count > 0)
                {
                    comboBoxRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetFieldsDefaultColors()
        {
            Color defaultColor = Color.FromArgb(133, 223, 234);
            Color filledColor = Color.FromArgb(145, 195, 173);

            textBoxSurname.BackColor = string.IsNullOrEmpty(textBoxSurname.Text) ? defaultColor : filledColor;
            textBoxName.BackColor = string.IsNullOrEmpty(textBoxName.Text) ? defaultColor : filledColor;
            textBoxPatronymic.BackColor = string.IsNullOrEmpty(textBoxPatronymic.Text) ? defaultColor : filledColor;
            maskedTextBoxDateofBirth.BackColor = string.IsNullOrEmpty(maskedTextBoxDateofBirth.Text.Replace(" ", "").Replace("/", "")) ? defaultColor : filledColor;
            maskedTextBoxPassportSeries.BackColor = string.IsNullOrEmpty(maskedTextBoxPassportSeries.Text.Replace(" ", "")) ? defaultColor : filledColor;
            maskedTextBoxPassportNumber.BackColor = string.IsNullOrEmpty(maskedTextBoxPassportNumber.Text.Replace(" ", "")) ? defaultColor : filledColor;
            maskedTextBoxSNILS.BackColor = string.IsNullOrEmpty(maskedTextBoxSNILS.Text.Replace(" ", "").Replace("-", "")) ? defaultColor : filledColor;
            maskedTextBoxPhoneNumber.BackColor = string.IsNullOrEmpty(maskedTextBoxPhoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "")) ? defaultColor : filledColor;
            textBoxEmail.BackColor = string.IsNullOrEmpty(textBoxEmail.Text) ? defaultColor : filledColor;
            textBoxSalary.BackColor = string.IsNullOrEmpty(textBoxSalary.Text) ? defaultColor : filledColor;
            textBoxLogin.BackColor = string.IsNullOrEmpty(textBoxLogin.Text) ? defaultColor : filledColor;
            textBoxPassword.BackColor = string.IsNullOrEmpty(textBoxPassword.Text) ? defaultColor : filledColor;
            comboBoxRole.BackColor = comboBoxRole.SelectedIndex == -1 ? defaultColor : filledColor;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBoxBase;
            if (textBox != null)
            {
                textBox.BackColor = string.IsNullOrEmpty(textBox.Text) ?
                    Color.FromArgb(133, 223, 234) :
                    Color.FromArgb(145, 195, 173);
            }
        }

        private void MaskedTextBox_TextChanged(object sender, EventArgs e)
        {
            var maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null)
            {
                string cleanText = maskedTextBox.Text.Replace(" ", "").Replace("/", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");
                maskedTextBox.BackColor = string.IsNullOrEmpty(cleanText) ?
                    Color.FromArgb(133, 223, 234) :
                    Color.FromArgb(145, 195, 173);
            }
        }

        private void comboBoxRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxRole.BackColor = comboBoxRole.SelectedIndex == -1 ?
                Color.FromArgb(133, 223, 234) :
                Color.FromArgb(145, 195, 173);
        }

        private void pictureBoxeye_Click(object sender, EventArgs e)
        {

        }

        private void buttonUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        uploadedPhoto = Image.FromFile(openFileDialog.FileName);
                        pictureBoxEmployees.Image = uploadedPhoto;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверка заполнения обязательных полей
            if (string.IsNullOrEmpty(textBoxSurname.Text) ||
                string.IsNullOrEmpty(textBoxName.Text) ||
                string.IsNullOrEmpty(textBoxPatronymic.Text) ||
                string.IsNullOrEmpty(textBoxLogin.Text) ||
                string.IsNullOrEmpty(textBoxPassword.Text) ||
                comboBoxRole.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Получаем выбранную роль
                var selectedRole = (KeyValuePair<int, string>)comboBoxRole.SelectedItem;
                int roleId = selectedRole.Key;

                // Создание пользователя
                var newUser = new DBModel.User
                {
                    UserLogin = textBoxLogin.Text,
                    UserPassword = HashPassword(textBoxPassword.Text), // Хэширование пароля
                    RoleID = roleId
                };

                Helper.DB.User.Add(newUser);
                Helper.DB.SaveChanges();

                // Создание работника
                var newEmployee = new DBModel.Employees
                {
                    EmployeesName = textBoxName.Text,
                    EmployeesSurname = textBoxSurname.Text,
                    EmployeesPatronymic = textBoxPatronymic.Text,
                    EmployeesUserID = newUser.UserID,
                    EmployeesPhoneNumber = maskedTextBoxPhoneNumber.Text,
                    EmployeesEmail = textBoxEmail.Text,
                    EmployeesSNILS = maskedTextBoxSNILS.Text,
                    EmployeesPassportSeries = maskedTextBoxPassportSeries.Text,
                    EmployeesPassportNumber = maskedTextBoxPassportNumber.Text
                };

                // Парсинг даты рождения
                if (DateTime.TryParse(maskedTextBoxDateofBirth.Text, out DateTime birthDate))
                {
                    newEmployee.EmployeesDateofBirth = birthDate;
                }

                // Парсинг зарплаты
                if (decimal.TryParse(textBoxSalary.Text, out decimal salary))
                {
                    newEmployee.EmployeesSalary = salary;
                }

                Helper.DB.Employees.Add(newEmployee);
                Helper.DB.SaveChanges();

                // Сохранение фотографии
                if (uploadedPhoto != null)
                {
                    if (!Directory.Exists(pathPhotoUsers))
                    {
                        Directory.CreateDirectory(pathPhotoUsers);
                    }

                    string photoPath = Path.Combine(pathPhotoUsers, $"{newUser.UserID}.png");
                    uploadedPhoto.Save(photoPath);
                }

                MessageBox.Show("Работник успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
            // Простая реализация хэширования пароля (в реальном проекте используйте более надежные методы)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void buttonCancellation_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите отменить добавление работника? Все введенные данные будут потеряны.",
                "Подтверждение отмены",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}