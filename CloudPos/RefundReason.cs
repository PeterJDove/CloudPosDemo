using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.CloudPos
{
    /// <summary>
    /// A code which is passed when <see cref="API.Refund"/> is called to show why the product is being returned.
    /// </summary>
    public enum RefundReason
    {
        /// <summary>
        /// The wrong product was supplied.
        /// </summary>
        INCORRECT_PRODUCT,
        /// <summary>
        /// The correct product was supplied, but it did not work properly.
        /// </summary>
        FAULTY_PRODUCT,
        /// <summary>
        /// The customer changed their mind.
        /// </summary>
        CUSTOMER_CHANGED_MIND,
        /// <summary>
        /// A reason not covered above.
        /// </summary>
        OTHER
    }
}
