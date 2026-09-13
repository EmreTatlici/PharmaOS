import { useEffect, useState } from "react";
import { BrowserRouter, NavLink, Route, Routes } from "react-router-dom";

import DrugSearch from "./components/DrugSearch";
import DrugTable from "./components/DrugTable";

import "./App.css";

const menuGroups = [
  {
    title: "ANA MENÜ",
    items: [
      { path: "/", icon: "⌂", label: "Dashboard" },
      { path: "/stok", icon: "💊", label: "İlaç & Stok" },
      { path: "/hastalar", icon: "👤", label: "Hastalar" },
      { path: "/tedarikciler", icon: "🏢", label: "Tedarikçiler" },
    ],
  },
  {
    title: "İŞLEMLER",
    items: [
      { path: "/alis-faturalari", icon: "📦", label: "Alış Faturaları" },
      { path: "/satis", icon: "🧾", label: "Satış İşlemleri" },
      { path: "/receteler", icon: "📋", label: "Reçeteler" },
    ],
  },
  {
    title: "YÖNETİM",
    items: [
      { path: "/raporlar", icon: "📊", label: "Raporlar" },
      { path: "/ayarlar", icon: "⚙️", label: "Ayarlar" },
    ],
  },
];

function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="logo">
        <div className="logo-icon">P</div>

        <div>
          <h1>PharmaOS</h1>
          <span>Eczane Yönetim Sistemi</span>
        </div>
      </div>

      <nav className="menu">
        {menuGroups.map((group) => (
          <div key={group.title}>
            <div className="menu-title">{group.title}</div>

            {group.items.map((item) => (
              <NavLink
                key={item.path}
                to={item.path}
                className={({ isActive }) =>
                  `menu-item ${isActive ? "active" : ""}`
                }
              >
                <span>{item.icon}</span>
                {item.label}
              </NavLink>
            ))}
          </div>
        ))}
      </nav>

      <div className="sidebar-bottom">
        <div className="user-avatar">E</div>

        <div className="user-info">
          <strong>Eczacı</strong>
          <span>Yönetici</span>
        </div>
      </div>
    </aside>
  );
}

function Topbar() {
  return (
    <header className="topbar">
      <div>
        <p className="welcome">Günaydın 👋</p>
        <h2>PharmaOS</h2>
      </div>

      <div className="topbar-actions">
        <div className="search">
          <span>⌕</span>
          <input placeholder="İlaç, hasta veya barkod ara..." />
          <kbd>⌘ K</kbd>
        </div>

        <button className="notification">🔔</button>

        <div className="profile">
          <div className="profile-avatar">E</div>

          <div>
            <strong>Eczacı</strong>
            <span>Yönetici</span>
          </div>
        </div>
      </div>
    </header>
  );
}

