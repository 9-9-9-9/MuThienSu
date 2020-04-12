using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using RestSharp;

// ReSharper disable StringLiteralTypo

namespace MUThienSu.CommandHandlers
{
    public interface ICommandHandler
    {
        Task ExecuteAsync(string[] args);
    }

    public abstract class AbstractCommandHandler : ICommandHandler
    {
        protected readonly string Account;
        protected readonly string Password;
        protected readonly string Character;
        protected readonly string SessionId = Guid.NewGuid().ToString();

        protected AbstractCommandHandler(string account, string password, string character)
        {
            Account = account;
            Password = password;
            Character = character;
        }

        public virtual async Task ExecuteAsync(string[] args)
        {
            ValidateParameters(args);
            Console.WriteLine(nameof(InternalExecutionAsync));
            await InternalExecutionAsync(args);
        }

        protected abstract void ValidateParameters(string[] args);
        protected abstract Task InternalExecutionAsync(string[] args);

        protected async Task LoginAsync()
        {
            Console.WriteLine(nameof(LoginAsync));
            var client = new RestClient("http://id.muthiensu.vn/losttower/") {Timeout = -1};
            var request = new RestRequest(Method.POST);
            request.AddHeader("Origin", "http://id.muthiensu.vn");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Referer", "http://id.muthiensu.vn/losttower/");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("login", "login");
            request.AddParameter("nweb_security_login_code1", "");
            request.AddParameter("nweb_security_login_code2", "");
            request.AddParameter("nweb_security_login_code3", "");
            request.AddParameter("username", Account);
            request.AddParameter("password", Password);
            request.AddParameter("submit", "");
            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Dang nhap that bai, status {restResponse.StatusCode}");

            if (!restResponse.Content.Contains("Gcoin hiện có"))
                throw new Exception("Dang nhap that bai (kiem tra lai pass)");

            await Task.CompletedTask;
        }

        protected async Task SelectCharacterAsync()
        {
            Console.WriteLine(nameof(SelectCharacterAsync));
            var client =
                new RestClient(
                    $"http://id.muthiensu.vn/losttower/ajax_action.php?ajax=char_choise&char_choise={Character}")
                {
                    Timeout = -1
                };
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Referer", "http://id.muthiensu.vn/losttower/");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");
            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Chon nhan vat that bai, status {restResponse.StatusCode}");

            if (!restResponse.Content.Contains("|OK|"))
                throw new Exception($"Chon nhan vat that bai ({restResponse.Content})");

            await Task.CompletedTask;
        }

