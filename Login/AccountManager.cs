using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Login
{
    class AccountManager
    {
        private static readonly Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        public static readonly string FileName;

        static AccountManager()
        {
            var exe = AppDomain.CurrentDomain.FriendlyName;
            exe = exe.Substring(0, exe.Length - 4);
            FileName = exe + ".txt";
        }

        public static void Load()
        {
            accounts.Clear();
            Account account = null;

            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(FileName);
            }
            catch
            {
                //
            }
            if (lines != null)
            {
                foreach (var line in lines)
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
                // ReSharper disable StringLiteralTypo
                throw new Exception("File " + FileName + " bị sai định dạng rồi");
                // ReSharper restore StringLiteralTypo
            }
        }

        public static List<Account> Accounts
        {
            get
            {
                var result = new List<Account>();
                foreach (var item in accounts.Values)
                {
                    result.Add(item);
                }
                return result;
            }
        }

        public static void Save(string accountName, string password)
        {
            var account = new Account { AccountName = accountName.Trim(), Password = password };
            if (accounts.ContainsKey(account.AccountName))
            {
                accounts[account.AccountName] = account;
            }
            else
            {
                accounts.Add(account.AccountName, account);
            }

            var lines = new List<string>();
            foreach (var acc in Accounts)
            {
                lines.Add(acc.AccountName);
                lines.Add(acc.EncryptedPassword);
                lines.Add("");
            }
            File.WriteAllLines(FileName, lines.ToArray(), Encoding.UTF8);
        }
    }
}
