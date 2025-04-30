using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public partial class ScheduleWindow : Form
    {
        public ScheduleWindow()
        {
            InitializeComponent();
        }

        private void ScheduleWindow_Load(object sender, EventArgs e)
        {
            UserInfoService.LoadUserInfo(pictureBoxPhotoUsers, labelUsername, labelUserEmaile);
            MenuService.LoadScheduleMenu(pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10,
            label1, label2, label3, label4, label5,
            label6, label7, label8, label9, label10);
            ScheduleGroups();
        }

        private void ScheduleGroups()
        {
            List<DBModel.Schedule> schedules = Helper.DB.Schedule.ToList();

        }

    }
}
