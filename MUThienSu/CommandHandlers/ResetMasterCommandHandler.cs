using System;
using System.Threading.Tasks;

namespace MUThienSu.CommandHandlers
{
    [Command("reset-master")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ResetMasterCommandHandler : AbstractCommandHandler
    {
        public ResetMasterCommandHandler(string account, string password, string character) : base(account, password, character)
        {
        }

        protected override void ValidateParameters(string[] args)
        {
            //
        }

        protected override async Task InternalExecutionAsync(string[] args)
        {
            await LoginAsync();
            await SelectCharacterAsync();
            await ResetMasterAsync();
            // ReSharper disable once StringLiteralTypo
            Console.WriteLine("Xong");
        }
    }
}