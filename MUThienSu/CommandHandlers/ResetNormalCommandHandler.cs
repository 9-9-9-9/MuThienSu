using System;
using System.Threading.Tasks;
using MUThienSu.Exceptions;

namespace MUThienSu.CommandHandlers
{
    [Command("reset")]
    public class ResetNormalCommandHandler : AbstractCommandHandler
    {
        public ResetNormalCommandHandler(string account, string password, string character) : base(account, password, character)
        {
        }

        protected override void ValidateParameters(string[] args)
        {
            if (args.Length == 0)
                return;

            if (args.Length != 4)
                this.ThrowInvalidCommandArgumentsException("[<str> <agi> <vit> <ene>]");

            args.Validate4Stats();
        }

        protected override async Task InternalExecutionAsync(string[] args)
        {
            await LoginAsync();
            await SelectCharacterAsync();

            if (args.Length == 0)
            {
                await ResetNormalAsync(0, 0, 0, 0);
            }
            else
            {
                var str = int.Parse(args[0]);
                var agi = int.Parse(args[1]);
                var vit = int.Parse(args[2]);
                var ene = int.Parse(args[3]);

                TryUpdatePointValue(ref str, ref agi, ref vit, ref ene);

                await ResetNormalAsync(str, agi, vit, ene);
            }
        }
    }
}