using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Login
{
    public partial class MainForm : Form
    {
        // ReSharper disable InconsistentNaming
        private const string HKEY_CURRENT_USER_SOFTWARE = @"Software\Webzen\MU\Config";
        private const string HKEY_CURRENT_USER_SUB_KEY = "ID";
        // ReSharper restore InconsistentNaming

        public MainForm()
        {
            InitializeComponent();
            this.btnLogin.FlatAppearance.BorderSize = //
            this.btnSetting.FlatAppearance.BorderSize = //
            this.btnCopyPassword.FlatAppearance.BorderSize = 0;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            new SettingForm().ShowDialog();
            ReloadConfig();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (cbbAccounts.SelectedIndex < 0)
                return;

            Account selected = GetSelectedAccount();

            using (var explorerKey =
                Registry.CurrentUser.OpenSubKey(HKEY_CURRENT_USER_SOFTWARE, writable: true))
            {
                explorerKey?.SetValue(HKEY_CURRENT_USER_SUB_KEY, selected.AccountName);
            }

            ClipboardSetText(selected.Password);
            StartClient();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!File.Exists("main.exe"))
            {
                // ReSharper disable once LocalizableElement
                MessageBox.Show("Bác cho file này vào thư mục MU Legend thì mới được nhé");
                Environment.Exit(1);
            }

            ToolTip toolTip1 = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 500,
                ShowAlways = true
            };
            toolTip1.SetToolTip(this.btnSetting, "Cài đặt");
            toolTip1.SetToolTip(this.cbbAccounts, "Danh sách tài khoản");
            toolTip1.SetToolTip(this.btnCopyPassword, "Copy mật khẩu (Ctrl V để paste)");
            toolTip1.SetToolTip(this.btnLogin, "Khởi động");
            ReloadConfig();
        }

        private void ReloadConfig()
        {
            AccountManager.Load();
            this.cbbAccounts.DataSource = AccountManager.Accounts;
            this.cbbAccounts.DisplayMember = "AccountName";
        }

        private void StartClient()
        {
            var dir = Directory.GetCurrentDirectory();
            var exe = Path.Combine(dir, "main.exe");
            var prc = new Process();
            var prs = new ProcessStartInfo
            {
                WorkingDirectory = dir,
                UseShellExecute = false,
                Arguments = "66316802",
                FileName = exe
            };
            prc.StartInfo = prs;
            prc.Start();
        }

        private void ClipboardSetText(string inTextToCopy)
        {
            var clipboardThread = new Thread(() => ClipBoardThreadWorker(inTextToCopy));
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.IsBackground = false;
            clipboardThread.Start();
        }

        private void ClipBoardThreadWorker(string inTextToCopy)
        {
            Clipboard.SetText(inTextToCopy, TextDataFormat.Text);
        }

        private void btnCopyPassword_Click(object sender, EventArgs e)
        {
            if (this.cbbAccounts.SelectedIndex < 0)
                return;
            var selected = GetSelectedAccount();
            ClipboardSetText(selected.Password);
        }

        private Account GetSelectedAccount()
        {
            return (Account)this.cbbAccounts.SelectedItem;
        }
    }
}
