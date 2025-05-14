using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    public static class FormManager
    {
        // Текущее открытое окно (для контроля закрытия)
        public static Form CurrentForm { get; private set; }

        // Метод для перехода между формами
        public static void OpenForm(Form newForm, Form currentForm)
        {
            CurrentForm = newForm;
            currentForm.FormClosed += (s, args) => newForm.Show();
            newForm.FormClosed += (s, args) => Application.Exit(); // Закрытие приложения, если новое окно закрыто

            currentForm.Hide();
            newForm.Show();
        }

        // Метод для возврата к Authorization
        public static void ReturnToAuthorization(Form currentForm)
        {
            var authForm = new Authorization();
            currentForm.FormClosed += (s, args) => authForm.Show();
            currentForm.Close();
        }
    }
}
