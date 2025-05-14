using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public class Helper
    {
        public static DBModel.KindergartenInformationSystemEntities DB;
        public static DBModel.User users;
        public static DBModel.Role role;
        public static DBModel.Employees employees;
        public static DBModel.Parents parents;
    }

    public static class FormHelper
    {
        public static void ShowFormAndHideCurrent(Form currentForm, Form newForm)
        {
            newForm.FormClosed += (s, args) =>
            {
                if (Application.OpenForms.Count == 0)
                {
                    currentForm.Show();
                }
            };
            newForm.Show();
            currentForm.Hide();
        }
    }
}