function Dashboard() {
  const [dailySales, setDailySales] = useState(null);
  const [totalStock, setTotalStock] = useState(0);
  const [criticalStockCount, setCriticalStockCount] = useState(0);
  const [criticalStockItems, setCriticalStockItems] = useState([]);
  const [expiringStockCount, setExpiringStockCount] = useState(0);
  const [expiringStockItems, setExpiringStockItems] = useState([]);

  useEffect(() => {
    Promise.all([
      fetch(
        "http:" +
          "//localhost:5116/api/Sales/daily?pharmacyId=1"
      ),
      fetch("http:" + "//localhost:5116/api/InventoryItems"),
      fetch(
        "http:" +
          "//localhost:5116/api/StockPolicies?pharmacyId=1"
      ),
    ])
      .then(
        async ([
          salesResponse,
          inventoryResponse,
          criticalStockResponse,
        ]) => {
          if (
            !salesResponse.ok ||
            !inventoryResponse.ok ||
            !criticalStockResponse.ok
          ) {
            throw new Error("Dashboard verileri alınamadı.");
          }

          const salesData = await salesResponse.json();
          const inventoryData = await inventoryResponse.json();
          const criticalStockData =
            await criticalStockResponse.json();

          // Daily sales
          setDailySales(salesData);

          // Total stock
          const stockTotal = inventoryData.reduce(
            (total, item) => total + item.quantity,
            0
          );

          setTotalStock(stockTotal);

          // Critical stock
          const criticalItems = criticalStockData.filter(
            (item) => item.isCritical
          );

          setCriticalStockItems(criticalItems);
          setCriticalStockCount(criticalItems.length);

          // Expiring stock
          const today = new Date();
          const thirtyDaysLater = new Date();

          thirtyDaysLater.setDate(today.getDate() + 30);

          const expiringItems = inventoryData.filter((item) => {
            const expirationDate = new Date(item.expirationDate);

            return (
              item.quantity > 0 &&
              expirationDate >= today &&
              expirationDate <= thirtyDaysLater
            );
          });

          setExpiringStockItems(expiringItems);
          setExpiringStockCount(expiringItems.length);
        }
      )
      .catch(() => {
        setDailySales(null);
        setTotalStock(0);
        setCriticalStockCount(0);
        setCriticalStockItems([]);
        setExpiringStockCount(0);
        setExpiringStockItems([]);
      });
  }, []);

  return (
    <section className="dashboard">
      <div className="section-heading">
        <div>
          <h3>Genel Bakış</h3>
          <p>
            Eczanenizin güncel durumunu buradan takip edin.
          </p>
        </div>

        <button className="primary-button">+ Yeni İşlem</button>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon green">💰</div>

          <div>
            <span>Bugünkü Satış</span>

            <strong>
              {dailySales
                ? `₺${dailySales.totalAmount.toLocaleString(
                    "tr-TR"
                  )}`
                : "₺0"}
            </strong>

            <small className="positive">
              Günlük satış toplamı
            </small>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon blue">📦</div>

          <div>
            <span>Toplam Stok</span>

            <strong>
              {totalStock.toLocaleString("tr-TR")}
            </strong>

            <small>Mevcut fiziksel stok</small>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon orange">⚠️</div>

          <div>
            <span>Kritik Stok</span>

            <strong>{criticalStockCount}</strong>

            <small className="warning">
              Kontrol gerekiyor
            </small>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon red">⏰</div>

          <div>
            <span>Yaklaşan SKT</span>

            <strong>{expiringStockCount}</strong>

            <small className="danger">
              30 gün içinde
            </small>
          </div>
        </div>
      </div>

      <div className="content-grid">
        <div className="panel">
          <div className="panel-header">
            <div>
              <h3>Kritik Stoklar</h3>
              <p>Stok seviyesi düşük ürünler</p>
            </div>

            <button className="text-button">
              Tümünü Gör →
            </button>
          </div>

          <div className="table-header">
            <span>Ürün</span>
            <span>Barkod</span>
            <span>Stok</span>
          </div>

          {criticalStockItems.length === 0 ? (
            <div className="table-row">
              <span>
                Şu anda kritik stokta ürün bulunmuyor.
              </span>
            </div>
          ) : (
            criticalStockItems.map((item) => (
              <div className="table-row" key={item.id}>
                <div className="product">
                  <div className="product-icon">💊</div>

                  <strong>{item.drugName}</strong>
                </div>

                <span>{item.barcode}</span>

                <span className="stock critical">
                  {item.currentStock} adet
                </span>
              </div>
            ))
          )}
        </div>
      </div>

      <div className="panel">
        <div className="panel-header">
          <div>
            <h3>Yaklaşan SKT&apos;ler</h3>
            <p>Son kullanma tarihi yaklaşanlar</p>
          </div>

          <button className="text-button">
            Tümünü Gör →
          </button>
        </div>

        <div className="expiry-list">
          {expiringStockItems.length === 0 ? (
            <div className="expiry-item">
              <div className="product-icon">✓</div>

              <div className="expiry-info">
                <strong>Yaklaşan SKT bulunmuyor</strong>

                <span>
                  Önümüzdeki 30 gün içinde süresi dolacak
                  stok yok.
                </span>
              </div>
            </div>
          ) : (
            expiringStockItems.map((item) => {
              const expirationDate = new Date(
                item.expirationDate
              );

              const today = new Date();

              const daysRemaining = Math.ceil(
                (expirationDate - today) /
                  (1000 * 60 * 60 * 24)
              );

              return (
                <div
                  className="expiry-item"
                  key={item.id}
                >
                  <div className="product-icon">💊</div>

                  <div className="expiry-info">
                    <strong>{item.drugName}</strong>

                    <span>
                      {item.quantity} adet · Lot:{" "}
                      {item.batchNumber}
                    </span>
                  </div>

                  <div className="expiry-date">
                    <strong>{daysRemaining} gün</strong>
                    <span>kaldı</span>
                  </div>
                </div>
              );
            })
          )}
        </div>
      </div>

      <div className="panel recent-panel">
        <div className="panel-header">
          <div>
            <h3>Son İşlemler</h3>
            <p>Bugün gerçekleştirilen son işlemler</p>
          </div>

          <button className="text-button">
            Tüm işlemler →
          </button>
        </div>

        <div className="recent-grid">
          <div className="recent-item">
            <div className="recent-icon sale">₺</div>

            <div>
              <strong>Satış işlemi</strong>
              <span>Parol 500 mg × 2</span>
            </div>

            <b>+ ₺85,40</b>
            <time>2 dk önce</time>
          </div>

          <div className="recent-item">
            <div className="recent-icon purchase">
              📦
            </div>

            <div>
              <strong>Alış faturası</strong>
              <span>ABC İlaç Deposu</span>
            </div>

            <b>₺12.450</b>
            <time>18 dk önce</time>
          </div>

          <div className="recent-item">
            <div className="recent-icon patient">
              👤
            </div>

            <div>
              <strong>Hasta kaydı</strong>
              <span>Yeni hasta eklendi</span>
            </div>

            <b>---</b>
            <time>32 dk önce</time>
          </div>
        </div>
      </div>
    </section>
  );
}

