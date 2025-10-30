using System.Text.RegularExpressions;

namespace MISA.AssetManagement.Fresher.Conventions
{
    /// <summary>
    /// Tự động chuyển tên controller sang chữ thường hoặc dash-case
    /// Created by: HMTuan (28/10/2025)
    /// </summary>
    public class LowercaseControllerTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        }
    }
}
