import { useEffect, useState } from "react";
import { BrowserRouter, NavLink, Route, Routes } from "react-router-dom";

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

import DrugSearch from "./components/DrugSearch";
import DrugTable from "./components/DrugTable";

function Dashboard() {
  return (
    <section className="dashboard">
      <div className="section-heading">
        <div>
          <h3>Genel Bakış</h3>
          <p>Eczanenizin güncel durumunu buradan takip edin.</p>
        </div>

        <button className="primary-button">+ Yeni İşlem</button>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon green">💰</div>
          <div>
            <span>Bugünkü Satış</span>
            <strong>₺24.850</strong>
            <small className="positive">↑ %12,5 geçen haftaya göre</small>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon blue">📦</div>
          <div>
            <span>Toplam Stok</span>
            <strong>8.426</strong>
            <small>1.284 farklı ürün</small>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon orange">⚠️</div>
          <div>
            <span>Kritik Stok</span>
            <strong>24</strong>
            <small className="warning">Kontrol gerekiyor</small>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon red">⏰</div>
          <div>
            <span>Yaklaşan SKT</span>
            <strong>17</strong>
            <small className="danger">30 gün içinde</small>
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

            <button className="text-button">Tümünü Gör →</button>
          </div>

          <div className="table">
            <span>Ürün</span>
            <span>Barkod</span>
            <span>Etken Madde</span>
            <span>Üretici</span>
          </div>
          <div className="table-header">
            <span>Ürün</span>
            <span>Barkod</span>
            <span>Üretici</span>
            <span>Stok</span>
            <span>Satış Fiyatı</span>
          </div>

          {[
            ["Parol 500 mg", "8699514090012", "3 adet", "critical"],
            ["Augmentin 1000 mg", "8699546011234", "5 adet", "critical"],
            ["Nexium 40 mg", "8699825098765", "8 adet", "low"],
            ["Ventolin 100 mcg", "8699567012345", "9 adet", "low"],
          ].map(([name, barcode, stock, type]) => (
            <div className="table-row" key={barcode}>
              <div className="product">
                <div className="product-icon">💊</div>
                <strong>{name}</strong>
              </div>

              <span>{barcode}</span>

              <span className={`stock ${type}`}>{stock}</span>
            </div>
          ))}
        </div>
      </div>

      <div className="panel">
        <div className="panel-header">
          <div>
            <h3>Yaklaşan SKT&apos;ler</h3>
            <p>Son kullanma tarihi yaklaşanlar</p>
          </div>

          <button className="text-button">Tümünü Gör →</button>
        </div>

        <div className="expiry-list">
          {[
            ["Dolorex 50 mg", "12 adet · Lot: DLR4521", "18 gün"],
            ["Voltaren Emulgel", "8 adet · Lot: VLT2319", "24 gün"],
            ["Calpol 120 mg", "6 adet · Lot: CPL7742", "29 gün"],
            ["Lasix 40 mg", "4 adet · Lot: LSX1028", "30 gün"],
          ].map(([name, detail, days]) => (
            <div className="expiry-item" key={name}>
              <div className="product-icon">💊</div>

              <div className="expiry-info">
                <strong>{name}</strong>
                <span>{detail}</span>
              </div>

              <div className="expiry-date">
                <strong>{days}</strong>
                <span>kaldı</span>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="panel recent-panel">
        <div className="panel-header">
          <div>
            <h3>Son İşlemler</h3>
            <p>Bugün gerçekleştirilen son işlemler</p>
          </div>

          <button className="text-button">Tüm işlemler →</button>
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
            <div className="recent-icon purchase">📦</div>

            <div>
              <strong>Alış faturası</strong>
              <span>ABC İlaç Deposu</span>
            </div>

            <b>₺12.450</b>
            <time>18 dk önce</time>
          </div>

          <div className="recent-item">
            <div className="recent-icon patient">👤</div>

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
      const [drugsResponse, inventoryResponse] = await Promise.all([
        fetch("http:" + "//localhost:5116/api/Drugs"),
        fetch("http:" + "//localhost:5116/api/InventoryItems"),
      ]);

      if (!drugsResponse.ok || !inventoryResponse.ok) {
        throw new Error("Veriler alınamadı.");
      }

      const drugsData = await drugsResponse.json();
      const inventoryData = await inventoryResponse.json();

      const drugsWithInventory = drugsData.map((drug) => {
        const drugInventory = inventoryData.filter(
          (item) => item.drugId === drug.id,
        );

        const totalStock = drugInventory.reduce(
          (total, item) => total + item.quantity,
          0,
        );

        const salePrice =
          drugInventory.length > 0 ? drugInventory[0].salePrice : null;

        return {
          ...drug,
          totalStock,
          salePrice,
        };
      });

      setDrugs(drugsWithInventory);
      setLoading(false);
    } catch {
      setError("İlaç ve stok verileri yüklenirken bir hata oluştu.");
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
      drug.activeIngredient.toLowerCase().includes(q)
    );
  });

  const handleSubmit = async (event) => {
    event.preventDefault();

    setError("");

    try {
      const response = await fetch("http:" + "//localhost:5116/api/Drugs", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

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
      setError("İlaç kaydedilirken bir hata oluştu.");
    }
  };

  return (
    <section className="dashboard">
      <div className="section-heading">
        <div>
          <h3>İlaç & Stok</h3>
          <p>İlaçlarınızı, stoklarınızı, lot ve SKT bilgilerinizi yönetin.</p>
        </div>

        <button className="primary-button" onClick={() => setShowForm(true)}>
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

            <button className="text-button" onClick={() => setShowForm(false)}>
              İptal
            </button>
          </div>

          <form onSubmit={handleSubmit} className="drug-form">
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
                  <option value="Normal">Normal</option>
                  <option value="Kırmızı">Kırmızı</option>
                  <option value="Yeşil">Yeşil</option>
                  <option value="Turuncu">Turuncu</option>
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

              <button type="submit" className="primary-button">
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
          <div style={{ padding: "40px", color: "#64748b" }}>
            İlaçlar yükleniyor...
          </div>
        )}

        {error && (
          <div style={{ padding: "40px", color: "#dc2626" }}>{error}</div>
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

        <div style={{ padding: "40px", color: "#94a3b8" }}>
          PharmaOS bu ekranı yakında kullanıma hazır hale getirecek.
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
            <Route path="/" element={<Dashboard />} />

            <Route path="/stok" element={<DrugsPage />} />

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
