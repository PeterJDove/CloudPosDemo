using Touch.SmartCards;

namespace CloudSmartCards
{
    internal enum ResultStatus
    {
        NEW,
        OK,
        ERROR,
        TIMEOUT,
        SKIPPED,
    }


    abstract class CardCommand
    {
        public ResultStatus ResultStatus { get; set; }

        public ResultStatus Skip()
        {
            ResultStatus = ResultStatus.SKIPPED;
            return ResultStatus;
        }

        public abstract ResultStatus Execute(Cloud47x0 cloud47x0);
    }
}
