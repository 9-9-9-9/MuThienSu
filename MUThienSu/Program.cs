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

        internal static async Task Main(string[] args)
        {
            Register<ResetVipCommandHandler>("reset-vip");
            Register<AddPointCommandHandler>("add-point");
            
            if (args.Length < 3)
                throw new InvalidOperationException(
                    "Require at least 3 parameters: <account> <character name> <command-name> [others args]");

            var allAccPass = LoadAccountPasswords();
            var account = args[0];
            var character = args[1];
            var command = args[2];
            
            if (!allAccPass.TryGetValue(account, out var password))
                throw new InvalidOperationException($"Khong tim thay pass cua acc {account} trong file '{AccPassFile}'. Format: account <tab> password");

            if (!RegisteredCommandHandlers.TryGetValue(command.ToLower(), out var typeOfCommandHandler))
                throw new ArgumentException($"No handler for command '{command}'");

            var instance = Activator.CreateInstance(typeOfCommandHandler, new object[]
            {
                account,
                password,
                character
            }) as ICommandHandler;

            // ReSharper disable once PossibleNullReferenceException
            await instance.ExecuteAsync(args.Skip(3).ToArray());
        }

        private static void Register<T>(string name) where T : ICommandHandler
        {
            RegisteredCommandHandlers.Add(name.ToLower(), typeof(T));
        }

        private const string AccPassFile = "accounts.muts.txt";

        private static IDictionary<string, string> LoadAccountPasswords()
        {
            try
            {
                return File.ReadAllLines(AccPassFile)
                    .Where(x => !x.IsBlank())
                    .Select(x => x.Split(new []{'\t', ' '}, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length == 2)
                    .Select(x => x.Select(y => y.Trim()).ToArray())
                    .ToDictionary(x => x[0], x => x[1]);
            }
            catch (FileNotFoundException e)
            {
                return new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return new Dictionary<string, string>();
            }
        }
    }
}