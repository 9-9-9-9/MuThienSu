using System;
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
            if (args.Length == 0)
                return;

            if (args.Length != 4)
                throw new ArgumentException($"After command name, require extra 4 arguments <str> <agi> <vit> <ene>");

            var name = new[]
            {
                "str", "agi", "vit", "ene"
            };

            var anyMinus1 = false;
            for (var i = 0; i <= 3; i++)
            {
                if (!int.TryParse(args[i], out var val))
                    throw new ArgumentException($"value of '{name[i]}' at index {i} is not an integer");

                if (val < -1)
                    throw new ArgumentException($"So point cong co the la so bat ky >= 0 (hoac -1 de bieu thi thang toan bo so du se duoc cong vao day, chi dc toi da mot lan -1 dc xuat hien trong 4 chi so)");

                if (val == -1)
                {
                    if (anyMinus1)
                        throw new ArgumentException("'-1' chi duoc xuat hien toi da 1 lan");
                    anyMinus1 = true;
                }
            }
        }

        protected override async Task InternalExecutionAsync(string[] args)
        {
            await LoginAsync();
            await SelectCharacterAsync();

            if (args.Length == 0)
            {
                await ResetVipAsync(0, 0, 0, 0);
            }
            else
            {
                var str = int.Parse(args[0]);
                var agi = int.Parse(args[1]);
                var vit = int.Parse(args[2]);
                var ene = int.Parse(args[3]);

                TryUpdatePointValue(ref str, ref agi, ref vit, ref ene);

                await ResetVipAsync(str, agi, vit, ene);
            }
        }
    }
}