function DrugsPage() {
  const [drugs, setDrugs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");

  const [formData, setFormData] = useState({
    barcode: "",
    name: "",
    activeIngredient: "",
    manufacturer: "",
    form: "",
    prescriptionType: "Normal",
  });

  const fetchDrugs = async () => {
    setLoading(true);
    setError("");

    try {
      const [drugsResponse, inventoryResponse] =
        await Promise.all([
          fetch("http:" + "//localhost:5116/api/Drugs"),
          fetch(
            "http:" +
              "//localhost:5116/api/InventoryItems"
          ),
        ]);

      if (
        !drugsResponse.ok ||
        !inventoryResponse.ok
      ) {
        throw new Error("Veriler alınamadı.");
      }

      const drugsData = await drugsResponse.json();
      const inventoryData =
        await inventoryResponse.json();

      const drugsWithInventory = drugsData.map(
        (drug) => {
          const drugInventory =
            inventoryData.filter(
              (item) => item.drugId === drug.id
            );

          const totalStock =
            drugInventory.reduce(
              (total, item) =>
                total + item.quantity,
              0
            );

          const salePrice =
            drugInventory.length > 0
              ? drugInventory[0].salePrice
              : null;

          return {
            ...drug,
            totalStock,
            salePrice,
          };
        }
      );

      setDrugs(drugsWithInventory);
      setLoading(false);
    } catch {
      setError(
        "İlaç ve stok verileri yüklenirken bir hata oluştu."
      );

      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDrugs();
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;

    setFormData((previous) => ({
      ...previous,
      [name]: value,
    }));
  };

  const filteredDrugs = drugs.filter((drug) => {
    const q = searchTerm.toLowerCase();

    return (
      drug.name.toLowerCase().includes(q) ||
      drug.barcode.includes(searchTerm) ||
      drug.activeIngredient
        .toLowerCase()
        .includes(q)
    );
  });

  const handleSubmit = async (event) => {
    event.preventDefault();

    setError("");

    try {
      const response = await fetch(
        "http:" + "//localhost:5116/api/Drugs",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(formData),
        }
      );

      if (!response.ok) {
        throw new Error("İlaç kaydedilemedi.");
      }

      setFormData({
        barcode: "",
        name: "",
        activeIngredient: "",
        manufacturer: "",
        form: "",
        prescriptionType: "Normal",
      });

      setShowForm(false);

      fetchDrugs();
    } catch {
      setError(
        "İlaç kaydedilirken bir hata oluştu."
      );
    }
  };

  return (
    <section className="dashboard">
      <div className="section-heading">
        <div>
          <h3>İlaç & Stok</h3>
          <p>
            İlaçlarınızı, stoklarınızı, lot ve SKT
            bilgilerinizi yönetin.
          </p>
        </div>

        <button
          className="primary-button"
          onClick={() => setShowForm(true)}
        >
          + Yeni İlaç
        </button>
      </div>

      {showForm && (
        <div className="panel">
          <div className="panel-header">
            <div>
              <h3>Yeni İlaç Ekle</h3>
              <p>Yeni ilaç bilgilerini girin.</p>
            </div>

            <button
              className="text-button"
              onClick={() => setShowForm(false)}
            >
              İptal
            </button>
          </div>

          <form
            onSubmit={handleSubmit}
            className="drug-form"
          >
            <div className="form-grid">
              <div className="form-field">
                <label>Barkod</label>

                <input
                  name="barcode"
                  value={formData.barcode}
                  onChange={handleChange}
                  placeholder="869..."
                  required
                />
              </div>

              <div className="form-field">
                <label>İlaç Adı</label>

                <input
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  placeholder="Örn. Parol 500 mg"
                  required
                />
              </div>

              <div className="form-field">
                <label>Etken Madde</label>

                <input
                  name="activeIngredient"
                  value={formData.activeIngredient}
                  onChange={handleChange}
                  placeholder="Örn. Parasetamol"
                  required
                />
              </div>

              <div className="form-field">
                <label>Üretici</label>

                <input
                  name="manufacturer"
                  value={formData.manufacturer}
                  onChange={handleChange}
                  placeholder="Örn. Atabay"
                  required
                />
              </div>

              <div className="form-field">
                <label>Form</label>

                <input
                  name="form"
                  value={formData.form}
                  onChange={handleChange}
                  placeholder="Örn. Tablet"
                  required
                />
              </div>

              <div className="form-field">
                <label>Reçete Tipi</label>

                <select
                  name="prescriptionType"
                  value={formData.prescriptionType}
                  onChange={handleChange}
                >
                  <option value="Normal">
                    Normal
                  </option>
                  <option value="Kırmızı">
                    Kırmızı
                  </option>
                  <option value="Yeşil">
                    Yeşil
                  </option>
                  <option value="Turuncu">
                    Turuncu
                  </option>
                </select>
              </div>
            </div>

            <div className="form-actions">
              <button
                type="button"
                className="secondary-button"
                onClick={() => setShowForm(false)}
              >
                Vazgeç
              </button>

              <button
                type="submit"
                className="primary-button"
              >
                İlacı Kaydet
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="panel">
        <DrugSearch
          value={searchTerm}
          onChange={setSearchTerm}
          count={filteredDrugs.length}
        />

        {loading && (
          <div
            style={{
              padding: "40px",
              color: "#64748b",
            }}
          >
            İlaçlar yükleniyor...
          </div>
        )}

        {error && (
          <div
            style={{
              padding: "40px",
              color: "#dc2626",
            }}
          >
            {error}
          </div>
        )}

        {!loading && !error && (
          <DrugTable drugs={filteredDrugs} />
        )}
      </div>
    </section>
  );
}

function Page({ title, description }) {
  return (
    <section className="dashboard">
      <div className="section-heading">
        <div>
          <h3>{title}</h3>
          <p>{description}</p>
        </div>
      </div>

      <div className="panel">
        <div className="panel-header">
          <div>
            <h3>{title}</h3>
            <p>Bu bölüm üzerinde çalışıyoruz.</p>
          </div>
        </div>

        <div
          style={{
            padding: "40px",
            color: "#94a3b8",
          }}
        >
          PharmaOS bu ekranı yakında kullanıma hazır
          hale getirecek.
        </div>
      </div>
    </section>
  );
}

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <Sidebar />

        <main className="main-content">
          <Topbar />

          <Routes>
            <Route
              path="/"
              element={<Dashboard />}
            />

            <Route
              path="/stok"
              element={<DrugsPage />}
            />

            <Route
              path="/hastalar"
              element={
                <Page
                  title="Hastalar"
                  description="Hasta kayıtlarını ve hasta bilgilerini yönetin."
                />
              }
            />

            <Route
              path="/tedarikciler"
              element={
                <Page
                  title="Tedarikçiler"
                  description="Tedarikçi ve ilaç deposu bilgilerini yönetin."
                />
              }
            />

            <Route
              path="/alis-faturalari"
              element={
                <Page
                  title="Alış Faturaları"
                  description="Alış faturalarını ve fatura kalemlerini yönetin."
                />
              }
            />

            <Route
              path="/satis"
              element={
                <Page
                  title="Satış İşlemleri"
                  description="Eczane satış işlemlerini yönetin."
                />
              }
            />

            <Route
              path="/receteler"
              element={
                <Page
                  title="Reçeteler"
                  description="Reçete işlemlerini ve reçete geçmişini yönetin."
                />
              }
            />

            <Route
              path="/raporlar"
              element={
                <Page
                  title="Raporlar"
                  description="Eczanenizin finansal ve operasyonel raporlarını görüntüleyin."
                />
              }
            />

            <Route
              path="/ayarlar"
              element={
                <Page
                  title="Ayarlar"
                  description="PharmaOS uygulama ve eczane ayarlarını yönetin."
                />
              }
            />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;