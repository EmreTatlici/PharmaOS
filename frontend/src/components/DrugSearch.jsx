function DrugSearch({ value, onChange, count }) {
  return (
    <div className="panel-header">
      <div>
        <h3>İlaç Listesi</h3>
        <p>Ürün adı, barkod veya etken madde ile arayın.</p>
      </div>

      <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
        <input
          type="text"
          autoComplete="off"
          value={value}
          onChange={(event) => onChange(event.target.value)}
          placeholder="🔎 Ürün veya barkod ara..."
          style={{
            width: "280px",
            padding: "10px 14px",
            border: "1px solid #e2e8f0",
            borderRadius: "8px",
          }}
        />

        <span>{count} ürün</span>
      </div>
    </div>
  );
}

export default DrugSearch;
