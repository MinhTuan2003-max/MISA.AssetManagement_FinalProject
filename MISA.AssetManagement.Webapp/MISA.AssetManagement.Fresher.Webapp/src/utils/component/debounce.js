/**
 * - Giới hạn tần suất gọi của 1 hàm (fn) trong khoảng thời gian delay nhất định.
 * - Nếu người dùng tiếp tục gọi hàm trong khi delay chưa hết,
 *   hàm sẽ reset timer và chỉ thực thi sau khi người dùng dừng lại đủ lâu.
 * @param {Function} fn - Hàm cần debounce
 * @param {number} delay - Khoảng thời gian chờ (ms) trước khi fn được thực thi
 *
 * @returns {Function} Hàm mới có behavior debounce, trả về Promise kết quả của fn
 * CreatedBy: HMTuan (30/10/2025)
 */
export function debounce(fn, delay) {
  let timeoutId

  return function (...args) {
    return new Promise((resolve) => {
      // Xóa timeout cũ để reset nếu người dùng tiếp tục thao tác
      clearTimeout(timeoutId)

      // Đặt timeout mới - chỉ gọi fn sau khi hết delay
      timeoutId = setTimeout(() => {
        // Gọi hàm gốc với context và tham số ban đầu
        resolve(fn.apply(this, args))
      }, delay)
    })
  }
}
