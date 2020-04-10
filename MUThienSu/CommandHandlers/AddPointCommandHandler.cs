using System;
using System.Threading.Tasks;
using MUThienSu.Exceptions;

namespace MUThienSu.CommandHandlers
{
    [Command("add-point")]
    public class AddPointCommandHandler : AbstractCommandHandler
    {
        public AddPointCommandHandler(string account, string password, string character) : base(account, password,
            character)
        {
        }

        protected override void ValidateParameters(string[] args)
        {
            if (args.Length != 4)
                this.ThrowInvalidCommandArgumentsException("[<str> <agi> <vit> <ene>]");

            args.Validate4Stats();
        }

        protected override async Task InternalExecutionAsync(string[] args)
        {
            await LoginAsync();
            await SelectCharacterAsync();

            var str = int.Parse(args[0]);
            var agi = int.Parse(args[1]);
            var vit = int.Parse(args[2]);
            var ene = int.Parse(args[3]);

            TryUpdatePointValue(ref str, ref agi, ref vit, ref ene);

            await AddPointAsync(str, agi, vit, ene);
        }
    }
}