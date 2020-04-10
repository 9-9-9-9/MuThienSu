using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MUThienSu.CommandHandlers;

namespace MUThienSu
{
    internal static class Program
    {
        private static readonly IDictionary<string, Type> RegisteredCommandHandlers = new Dictionary<string, Type>();

        public const int OffSetIndex = 3;

        internal static async Task Main(string[] args)
        {
            Register<ResetVipCommandHandler>();
            Register<ResetNormalCommandHandler>();
            Register<AddPointCommandHandler>();

            if (args.Length < 3)
                Exit("Yêu cầu tối thiểu 3 param: <account> <character name> <command-name> [others args]");

            if (!File.Exists(AccPassFile))
                Exit($"Không tìm thấy file '{AccPassFile}'");

            var allAccPass = LoadAccountPasswords();
            var account = args[0];
            var character = args[1];
            var command = args[2];

            if (!allAccPass.TryGetValue(account, out var password))
                Exit($"Không tìm thấy account '{account}' trong file '{AccPassFile}'. Format: account <tab> password");

            if (!RegisteredCommandHandlers.TryGetValue(command.ToLower(), out var typeOfCommandHandler))
                Exit($"Command '{command}' không tồn tại");

            var instance = Activator.CreateInstance(typeOfCommandHandler, new object[]
            {
                account,
                password,
                character
            }) as ICommandHandler;

            // ReSharper disable once PossibleNullReferenceException
            await instance.ExecuteAsync(args.Skip(3).ToArray());
        }

        private static void Exit(string msg, int exitCode = 0)
        {
            Console.WriteLine(msg);
            Environment.Exit(exitCode);
        }

        private static void Register<T>() where T : ICommandHandler
        {
            RegisteredCommandHandlers.Add(typeof(T).RegisteredCommandName(), typeof(T));
        }

        private const string AccPassFile = "accounts.muts.txt";

        private static IDictionary<string, string> LoadAccountPasswords()
        {
            try
            {
                return File.ReadAllLines(AccPassFile)
                    .Where(x => !x.IsBlank())
                    .Select(x => x.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length == 2)
                    .Select(x => x.Select(y => y.Trim()).ToArray())
                    .ToDictionary(x => x[0], x => x[1]);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return new Dictionary<string, string>();
            }
        }
    }
}