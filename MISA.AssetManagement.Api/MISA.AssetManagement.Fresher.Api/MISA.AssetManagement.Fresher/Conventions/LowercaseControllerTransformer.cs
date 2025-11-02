using System.Text.RegularExpressions;

namespace MISA.AssetManagement.Fresher.Conventions
{
    /// <summary>
    /// Tự động chuyển tên controller sang chữ thường hoặc dash-case
    /// Created by: HMTuan (28/10/2025)
    /// </summary>
    public class LowercaseControllerTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        /// Chuyển đổi tên controller sang định dạng chữ thường hoặc dash-case (vd: "FixedAsset" → "fixed-asset").
        /// <param name="value">Giá trị cần chuyển đổi (thường là tên class controller, ví dụ: "FixedAsset").</param>
        ///<returns>Tên class controller sau khi chuyển đổi.</returns>
        /// Created by: HMTuan (28/10/2025)
        /// </summary>
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        }
    }
}
