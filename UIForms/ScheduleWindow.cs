using KINDERGARDENIS.DBModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class ScheduleWindow : Form
    {
        public ScheduleWindow()
        {
            InitializeComponent();
            LoadGroups();
            LoadDaysOfWeek();
        }

        private void ScheduleWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadScheduleMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9,
            label1, label2, label3, label4, label5,
            label6, label7, label8, label9);

            // Устанавливаем текущий день недели по умолчанию
            comboBoxDayOfWeek.SelectedIndex = (int)DateTime.Now.DayOfWeek;
        }

        private void LoadGroups()
        {
            var groups = Helper.DB.Groups.ToList();

            comboBoxGroupName.Items.Clear();
            comboBoxGroupName.Items.Add("Выберите группу");

            foreach (var group in groups)
            {
                comboBoxGroupName.Items.Add(group.GroupsGroupName);
            }

            comboBoxGroupName.SelectedIndex = 0;
        }

        private void LoadDaysOfWeek()
        {
            comboBoxDayOfWeek.Items.Clear();

            // Добавляем дни недели в соответствии с System.DayOfWeek
            comboBoxDayOfWeek.Items.Add("Воскресенье");
            comboBoxDayOfWeek.Items.Add("Понедельник");
            comboBoxDayOfWeek.Items.Add("Вторник");
            comboBoxDayOfWeek.Items.Add("Среда");
            comboBoxDayOfWeek.Items.Add("Четверг");
            comboBoxDayOfWeek.Items.Add("Пятница");
            comboBoxDayOfWeek.Items.Add("Суббота");
        }

        private void comboBoxGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGroupName.SelectedIndex <= 0)
            {
                // Сброс всех полей
                ResetGroupInfo();
                ResetScheduleLabels();
                return;
            }

            string selectedGroupName = comboBoxGroupName.SelectedItem.ToString();
            var group = Helper.DB.Groups.FirstOrDefault(g => g.GroupsGroupName == selectedGroupName);

            if (group != null)
            {
                UpdateGroupInfo(group);
                LoadScheduleForGroup(group.GroupsID);
            }
        }

        private void comboBoxDayOfWeek_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGroupName.SelectedIndex > 0 && comboBoxDayOfWeek.SelectedIndex >= 0)
            {
                string selectedGroupName = comboBoxGroupName.SelectedItem.ToString();
                var group = Helper.DB.Groups.FirstOrDefault(g => g.GroupsGroupName == selectedGroupName);

                if (group != null)
                {
                    LoadScheduleForGroup(group.GroupsID);
                }
            }
        }

        private void ResetGroupInfo()
        {
            labelKolvoChild.Text = "0";
            labelNumber.Text = "";
            labelVosp.Text = "";
            labelMlVosp.Text = "";
            labelModeType.Text = "";
        }

        private void ResetScheduleLabels()
        {
            // Сбрасываем все метки расписания
            for (int i = 0; i <= 12; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    var labelName = $"labeL_{i}{j}";
                    var label = Controls.Find(labelName, true).FirstOrDefault() as Label;
                    if (label != null)
                    {
                        label.Text = "";
                    }
                }
            }
        }

        private void UpdateGroupInfo(Groups group)
        {
            labelNumber.Text = group.GroupsGroupNumber;

            int childrenCount = Helper.DB.Children.Count(c => c.ChildrenGroupsID == group.GroupsID);
            labelKolvoChild.Text = childrenCount.ToString();

            var educators = Helper.DB.Educators
                .Where(ed => ed.EducatorsGroupsID == group.GroupsID)
                .Include(ed => ed.Employees)
                .ToList();

            var mainEducator = educators.FirstOrDefault();
            var assistantEducator = educators.Skip(1).FirstOrDefault();

            labelVosp.Text = mainEducator != null ?
                $"{mainEducator.Employees.EmployeesSurname} {mainEducator.Employees.EmployeesName} {mainEducator.Employees.EmployeesPatronymic}" :
                "Не назначен";

            labelMlVosp.Text = assistantEducator != null ?
                $"{assistantEducator.Employees.EmployeesSurname} {assistantEducator.Employees.EmployeesName} {assistantEducator.Employees.EmployeesPatronymic}" :
                "Не назначен";

            var modeType = Helper.DB.ModeType.FirstOrDefault(m => m.ModTypeGroupID == group.GroupsID);
            labelModeType.Text = modeType?.ModeTypeIDName ?? "Не указан";
        }

        private void LoadScheduleForGroup(int groupId)
        {
            if (comboBoxDayOfWeek.SelectedIndex < 0) return;

            int dayOfWeekId = comboBoxDayOfWeek.SelectedIndex; // Соответствует System.DayOfWeek

            // Получаем режим группы
            var modeType = Helper.DB.ModeType.FirstOrDefault(m => m.ModTypeGroupID == groupId);
            if (modeType == null) return;

            // Получаем расписание для этого режима и дня недели
            var schedules = Helper.DB.Schedule
                .Where(s => s.ScheduleModeTypeID == modeType.ModeTypeID &&
                           s.DayOfWeek.DayOfWeekNumber == dayOfWeekId)
                .OrderBy(s => s.Time.TimeClassTime)
                .ToList();

            // Очищаем все метки
            ResetScheduleLabels();

            // Заполняем метки данными из расписания
            for (int i = 0; i < schedules.Count && i <= 12; i++)
            {
                var schedule = schedules[i];

                // Время начала
                var startLabel = Controls.Find($"labeL_{i}0", true).FirstOrDefault() as Label;
                if (startLabel != null && schedule.Time != null)
                {
                    startLabel.Text = schedule.Time.TimeClassTime.ToString(@"hh\:mm");
                }

                // Время окончания
                var endLabel = Controls.Find($"labeL_{i}1", true).FirstOrDefault() as Label;
                if (endLabel != null && schedule.Time != null)
                {
                    endLabel.Text = schedule.Time1.TimeClassTime.ToString(@"hh\:mm");
                }

                // Активность
                var activityLabel = Controls.Find($"labeL_{i}2", true).FirstOrDefault() as Label;
                if (activityLabel != null && schedule.Time != null)
                {
                    activityLabel.Text = schedule.Activity.ActivityName;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}