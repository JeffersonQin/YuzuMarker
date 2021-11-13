using System;

namespace YuzuMarker.Utils
{
    public static class BitOperation
    {
        /// <summary>
        /// 取指针所在高位数值。
        /// </summary>
        public static int HIWORD(IntPtr ptr)
        {
            unchecked
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    var val64 = ptr.ToInt64();
                    return (short) ((val64 >> 16) & 0xFFFF);
                }
                var val32 = ptr.ToInt32();
                return (short) ((val32 >> 16) & 0xFFFF);   
            }
        }

        /// <summary>
        /// 取指针所在低位数值。
        /// </summary>
        public static int LOWORD(IntPtr ptr)
        {
            unchecked
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    var val64 = ptr.ToInt64();
                    return (short)(val64 & 0xFFFF);
                }

                var val32 = ptr.ToInt32();
                return (short)(val32 & 0xFFFF);
            }
        }
    }
}