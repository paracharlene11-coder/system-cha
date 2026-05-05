package com.example.foodordering.controller;
import org.springframework.web.bind.annotation.*;
import java.util.Map;
@RestController
@RequestMapping("/api/health")
@CrossOrigin
public class HealthController { @GetMapping public Map<String,String> health(){ return Map.of("status", "UP"); } }
