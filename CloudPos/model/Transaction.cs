using System;
using System.Runtime.Serialization;

namespace CloudPos
{
    public class ResultType
    {
        public const string SUCCESSFUL = "SUCESSFULL";
        public const string FAILED = "FAILED";
    }

    [Serializable]
    public class Transaction
    {
        [DataMember(Name = "retailerId")]
        public string RetailerId { get; set; }

        [DataMember(Name = "supplierId")]
        public string SupplierId { get; set; }

        [DataMember(Name = "touchId")]
        public string TouchId { get; set; }

        [DataMember(Name = "completedAt")]
        public DateTimeOffset CompletedAt { get; set; }

        [DataMember(Name = "result")]
        public string Result { get; set; }
    }
}
