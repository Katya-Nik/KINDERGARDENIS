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
    public partial class AddParent : Form
    {
        private static string pathImage = Environment.CurrentDirectory + @"\Resources\";
        private static string pathPhotoUsers = Environment.CurrentDirectory + @"\PhotoUsers\";
        private bool isPasswordVisible = false;
        private Image uploadedPhoto = null;

        public AddParent()
        {
            InitializeComponent();
        }

        private void AddParent_Load(object sender, EventArgs e)
        {
            // Установка начальных изображений
            pictureBoxEmployees.Image = Image.FromFile(pathImage + "AddImage.png");
            pictureBoxeye.Image = Image.FromFile(pathImage + "eyeoff.png");

            // Настройка масок ввода
            maskedTextBoxPassportSeries.Mask = "0000";
            maskedTextBoxPassportNumber.Mask = "000000";
            maskedTextBoxPhoneNumber.Mask = "+7 (000) 000 00 00";

            // Настройка цвета полей ввода
            SetTextBoxColors();

            // Подписка на события изменения текста
            textBoxSurname.TextChanged += TextBox_TextChanged;
            textBoxName.TextChanged += TextBox_TextChanged;
            textBoxPatronymic.TextChanged += TextBox_TextChanged;
            maskedTextBoxPassportSeries.TextChanged += TextBox_TextChanged;
            maskedTextBoxPassportNumber.TextChanged += TextBox_TextChanged;
            maskedTextBoxPhoneNumber.TextChanged += TextBox_TextChanged;
            textBoxEmail.TextChanged += TextBox_TextChanged;
            textBoxLogin.TextChanged += TextBox_TextChanged;
            textBoxPassword.TextChanged += TextBox_TextChanged;
        }



            private void TextBox_TextChanged(object sender, EventArgs e)
            {
                SetTextBoxColors();
            }

            private void SetTextBoxColors()
            {
                Color emptyColor = Color.FromArgb(133, 223, 234);
                Color filledColor = Color.FromArgb(145, 195, 173);

                textBoxSurname.BackColor = string.IsNullOrEmpty(textBoxSurname.Text) ? emptyColor : filledColor;
                textBoxName.BackColor = string.IsNullOrEmpty(textBoxName.Text) ? emptyColor : filledColor;
                textBoxPatronymic.BackColor = string.IsNullOrEmpty(textBoxPatronymic.Text) ? emptyColor : filledColor;
                maskedTextBoxPassportSeries.BackColor = string.IsNullOrEmpty(maskedTextBoxPassportSeries.Text) ? emptyColor : filledColor;
                maskedTextBoxPassportNumber.BackColor = string.IsNullOrEmpty(maskedTextBoxPassportNumber.Text) ? emptyColor : filledColor;
                maskedTextBoxPhoneNumber.BackColor = string.IsNullOrEmpty(maskedTextBoxPhoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+7", "")) ? emptyColor : filledColor;
                textBoxEmail.BackColor = string.IsNullOrEmpty(textBoxEmail.Text) ? emptyColor : filledColor;
                textBoxLogin.BackColor = string.IsNullOrEmpty(textBoxLogin.Text) ? emptyColor : filledColor;
                textBoxPassword.BackColor = string.IsNullOrEmpty(textBoxPassword.Text) ? emptyColor : filledColor;
            }

            private void pictureBoxeye_Click(object sender, EventArgs e)
            {
                isPasswordVisible = !isPasswordVisible;
                textBoxPassword.UseSystemPasswordChar = !isPasswordVisible;
                pictureBoxeye.Image = isPasswordVisible
                    ? Image.FromFile(pathImage + "eyeon.png")
                    : Image.FromFile(pathImage + "eyeoff.png");
            }

            private void buttonUploadPhoto_Click(object sender, EventArgs e)
            {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            try
            {
                // Создаем пользователя
                var user = new DBModel.User
                {
                    UserLogin = textBoxLogin.Text,
                    UserPassword = HashPassword(textBoxPassword.Text), // Хэшируем пароль
                    RoleID = 16 // RoleID для родителя
                };

                Helper.DB.User.Add(user);
                Helper.DB.SaveChanges();

                // Сохраняем фото, если оно было загружено
                if (uploadedPhoto != null)
                {
                    SaveUserPhoto(user.UserID);
                }

                // Создаем запись родителя
                var parent = new DBModel.Parents
                {
                    ParentsName = textBoxName.Text,
                    ParentsSurname = textBoxSurname.Text,
                    ParentsPatronymic = textBoxPatronymic.Text,
                    ParentsPassportSeries = maskedTextBoxPassportSeries.Text,
                    ParentsPassportNumber = maskedTextBoxPassportNumber.Text,
                    ParentsPhoneNumber = maskedTextBoxPhoneNumber.Text,
                    ParentsEmail = textBoxEmail.Text,
                     ParentsUserID = user.UserID
                };

                Helper.DB.Parents.Add(parent);
                Helper.DB.SaveChanges();

                MessageBox.Show("Родитель успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AddChild addChild = new AddChild();
                this.Close();
                addChild.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(textBoxSurname.Text) ||
                string.IsNullOrEmpty(textBoxName.Text) ||
                string.IsNullOrEmpty(textBoxPatronymic.Text) ||
                string.IsNullOrEmpty(maskedTextBoxPassportSeries.Text) ||
                string.IsNullOrEmpty(maskedTextBoxPassportNumber.Text) ||
                string.IsNullOrEmpty(maskedTextBoxPhoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+7", "")) ||
                string.IsNullOrEmpty(textBoxEmail.Text) ||
                string.IsNullOrEmpty(textBoxLogin.Text) ||
                string.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Проверка уникальности логина
            if (Helper.DB.User.Any(u => u.UserLogin == textBoxLogin.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private string HashPassword(string password)
        {
            // Простейшая реализация хэширования (в реальном проекте используйте более надежные методы)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void SaveUserPhoto(int userId)
        {
            try
            {
                if (!Directory.Exists(pathPhotoUsers))
                {
                    Directory.CreateDirectory(pathPhotoUsers);
                }
                
                string photoPath = Path.Combine(pathPhotoUsers, $"{userId}.jpg");
                uploadedPhoto.Save(photoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении фотографии: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonCancellation_Click(object sender, EventArgs e)
        {
            if (IsFormDirty())
            {
                var result = MessageBox.Show("Вы действительно хотите отменить изменения? Все введенные данные будут потеряны.",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            this.Close();
        }

        private bool IsFormDirty()
        {
            return !string.IsNullOrEmpty(textBoxSurname.Text) ||
                   !string.IsNullOrEmpty(textBoxName.Text) ||
                   !string.IsNullOrEmpty(textBoxPatronymic.Text) ||
                   !string.IsNullOrEmpty(maskedTextBoxPassportSeries.Text) ||
                   !string.IsNullOrEmpty(maskedTextBoxPassportNumber.Text) ||
                   !string.IsNullOrEmpty(maskedTextBoxPhoneNumber.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+7", "")) ||
                   !string.IsNullOrEmpty(textBoxEmail.Text) ||
                   !string.IsNullOrEmpty(textBoxLogin.Text) ||
                   !string.IsNullOrEmpty(textBoxPassword.Text) ||
                 uploadedPhoto != null;
        }

        private void labelParentIs_Click(object sender, EventArgs e)
        {
            AddChild addChild = new AddChild();
            this.Close();
            addChild.ShowDialog();
            this.Show();
        }
    }
}
