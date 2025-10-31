/**
 * Enum ánh xạ mã loại tài sản sang tên viết tắt hoặc nhãn hiển thị
 * CreatedBy: HMTuan (29/10/2025)
 */
export const FixedAssetCategoryEnum = Object.freeze({
  1: { code: "HOUSE", name: "Nhà, công trình xây dựng" },
  2: { code: "ARCH", name: "Vật kiến trúc" },
  3: { code: "CAR", name: "Xe ô tô" },
  4: { code: "TRANSPORT", name: "Phương tiện vận tải khác" },
  5: { code: "EQUIP", name: "Máy móc, thiết bị" },
  6: { code: "PLANT", name: "Cây lâu năm, súc vật" },
  7: { code: "OTHER", name: "TSCĐ hữu hình khác" },
});

/**
 * Hàm tiện ích: trả về mảng select-option (code + name)
 */
export const getFixedAssetCategoryOptions = () => {
  return Object.entries(FixedAssetCategoryEnum).map(([id, { code, name }]) => ({
    id: parseInt(id),
    code,
    name,
  }));
};
