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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке формы: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRoles()
        {
            try
            {
                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    // Получаем список ролей из базы данных
                    var roles = db.Role.ToList();

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

                    // Устанавливаем значение по умолчанию
                    if (comboBoxRole.Items.Count > 0)
                    {
                        comboBoxRole.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetFieldsDefaultColors()
        {
            Color defaultColor = Color.FromArgb(222, 238, 225);
            Color filledColor = Color.FromArgb(222, 238, 225);

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
            isPasswordVisible = !isPasswordVisible;
            textBoxPassword.UseSystemPasswordChar = !isPasswordVisible;
            pictureBoxeye.Image = isPasswordVisible ?
                Image.FromFile(pathImage + "eyeon.png") :
                Image.FromFile(pathImage + "eyeoff.png");
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
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (Фамилия, Имя, Отчество, Логин, Пароль, Роль)!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка формата СНИЛС
            if (!maskedTextBoxSNILS.MaskCompleted)
            {
                MessageBox.Show("Пожалуйста, введите корректный СНИЛС!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка формата номера телефона
            if (!maskedTextBoxPhoneNumber.MaskCompleted)
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка даты рождения
            if (!DateTime.TryParse(maskedTextBoxDateofBirth.Text, out DateTime birthDate))
            {
                MessageBox.Show("Пожалуйста, введите корректную дату рождения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка зарплаты
            if (!decimal.TryParse(textBoxSalary.Text, out decimal salary) && !string.IsNullOrEmpty(textBoxSalary.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректное значение зарплаты!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var db = new DBModel.KindergartenInformationSystemEntities())
                {
                    // Проверка уникальности логина
                    if (db.User.Any(u => u.UserLogin == textBoxLogin.Text))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Получаем выбранную роль
                    var selectedRole = (KeyValuePair<int, string>)comboBoxRole.SelectedItem;
                    int roleId = selectedRole.Key;

                    // Создание пользователя
                    var newUser = new DBModel.User
                    {
                        UserLogin = textBoxLogin.Text,
                        UserPassword = HashPassword(textBoxPassword.Text),
                        RoleID = roleId
                    };

                    db.User.Add(newUser);
                    db.SaveChanges();

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
                        EmployeesPassportNumber = maskedTextBoxPassportNumber.Text,
                        EmployeesDateofBirth = birthDate
                    };

                    // Добавляем зарплату, если она указана
                    if (!string.IsNullOrEmpty(textBoxSalary.Text))
                    {
                        newEmployee.EmployeesSalary = salary;
                    }

                    db.Employees.Add(newEmployee);
                    db.SaveChanges();

                    // Сохранение фотографии
                    if (uploadedPhoto != null)
                    {
                        try
                        {
                            if (!Directory.Exists(pathPhotoUsers))
                            {
                                Directory.CreateDirectory(pathPhotoUsers);
                            }

                            string photoPath = Path.Combine(pathPhotoUsers, $"{newUser.UserID}.png");
                            uploadedPhoto.Save(photoPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Фото не было сохранено: {ex.Message}", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    MessageBox.Show("Работник успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Получаем самую вложенную исключение
                Exception innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                MessageBox.Show($"Ошибка при сохранении: {innerEx.Message}\n\nПодробности: {innerEx.StackTrace}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
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
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}