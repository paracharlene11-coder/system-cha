package com.example.foodordering.controller;

import com.example.foodordering.model.Receipt;
import com.example.foodordering.service.OrderService;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/receipts")
@CrossOrigin
public class ReceiptController {
    private final OrderService orderService;
    public ReceiptController(OrderService orderService) { this.orderService = orderService; }
    @GetMapping("/{orderId}") public Receipt getByOrderId(@PathVariable Long orderId) { return orderService.getReceiptByOrderId(orderId); }
}
