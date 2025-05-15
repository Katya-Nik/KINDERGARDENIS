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
    public partial class AddChild : Form
    {
        private static string pathImage = Environment.CurrentDirectory + @"\Resources\";
        private static string pathPhotoChildren = Environment.CurrentDirectory + @"\PhotoChildren\";
        private string photoPath = "";
        private bool isPhotoUploaded = false;

        public AddChild()
        {
            InitializeComponent();
        }

        private void AddChild_Load(object sender, EventArgs e)
        {
            // Загрузка пола
            comboBoxGender.Items.Add("мужчина");
            comboBoxGender.Items.Add("женщина");

            // Загрузка родителей
            LoadParents();

            // Загрузка групп
            LoadGroups();

            // Установка масок
            maskedTextBoxDateofBirth.Mask = "00/00/0000";
            maskedTextBoxSNILS.Mask = "000-000-000 00";
            maskedTextBoxPolisOMS.Mask = "0000000000000000";

            // Установка изображения по умолчанию
            pictureBoxChild.Image = Image.FromFile(pathImage + "AddImage.png");

            // Подписка на события изменения текста
            SubscribeToTextChangedEvents();
        }

        private void LoadParents()
        {
            var parents = Helper.DB.Parents.ToList();
            foreach (var parent in parents)
            {
                comboBoxParent.Items.Add($"{parent.ParentsSurname} {parent.ParentsName}");
            }
        }

        private void LoadGroups()
        {
            var groups = Helper.DB.Groups.ToList();
            foreach (var group in groups)
            {
                comboBoxGroups.Items.Add(group.GroupsGroupName);
            }
        }

        private void SubscribeToTextChangedEvents()
        {
            textBoxSurname.TextChanged += TextBox_TextChanged;
            textBoxName.TextChanged += TextBox_TextChanged;
            textBoxPatronymic.TextChanged += TextBox_TextChanged;
            comboBoxGender.TextChanged += TextBox_TextChanged;
            maskedTextBoxDateofBirth.TextChanged += TextBox_TextChanged;
            maskedTextBoxSNILS.TextChanged += TextBox_TextChanged;
            textBoxCHNDOC.TextChanged += TextBox_TextChanged;
            textBoxCHSDOC.TextChanged += TextBox_TextChanged;
            maskedTextBoxPolisOMS.TextChanged += TextBox_TextChanged;
            textBoxAddress.TextChanged += TextBox_TextChanged;
            textBoxTuitionFee.TextChanged += TextBox_TextChanged;
            comboBoxParent.TextChanged += TextBox_TextChanged;
            comboBoxGroups.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (string.IsNullOrWhiteSpace(control.Text))
            {
                control.BackColor = Color.FromArgb(133, 223, 234);
            }
            else
            {
                control.BackColor = Color.FromArgb(145, 195, 173);
            }
        }

        private void buttonUploadPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                photoPath = openFileDialog.FileName;
                pictureBoxChild.Image = Image.FromFile(photoPath);
                isPhotoUploaded = true;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Получаем выбранного родителя
                string selectedParent = comboBoxParent.SelectedItem.ToString();
                string[] parentParts = selectedParent.Split(' ');
                var parent = Helper.DB.Parents.FirstOrDefault(p => p.ParentsSurname == parentParts[0] && p.ParentsName == parentParts[1]);

                // Получаем выбранную группу
                string selectedGroup = comboBoxGroups.SelectedItem.ToString();
                var group = Helper.DB.Groups.FirstOrDefault(g => g.GroupsGroupName == selectedGroup);

                // Создаем нового ребенка
                var child = new DBModel.Children
                {
                    ChildrenSurname = textBoxSurname.Text,
                    ChildrenName = textBoxName.Text,
                    ChildrenPatronymic = textBoxPatronymic.Text,
                    ChildrenGender = comboBoxGender.SelectedItem.ToString(),
                    ChildrenDateofBirth = DateTime.ParseExact(maskedTextBoxDateofBirth.Text, "dd/MM/yyyy", null),
                    ChildrenSNILS = maskedTextBoxSNILS.Text,
                    ChildrenCHNDOC = textBoxCHNDOC.Text,
                    ChildrenCHSDOC = textBoxCHSDOC.Text,
                    ChildrenPolisOMS = maskedTextBoxPolisOMS.Text,
                    ChildrenAddressinSaintPetersburg = textBoxAddress.Text,
                    ChildrenTuitionFee = decimal.Parse(textBoxTuitionFee.Text),
                    ChildrenParentsID = parent.ParentsID,
                    ChildrenGroupsID = group.GroupsID
                };

                // Сохраняем ребенка в БД
                Helper.DB.Children.Add(child);
                Helper.DB.SaveChanges();

                // Сохраняем фото, если оно было загружено
                if (isPhotoUploaded)
                {
                    if (!Directory.Exists(pathPhotoChildren))
                    {
                        Directory.CreateDirectory(pathPhotoChildren);
                    }

                    string photoExtension = Path.GetExtension(photoPath);
                    string newPhotoPath = Path.Combine(pathPhotoChildren, $"{child.ChildrenID}{photoExtension}");
                    File.Copy(photoPath, newPhotoPath, true);
                }

                MessageBox.Show("Ребенок успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(textBoxSurname.Text)) isValid = false;
            if (string.IsNullOrWhiteSpace(textBoxName.Text)) isValid = false;
            if (string.IsNullOrWhiteSpace(textBoxPatronymic.Text)) isValid = false;
            if (comboBoxGender.SelectedIndex == -1) isValid = false;
            if (!maskedTextBoxDateofBirth.MaskCompleted) isValid = false;
            if (!maskedTextBoxSNILS.MaskCompleted) isValid = false;
            if (string.IsNullOrWhiteSpace(textBoxCHNDOC.Text)) isValid = false;
            if (string.IsNullOrWhiteSpace(textBoxCHSDOC.Text)) isValid = false;
            if (!maskedTextBoxPolisOMS.MaskCompleted) isValid = false;
            if (string.IsNullOrWhiteSpace(textBoxAddress.Text)) isValid = false;
            if (string.IsNullOrWhiteSpace(textBoxTuitionFee.Text)) isValid = false;
            if (comboBoxParent.SelectedIndex == -1) isValid = false;
            if (comboBoxGroups.SelectedIndex == -1) isValid = false;

            return isValid;
        }

        private void buttonCancellation_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите отменить добавление ребенка? Все введенные данные будут потеряны.",
                                                "Подтверждение отмены",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}