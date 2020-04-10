using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Login
{
    class AccountManager
    {
        private static Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        public static readonly string FILE_NAME;

        static AccountManager() {
            string exe = System.AppDomain.CurrentDomain.FriendlyName;
            exe = exe.Substring(0, exe.Length - 4);
            FILE_NAME = exe + ".txt";
        }

        public static void Load()
        {
            accounts.Clear();
            Account account = null;

            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(FILE_NAME);
            }
            catch
            {
            }
            if (lines != null)
            {
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (account == null)
                    {
                        account = new Account { AccountName = line.Trim() };
                    }
                    else
                    {
                        account.EncryptedPassword = line.Trim();
                        accounts.Add(account.AccountName, account);
                        account = null;
                    }
                }
            }
            if (account != null)
            {
                throw new Exception("File " + FILE_NAME + " bị sai định dạng rồi");
            }
        }

        public static List<Account> Accounts
        {
            get
            {
                List<Account> result = new List<Account>();
                foreach (var item in accounts.Values)
                {
                    result.Add(item);
                };
                return result;
            }
        }

        public static void Save(string AccountName, string Password)
        {
            Account account = new Account { AccountName = AccountName.Trim(), Password = Password };
            if (accounts.ContainsKey(account.AccountName))
            {
                accounts[account.AccountName] = account;
            }
            else
            {
                accounts.Add(account.AccountName, account);
            }

            List<string> lines = new List<string>();
            foreach (var acc in Accounts)
            {
                lines.Add(acc.AccountName);
                lines.Add(acc.EncryptedPassword);
                lines.Add("");
            }
            File.WriteAllLines(FILE_NAME, lines.ToArray(), Encoding.UTF8);
        }
    }
}
