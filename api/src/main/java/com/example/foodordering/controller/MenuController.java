package com.example.foodordering.controller;

import com.example.foodordering.model.MenuItem;
import com.example.foodordering.repository.MenuItemRepository;
import org.springframework.web.bind.annotation.*;
import java.util.List;

@RestController
@RequestMapping("/api/menu")
@CrossOrigin
public class MenuController {
    private final MenuItemRepository menuItemRepository;
    public MenuController(MenuItemRepository menuItemRepository) { this.menuItemRepository = menuItemRepository; }
    @GetMapping
    public List<MenuItem> list() { return menuItemRepository.findByActiveTrueOrderByCategoryAscNameAsc(); }
}
