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

            if (args.Length != 5)
                this.ThrowInvalidCommandArgumentsException("[<str> <agi> <vit> <ene> <cmd>]");

            args.Validate5Stats();
        }

        protected override async Task InternalExecutionAsync(string[] args)
        {
            await LoginAsync();
            await SelectCharacterAsync();

            if (args.Length == 0)
            {
                await ResetNormalAsync();
            }
            else
            {
                var str = int.Parse(args[0]);
                var agi = int.Parse(args[1]);
                var vit = int.Parse(args[2]);
                var ene = int.Parse(args[3]);
                var cmd = int.Parse(args[4]);

                var success = await ResetNormalAsync();

                if (!success)
                    return;
                
                TryUpdatePointValue(ref str, ref agi, ref vit, ref ene, ref cmd);

                await AddPointAsync(str, agi, vit, ene, cmd);
            }
        }
    }
}