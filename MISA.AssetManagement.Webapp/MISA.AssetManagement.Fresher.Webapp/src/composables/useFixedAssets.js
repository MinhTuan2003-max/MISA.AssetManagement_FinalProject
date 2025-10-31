import { ref, computed, watch } from 'vue';
import fixedAssetApi from '@/domains/api/fixedAssetApi';
import fixedAssetCategoryApi from '@/domains/api/fixedAssetCategoryApi';
import departmentApi from '@/domains/api/departmentApi';

export function useFixedAssets() {
  const fixedAssets = ref([]);
  const loading = ref(false);
  const error = ref(null);

  const selectedIds = ref([]);

  const currentPage = ref(1);
  const pageSize = ref(20);
  const totalRecords = ref(0);
  const totalPages = ref(1);

  const searchQuery = ref('');
  const filterCategory = ref(null);
  const filterDepartment = ref(null);

  const categoryOptions = ref([]);
  const departmentOptions = ref([]);

  /**
   * Hàm tải danh sách lựa chọn bộ lọc (Loại tài sản, Phòng ban)
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function loadFilterOptions() {
    try {
      const [categories, departments] = await Promise.all([
        fixedAssetCategoryApi.getFixedAssetCategories(),
        departmentApi.getDepartments()
      ]);

      categoryOptions.value = categories.map(cat => cat.toOption());
      departmentOptions.value = departments.map(dept => dept.toOption());

    } catch (err) {
      console.error('Load filter options error:', err);
    }
  }

  /**
   * Hàm tải danh sách tên tắt - mã cho select (Loại tài sản, Phòng ban)
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function loadShortNameSelectOptions() {
    try {
      const [categories, departments] = await Promise.all([
        fixedAssetCategoryApi.getFixedAssetCategories(),
        departmentApi.getDepartments()
      ]);

      categoryOptions.value = categories.map(cat => cat.toSelectOption());
      departmentOptions.value = departments.map(dept => dept.toSelectOption());

    } catch (err) {
      console.error('Load filter options error:', err);
    }
  }

  /**
   * Hàm tải danh sách tài sản theo bộ lọc và phân trang
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function loadFixedAssets() {
    loading.value = true;
    error.value = null;

    try {
      const filterDto = {
        keyword: searchQuery.value || null,
        department_code: filterDepartment.value || null,
        fixed_asset_category_code: filterCategory.value || null,
        page_number: currentPage.value,
        page_size: pageSize.value
      };

      const response = await fixedAssetApi.getFixedAssets(filterDto);

      fixedAssets.value = response.assets;
      totalRecords.value = response.totalRecords;
      totalPages.value = response.totalPages;
      currentPage.value = response.currentPage;

    } catch (err) {
      error.value = err.message || 'Lỗi khi tải danh sách tài sản';
    } finally {
      loading.value = false;
    }
  }

  /**
   * Computed: danh sách tài sản đã lọc (có thể mở rộng filter nội bộ)
   * CreatedBy: HMTuan (29/10/2025)
   */
  const filteredAssets = computed(() => fixedAssets.value);

  /**
   * Computed: danh sách tài sản phân trang (hiện tại lấy toàn bộ vì API trả theo trang)
   * CreatedBy: HMTuan (29/10/2025)
   */
  const paginatedAssets = computed(() => fixedAssets.value);

  /**
   * Computed: dữ liệu hiển thị bảng
   * CreatedBy: HMTuan (29/10/2025)
   */
  const tableData = computed(() => {
    return paginatedAssets.value.map(asset => asset.toTableFormat());
  });

  /**
   * Computed: tổng số trang
   * CreatedBy: HMTuan (29/10/2025)
   */
  const computedTotalPages = computed(() => totalPages.value);

  /**
   * Computed: kiểm tra xem tất cả tài sản trên trang hiện tại có được chọn không
   * CreatedBy: HMTuan (29/10/2025)
   */
  const isAllSelected = computed(() =>
    paginatedAssets.value.length > 0 &&
    paginatedAssets.value.every(asset =>
      selectedIds.value.includes(asset?.fixed_asset_id)
    )
  );

  /**
   * Hàm chọn tất cả tài sản trên trang hiện tại
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handleSelectAll() {
    if (isAllSelected.value) {
      const currentPageIds = paginatedAssets.value
        .map(asset => asset?.fixed_asset_id)
        .filter(id => id !== null);

      selectedIds.value = selectedIds.value.filter(
        id => !currentPageIds.includes(id)
      );
    } else {
      const newIds = paginatedAssets.value
        .map(asset => asset?.fixed_asset_id)
        .filter(id => id !== null && !selectedIds.value.includes(id));

      selectedIds.value = [...selectedIds.value, ...newIds];
    }
  }

  /**
   * Hàm chọn hoặc bỏ chọn một tài sản
   * @param {string|number} id - ID tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handleSelectItem(id) {
    const index = selectedIds.value.indexOf(id);
    if (index > -1) {
      selectedIds.value.splice(index, 1);
    } else {
      selectedIds.value.push(id);
    }
  }

  /**
   * Hàm chuyển trang
   * @param {number} page - số trang
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handlePageChange(page) {
    currentPage.value = page;
    loadFixedAssets();
  }

  /**
   * Hàm thay đổi số lượng bản ghi trên mỗi trang
   * @param {number} size - số bản ghi
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handlePageSizeChange(size) {
    pageSize.value = size;
    currentPage.value = 1;
    loadFixedAssets();
  }

  /**
   * Hàm thực hiện tìm kiếm tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handleSearch() {
    currentPage.value = 1;
    loadFixedAssets();
  }

  /**
   * Hàm reset tất cả bộ lọc và tìm kiếm
   * CreatedBy: HMTuan (29/10/2025)
   */
  function resetFilters() {
    filterCategory.value = null;
    filterDepartment.value = null;
    searchQuery.value = '';
    currentPage.value = 1;
    loadFixedAssets();
  }

  /**
   * Computed: tổng số lượng tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  const totalQuantity = computed(() =>
    filteredAssets.value.reduce((sum, asset) => sum + (asset.quantity || 0), 0)
  );

  /**
   * Computed: tổng giá trị tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  const totalCost = computed(() =>
    filteredAssets.value.reduce((sum, asset) => sum + (asset.cost || 0), 0)
  );

  /**
   * Computed: tổng khấu hao lũy kế
   * CreatedBy: HMTuan (29/10/2025)
   */
  const totalDepreciation = computed(() =>
    filteredAssets.value.reduce((sum, asset) => sum + (asset.accumulated_depreciation || 0), 0)
  );

  /**
   * Computed: tổng giá trị còn lại
   * CreatedBy: HMTuan (29/10/2025)
   */
  const totalResidual = computed(() =>
    filteredAssets.value.reduce((sum, asset) => sum + (asset.remaining_value || 0), 0)
  );

  /**
   * Watcher: theo dõi thay đổi bộ lọc (Loại tài sản, Phòng ban)
   * Khi thay đổi sẽ reset trang hiện tại về 1 và load lại danh sách tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  watch([filterCategory, filterDepartment], () => {
    console.log('Filter changed:', {
      category: filterCategory.value,
      department: filterDepartment.value
    });
    currentPage.value = 1;
    loadFixedAssets();
  });

  /**
   * Lấy chi tiết một tài sản theo ID
   * @param {string|number} id - ID tài sản
   * @returns {Object} chi tiết tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function getFixedAssetDetail(id) {
    loading.value = true;
    error.value = null;
    try {
      return await fixedAssetApi.getFixedAssetById(id);
    } catch (err) {
      error.value = err.message || 'Lỗi khi tải chi tiết tài sản';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Thêm mới tài sản
   * @param {Object} assetData - dữ liệu tài sản
   * @returns {Object} tài sản mới tạo
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function createFixedAsset(assetData) {
    loading.value = true;
    error.value = null;
    try {
      const newAsset = await fixedAssetApi.createFixedAsset(assetData);
      await loadFixedAssets();
      return newAsset;
    } catch (err) {
      error.value = err.message || 'Lỗi khi thêm tài sản';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Cập nhật tài sản
   * @param {string|number} id - ID tài sản
   * @param {Object} assetData - dữ liệu cập nhật
   * @returns {Object} tài sản đã cập nhật
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function updateFixedAsset(id, assetData) {
    loading.value = true;
    error.value = null;
    try {
      const updatedAsset = await fixedAssetApi.updateFixedAsset(id, assetData);
      await loadFixedAssets();
      return updatedAsset;
    } catch (err) {
      error.value = err.message || 'Lỗi khi cập nhật tài sản';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Xóa một tài sản
   * @param {string|number} id - ID tài sản
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function deleteFixedAsset(id) {
    loading.value = true;
    error.value = null;
    try {
      await fixedAssetApi.deleteFixedAsset(id);
      await loadFixedAssets();
    } catch (err) {
      error.value = err.message || 'Lỗi khi xóa tài sản';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Xóa nhiều tài sản đã chọn
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function deleteSelectedAssets() {
    if (selectedIds.value.length === 0) return;
    loading.value = true;
    error.value = null;
    try {
      await fixedAssetApi.deleteFixedAssets(selectedIds.value);
      selectedIds.value = [];
      await loadFixedAssets();
    } catch (err) {
      error.value = err.message || 'Lỗi khi xóa tài sản';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Nhân bản tài sản
   * @param {string|number} id - ID tài sản cần nhân bản
   * @returns {Object} tài sản mới nhân bản
   * CreatedBy: HMTuan (29/10/2025)
   */
  async function duplicateFixedAsset(id) {
    loading.value = true;
    error.value = null;

    try {
      const duplicatedAsset = await fixedAssetApi.duplicateFixedAsset(id);
      await loadFixedAssets();
      return duplicatedAsset;
    } catch (err) {
      error.value = err.message || 'Lỗi khi nhân bản tài sản';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  return {
    fixedAssets,
    loading,
    error,
    selectedIds,
    currentPage,
    pageSize,
    searchQuery,
    filterCategory,
    filterDepartment,
    totalRecords,

    categoryOptions,
    departmentOptions,

    filteredAssets,
    paginatedAssets,
    tableData,
    totalPages: computedTotalPages,
    isAllSelected,
    totalQuantity,
    totalCost,
    totalDepreciation,
    totalResidual,

    loadFixedAssets,
    loadFilterOptions,
    loadShortNameSelectOptions,
    getFixedAssetDetail,
    createFixedAsset,
    updateFixedAsset,
    deleteFixedAsset,
    duplicateFixedAsset,
    deleteSelectedAssets,
    handleSelectAll,
    handleSelectItem,
    handlePageChange,
    handlePageSizeChange,
    handleSearch,
    resetFilters
  };
}
