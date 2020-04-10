using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Login
{
    class Account
    {
        public string AccountName { get; set; }

        public string Password { get; set; }

        public string EncryptedPassword
        {
            get
            {
                byte[] b = Encoding.UTF8.GetBytes(this.Password);
                BitArray bits = new BitArray(b);
                Reverse(bits);
                bits.CopyTo(b, 0);
                return System.Convert.ToBase64String(b);
            }
            set
            {
                byte[] b = System.Convert.FromBase64String(value);
                BitArray bits = new BitArray(b);
                Reverse(bits);
                bits.CopyTo(b, 0);
                this.Password = Encoding.UTF8.GetString(b);
            }
        }

        private void Reverse(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }
    }
}
