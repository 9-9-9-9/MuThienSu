using System;
using System.Collections.Generic;
using System.Text;
using MUThienSu.CommandHandlers;

namespace MUThienSu.Exceptions
{
    public class InvalidCommandArgumentsException : Exception
    {
        public InvalidCommandArgumentsException(ICommandHandler commandHandler, string msg)
        : base($"Sai câu lệnh: .\\{System.AppDomain.CurrentDomain.FriendlyName} <account> <character> {commandHandler.RegisteredCommandName()} {msg}")
        {
        }
    }

    public static class InvalidCommandArgumentsExceptionExtensions
    {
        public static void ThrowInvalidCommandArgumentsException(
            this ICommandHandler commandHandler, string msg = null)
        {
            throw new InvalidCommandArgumentsException(commandHandler, msg ?? string.Empty);
        }
    }
}
