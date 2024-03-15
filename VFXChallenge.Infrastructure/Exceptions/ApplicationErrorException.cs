namespace VFXChallenge.Infrastructure.Exceptions
{
    public class ApplicationErrorException : Exception
    {
        public ApplicationErrorException(string message) : base(message)
        {
            
        }
    }
}