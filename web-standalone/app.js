window.API_URL='http://localhost:8080';

const api = window.API_URL || '';
let menuItems = [];
let cart = new Map();

const money = v => new Intl.NumberFormat('en-PH',{style:'currency',currency:'PHP'}).format(v);

async function loadMenu(){
  const res = await fetch(`${api}/api/menu`);
  menuItems = await res.json();
  renderCategories();
  renderMenu();
}

function renderCategories(){
  const select = document.getElementById('categoryFilter');
  [...new Set(menuItems.map(x=>x.category))].forEach(c=>{
    const opt=document.createElement('option'); opt.value=c; opt.textContent=c; select.appendChild(opt);
  });
  select.addEventListener('change', renderMenu);
}

function renderMenu(){
  const category = document.getElementById('categoryFilter').value;
  const grid = document.getElementById('menuGrid');
  grid.innerHTML = '';
  menuItems.filter(x=>category==='all'||x.category===category).forEach(item=>{
    const card=document.createElement('article'); card.className='menu-card';
    card.innerHTML=`<img src="${item.imageUrl}" alt="${item.name}"><h3>${item.name}</h3><p>${item.description||''}</p><span class="price">${money(item.price)}</span><button>Add to Cart</button>`;
    card.querySelector('button').onclick=()=>addToCart(item.id);
    grid.appendChild(card);
  });
}

function addToCart(id){ cart.set(id, (cart.get(id)||0)+1); renderCart(); }
function changeQty(id, delta){
  const next = (cart.get(id)||0)+delta;
  if(next<=0) cart.delete(id); else cart.set(id,next);
  renderCart();
}
function renderCart(){
  const box=document.getElementById('cartItems'); box.innerHTML='';
  let total=0, count=0;
  for(const [id, qty] of cart.entries()){
    const item=menuItems.find(x=>x.id===id); if(!item) continue;
    total += item.price*qty; count += qty;
    const row=document.createElement('div'); row.className='cart-line';
    row.innerHTML=`<div><strong>${item.name}</strong><br>${money(item.price)} x ${qty}</div><div class="cart-controls"><button>-</button><span>${qty}</span><button>+</button></div>`;
    row.querySelectorAll('button')[0].onclick=()=>changeQty(id,-1);
    row.querySelectorAll('button')[1].onclick=()=>changeQty(id,1);
    box.appendChild(row);
  }
  if(count===0) box.innerHTML='<p>Your cart is empty.</p>';
  document.getElementById('cartCount').textContent=count;
  document.getElementById('cartTotal').textContent=money(total);
}

async function placeOrder(){
  if(cart.size===0){ alert('Please add items to cart first.'); return; }
  const paymentMethod = document.querySelector('input[name="payment"]:checked').value;
  const request = {
    customerName: document.getElementById('name').value,
    customerEmail: document.getElementById('email').value,
    customerPhone: document.getElementById('phone').value,
    deliveryAddress: document.getElementById('address').value,
    paymentMethod,
    items: [...cart.entries()].map(([menuItemId, quantity])=>({menuItemId, quantity}))
  };
  const res = await fetch(`${api}/api/orders`, {method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(request)});
  if(!res.ok){ const err=await res.json(); alert(err.error || 'Could not place order'); return; }
  const order = await res.json();
  cart.clear(); renderCart();
  document.getElementById('confirmation').classList.remove('hidden');
  document.getElementById('orderCode').textContent=order.orderCode;
  document.getElementById('orderStatus').textContent=order.status;
  document.getElementById('viewReceiptButton').onclick=()=>viewReceipt(order.id);
  document.getElementById('confirmation').scrollIntoView({behavior:'smooth'});
}

async function viewReceipt(orderId){
  const res = await fetch(`${api}/api/receipts/${orderId}`);
  const receipt = await res.json();
  const order = receipt.order;
  const lines = order.items.map(i=>`${i.itemName} x ${i.quantity}   ${money(i.lineTotal)}`).join('\n');
  document.getElementById('receiptBox').textContent = `Charlie's Food\nReceipt: ${receipt.receiptNumber}\nOrder: ${order.orderCode}\nCustomer: ${order.customerName}\n\n${lines}\n\nTotal: ${money(order.totalAmount)}\nPayment: ${order.paymentMethod}\nStatus: ${order.paymentStatus}\nGenerated: ${receipt.generatedAt}`;
  document.getElementById('receiptBox').classList.remove('hidden');
}

document.getElementById('placeOrderButton').addEventListener('click', placeOrder);
document.getElementById('cartButton').addEventListener('click',()=>document.querySelector('.checkout').scrollIntoView({behavior:'smooth'}));
document.getElementById('name').addEventListener('input',e=>document.getElementById('customerTitle').textContent=e.target.value||'Customer');
loadMenu().catch(err=>alert('Cannot connect to API. Start the backend first.'));
renderCart();
