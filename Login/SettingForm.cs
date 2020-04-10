using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
            string acc = this.txtAccount.Text.Trim();
            string pwd = this.txtPassword.Text.Trim();
            if (acc.Length == 0 || pwd.Length == 0)
            {
                MessageBox.Show("Tài khoản và mật khẩu không được để trống", "Kiểm tra lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
