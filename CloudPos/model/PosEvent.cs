using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace CloudPos
{
    public class PosEventType
    {
        public const string READY = "ready";
        public const string HIDE_GUI = "hideGui";
        public const string SHOW_GUI = "showGui";
        public const string ADD_TO_BASKET = "addToBasket";
        public const string REMOVE_FROM_BASKET = "removeFromBasket"; 
        public const string BASKET_COMMITTED = "basketCommitted";
        public const string PRINT_VOUCHER = "printVoucher";
        public const string VOUCHER_HTML = "voucherHtml";
        public const string START_DEVICE = "startDevice";
        public const string START_DEVICE_V2 = "startDeviceV2";
        public const string STOP_DEVICE = "stopDevice";
        public const string DISPLAY_MESSAGE = "displayMessage";
        public const string SYNC_BASKET = "syncBasket";
        public const string ERROR = "error";
    }

    [Serializable]
    public abstract class PosEvent
    {
        public PosEvent(string eventName)
        {
            this.EventName = eventName;
        }

        [DataMember(Name = "event")]
        public string EventName { get; private set; }
    }

    public class ReadyEvent : PosEvent
    {
        public ReadyEvent() : base(PosEventType.READY) { }
    }

    public class ShowGuiEvent : PosEvent
    {
        public ShowGuiEvent() : base(PosEventType.SHOW_GUI) { }
    }

    public class HideGuiEvent: PosEvent
    {
        public HideGuiEvent() : base(PosEventType.HIDE_GUI) { }
        
        [DataMember(Name = "reason")]
        public string Reason { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }

    public class AddToBasketEvent : PosEvent
    {
        public AddToBasketEvent() : base(PosEventType.ADD_TO_BASKET) { }

        [DataMember(Name = "basketId")]
        public int BasketId { get; set; }

        [JsonConverter(typeof(BasketItemJsonConverter))]
        [DataMember(Name = "basketItem")]
        public BasketItem BasketItem { get; set; }
    }

    public class RemoveFromBasketEvent : PosEvent
    {
        public RemoveFromBasketEvent() : base(PosEventType.REMOVE_FROM_BASKET) { }

        [DataMember(Name = "basketId")]
        public int BasketId { get; set; }

        [DataMember(Name = "basketItemId")]
        public int BasketItemId { get; set; }

    }

    public class BasketCommittedEvent : PosEvent
    {
        public BasketCommittedEvent() : base(PosEventType.BASKET_COMMITTED) { }

        [DataMember(Name = "basketId")]
        public int BasketId { get; set; }

        [JsonConverter(typeof(BasketJsonConverter))]
        [DataMember(Name = "basket")]
        public Basket Basket { get; set; }

    }

    public class PrintVoucherEvent : PosEvent
    {
        public PrintVoucherEvent() : base(PosEventType.PRINT_VOUCHER) { }

        [DataMember(Name = "mediaType")]
        public string MediaType { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }
    }

    public class VoucherHtmlEvent : PosEvent
    {
        public VoucherHtmlEvent() : base(PosEventType.VOUCHER_HTML) { }

        [DataMember(Name = "voucherUrl")]
        public string VoucherUrl { get; set; }

        [DataMember(Name = "successful")]
        public bool Successful { get; set; }

        [DataMember(Name = "voucherHtml")]
        public string VoucherHtml { get; set; }
    }

    public class StartDeviceEvent : PosEvent
    {
        public StartDeviceEvent() : base(PosEventType.START_DEVICE) { }

        [DataMember(Name = "device")]
        public string Device { get; set; }
    }

    public class StopDeviceEvent : PosEvent
    {
        public StopDeviceEvent() : base(PosEventType.STOP_DEVICE) { }

        [DataMember(Name = "device")]
        public string Device { get; set; }
    }

    public class DisplayMessageEvent : PosEvent
    {
        public DisplayMessageEvent() : base(PosEventType.DISPLAY_MESSAGE) { }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "display")]
        public string Display { get; set; }

        [DataMember(Name = "timeout")]
        public int Timeout { get; set; }
    }

    public class SyncBasketEvent : PosEvent
    {
        public SyncBasketEvent() : base(PosEventType.SYNC_BASKET) { }

        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }
    }


    public class ErrorEvent : PosEvent
    {
        public ErrorEvent() : base(PosEventType.ERROR) { }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "reason")]
        public string Reason { get; set; }

    }

    




    public class PosEventJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PosEvent).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            var eventName = item["event"].Value<string>();
            switch (eventName)
            {
                case PosEventType.READY:
                    return item.ToObject<ReadyEvent>();
                case PosEventType.HIDE_GUI:
                    return item.ToObject<HideGuiEvent>();
                case PosEventType.SHOW_GUI:
                    return item.ToObject<ShowGuiEvent>();
                case PosEventType.ADD_TO_BASKET:
                    return item.ToObject<AddToBasketEvent>();
                case PosEventType.REMOVE_FROM_BASKET:
                    return item.ToObject<RemoveFromBasketEvent>();
                case PosEventType.BASKET_COMMITTED:
                    return item.ToObject<BasketCommittedEvent>();
                case PosEventType.PRINT_VOUCHER:
                    return item.ToObject<PrintVoucherEvent>();
                case PosEventType.VOUCHER_HTML:
                    return item.ToObject<VoucherHtmlEvent>();
                case PosEventType.START_DEVICE:
                case PosEventType.START_DEVICE_V2:
                    return item.ToObject<StartDeviceEvent>();
                case PosEventType.STOP_DEVICE:
                    return item.ToObject<StopDeviceEvent>();
                case PosEventType.DISPLAY_MESSAGE:
                    return item.ToObject<DisplayMessageEvent>();
                case PosEventType.SYNC_BASKET:
                    return item.ToObject<SyncBasketEvent>();
                case PosEventType.ERROR:
                    return item.ToObject<ErrorEvent>();
                default:
                    return null;
                    //throw new ApplicationException("Unknown event name " + eventName);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}