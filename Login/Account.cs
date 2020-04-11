using System.Collections;
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
                var b = Encoding.UTF8.GetBytes(this.Password);
                var bits = new BitArray(b);
                Reverse(bits);
                bits.CopyTo(b, 0);
                return System.Convert.ToBase64String(b);
            }
            set
            {
                var b = System.Convert.FromBase64String(value);
                var bits = new BitArray(b);
                Reverse(bits);
                bits.CopyTo(b, 0);
                this.Password = Encoding.UTF8.GetString(b);
            }
        }

        private void Reverse(BitArray array)
        {
            var length = array.Length;
            var mid = (length / 2);

            for (var i = 0; i < mid; i++)
            {
                var bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }
    }
}
