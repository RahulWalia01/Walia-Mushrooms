// ─── CONFIG ───────────────────────────────────────────────────────────────────
// Replace these values with your real credentials before going live
const API_BASE_URL        = "http://localhost:5170";       // Your C# API URL

// ─── PRODUCT DATA ───────────────────────────────────────────────────────────
const products = [
  {
    id: 1,
    name: "Premium Button Mushrooms",
    origin: "Haryana Farm, India",
    price: 120,
    unit: "500g",
    category: "fresh",
    export: false,
    icon: "🍄",
    bg: "green",
  },
  {
    id: 2,
    name: "Dried Shiitake (Export)",
    origin: "Cold-dried, Pesticide-free",
    price: 850,
    unit: "200g",
    category: "dried",
    export: true,
    icon: "🌰",
    bg: "brown",
  },
  {
    id: 3,
    name: "Oyster Mushrooms",
    origin: "Controlled Greenhouse",
    price: 160,
    unit: "400g",
    category: "fresh",
    export: true,
    icon: "🍄",
    bg: "green",
  },
  {
    id: 4,
    name: "Porcini (Dried, Export)",
    origin: "Wild-harvested blend",
    price: 1200,
    unit: "150g",
    category: "dried",
    export: true,
    icon: "🌿",
    bg: "dark",
  },
  {
    id: 5,
    name: "Portobello Caps",
    origin: "Himachal Pradesh",
    price: 280,
    unit: "300g",
    category: "fresh",
    export: false,
    icon: "🍄",
    bg: "brown",
  },
  {
    id: 6,
    name: "Mixed Exotic Pack",
    origin: "Curated Variety Box",
    price: 350,
    unit: "500g",
    category: "fresh",
    export: true,
    icon: "🎁",
    bg: "green",
  },
  {
    id: 7,
    name: "King Oyster (Dried)",
    origin: "Slow-dried, Umami-rich",
    price: 680,
    unit: "100g",
    category: "dried",
    export: true,
    icon: "🌾",
    bg: "dark",
  },
  {
    id: 8,
    name: "Cremini Mushrooms",
    origin: "North India Farm",
    price: 140,
    unit: "500g",
    category: "fresh",
    export: false,
    icon: "🍄",
    bg: "brown",
  },
];

// ─── CART STATE ──────────────────────────────────────────────────────────────
let cartCount = 0;

// ─── RENDER PRODUCTS ─────────────────────────────────────────────────────────
function renderProducts(filter) {
  const grid = document.getElementById("product-grid");

  const filtered =
    filter === "all"
      ? products
      : filter === "export"
      ? products.filter((p) => p.export)
      : products.filter((p) => p.category === filter);

  grid.innerHTML = filtered
    .map(
      (p) => `
      <div class="product-card">
        <div class="product-img ${p.bg}">
          <span style="font-size:4rem">${p.icon}</span>
          ${p.export ? '<span class="export-badge">Export Ready</span>' : ""}
        </div>
        <div class="product-info">
          <div class="product-name">${p.name}</div>
          <div class="product-origin">${p.origin}</div>
          <div class="product-footer">
            <div class="product-price">
              ₹${p.price} <span>/ ${p.unit}</span>
            </div>
            <button class="add-btn" onclick="addToCart('${p.name}')">+</button>
          </div>
        </div>
      </div>
    `
    )
    .join("");
}

// ─── FILTER TABS ─────────────────────────────────────────────────────────────
function filterProducts(category, btn) {
  document
    .querySelectorAll(".filter-tab")
    .forEach((t) => t.classList.remove("active"));
  btn.classList.add("active");
  renderProducts(category);
}

// ─── ADD TO CART ─────────────────────────────────────────────────────────────
function addToCart(name) {
  cartCount++;
  document.getElementById("cart-count").textContent = cartCount;
  showToast(`✓ ${name} added to cart`);
}

// ─── OPEN CART ───────────────────────────────────────────────────────────────
function openCart() {
  showToast(
    `🛒 You have ${cartCount} item${cartCount !== 1 ? "s" : ""} in your cart`
  );
}

// ─── TOAST NOTIFICATION ──────────────────────────────────────────────────────
function showToast(message) {
  const toast = document.getElementById("toast");
  toast.textContent = message;
  toast.classList.add("show");
  setTimeout(() => toast.classList.remove("show"), 2500);
}

// ─── INQUIRY FORM SUBMIT ─────────────────────────────────────────────────────
async function submitForm(event) {
  event.preventDefault();

  const form = event.target;
  const submitBtn = form.querySelector(".submit-btn");

  // Collect form values
  const payload = {
    fullName:     form.querySelector("input[name='fullName']").value.trim(),
    company:      form.querySelector("input[name='company']").value.trim(),
    email:        form.querySelector("input[name='email']").value.trim(),
    phone:        form.querySelector("input[name='phone']").value.trim(),
    orderType:    form.querySelector("select[name='orderType']").value,
    product:      form.querySelector("select[name='product']").value,
    quantity:     form.querySelector("input[name='quantity']").value.trim(),
    message:      form.querySelector("textarea[name='message']").value.trim(),
    submittedAt:  new Date().toISOString(),
  };

  // Disable button during submission
  submitBtn.disabled = true;
  submitBtn.textContent = "Sending...";

  try {
    // ── Step 1: Save to C# backend ──────────────────────────────────────────
    const apiResponse = await fetch(`${API_BASE_URL}/api/inquiries`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    if (!apiResponse.ok) {
      throw new Error(`API error: ${apiResponse.status}`);
    }

    showToast("✓ Inquiry sent! We'll respond within 24 hours.");
    form.reset();

  } catch (error) {
    console.error("Submission error:", error);
    showToast("⚠ Something went wrong. Please try again.");
  } finally {
    submitBtn.disabled = false;
    submitBtn.textContent = "Submit Inquiry →";
  }
}

// ─── SCROLL FADE-IN ANIMATION ────────────────────────────────────────────────
function initScrollAnimations() {
  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          entry.target.classList.add("visible");
        }
      });
    },
    { threshold: 0.1 }
  );

  document.querySelectorAll(".fade-in").forEach((el) => observer.observe(el));
}

// ─── INIT ─────────────────────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
  renderProducts("all");
  initScrollAnimations();
});
