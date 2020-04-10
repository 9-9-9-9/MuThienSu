using System;
using System.Net;

namespace MUThienSu
{
    public static class Extensions
    {
        public static bool IsBlank(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static int Index(this int index) => index + Program.OffSetIndex;

        public static void Validate4Stats(this string[] args)
        {
            if (args.Length != 4)
                throw new ArgumentException($"Tham số '{nameof(args)}' yêu cầu 4 phần tử (str, agi, vit, ene)");

            var name = new[]
            {
                "str", "agi", "vit", "ene"
            };

            var anyMinus1 = false;
            for (var i = 0; i <= 3; i++)
            {
                if (!int.TryParse(args[i], out var val))
                    throw new ArgumentException($"'{name[i]}' tại index {i.Index()} không thỏa mãn số integer");

                if (val < -1)
                    throw new ArgumentException($"'{name[i]}' tại index {i.Index()} không được phép nhỏ hơn -1");

                if (val == -1)
                {
                    if (anyMinus1)
                        throw new ArgumentException("'-1' chỉ được xuất hiện tối đa 1 lần");
                    anyMinus1 = true;
                }
            }
        }
    }
}