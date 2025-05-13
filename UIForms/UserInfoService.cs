using KINDERGARDENIS.UIForms;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public static class UserInfoService
{
    private static string pathPhotoUsers = Environment.CurrentDirectory + @"\PhotoUsers\";
    private static string pathChevron = Environment.CurrentDirectory + @"\Resources\";

    public static void LoadUserInfo(PictureBox pictureBoxPhotoUsers, PictureBox pictureBox12, Label labelUsername, Label labelUserEmaile)
    {
        if (Helper.users == null)
        {
            MessageBox.Show("Пользователь не загружен!");
            return;
        }

        string filePhotoUsers = pathPhotoUsers + Helper.users.UserID + ".jpg";
        if (!File.Exists(filePhotoUsers))
        {
            filePhotoUsers = pathPhotoUsers + "Stub.png";
        }
        pictureBoxPhotoUsers.Load(filePhotoUsers);
        
        string fileChevron = pathChevron + "chevron-forward.png";
        pictureBox12.Load(fileChevron);

        // ФИО и почта работника
        if (Helper.users != null && Helper.users.RoleID >= 1 && Helper.users.RoleID <= 15)
        {
            if (Helper.users != null && Helper.users.Employees != null)
            {
                var employee = Helper.users.Employees.FirstOrDefault();

                if (employee != null)
                {
                    labelUsername.Text = employee.EmployeesSurname + " " + employee.EmployeesName;
                    labelUserEmaile.Text = employee.EmployeesEmail;
                }
                else
                {
                    labelUsername.Text = "Нет данных о сотруднике";
                }
            }
        }
        else if (Helper.users != null && Helper.users.RoleID == 16) // ФИО и почта родителя
        {
            if (Helper.users != null && Helper.users.Parents != null)
            {
                var parents = Helper.users.Parents.FirstOrDefault();

                if (parents != null)
                {
                    labelUsername.Text = parents.ParentsSurname + " " + parents.ParentsName;
                    labelUserEmaile.Text = parents.ParentsEmail;
                }
                else
                {
                    labelUsername.Text = "Нет данных о родителе";
                }
            }
        }
    }

}