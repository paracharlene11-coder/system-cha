package com.example.foodordering.controller;

import com.example.foodordering.dto.PaymentRequest;
import com.example.foodordering.dto.PlaceOrderRequest;
import com.example.foodordering.dto.UpdateStatusRequest;
import com.example.foodordering.model.Order;
import com.example.foodordering.service.OrderService;
import jakarta.validation.Valid;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/orders")
@CrossOrigin
public class OrderController {

    private final OrderService orderService;

    public OrderController(OrderService orderService) {
        this.orderService = orderService;
    }

    @GetMapping
    public List<Order> list() {
        return orderService.listOrders();
    }

    @GetMapping("/{id}")
    public Order get(@PathVariable Long id) {
        return orderService.getOrder(id);
    }

    @PostMapping
    public Order place(@Valid @RequestBody PlaceOrderRequest request) {
        return orderService.placeOrder(request);
    }

    @PatchMapping("/{id}/status")
    public Order updateStatus(
            @PathVariable Long id,
            @Valid @RequestBody UpdateStatusRequest request
    ) {
        return orderService.updateStatus(id, request.getStatus());
    }

    @PutMapping("/{id}/payment")
    public Order updatePayment(
            @PathVariable Long id,
            @RequestBody PaymentRequest request
    ) {
        return orderService.updatePayment(id, request.getAmountPaid());
    }
}