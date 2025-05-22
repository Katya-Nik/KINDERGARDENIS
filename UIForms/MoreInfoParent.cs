using KINDERGARDENIS.DBModel;
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
    public partial class MoreInfoParent : Form
    {
        private Parents parent;
        private bool isPasswordVisible = false;
        private static string pathResources = Environment.CurrentDirectory + @"\Resources\";
        private static string pathPhotoUsers = Environment.CurrentDirectory + @"\PhotoUsers\";

        public MoreInfoParent(Parents parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void MoreInfoParent_Load(object sender, EventArgs e)
        {
            LoadParentData();
            LoadParentPhoto();
            pictureBoxeye.Image = Image.FromFile(pathResources + "eyeon.png");
        }

        private void LoadParentData()
        {
            textBoxSurname.Text = parent.ParentsSurname;
            textBoxName.Text = parent.ParentsName;
            textBoxPatronymic.Text = parent.ParentsPatronymic;
            maskedTextBoxPassportSeries.Text = parent.ParentsPassportSeries;
            maskedTextBoxPassportNumber.Text = parent.ParentsPassportNumber;
            maskedTextBoxPhoneNumber.Text = parent.ParentsPhoneNumber;
            textBoxEmail.Text = parent.ParentsEmail;
            textBoxLogin.Text = parent.User.UserLogin;
            textBoxPassword.Text = parent.User.UserPassword;
            textBoxPassword.UseSystemPasswordChar = true;
        }

        private void LoadParentPhoto()
        {
            string photoPath = pathPhotoUsers + parent.ParentsID + ".jpg";
            if (File.Exists(photoPath))
            {
                pictureBoxEmployees.Image = Image.FromFile(photoPath);
            }
            else
            {
                pictureBoxEmployees.Image = Image.FromFile(pathPhotoUsers + "images.png");
            }
        }

        private void pictureBoxeye_Click(object sender, EventArgs e)
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    using (var db = new KindergartenInformationSystemEntities())
                    {
                        var parentToUpdate = db.Parents.Find(parent.ParentsID);
                        if (parentToUpdate != null)
                        {
                            // Обновляем данные родителя
                            parentToUpdate.ParentsSurname = textBoxSurname.Text;
                            parentToUpdate.ParentsName = textBoxName.Text;
                            parentToUpdate.ParentsPatronymic = textBoxPatronymic.Text;
                            parentToUpdate.ParentsPassportSeries = maskedTextBoxPassportSeries.Text;
                            parentToUpdate.ParentsPassportNumber = maskedTextBoxPassportNumber.Text;
                            parentToUpdate.ParentsPhoneNumber = maskedTextBoxPhoneNumber.Text;
                            parentToUpdate.ParentsEmail = textBoxEmail.Text;

                            // Обновляем данные пользователя
                            var user = db.User.Find(parentToUpdate.User.UserID);
                            if (user != null)
                            {
                                user.UserLogin = textBoxLogin.Text;
                                user.UserPassword = textBoxPassword.Text;
                            }

                            db.SaveChanges();

                            // Сохраняем фото, если оно было изменено
                            if (pictureBoxEmployees.Image != null &&
                                !pictureBoxEmployees.ImageLocation.Contains("images.png"))
                            {
                                string photoPath = pathPhotoUsers + parentToUpdate.ParentsID + ".jpg";
                                pictureBoxEmployees.Image.Save(photoPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }

                            MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBoxSurname.Text) ||
                string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxPatronymic.Text) ||
                maskedTextBoxPassportSeries.Text.Length != 4 ||
                maskedTextBoxPassportNumber.Text.Length != 6 ||
                maskedTextBoxPhoneNumber.Text.Replace(" ", "").Length != 16 ||
                string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(textBoxLogin.Text) ||
                string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!textBoxEmail.Text.Contains("@") || !textBoxEmail.Text.Contains("."))
            {
                MessageBox.Show("Пожалуйста, введите корректный email!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void buttonCancellation_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите отменить изменения? Все несохраненные данные будут потеряны.",
                                          "Подтверждение отмены",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void pictureBoxEmployees_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фотографию"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBoxEmployees.Image = Image.FromFile(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
