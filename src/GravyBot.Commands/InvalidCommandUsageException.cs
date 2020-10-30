namespace GravyBot.Commands
{
    public class InvalidCommandUsageException
    {
        public string ErrorMessage { get; set; }

        public InvalidCommandUsageException(string errorMessage = null)
        {
            ErrorMessage = errorMessage;
        }
    }
}
