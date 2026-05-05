package com.example.foodordering.config;

import com.example.foodordering.model.MenuItem;
import com.example.foodordering.repository.MenuItemRepository;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;

@Component
public class DataInitializer implements CommandLineRunner {
    private final MenuItemRepository menuItemRepository;
    public DataInitializer(MenuItemRepository menuItemRepository) { this.menuItemRepository = menuItemRepository; }
    @Override
    public void run(String... args) {
        if (menuItemRepository.count() > 0) return;
        menuItemRepository.save(new MenuItem("Margherita Pizza", "Pizza", "Tomato sauce, mozzarella, basil", new BigDecimal("250.00"), "https://images.unsplash.com/photo-1604382355076-af4b0eb60143?w=400", true));
        menuItemRepository.save(new MenuItem("Beef Burger", "Burgers", "Beef patty, cheese, lettuce, tomato", new BigDecimal("200.00"), "https://images.unsplash.com/photo-1550547660-d9450f859349?w=400", true));
        menuItemRepository.save(new MenuItem("Carbonara Pasta", "Pasta", "Creamy pasta with bacon and parmesan", new BigDecimal("180.00"), "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=400", true));
        menuItemRepository.save(new MenuItem("Chicken Wings", "Burgers", "Crispy wings with house sauce", new BigDecimal("150.00"), "https://images.unsplash.com/photo-1567620832903-9fc6debc209f?w=400", true));
        menuItemRepository.save(new MenuItem("Iced Coffee", "Drinks", "Cold brewed coffee with milk", new BigDecimal("100.00"), "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=400", true));
        menuItemRepository.save(new MenuItem("French Fries", "Sides", "Golden crispy fries", new BigDecimal("120.00"), "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=400", true));
    }
}
