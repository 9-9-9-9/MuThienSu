using System;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace MUThienSu
{
    public static class Extensions
    {
        public static bool IsBlank(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static int Index(this int index) => index + Program.OffSetIndex;

        public static void Validate5Stats(this string[] args)
        {
            if (args.Length != 5)
                throw new ArgumentException($"Tham số '{nameof(args)}' yêu cầu 5 phần tử (str, agi, vit, ene, cmd)");

            var name = new[]
            {
                "str", "agi", "vit", "ene", "cmd"
            };

            var anyMinus1 = false;
            for (var i = 0; i <= 4; i++)
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

        public static Task<IRestResponse> ExecuteAsync(this RestClient client, RestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, response => taskCompletionSource.SetResult(response));
            return taskCompletionSource.Task;
        }
    }
}