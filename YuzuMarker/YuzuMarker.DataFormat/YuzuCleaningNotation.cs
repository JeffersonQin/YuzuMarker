using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenCvSharp;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    // TODO: refactor:
    // 弄成 YuzuCleaningPureColorNotation, YuzuCleaningImpaintingNotation 继承这个class
    // 一个增加 color, 一个增加 Mat, 还是用temp
    // 在class内部集成好 load, dispose, clone 之类的一系列操作，并且直接在这里就实现好 UndoRedo, 也可以用 RefreshImage 类似的办法来处理连续 UndoRedo
    // type可以留着，以后也可以考虑删掉，因为应该可以用type来判断是哪种类型的数据
    // 增加一个 ConvertTo(type)的方法，记得搞 UndoRedo, 然后 UndoRedoManager.IgnoreXXX = true
    public class YuzuCleaningNotation : NotifyObject
    {
        private YuzuCleaningNotationType _cleaningNotationType;

        [Undoable]
        public YuzuCleaningNotationType CleaningNotationType
        {
            get => _cleaningNotationType;
            set => SetProperty(value);
        }

        public UMat CleaningMask;

        public YuzuCleaningNotation(YuzuCleaningNotationType type)
        {
            CleaningNotationType = type;
        }
        
        public bool IsEmpty()
        {
            if (CleaningMask != null)
                if (!CleaningMask.IsDisposed)
                    if (CleaningMask.CvPtr != IntPtr.Zero)
                        return Cv2.CountNonZero(CleaningMask) == 0;
            return true;
        }
    }
}
