using MahApps.Metro.Controls;
using MC_Checker.Enums;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace MC_Checker
{
    public partial class MainWindow : MetroWindow
    {
        private List<Thread> ThreadsList = new List<Thread>();
        private Object Sync = new object();
        private Object Sync2 = new object();

        private int ThreadsCount;
        private string Path;
        private bool IsStopping;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadAccountsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text document | *.txt";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                Data.AccountsList.Clear();

                string[] accounts = File.ReadAllLines(openFileDialog.FileName);

                for (int i = 0; i < accounts.Length; i++)
                {
                    Data.AccountsList.Add(accounts[i]);
                }

                AccountsLabel.Content = Data.AccountsList.Count;
            }
        }

        private void LoadProxyBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text document | *.txt";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                Data.ProxyList.Clear();

                string[] accounts = File.ReadAllLines(openFileDialog.FileName);

                for (int i = 0; i < accounts.Length; i++)
                {
                    Data.ProxyList.Add(accounts[i]);
                }

                ProxyLabel.Content = Data.ProxyList.Count;
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Data.AccountsList.Count == 0)
            {
                MessageBox.Show("Please load accounts before checking.");
                return;
            }

            if (Data.ProxyType != ProxyType.NO_PROXY && Data.ProxyList.Count == 0)
            {
                MessageBox.Show("Please load proxy or disable their usage.");
                return;
            }

            Path = GetPath();
            Directory.CreateDirectory(Path);

            switch (ProxyTypeBox.SelectedIndex)
            {
                case 1:
                    Data.ProxyType = ProxyType.HTTPS;
                    break;
                case 2:
                    Data.ProxyType = ProxyType.SOCKS4;
                    break;
                case 3:
                    Data.ProxyType = ProxyType.SOCKS5;
                    break;
                default:
                    Data.ProxyType = ProxyType.NO_PROXY;
                    break;
            }

            Data.CurrentAccount = 0;
            Data.Goods = 0;

            Data.Premium = 0;
            Data.SecretQuestion = 0;
            Data.Gifts = 0;

            Data.Errors = 0;

            UpdateWindow();

            LoadAccountsBtn.IsEnabled = false;
            LoadProxyBtn.IsEnabled = false;

            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;
            SettingsBtn.IsEnabled = false;
            ProxyTypeBox.IsEnabled = false;

            for (int i = 0; i < 20; i++)
            {
                Thread thread = new Thread(Checker);
                thread.IsBackground = true;
                thread.Start();

                ThreadsCount++;
                ThreadsList.Add(thread);
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            StopBtn.IsEnabled = false;
            IsStopping = true;
        }

        private void Checker()
        {
            while (true)
            {
                string Account = string.Empty;

                if (IsStopping)
                {
                    RemoveThread();
                    break;
                }

                lock (Sync)
                {
                    if (Data.CurrentAccount >= Data.AccountsList.Count)
                    {
                        RemoveThread();
                        break;
                    }

                    Account = Data.GetCurrentAccount();
                    Data.CurrentAccount++;
                }

                var s = Account.Replace(":", ";").Split(";".ToCharArray());

                var username = s[0];
                var password = s[1];

                var account = (Data.AuthType == AuthType.SITE) ?
                    Mojang.GetAccountBySite(username, password) :
                    Mojang.GetAccountByClient(username, password);

                if (account.IsValid)
                {
                    Data.Goods++;

                    if (account.IsPremium)
                    {
                        Data.Premium++;
                    }

                    if (account.IsSecretQuestion)
                    {
                        Data.SecretQuestion++;
                    }

                    if (account.HasGifts)
                    {
                        Data.Gifts++;
                    }
                }
                else
                {
                    Data.Errors++;
                }

                UpdateWindow();

                lock (Sync2)
                {
                    SaveAccount(account);
                }
            }
        }

        private void UpdateWindow()
        {
            GoodsLabel.Invoke(new Action(() => GoodsLabel.Content = Data.Goods));
            PremiumLabel.Invoke(new Action(() => PremiumLabel.Content = Data.Premium));
            SQLabel.Invoke(new Action(() => SQLabel.Content = Data.SecretQuestion));
            GiftsLabel.Invoke(new Action(() => GiftsLabel.Content = Data.Gifts));
            ErrorLabel.Invoke(new Action(() => ErrorLabel.Content = Data.Errors));
        }

        private void RemoveThread()
        {
            Debug.WriteLine("ThreadsCount: " + ThreadsCount);
            ThreadsCount--;

            if (ThreadsCount == 0)
            {
                ResetWindow();
            }
        }

        private void ResetWindow()
        {
            IsStopping = false;
            LoadAccountsBtn.Invoke(new Action(() => LoadAccountsBtn.IsEnabled = true));
            LoadProxyBtn.Invoke(new Action(() => LoadProxyBtn.IsEnabled = true));
            StartBtn.Invoke(new Action(() => StartBtn.IsEnabled = true));
            StopBtn.Invoke(new Action(() => StopBtn.IsEnabled = false));
            SettingsBtn.Invoke(new Action(() => SettingsBtn.IsEnabled = true));
            ProxyTypeBox.Invoke(new Action(() => ProxyTypeBox.IsEnabled = true));
        }

        private void SaveAccount(MCAccount account)
        {
            string s = string.Empty;
            string acc = account.Email + ";" + account.Password;

            MessageBox.Show(acc + " | " + account.IsValid);

            if (!account.IsValid)
            {
                File.AppendAllText(Path + "bad.txt", acc + "\r\n");
                return;
            }

            if (Data.SaveSQ && account.IsSecretQuestion)
            {
                File.AppendAllText(Path + "question.txt", acc + "\r\n");
                return;
            }

            if (!account.IsPremium && !Data.SaveWithoutPremium)
            {
                return;
            }

            if (account.AuthType == AuthType.SITE)
            {
                s = acc +
                    "\r\n===================================" +
                    "\r\nPremium: " + account.IsPremium +
                    "\r\nSecret Question: " + account.IsSecretQuestion +
                    "\r\n===================================\r\n\r\n";
            }
            else
            {
                s = acc +
                    "\r\n===================================" +
                    "\r\nNickname: " + account.Name +
                    "\r\nPremium: " + account.IsPremium +
                    "\r\nEmail verified: " + account.IsEmailVerified +
                    "\r\nBlocked: " + account.IsBlocked +
                    "\r\nSuspended: " + account.IsSuspended +
                    "\r\n===================================\r\n\r\n";
            }

            File.AppendAllText(Path + "goods.txt", s);
        }

        public static string GetPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DCloud\\MC Checker" + "\\Results " + DateTime.Now.ToString("dd.MM.yy h_mm_ss") + "\\";
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new SettingsWindow();
            window.Show();
        }
    }
}
