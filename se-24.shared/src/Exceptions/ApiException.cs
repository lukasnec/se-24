using System.Net;

namespace se_24.shared.src.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {

        }
    }
}
