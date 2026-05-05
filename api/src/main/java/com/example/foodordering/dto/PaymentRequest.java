package com.example.foodordering.dto;

import java.math.BigDecimal;

public class PaymentRequest {
    private BigDecimal amountPaid;

    public BigDecimal getAmountPaid() {
        return amountPaid;
    }

    public void setAmountPaid(BigDecimal amountPaid) {
        this.amountPaid = amountPaid;
    }
}