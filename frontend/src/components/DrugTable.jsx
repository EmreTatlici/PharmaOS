function DrugTable({ drugs }) {
  return (
    <div className="table">
      <div className="table-header">
        <span>Ürün</span>
        <span>Barkod</span>
        <span>Etken Madde</span>
        <span>Üretici</span>
        <span>Stok</span>
        <span>Satış Fiyatı</span>
        <span>Durum</span>
      </div>

      {drugs.map((drug) => (
        <div className="table-row" key={drug.id}>
          <span className="product">
            <span className="product-icon">💊</span>
            <span>{drug.name}</span>
          </span>

          <span>{drug.barcode}</span>
          <span>{drug.activeIngredient}</span>
          <span>{drug.manufacturer}</span>
          <span>{drug.totalStock}</span>
          <span>
            {drug.salePrice !== null
              ? `${Number(drug.salePrice).toFixed(2)} ₺`
              : "-"}
          </span>

          <span>{drug.totalStock > 0 ? "Stokta" : "Stok Yok"}</span>
        </div>
      ))}
    </div>
  );
}

export default DrugTable;
