let menuItems = [];
let cart = [];
let lastOrderId = null;

document.addEventListener("DOMContentLoaded", async () => {
    await loadMenu();
    renderCart();

    const categoryFilter = document.getElementById("categoryFilter");
    if (categoryFilter) {
        categoryFilter.addEventListener("change", renderMenu);
    }

    const placeOrderBtn =
        document.getElementById("placeOrderBtn") ||
        document.getElementById("placeOrderButton");

    if (placeOrderBtn) {
        placeOrderBtn.addEventListener("click", placeOrder);
    }

    const cartButton = document.getElementById("cartButton");
    if (cartButton) {
        cartButton.addEventListener("click", scrollToCart);
    }

    const viewReceiptButton = document.getElementById("viewReceiptButton");
    if (viewReceiptButton) {
        viewReceiptButton.addEventListener("click", () => {
            if (lastOrderId) {
                viewReceipt(lastOrderId);
            } else {
                alert("No order selected.");
            }
        });
    }
});

async function loadMenu() {
    try {
        const response = await fetch("/api/menu");

        if (!response.ok) {
            throw new Error("Failed to load menu.");
        }

        menuItems = await response.json();

        renderCategories();
        renderMenu();
    } catch (error) {
        console.error("Menu loading error:", error);

        const menuGrid = document.getElementById("menuGrid");
        if (menuGrid) {
            menuGrid.innerHTML = `<p style="color:red;">Failed to load menu items.</p>`;
        }
    }
}

function renderCategories() {
    const categoryFilter = document.getElementById("categoryFilter");

    if (!categoryFilter) {
        return;
    }

    const categories = [...new Set(menuItems.map(item => item.category))];

    categoryFilter.innerHTML = `<option value="ALL">All categories</option>`;

    categories.forEach(category => {
        categoryFilter.innerHTML += `
            <option value="${category}">${category}</option>
        `;
    });
}

function renderMenu() {
    const menuGrid = document.getElementById("menuGrid");

    if (!menuGrid) {
        console.error("menuGrid element not found.");
        return;
    }

    const categoryFilter = document.getElementById("categoryFilter");
    const selectedCategory = categoryFilter ? categoryFilter.value : "ALL";

    let filteredItems = menuItems;

    if (
        selectedCategory &&
        selectedCategory !== "ALL" &&
        selectedCategory !== "all"
    ) {
        filteredItems = menuItems.filter(item => item.category === selectedCategory);
    }

    if (!filteredItems || filteredItems.length === 0) {
        menuGrid.innerHTML = `<p>No menu items found.</p>`;
        return;
    }

    menuGrid.innerHTML = filteredItems.map(item => `
        <div class="menu-card">
            <img src="${item.imageUrl || ""}" alt="${item.name}" class="menu-image">

            <div class="menu-info">
                <h3>${item.name}</h3>
                <p>${item.description || ""}</p>
                <strong>₱${Number(item.price || 0).toFixed(2)}</strong>

                <button type="button" onclick="addToCart(${item.id})">
                    Add to Cart
                </button>
            </div>
        </div>
    `).join("");
}

function addToCart(menuItemId) {
    const item = menuItems.find(menu => Number(menu.id) === Number(menuItemId));

    if (!item) {
        alert("Menu item not found.");
        return;
    }

    const existing = cart.find(cartItem => Number(cartItem.menuItemId) === Number(menuItemId));

    if (existing) {
        existing.quantity += 1;
    } else {
        cart.push({
            menuItemId: item.id,
            name: item.name,
            price: Number(item.price || 0),
            quantity: 1
        });
    }

    renderCart();
}

function removeFromCart(menuItemId) {
    cart = cart.filter(item => Number(item.menuItemId) !== Number(menuItemId));
    renderCart();
}

function increaseQuantity(menuItemId) {
    const item = cart.find(cartItem => Number(cartItem.menuItemId) === Number(menuItemId));

    if (item) {
        item.quantity += 1;
    }

    renderCart();
}

function decreaseQuantity(menuItemId) {
    const item = cart.find(cartItem => Number(cartItem.menuItemId) === Number(menuItemId));

    if (!item) {
        return;
    }

    item.quantity -= 1;

    if (item.quantity <= 0) {
        removeFromCart(menuItemId);
    } else {
        renderCart();
    }
}

function renderCart() {
    const cartItems = document.getElementById("cartItems");
    const cartTotal = document.getElementById("cartTotal");
    const cartCount = document.getElementById("cartCount");
    const cartButton = document.getElementById("cartButton");

    const total = cart.reduce((sum, item) => {
        return sum + Number(item.price || 0) * Number(item.quantity || 0);
    }, 0);

    const count = cart.reduce((sum, item) => {
        return sum + Number(item.quantity || 0);
    }, 0);

    if (cartCount) {
        cartCount.textContent = count;
    }

    if (cartButton) {
        cartButton.innerHTML = `Cart <span id="cartCount">${count}</span>`;
    }

    if (cartTotal) {
        cartTotal.textContent = `₱${total.toFixed(2)}`;
    }

    if (!cartItems) {
        return;
    }

    if (cart.length === 0) {
        cartItems.innerHTML = `<p>Your cart is empty.</p>`;
        return;
    }

    cartItems.innerHTML = cart.map(item => `
        <div class="cart-item">
            <div>
                <strong>${item.name}</strong>
                <p>₱${Number(item.price).toFixed(2)} x ${item.quantity}</p>
            </div>

            <div class="cart-actions">
                <button type="button" onclick="decreaseQuantity(${item.menuItemId})">-</button>
                <span>${item.quantity}</span>
                <button type="button" onclick="increaseQuantity(${item.menuItemId})">+</button>
                <button type="button" onclick="removeFromCart(${item.menuItemId})">Remove</button>
            </div>
        </div>
    `).join("");
}

