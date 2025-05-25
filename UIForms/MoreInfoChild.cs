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
using Word = Microsoft.Office.Interop.Word;

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
            string photoPath = Path.Combine(pathPhotoChildren + "{childId}.jpg");
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
            string defaultPhotoPath = Path.Combine(pathPhotoChildren + "images.png");
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
                        MessageBox.Show($"Данные ребенка обновлены!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void label1_Click(object sender, EventArgs e)
        {
            // Проверяем, что у нас есть выбранный ребенок
            if (childId == 0)
            {
                MessageBox.Show("Сначала сохраните данные ребенка", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Получаем данные ребенка и родителя из базы
                using (var db = new KindergartenInformationSystemEntities())
                {
                    var child = db.Children
                        .Include(c => c.Parents)
                        .FirstOrDefault(c => c.ChildrenID == childId);

                    if (child == null)
                    {
                        MessageBox.Show("Ребенок не найден", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Получаем данные заведующего (FIOZav и FIOZav2)
                    var zav = db.Employees
                        .FirstOrDefault(em => em.User.Role.RoleName.Contains("Заведующий"));

                    // Путь к шаблону
                    string templatePath = Path.Combine(Environment.CurrentDirectory,
                                                     @"Templates\WordStatementTemplate.docx");

                    if (!File.Exists(templatePath))
                    {
                        MessageBox.Show("Шаблон документа не найден", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Создаем экземпляр Word
                    var wordApp = new Word.Application();
                    wordApp.Visible = false;

                    // Открываем шаблон
                    var wordDoc = wordApp.Documents.Open(templatePath);

                    // Заполняем закладки
                    try
                    {
                        FillBookmark(wordDoc, "AdressChild", child.ChildrenAddressinSaintPetersburg);
                        FillBookmark(wordDoc, "Date1", DateTime.Now.ToString("dd.MM.yyyy"));
                        FillBookmark(wordDoc, "Date2", DateTime.Now.ToString("dd.MM.yyyy"));
                        FillBookmark(wordDoc, "DateOfBirthChild",
                                   child.ChildrenDateofBirth?.ToString("dd.MM.yyyy") ?? "");
                        FillBookmark(wordDoc, "FIOChild",
                                   $"{child.ChildrenSurname} {child.ChildrenName} {child.ChildrenPatronymic}");
                        FillBookmark(wordDoc, "FIOPar",
                                   $"{child.Parents.ParentsSurname} {child.Parents.ParentsName} {child.Parents.ParentsPatronymic}");

                        // Данные заведующего
                        if (zav != null)
                        {
                            FillBookmark(wordDoc, "FIOZav",
                                       $"{zav.EmployeesSurname} {zav.EmployeesName}");
                            FillBookmark(wordDoc, "FIOZav2",
                                       $"{zav.EmployeesSurname} {zav.EmployeesName} {zav.EmployeesPatronymic}");
                        }

                        FillBookmark(wordDoc, "ParEmail", child.Parents.ParentsEmail ?? "");
                        FillBookmark(wordDoc, "ParPhoneNum", child.Parents.ParentsPhoneNumber ?? "");
                        FillBookmark(wordDoc, "PassportNumber", child.Parents.ParentsPassportNumber ?? "");
                        FillBookmark(wordDoc, "PassportSeria", child.Parents.ParentsPassportSeries ?? "");

                        // Сохраняем документ
                        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
                        Directory.CreateDirectory(savePath);
                        string fileName = $"Заявление_{child.ChildrenSurname}_{DateTime.Now:yyyyMMdd}.docx";
                        string fullPath = Path.Combine(savePath, fileName);

                        wordDoc.SaveAs2(fullPath);
                        wordApp.Visible = true; // Показываем готовый документ

                        MessageBox.Show($"Документ сохранен: {savePath}", "Успешно",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при заполнении документа: {ex.Message}", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Не закрываем Word, чтобы пользователь мог увидеть документ
                        // Освобождаем ресурсы
                        if (wordDoc != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
                        }
                        if (wordApp != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                        }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный метод для заполнения закладки
        private void FillBookmark(Word.Document doc, string bookmarkName, string text)
        {
            try
            {
                if (doc.Bookmarks.Exists(bookmarkName))
                {
                    Word.Bookmark bookmark = doc.Bookmarks[bookmarkName];
                    bookmark.Range.Text = text;

                    // Восстанавливаем закладку (чтобы можно было заполнять несколько раз)
                    doc.Bookmarks.Add(bookmarkName, bookmark.Range);
                }
                else
                {
                    // Для отладки - можно закомментировать в релизе
                    // MessageBox.Show($"Закладка {bookmarkName} не найдена", "Предупреждение", 
                    //               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении закладки {bookmarkName}: {ex.Message}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}