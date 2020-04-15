using System;
using System.Threading.Tasks;

namespace MUThienSu.CommandHandlers
{
    [Command("reset-point")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ResetPointCommandHandler : AbstractCommandHandler
    {
        public ResetPointCommandHandler(string account, string password, string character) : base(account, password, character)
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
            await ResetPointAsync();

            var totalRemainingPoint = await GetTotalRemainingPointAsync();
            Console.WriteLine($"Nhân vật có {totalRemainingPoint} points");
        }
    }
}