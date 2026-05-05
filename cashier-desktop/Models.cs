using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FoodOrdering.Cashier;

public class Order
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("orderCode")]
    public string OrderCode { get; set; } = "";

    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = "";

    [JsonPropertyName("customerPhone")]
    public string CustomerPhone { get; set; } = "";

    [JsonPropertyName("deliveryAddress")]
    public string DeliveryAddress { get; set; } = "";

    [JsonPropertyName("paymentMethod")]
    public string PaymentMethod { get; set; } = "";

    [JsonPropertyName("paymentStatus")]
    public string PaymentStatus { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("amountPaid")]
    public decimal AmountPaid { get; set; }

    [JsonPropertyName("changeAmount")]
    public decimal ChangeAmount { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [JsonPropertyName("itemName")]
    public string ItemName { get; set; } = "";

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; set; }
}

public class Receipt
{
    [JsonPropertyName("receiptNumber")]
    public string ReceiptNumber { get; set; } = "";

    [JsonPropertyName("generatedAt")]
    public DateTime GeneratedAt { get; set; }

    [JsonPropertyName("order")]
    public Order Order { get; set; } = new();
}