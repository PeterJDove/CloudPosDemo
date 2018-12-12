using System;

namespace Touch.SmartCards
{
    public class SmartCardException : ApplicationException
    {
        public SmartCardException(string message) :
            base(message) { }

        public SmartCardException(string message, Exception innerException) :
            base(message, innerException) { }

    }
}
