using KINDERGARDENIS.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class MoreInfoChild : Form
    {
        private int childId;
        private bool isNewChild;
        private string pathPhotoChildren = Environment.CurrentDirectory + @"\PhotoChildren\";
        private KindergartenInformationSystemEntities db;
        private ErrorProvider errorProvider;

        public MoreInfoChild(int id = 0)
        {
            InitializeComponent();
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            childId = id;
            isNewChild = id == 0;
        }

        private void MoreInfoChild_Load(object sender, EventArgs e)
        {
            db = new KindergartenInformationSystemEntities();

            // Загрузка данных в комбобоксы
            comboBoxGender.Items.AddRange(new string[] { "мужчина", "женщина" });
            comboBoxParent.DataSource = db.Parents.Select(p => new { p.ParentsID, FullName = p.ParentsSurname + " " + p.ParentsName }).ToList();
            comboBoxParent.DisplayMember = "FullName";
            comboBoxParent.ValueMember = "ParentsID";

            comboBoxGroups.DataSource = db.Groups.ToList();
            comboBoxGroups.DisplayMember = "GroupsGroupName";
            comboBoxGroups.ValueMember = "GroupsID";

            if (!isNewChild)
            {
                // Загрузка данных существующего ребенка
                var child = db.Children.Find(childId);
                if (child != null)
                {
                    textBoxSurname.Text = child.ChildrenSurname;
                    textBoxName.Text = child.ChildrenName;
                    textBoxPatronymic.Text = child.ChildrenPatronymic;
                    comboBoxGender.SelectedItem = child.ChildrenGender;
                    maskedTextBoxDateofBirth.Text = child.ChildrenDateofBirth?.ToString("dd/MM/yyyy");
                    maskedTextBoxSNILS.Text = child.ChildrenSNILS;
                    textBoxCHNDOC.Text = child.ChildrenCHNDOC;
                    textBoxCHSDOC.Text = child.ChildrenCHSDOC;
                    maskedTextBoxPolisOMS.Text = child.ChildrenPolisOMS;
                    textBoxAddress.Text = child.ChildrenAddressinSaintPetersburg;
                    textBoxTuitionFee.Text = child.ChildrenTuitionFee.ToString();
                    comboBoxParent.SelectedValue = child.ChildrenParentsID;
                    comboBoxGroups.SelectedValue = child.ChildrenGroupsID;

                    // Загрузка фото
                    LoadChildPhoto(childId);
                }
            }
            else
            {
                // Установка значений по умолчанию для нового ребенка
                comboBoxGender.SelectedIndex = 0;
                LoadDefaultPhoto();
            }
        }

        private void LoadChildPhoto(int childId)
        {
            string photoPath = Path.Combine(pathPhotoChildren, $"{childId}.jpg");
            if (File.Exists(photoPath))
            {
                pictureBoxChild.Image = Image.FromFile(photoPath);
            }
            else
            {
                LoadDefaultPhoto();
            }
        }

        private Image LoadDefaultPhoto()
        {
            string defaultPhotoPath = Path.Combine(pathPhotoChildren, "images.png");
            if (File.Exists(defaultPhotoPath))
            {
                return Image.FromFile(defaultPhotoPath);
            }
            return null;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                var result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        Children child;
                        if (isNewChild)
                        {
                            child = new Children();
                            db.Children.Add(child);
                        }
                        else
                        {
                            child = db.Children.Find(childId);
                        }

                        // Заполнение данных ребенка
                        child.ChildrenSurname = textBoxSurname.Text;
                        child.ChildrenName = textBoxName.Text;
                        child.ChildrenPatronymic = textBoxPatronymic.Text;
                        child.ChildrenGender = comboBoxGender.SelectedItem.ToString();
                        child.ChildrenDateofBirth = DateTime.ParseExact(maskedTextBoxDateofBirth.Text, "dd/MM/yyyy", null);
                        child.ChildrenSNILS = maskedTextBoxSNILS.Text;
                        child.ChildrenCHNDOC = textBoxCHNDOC.Text;
                        child.ChildrenCHSDOC = textBoxCHSDOC.Text;
                        child.ChildrenPolisOMS = maskedTextBoxPolisOMS.Text;
                        child.ChildrenAddressinSaintPetersburg = textBoxAddress.Text;
                        child.ChildrenTuitionFee = decimal.Parse(textBoxTuitionFee.Text);
                        child.ChildrenParentsID = (int)comboBoxParent.SelectedValue;
                        child.ChildrenGroupsID = (int)comboBoxGroups.SelectedValue;

                        db.SaveChanges();

                        if (isNewChild)
                        {
                            // Сохранение фото для нового ребенка
                            childId = child.ChildrenID;
                            SaveChildPhoto();
                        }

                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveChildPhoto()
        {
            if (pictureBoxChild.Image != null && !pictureBoxChild.Image.Equals(LoadDefaultPhoto()))
            {
                if (!Directory.Exists(pathPhotoChildren))
                {
                    Directory.CreateDirectory(pathPhotoChildren);
                }

                string photoPath = Path.Combine(pathPhotoChildren, $"{childId}.jpg");
                pictureBoxChild.Image.Save(photoPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void buttonCancellation_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateChildren()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(textBoxSurname.Text))
            {
                errorProvider.SetError(textBoxSurname, "Введите фамилию");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                errorProvider.SetError(textBoxName, "Введите имя");
                isValid = false;
            }

            if (comboBoxGender.SelectedIndex == -1)
            {
                errorProvider.SetError(comboBoxGender, "Выберите пол");
                isValid = false;
            }

            if (!DateTime.TryParseExact(maskedTextBoxDateofBirth.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                errorProvider.SetError(maskedTextBoxDateofBirth, "Введите корректную дату рождения");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBoxAddress.Text))
            {
                errorProvider.SetError(textBoxAddress, "Введите адрес");
                isValid = false;
            }

            if (!decimal.TryParse(textBoxTuitionFee.Text, out _))
            {
                errorProvider.SetError(textBoxTuitionFee, "Введите корректную сумму оплаты");
                isValid = false;
            }

            return isValid;
        }

        private void buttonLoadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        pictureBoxChild.Image = Image.FromFile(openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}