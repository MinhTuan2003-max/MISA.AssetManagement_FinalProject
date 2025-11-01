/**
 * Enum ánh xạ mã loại tài sản sang tên viết tắt hoặc nhãn hiển thị
 * UpdatedBy: HMTuan (01/11/2025) - Convert to PascalCase
 */
export const FixedAssetCategoryEnum = Object.freeze({
  1: { Code: "HOUSE", Name: "Nhà, công trình xây dựng" },
  2: { Code: "ARCH", Name: "Vật kiến trúc" },
  3: { Code: "CAR", Name: "Xe ô tô" },
  4: { Code: "TRANSPORT", Name: "Phương tiện vận tải khác" },
  5: { Code: "EQUIP", Name: "Máy móc, thiết bị" },
  6: { Code: "PLANT", Name: "Cây lâu năm, súc vật" },
  7: { Code: "OTHER", Name: "TSCĐ hữu hình khác" },
});

/**
 * Hàm tiện ích: trả về mảng select-option (Code + Name)
 * UpdatedBy: HMTuan (01/11/2025)
 */
export const getFixedAssetCategoryOptions = () => {
  return Object.entries(FixedAssetCategoryEnum).map(([id, { Code, Name }]) => ({
    Id: parseInt(id),
    Code,
    Name,
  }));
};
