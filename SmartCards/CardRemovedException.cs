using System;

namespace Touch.SmartCards
{
    public class CardRemovedException : SmartCardException
    {
        public CardRemovedException() :
            base("Card has been removed") { }

        public CardRemovedException(string message) :
            base(message) { }

        public CardRemovedException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
