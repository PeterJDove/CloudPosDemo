using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CloudPos
{
    [Serializable]
    public class PosMessageType
    {
        //
        //  These strings end in "Event" because they are events from the Javascript POV.
        //
        public const string DEVICE_ACCESS_TOKEN = "DeviceAccessTokenEvent";
        public const string SHOW_UI = "ShowUserInterfaceEvent";
        public const string ADD_TO_BASKET = "BasketAddItemEvent";
        public const string COMMIT_BASKET = "BasketCommitEvent";
        public const string REMOVE_FROM_BASKET = "RemoveBasketItemEvent";
        public const string CLEAR_BASKET = "BasketClearEvent";
        public const string DEVICE_DATA = "DeviceDataEvent";
        public const string GET_VOUCHER = "BasketGetVoucherEvent";
        public const string SYNC_BASKET = "SyncBasket";
    }

    [Serializable]
    public abstract class PosMessage
    {
        public PosMessage(string eventIdentifier)
        {
            EventIdentifier = eventIdentifier;
        }

        [DataMember(Name = "eventIdentifier")]
        public string EventIdentifier { get; private set; }
    }

    [Serializable]
    public class SetDeviceAccessTokenMessage : PosMessage
    {
        public SetDeviceAccessTokenMessage() : base(PosMessageType.DEVICE_ACCESS_TOKEN) { }

        [DataMember(Name = "accessToken")]
        public string AccessToken { get; set; }
    }

    [Serializable]
    public class ShowUserInterfaceMessage : PosMessage
    {
        public ShowUserInterfaceMessage() : base(PosMessageType.SHOW_UI) { }

        [DataMember(Name = "shortcutOrEan")]
        public string ShortcutOrEan { get; set; }

        [DataMember(Name = "retailerTransactionId")]
        public string RetailerTransactionId { get; set; }

        [DataMember(Name = "basketId")]
        public string BasketId { get; set; }

        [DataMember(Name = "operatorId")]
        public string OperatorId { get; set; }
    }

    [Serializable]
    public class AddToBasketMessage : PosMessage
    {
        public AddToBasketMessage() : base(PosMessageType.ADD_TO_BASKET) { }

        [DataMember(Name = "shortcutOrEan")]
        public string ShortcutOrEan { get; set; }

        [DataMember(Name = "operatorId")]
        public string OperatorId { get; set; }

        [DataMember(Name = "retailerTransactionId")]
        public string RetailerTransactionId { get; set; }

        [DataMember(Name = "basketId")]
        public string BasketId { get; set; }

        [DataMember(Name = "data")]
        public Dictionary<string, string> Data { get; set; }
    }

    [Serializable]
    public class RemoveFromBasketMessage : PosMessage
    {
        public RemoveFromBasketMessage() : base(PosMessageType.REMOVE_FROM_BASKET) { }

        [DataMember(Name = "purchaseId")]
        public string PurchaseId { get; set; }

        [DataMember(Name = "operatorId")]
        public string OperatorId { get; set; }

        [DataMember(Name = "basketId")]
        public string BasketId { get; set; }

        [DataMember(Name = "reason")]
        public string Reason { get; set; }
    }

    [Serializable]
    public class ClearBasketMessage : PosMessage
    {
        public ClearBasketMessage() : base(PosMessageType.CLEAR_BASKET) { }

        [DataMember(Name = "basketId")]
        public string BasketId { get; set; }
    }

    [Serializable]
    public class CommitBasketMessage : PosMessage
    {
        public CommitBasketMessage() : base(PosMessageType.COMMIT_BASKET) { }

        [DataMember(Name = "basketId")]
        public string BasketId { get; set; }

        [DataMember(Name = "operatorId")]
        public string OperatorId { get; set; }

        [DataMember(Name = "committedAt")]
        public DateTimeOffset CommittedAt { get; set; }
    }

    [Serializable]
    public class GetVoucherMessage : PosMessage
    {
        public GetVoucherMessage() : base(PosMessageType.GET_VOUCHER) { }

        [DataMember(Name = "voucherUrl")]
        public string VoucherUrl { get; set; }
    }

    [Serializable]
    public class DeviceDataMessage : PosMessage
    {
        public DeviceDataMessage() : base(PosMessageType.DEVICE_DATA) { }

        [DataMember(Name = "device")]
        public string Device { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }
    }

    [Serializable]
    public class SyncBasketMessage : PosMessage
    {
        public SyncBasketMessage() : base(PosMessageType.SYNC_BASKET) { }

        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }

        // TODO: Flesh out SyncBasketMessage
    }




}
