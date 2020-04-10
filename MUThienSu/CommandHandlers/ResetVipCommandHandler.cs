using System.Threading.Tasks;

namespace MUThienSu.CommandHandlers
{
    public class ResetVipCommandHandler : AbstractCommandHandler
    {
        public ResetVipCommandHandler(string account, string password, string character) : base(account, password, character)
        {
        }

        protected override void ValidateParameters(string[] args)
        {
        }

        protected override async Task InternalExecutionAsync(string[] args)
        {
            await LoginAsync();
            await SelectCharacterAsync();

            await ResetVipAsync();
        }
    }
}