async function placeOrder() {
    if (cart.length === 0) {
        alert("Please add items to your cart first.");
        return;
    }

    const customerName = getInputValue("customerName", "name");
    const customerEmail = getInputValue("customerEmail", "email");
    const customerPhone = getInputValue("customerPhone", "phone");
    const deliveryAddress = getInputValue("deliveryAddress", "address");
    const paymentMethod = getSelectedPaymentMethod();

    if (!customerName || !customerEmail || !customerPhone || !deliveryAddress) {
        alert("Please complete customer information.");
        return;
    }

    const request = {
        customerName: customerName,
        customerEmail: customerEmail,
        customerPhone: customerPhone,
        deliveryAddress: deliveryAddress,
        paymentMethod: paymentMethod,
        items: cart.map(item => ({
            menuItemId: item.menuItemId,
            quantity: item.quantity
        }))
    };

    try {
        const response = await fetch("/api/orders", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            const error = await response.text();
            alert("Failed to place order:\n" + error);
            return;
        }

        const order = await response.json();

        lastOrderId = order.id;
        cart = [];
        renderCart();

        showOrderConfirmation(order);
    } catch (error) {
        console.error("Place order error:", error);
        alert("Place order error: " + error.message);
    }
}

function getInputValue(...ids) {
    for (const id of ids) {
        const element = document.getElementById(id);

        if (element && element.value !== undefined) {
            return element.value.trim();
        }
    }

    return "";
}

function getSelectedPaymentMethod() {
    const checkedPayment =
        document.querySelector('input[name="paymentMethod"]:checked') ||
        document.querySelector('input[name="payment"]:checked');

    if (checkedPayment) {
        return checkedPayment.value;
    }

    return "CASH_ON_DELIVERY";
}

function showOrderConfirmation(order) {
    lastOrderId = order.id;

    const confirmationSection =
        document.getElementById("orderConfirmation") ||
        document.getElementById("confirmation");

    const orderIdText =
        document.getElementById("orderIdText") ||
        document.getElementById("orderCode");

    const orderStatusText =
        document.getElementById("orderStatusText") ||
        document.getElementById("orderStatus");

    const receiptBox = document.getElementById("receiptBox");

    if (orderIdText) {
        orderIdText.textContent = order.orderCode;
    }

    if (orderStatusText) {
        orderStatusText.textContent = order.status;
    }

    if (confirmationSection) {
        confirmationSection.classList.remove("hidden");
        confirmationSection.style.display = "block";
        confirmationSection.scrollIntoView({ behavior: "smooth" });
    } else {
        alert("Order placed successfully!\nOrder ID: " + order.orderCode);
    }

    if (receiptBox) {
        receiptBox.classList.remove("hidden");
        receiptBox.style.display = "block";
        receiptBox.textContent = "";
    }

    viewReceipt(order.id);
}

async function viewReceipt(orderId) {
    if (!orderId) {
        alert("No order selected.");
        return;
    }

    const response = await fetch(`/api/orders/${orderId}`);

    if (!response.ok) {
        alert("Receipt/order not found.");
        return;
    }

    const order = await response.json();

    const totalAmount = Number(order.totalAmount || 0);
    const amountPaid = Number(order.amountPaid || 0);
    const changeAmount = Number(order.changeAmount || 0);

    let receipt = "";

    receipt += "Charlie's Food\n";
    receipt += `Receipt: RCPT-${order.orderCode}\n`;
    receipt += `Order: ${order.orderCode}\n`;
    receipt += `Customer: ${order.customerName}\n\n`;

    if (order.items && order.items.length > 0) {
        order.items.forEach(item => {
            receipt += `${item.itemName} x ${item.quantity}   ₱${Number(item.lineTotal || 0).toFixed(2)}\n`;
        });
    }

    receipt += `\nTotal: ₱${totalAmount.toFixed(2)}\n`;
    receipt += `Payment Method: ${order.paymentMethod}\n`;
    receipt += `Payment Status: ${order.paymentStatus}\n`;
    receipt += `Amount Paid: ₱${amountPaid.toFixed(2)}\n`;
    receipt += `Change: ₱${changeAmount.toFixed(2)}\n`;
    receipt += `Order Status: ${order.status}\n`;
    receipt += `Generated: ${new Date().toISOString()}`;

    const receiptBox = document.getElementById("receiptBox");

    if (receiptBox) {
        receiptBox.classList.remove("hidden");
        receiptBox.style.display = "block";
        receiptBox.textContent = receipt;
    } else {
        alert(receipt);
    }
}

function scrollToCart() {
    const cartSection =
        document.getElementById("cartSection") ||
        document.querySelector(".checkout");

    if (cartSection) {
        cartSection.scrollIntoView({ behavior: "smooth" });
    }
}