        protected async Task AddPointAsync(int str, int agi, int vit, int ene, int cmd)
        {
            Console.WriteLine($"{nameof(AddPointAsync)} s+{str} a+{agi} v+{vit} e+{ene} c+{cmd}");
            var client =
                new RestClient("http://id.muthiensu.vn/losttower/index.php?mod=char_manager&act=addpoint")
                {
                    Timeout = -1
                };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Origin", "http://id.muthiensu.vn");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Referer", "http://id.muthiensu.vn/losttower/");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("action", "addpoint");
            request.AddParameter("character", Character);
            request.AddParameter("str", str.ToString());
            request.AddParameter("dex", agi.ToString());
            request.AddParameter("vit", vit.ToString());
            request.AddParameter("ene", ene.ToString());
            if (cmd > 0)
                request.AddParameter("ml", cmd.ToString());
            request.AddParameter("Submit", "");

            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Cộng point thất bại, status {restResponse.StatusCode}");

            var content = restResponse.Content;

            if (!content.IsBlank())
            {
                var idxFormErr = content.IndexOf("class='form-error'", StringComparison.InvariantCulture);
                if (idxFormErr > -1)
                {
                    var idxEnd = content.IndexOf("div", idxFormErr + 1, StringComparison.InvariantCulture);
                    if (idxEnd > -1)
                    {
                        var partialHtml = content.Substring(idxFormErr, idxEnd - idxFormErr + 1);
                        var idxErrMsg = partialHtml.IndexOf("Dữ liệu lỗi", StringComparison.InvariantCulture);
                        if (idxErrMsg > -1)
                        {
                            var idxEndOfErrMsg = partialHtml.IndexOf('.', idxErrMsg + 1);
                            if (idxEndOfErrMsg > -1)
                                throw new Exception(partialHtml.Substring(idxErrMsg, idxEndOfErrMsg - idxErrMsg + 1));
                            else
                                throw new Exception(partialHtml.Substring(idxErrMsg, 200));
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }

        protected async Task<bool> ResetVipAsync()
        {
            Console.WriteLine(nameof(ResetVipAsync));
            var client =
                new RestClient(
                    "http://id.muthiensu.vn/losttower/ajax_action.php?ajax=char_rs&action=reset_vip&tiente=gcoin")
                {
                    Timeout = -1
                };
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");

            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Reset VIP thất bại, status {restResponse.StatusCode}");

            if (restResponse.Content?.StartsWith("<nbb>OK<nbb>") != true)
            {
                Console.WriteLine(restResponse.Content);
                Console.WriteLine("Reset VIP thất bại");

                try
                {
                    var lvl = await GetCharacterLevelAsync();
                    Console.WriteLine($"Cấp độ hiện tại là {lvl}");
                }
                catch
                {
                    //
                }

                return false;
            }

            return true;
        }

        protected async Task<bool> ResetNormalAsync()
        {
            Console.WriteLine(nameof(ResetNormalAsync));
            var client =
                new RestClient(
                    "http://id.muthiensu.vn/losttower/ajax_action.php?ajax=char_rs&action=reset"
                ) {Timeout = -1};
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");

            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Reset thất bại, status {restResponse.StatusCode}");

            if (restResponse.Content?.StartsWith("<nbb>OK<nbb>") != true)
            {
                Console.WriteLine(restResponse.Content);
                Console.WriteLine("Reset thất bại");

                try
                {
                    var lvl = await GetCharacterLevelAsync();
                    Console.WriteLine($"Cấp độ hiện tại là {lvl}");
                }
                catch
                {
                    //
                }

                return false;
            }

            return true;
        }

        protected void TryUpdatePointValue(ref int str, ref int agi, ref int vit, ref int ene, ref int cmd)
        {
            try
            {
                if (str >= 0 && agi >= 0 && vit >= 0 && ene >= 0 && cmd >= 0)
                    return;

                var totalPoint = GetTotalRemainingPointAsync().Result;

                var allStats = new[] {str, agi, vit, ene, cmd};

                if (totalPoint < allStats.Where(x => x > 0).Sum())
                    throw new ArgumentException(
                        $"Tổng số point yêu cầu vượt quá số dư hiện tại là {totalPoint} (yêu cầu {allStats.Where(x => x > 0).Sum()})"
                    );


                var remainingPoint = totalPoint - allStats.Where(x => x >= 0).Sum();
                var avg = (int) Math.Floor((double) remainingPoint / allStats.Count(x => x < 0));

                if (str < 0)
                    str = avg;
                if (agi < 0)
                    agi = avg;
                if (vit < 0)
                    vit = avg;
                if (ene < 0)
                    ene = avg;
                if (cmd < 0)
                    cmd = avg;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);

                if (str < 0)
                    str = 0;
                if (agi < 0)
                    agi = 0;
                if (vit < 0)
                    vit = 0;
                if (ene < 0)
                    ene = 0;
                if (cmd < 0)
                    cmd = 0;
            }
        }

        private async Task<int> GetTotalRemainingPointAsync()
        {
            var client =
                new RestClient("http://id.muthiensu.vn/losttower/index2.php?mod=char_manager&act=addpoint")
                {
                    Timeout = -1
                };
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Referer", "http://id.muthiensu.vn/losttower/");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");
            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Lấy thông tin total point thất bại, status {restResponse.StatusCode}");

            var content = restResponse.Content;
            var idxTongDiemCoTheCong = content.IndexOf("Tổng Điểm có thể cộng", StringComparison.InvariantCulture);
            if (idxTongDiemCoTheCong < 0)
                throw new Exception("Không thể xác định được pattern 1");
            var idxPointTotal = content.IndexOf("point_total", idxTongDiemCoTheCong, StringComparison.InvariantCulture);
            if (idxPointTotal < 0)
                throw new Exception("Không thể xác định được pattern 2");
            var idxCloseGt = content.IndexOf('>', idxPointTotal);
            if (idxCloseGt < 0)
                throw new Exception("Không thể xác định được pattern 3");
            var idxOpenLt = content.IndexOf('<', idxCloseGt);
            if (idxOpenLt < 0)
                throw new Exception("Không thể xác định được pattern 4");

            var strTotalPoint = content
                .Substring(idxCloseGt + 1, idxOpenLt - idxCloseGt - 1)
                .Replace(".", "");
            Console.WriteLine($"Raw: {strTotalPoint}");

            if (!int.TryParse(strTotalPoint, out var totalPoint))
                throw new ArgumentException($"Invalid total point value '{strTotalPoint}'");

            return totalPoint;
        }

        protected async Task<int> GetCharacterLevelAsync()
        {
            await Task.CompletedTask;

            Console.WriteLine(nameof(GetCharacterLevelAsync));
            var client =
                new RestClient(
                    $"http://id.muthiensu.vn/losttower/ajax_action.php?ajax=char_choise&char_choise={Character}")
                {
                    Timeout = -1
                };
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Referer", "http://id.muthiensu.vn/losttower/");
            request.AddHeader("Cookie", $"PHPSESSID={SessionId}");
            var restResponse = await client.ExecuteAsync(request);
            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Lấy thông tin level nhân vật thất bại, status {restResponse.StatusCode}");

            var content = restResponse.Content;
            if (!content.Contains("|OK|"))
                throw new Exception($"Lấy thông tin level nhân vật thất bại ({restResponse.Content})");

            Console.WriteLine(content);

            var idxCharInfo = content.IndexOf("char-info'", StringComparison.InvariantCulture);
            if (idxCharInfo < 0)
                throw new Exception("Không thể xác định được pattern 5");

            var idxDoubleDot = content.IndexOf(':', idxCharInfo);
            if (idxDoubleDot < 0)
                throw new Exception("Không thể xác định được pattern 6");

            var idxReset = content.IndexOf("Reset", idxDoubleDot, StringComparison.InvariantCulture);
            if (idxReset < 0)
                throw new Exception("Không thể xác định được pattern 7");

            var idxBegin = idxDoubleDot + 2;
            var len = idxReset - idxBegin;

            if (len < 1)
                throw new Exception("Không thể xác định được pattern 8");
            var strLevel = content.Substring(idxBegin, len);
            Console.WriteLine($"Raw: {strLevel}");

            if (!int.TryParse(strLevel, out var lvl))
                throw new ArgumentException($"Invalid level value '{lvl}'");

            return lvl;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Command : Attribute
    {
        public string Name { get; }

        public Command(string name)
        {
            Name = name?.ToLower();
        }
    }

    public static class CommandHandlerExtension
    {
        public static string RegisteredCommandName(this Type type)
        {
            var ca = type.GetCustomAttribute(typeof(Command));
            if (ca is Command cm) return cm.Name;
            return null;
        }

        public static string RegisteredCommandName<T>(this T type) where T : ICommandHandler
        {
            return typeof(T).RegisteredCommandName();
        }
    }
}