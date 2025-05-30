﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KINDERGARDENIS.UIForms
{
    internal class MenuService
    {
        private static string pathIconlyON = Environment.CurrentDirectory + @"\IconlyON\";
        private static string pathIconlyOFF = Environment.CurrentDirectory + @"\IconlyOFF\";

        public static void LoadMainMenu(
            PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
            PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
            Label label1, Label label2, Label label3, Label label4, Label label5,
            Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyON + "home.png");
            pictureBox2.Load(pathIconlyOFF + "calendar.png");
            pictureBox3.Load(pathIconlyOFF + "man.png");
            pictureBox4.Load(pathIconlyOFF + "people.png");
            pictureBox5.Load(pathIconlyOFF + "happy.png");
            pictureBox6.Load(pathIconlyOFF + "woman.png");
            pictureBox7.Load(pathIconlyOFF + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }

        public static void LoadScheduleMenu(
            PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
            PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
            Label label1, Label label2, Label label3, Label label4, Label label5,
            Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyOFF + "home.png");
            pictureBox2.Load(pathIconlyON + "calendar.png");
            pictureBox3.Load(pathIconlyOFF + "man.png");
            pictureBox4.Load(pathIconlyOFF + "people.png");
            pictureBox5.Load(pathIconlyOFF + "happy.png");
            pictureBox6.Load(pathIconlyOFF + "woman.png");
            pictureBox7.Load(pathIconlyOFF + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }

        public static void LoadEducatorsMenu(
            PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
            PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
            Label label1, Label label2, Label label3, Label label4, Label label5,
            Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyOFF + "home.png");
            pictureBox2.Load(pathIconlyOFF + "calendar.png");
            pictureBox3.Load(pathIconlyON + "man.png");
            pictureBox4.Load(pathIconlyOFF + "people.png");
            pictureBox5.Load(pathIconlyOFF + "happy.png");
            pictureBox6.Load(pathIconlyOFF + "woman.png");
            pictureBox7.Load(pathIconlyOFF + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }

        public static void LoadGroupMenu(
            PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
            PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
            Label label1, Label label2, Label label3, Label label4, Label label5,
            Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyOFF + "home.png");
            pictureBox2.Load(pathIconlyOFF + "calendar.png");
            pictureBox3.Load(pathIconlyOFF + "man.png");
            pictureBox4.Load(pathIconlyON + "people.png");
            pictureBox5.Load(pathIconlyOFF + "happy.png");
            pictureBox6.Load(pathIconlyOFF + "woman.png");
            pictureBox7.Load(pathIconlyOFF + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }

        public static void LoadChidrenlMenu(
           PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
           PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
           Label label1, Label label2, Label label3, Label label4, Label label5,
           Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyOFF + "home.png");
            pictureBox2.Load(pathIconlyOFF + "calendar.png");
            pictureBox3.Load(pathIconlyOFF + "man.png");
            pictureBox4.Load(pathIconlyOFF + "people.png");
            pictureBox5.Load(pathIconlyON + "happy.png");
            pictureBox6.Load(pathIconlyOFF + "woman.png");
            pictureBox7.Load(pathIconlyOFF + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }

        public static void LoadParentlMenu(
           PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
           PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
           Label label1, Label label2, Label label3, Label label4, Label label5,
           Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyOFF + "home.png");
            pictureBox2.Load(pathIconlyOFF + "calendar.png");
            pictureBox3.Load(pathIconlyOFF + "man.png");
            pictureBox4.Load(pathIconlyOFF + "people.png");
            pictureBox5.Load(pathIconlyOFF + "happy.png");
            pictureBox6.Load(pathIconlyON + "woman.png");
            pictureBox7.Load(pathIconlyOFF + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }

        public static void LoadUserlMenu(
           PictureBox pictureBox1, PictureBox pictureBox2, PictureBox pictureBox3, PictureBox pictureBox4, PictureBox pictureBox5,
           PictureBox pictureBox6, PictureBox pictureBox7, PictureBox pictureBox8, PictureBox pictureBox9,
           Label label1, Label label2, Label label3, Label label4, Label label5,
           Label label6, Label label7, Label label8, Label label9)
        {
            if (Helper.users == null)
            {
                MessageBox.Show("Пользователь не загружен!");
                return;
            }

            // Загрузка иконок
            pictureBox1.Load(pathIconlyOFF + "home.png");
            pictureBox2.Load(pathIconlyOFF + "calendar.png");
            pictureBox3.Load(pathIconlyOFF + "man.png");
            pictureBox4.Load(pathIconlyOFF + "people.png");
            pictureBox5.Load(pathIconlyOFF + "happy.png");
            pictureBox6.Load(pathIconlyOFF + "woman.png");
            pictureBox7.Load(pathIconlyON + "person.png");

            // Установка текста меток
            label1.Text = "Главная";
            label2.Text = "Расписание";
            label3.Text = "Сотрудники";
            label4.Text = "Группы";
            label5.Text = "Дети";
            label6.Text = "Родители";
            label7.Text = "Пользователи";
        }
    }
}
