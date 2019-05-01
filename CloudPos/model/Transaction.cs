using System;
using System.Runtime.Serialization;

namespace Touch.CloudPos.Model
{

    internal class ResultType
    {
        public const string SUCCESSFUL = "SUCESSFULL";
        public const string FAILED = "FAILED";
    }

    /// <summary>
    /// The interaction between client and host that represents the purchase (or refund) of an eService product.
    /// </summary>
    [Serializable]
    public class Transaction
    {
        /// <summary>
        /// Gets the ID by which the Retailer is known.
        /// </summary>
        [DataMember(Name = "retailerId")]
        public string RetailerId { get; internal set; }

        /// <summary>
        /// Gets the ID of the third party Supplier of the product.
        /// </summary>
        [DataMember(Name = "supplierId")]
        public string SupplierId { get; internal set; }

        /// <summary>
        /// Gets the unique ID assigned to this transaction.
        /// </summary>
        /// <remarks>
        /// The Transaction TouchId is not available until after the Basket has been committed.
        /// </remarks>
        [DataMember(Name = "touchId")]
        public string TouchId { get; internal set; }

        /// <summary>
        /// Gets the date and time at which the transaction was completed.
        /// </summary>
        [DataMember(Name = "completedAt")]
        public DateTimeOffset CompletedAt { get; internal set; }

        /// <summary>
        /// Gets the result of the transaction: "SUCCESSFUL" or "FAILED".
        /// </summary>
        [DataMember(Name = "result")]
        public string Result { get; internal set; }
    }
}
