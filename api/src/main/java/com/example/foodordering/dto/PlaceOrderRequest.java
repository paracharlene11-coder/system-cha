package com.example.foodordering.dto;

import com.example.foodordering.model.PaymentMethod;
import jakarta.validation.Valid;
import jakarta.validation.constraints.*;
import java.util.List;

public class PlaceOrderRequest {
    @NotBlank private String customerName;
    private String customerEmail;
    @NotBlank private String customerPhone;
    @NotBlank private String deliveryAddress;
    @NotNull private PaymentMethod paymentMethod;
    @NotEmpty @Valid private List<OrderLineRequest> items;

    public String getCustomerName() { return customerName; }
    public void setCustomerName(String customerName) { this.customerName = customerName; }
    public String getCustomerEmail() { return customerEmail; }
    public void setCustomerEmail(String customerEmail) { this.customerEmail = customerEmail; }
    public String getCustomerPhone() { return customerPhone; }
    public void setCustomerPhone(String customerPhone) { this.customerPhone = customerPhone; }
    public String getDeliveryAddress() { return deliveryAddress; }
    public void setDeliveryAddress(String deliveryAddress) { this.deliveryAddress = deliveryAddress; }
    public PaymentMethod getPaymentMethod() { return paymentMethod; }
    public void setPaymentMethod(PaymentMethod paymentMethod) { this.paymentMethod = paymentMethod; }
    public List<OrderLineRequest> getItems() { return items; }
    public void setItems(List<OrderLineRequest> items) { this.items = items; }

    public static class OrderLineRequest {
        @NotNull private Long menuItemId;
        @Min(1) private int quantity;
        public Long getMenuItemId() { return menuItemId; }
        public void setMenuItemId(Long menuItemId) { this.menuItemId = menuItemId; }
        public int getQuantity() { return quantity; }
        public void setQuantity(int quantity) { this.quantity = quantity; }
    }
}
