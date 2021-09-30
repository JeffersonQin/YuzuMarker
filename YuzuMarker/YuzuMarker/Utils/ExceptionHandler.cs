using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace YuzuMarker.Utils
{
    public static class ExceptionHandler
    {
        public static void ShowExceptionMessage(Exception e)
        {
            MessageBox.Show("发生错误：" + e.Message + "\n堆栈信息：\n" + e.StackTrace);
        }
    }
}
