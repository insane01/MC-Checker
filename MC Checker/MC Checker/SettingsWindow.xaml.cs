using MahApps.Metro.Controls;
using MC_Checker.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MC_Checker
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            if (Data.AuthType == AuthType.SITE)
            {
                AuthMethod.SelectedIndex = 0;
            }
            else
            {
                AuthMethod.SelectedIndex = 1;
            }

            SaveSQBox.IsChecked = Data.SaveSQ;
            SaveWPBox.IsChecked = Data.SaveWithoutPremium;
        }

        private void OpenResutlsBtn_Click(object sender, RoutedEventArgs e)
        {
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DCloud\\MC Checker";

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            Process.Start(Path);
        }

        private void AuthMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthMethod.SelectedIndex == 0)
            {
                Data.AuthType = AuthType.SITE;
            }
            else
            {
                Data.AuthType = AuthType.CLIENT;
            }
        }

        private void SaveSQBox_Checked(object sender, RoutedEventArgs e)
        {
            Data.SaveSQ = (bool)SaveSQBox.IsChecked;
        }

        private void SaveWPBox_Checked(object sender, RoutedEventArgs e)
        {
            Data.SaveWithoutPremium = (bool)SaveWPBox.IsChecked;
        }
    }
}
