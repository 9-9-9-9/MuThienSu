using System;
using System.Windows.Forms;

namespace Login
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            this.txtAccountFileName.Text = AccountManager.FILE_NAME;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var acc = this.txtAccount.Text.Trim();
            var pwd = this.txtPassword.Text.Trim();
            if (acc.Length == 0 || pwd.Length == 0)
            {
                // ReSharper disable StringLiteralTypo
                // ReSharper disable LocalizableElement
                MessageBox.Show("Tài khoản và mật khẩu không được để trống", "Kiểm tra lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // ReSharper restore LocalizableElement
                // ReSharper restore StringLiteralTypo
                return;
            }
            AccountManager.Save(acc, pwd);
            this.Close();
        }

        private void SettingForm_Shown(object sender, EventArgs e)
        {
            this.txtAccount.Focus();
        }
    }
}
