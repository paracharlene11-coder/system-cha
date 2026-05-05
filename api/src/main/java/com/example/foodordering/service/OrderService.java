package com.example.foodordering.service;

import com.example.foodordering.dto.PlaceOrderRequest;
import com.example.foodordering.model.*;
import com.example.foodordering.repository.*;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

@Service
public class OrderService {

    private final MenuItemRepository menuItemRepository;
    private final OrderRepository orderRepository;
    private final PaymentRepository paymentRepository;
    private final ReceiptRepository receiptRepository;

    public OrderService(
            MenuItemRepository menuItemRepository,
            OrderRepository orderRepository,
            PaymentRepository paymentRepository,
            ReceiptRepository receiptRepository
    ) {
        this.menuItemRepository = menuItemRepository;
        this.orderRepository = orderRepository;
        this.paymentRepository = paymentRepository;
        this.receiptRepository = receiptRepository;
    }

    public List<Order> listOrders() {
        return orderRepository.findAllByOrderByCreatedAtDesc();
    }

    public Order getOrder(Long id) {
        return orderRepository.findById(id)
                .orElseThrow(() -> new IllegalArgumentException("Order not found: " + id));
    }

    @Transactional
    public Order placeOrder(PlaceOrderRequest request) {
        Order order = new Order();

        order.setOrderCode("ORD-" + System.currentTimeMillis());
        order.setCustomerName(request.getCustomerName());
        order.setCustomerEmail(request.getCustomerEmail());
        order.setCustomerPhone(request.getCustomerPhone());
        order.setDeliveryAddress(request.getDeliveryAddress());
        order.setPaymentMethod(request.getPaymentMethod());

        if (request.getPaymentMethod() == PaymentMethod.ONLINE_PAYMENT) {
            order.setPaymentStatus(PaymentStatus.PAID);
        } else {
            order.setPaymentStatus(PaymentStatus.UNPAID);
        }

        order.setStatus(OrderStatus.PREPARING);
        order.setAmountPaid(BigDecimal.ZERO);
        order.setChangeAmount(BigDecimal.ZERO);

        BigDecimal total = BigDecimal.ZERO;

        for (PlaceOrderRequest.OrderLineRequest line : request.getItems()) {
            MenuItem menuItem = menuItemRepository.findById(line.getMenuItemId())
                    .filter(MenuItem::isActive)
                    .orElseThrow(() -> new IllegalArgumentException("Invalid menu item: " + line.getMenuItemId()));

            BigDecimal lineTotal = menuItem.getPrice()
                    .multiply(BigDecimal.valueOf(line.getQuantity()));

            OrderItem orderItem = new OrderItem();
            orderItem.setMenuItemId(menuItem.getId());
            orderItem.setItemName(menuItem.getName());
            orderItem.setUnitPrice(menuItem.getPrice());
            orderItem.setQuantity(line.getQuantity());
            orderItem.setLineTotal(lineTotal);

            order.addItem(orderItem);

            total = total.add(lineTotal);
        }

        order.setTotalAmount(total);

        if (request.getPaymentMethod() == PaymentMethod.ONLINE_PAYMENT) {
            order.setAmountPaid(total);
            order.setChangeAmount(BigDecimal.ZERO);
        }

        Order saved = orderRepository.save(order);

        Payment payment = new Payment();
        payment.setOrder(saved);
        payment.setMethod(saved.getPaymentMethod());
        payment.setStatus(saved.getPaymentStatus());
        payment.setAmount(saved.getTotalAmount());

        if (payment.getStatus() == PaymentStatus.PAID) {
            payment.setPaidAt(LocalDateTime.now());
        }

        paymentRepository.save(payment);

        Receipt receipt = new Receipt();
        receipt.setOrder(saved);
        receipt.setReceiptNumber("RCPT-" + saved.getOrderCode());
        receipt.setTotalAmount(saved.getTotalAmount());
        receiptRepository.save(receipt);

        return saved;
    }

    @Transactional
    public Order updateStatus(Long id, OrderStatus status) {
        Order order = getOrder(id);

        order.setStatus(status);

        if (status == OrderStatus.COMPLETED &&
                order.getPaymentMethod() == PaymentMethod.CASH_ON_DELIVERY) {
            order.setPaymentStatus(PaymentStatus.PAID);

            if (order.getAmountPaid() == null ||
                    order.getAmountPaid().compareTo(BigDecimal.ZERO) == 0) {
                order.setAmountPaid(order.getTotalAmount());
                order.setChangeAmount(BigDecimal.ZERO);
            }

            markPaymentAsPaid(order);
        }

        return orderRepository.save(order);
    }

    @Transactional
    public Order updatePayment(Long id, BigDecimal amountPaid) {
        Order order = getOrder(id);

        if (amountPaid == null) {
            throw new IllegalArgumentException("Amount paid is required.");
        }

        if (order.getTotalAmount() == null) {
            throw new IllegalArgumentException("Order total amount is missing.");
        }

        if (amountPaid.compareTo(order.getTotalAmount()) < 0) {
            throw new IllegalArgumentException("Amount paid is less than the total amount.");
        }

        BigDecimal changeAmount = amountPaid.subtract(order.getTotalAmount());

        order.setAmountPaid(amountPaid);
        order.setChangeAmount(changeAmount);
        order.setPaymentStatus(PaymentStatus.PAID);
        order.setStatus(OrderStatus.COMPLETED);

        markPaymentAsPaid(order);

        return orderRepository.save(order);
    }

    private void markPaymentAsPaid(Order order) {
        Payment payment = paymentRepository.findByOrderId(order.getId())
                .orElse(null);

        if (payment != null) {
            payment.setStatus(PaymentStatus.PAID);
            payment.setAmount(order.getTotalAmount());
            payment.setPaidAt(LocalDateTime.now());
            paymentRepository.save(payment);
        }
    }

    public Receipt getReceiptByOrderId(Long orderId) {
        return receiptRepository.findByOrderId(orderId)
                .orElseThrow(() -> new IllegalArgumentException("Receipt not found for order: " + orderId));
    }
}