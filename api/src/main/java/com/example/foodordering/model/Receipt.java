package com.example.foodordering.model;

import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Entity
@Table(name = "receipts")
public class Receipt {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    @Column(unique = true, nullable = false)
    private String receiptNumber;
    @OneToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "order_id")
    private Order order;
    @Column(precision = 10, scale = 2)
    private BigDecimal totalAmount;
    private LocalDateTime generatedAt;
    @PrePersist public void prePersist() { generatedAt = LocalDateTime.now(); }
    public Long getId() { return id; }
    public String getReceiptNumber() { return receiptNumber; }
    public void setReceiptNumber(String receiptNumber) { this.receiptNumber = receiptNumber; }
    public Order getOrder() { return order; }
    public void setOrder(Order order) { this.order = order; }
    public BigDecimal getTotalAmount() { return totalAmount; }
    public void setTotalAmount(BigDecimal totalAmount) { this.totalAmount = totalAmount; }
    public LocalDateTime getGeneratedAt() { return generatedAt; }
}
