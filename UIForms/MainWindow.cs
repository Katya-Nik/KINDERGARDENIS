using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KINDERGARDENIS.UIForms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            this.FormClosed += (s, args) => new Authorization().Show();
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, pictureBox12, labelUsername, labelUserEmaile);
            MenuService.LoadMainMenu( pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9,
            label1, label2, label3, label4, label5,
            label6, label7, label8, label9);
            MainWin();
        }

        private void MainWin()
        {
            List<DBModel.Children> children = Helper.DB.Children.ToList();
            List<DBModel.Groups> groups = Helper.DB.Groups.ToList();
            List<DBModel.Employees> employees = Helper.DB.Employees.ToList();

            // Количество детей/груп/сотрудников
            labelKD.Text = children.Count.ToString();
            labelKG.Text = groups.Count.ToString();
            labelKS.Text = employees.Count.ToString();

            // Колличество детей по группам
            // Очищаем предыдущие данные
            chartKDVG.Series.Clear();
            chartKDVG.Titles.Clear();

            // Настройка внешнего вида chart
            chartKDVG.BackColor = Color.FromArgb(238, 245, 245);
            chartKDVG.ChartAreas[0].BackColor = Color.FromArgb(238, 245, 245);

            // Настройка области диаграммы
            var chartArea = chartKDVG.ChartAreas[0];
            chartArea.AxisX.Title = "Группы";
            chartArea.AxisY.Title = "Количество детей";
            chartArea.AxisX.MajorGrid.LineColor = Color.Black;
            chartArea.AxisY.MajorGrid.LineColor = Color.Black;

            // Группируем детей по группам и считаем количество
            var childrenCountByGroup = children
                .GroupBy(c => c.ChildrenGroupsID)
                .Select(g => new
                {
                    GroupId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Создаем серию данных
            var series = new Series
            {
                Name = "Количество детей",
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(100, 145, 145),
                IsValueShownAsLabel = false // Отключаем подписи значений над столбцами
            };

            // Заполняем данными только активные группы (где есть дети)
            foreach (var group in groups.OrderBy(g => g.GroupsID))
            {
                var countInfo = childrenCountByGroup.FirstOrDefault(c => c.GroupId == group.GroupsID);
                if (countInfo != null)
                {
                    series.Points.AddXY(group.GroupsGroupName, countInfo.Count);
                }
            }

            // Добавляем серию на chart
            chartKDVG.Series.Add(series);

            // Настраиваем оси
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Interval = 5;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Эта форма
        }

        private void label2_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ScheduleWindow(), this);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new EmployeesWindow(), this);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new GroupWindow(), this);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ChildrenWindow(), this);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new UsersWindow(), this);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm(new ParentsWindow(), this);
        }
    }
}
