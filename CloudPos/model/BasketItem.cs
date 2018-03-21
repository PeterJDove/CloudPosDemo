using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CloudPos
{
    [Serializable]
    public class Basket
    {
        [DataMember(Name = "committed")]
        public bool Committed { get; set; }

        [JsonProperty(ItemConverterType = typeof(BasketItemJsonConverter))]
        public List<BasketItem> basketItems { get; set; }
    }

    public class BasketJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Basket).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            return item.ToObject<Basket>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


    public abstract class BasketItem
    {
        public BasketItem(string type)
        {
            Type = type;
        }

        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "removable")]
        public bool Removable { get; set; }
    }

    public class PurchaseBasketItem: BasketItem
    {
        public PurchaseBasketItem() : base("PURCHASE") { }

        [DataMember(Name = "purchasedAt")]
        public DateTimeOffset PurchasedAt { get; set; }

        [DataMember(Name = "transaction")]
        public Transaction Transaction { get; set; }

        [DataMember(Name = "product")]
        public Product Product { get; set; }

        [DataMember(Name = "vouchers")]
        public List<Voucher> Vouchers { get; set; }
    }

    public class RefundBasketItem: BasketItem
    {
        public RefundBasketItem() : base("REFUND") { }

        [DataMember(Name = "purchaseTouchTransactionId")]
        public string PurchaseTouchTransactionId { get; set; }

        [DataMember(Name = "transaction")]
        public Transaction Transaction { get; set; }

        [DataMember(Name = "product")]
        public Product Product { get; set; }

        [DataMember(Name = "refundVoucher")]
        public Voucher RefundVoucher { get; set; }
    }

    public class BasketItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BasketItem).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            var type = item["type"].Value<string>();

            switch (type)
            {
                case "PURCHASE":
                    return item.ToObject<PurchaseBasketItem>();
                case "REFUND":
                    return item.ToObject<RefundBasketItem>();
                default:
                    throw new ApplicationException("Unknown basket item type " + type);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}