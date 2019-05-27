using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// This namespace is subordinate to <see cref="Touch.CloudPos"/>. It contains the data model used therein.
/// </summary>
namespace Touch.CloudPos.Model
{
    [Serializable]
    internal class Basket
    {
        [DataMember(Name = "committed")]
        public bool Committed { get; set; }

        [JsonProperty(ItemConverterType = typeof(BasketItemJsonConverter))]
        public List<BasketItem> basketItems { get; set; }
    }

    internal class BasketJsonConverter : JsonConverter
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

    /// <summary>
    /// This abstract class serves as the base for a number of different types of basket item: <see cref="PurchaseBasketItem"/> or <see cref="RefundBasketItem"/>
    /// </summary>
    public abstract class BasketItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketItem"/> class
        /// </summary>
        /// <param name="type">The type of BasketItem being created: "PURCHASE" or "REFUND"</param>
        public BasketItem(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets a string representing the type of <see cref="BasketItem"/> (as set by the constructor).
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; private set; }

        /// <summary>
        /// Gets the unique ID of the <see cref="BasketItem"/>.
        /// </summary>
        [DataMember(Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets a flag indicating whether this <see cref="BasketItem"/> is allowed to be removed from the Basket.
        /// </summary>
        [DataMember(Name = "removable")]
        public bool Removable { get; set; }
    }

    /// <summary>
    /// A <see cref="BasketItem"/> that represents a product purchase.
    /// </summary>
    public class PurchaseBasketItem: BasketItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseBasketItem"/> class, setting its base type to "PURCHASE".
        /// </summary>
        public PurchaseBasketItem() : base("PURCHASE") { }

        /// <summary>
        /// Gets the Date and Time of the transaction, together with the offset from UTC.
        /// </summary>
        [DataMember(Name = "purchasedAt")]
        public DateTimeOffset PurchasedAt { get; set; }

        /// <summary>
        /// Gets the <see cref="Model.Transaction"/> sub-object.
        /// </summary>
        [DataMember(Name = "transaction")]
        public Transaction Transaction { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Model.Product"/> sub-object.
        /// </summary>
        [DataMember(Name = "product")]
        public Product Product { get; set; }

        /// <summary>
        /// Gets a List of <see cref="Model.Voucher"/>s associated with this purchase.  (Usually a list of one)
        /// </summary>
        [DataMember(Name = "vouchers")]
        public List<Voucher> Vouchers { get; set; }
    }

    /// <summary>
    /// A <see cref="BasketItem"/> that represents a refund or return.
    /// </summary>
    public class RefundBasketItem: BasketItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefundBasketItem"/> class, setting its base type to "REFUND".
        /// </summary>
        public RefundBasketItem() : base("REFUND") { }

        /// <summary>
        /// Gets the ID of the original Purchase transaction.
        /// </summary>
        [DataMember(Name = "purchaseTouchTransactionId")]
        public string PurchaseTouchTransactionId { get; set; }

        /// <summary>
        /// Gets the <see cref="Model.Transaction"/> sub-object.
        /// </summary>
        [DataMember(Name = "transaction")]
        public Transaction Transaction { get; set; }

        /// <summary>
        /// Gets the <see cref="Model.Product"/> sub-object.
        /// </summary>
        [DataMember(Name = "product")]
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the printable <see cref="Model.Voucher"/> associated with this return.
        /// </summary>
        [DataMember(Name = "refundVoucher")]
        public Voucher RefundVoucher { get; set; }
    }

    internal class BasketItemJsonConverter : JsonConverter
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