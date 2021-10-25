using System;
using System.Collections.Generic;
using System.Text;
using YuzuMarker.BasicDataFormat;

namespace YuzuMarker.DataFormat
{
    public class YuzuNotationGroup : BasicYuzuNotationGroup
    {
        public YuzuCleaningNotation CleaningNotation { get; set; }

        public YuzuNotationGroup(int x, int y, string text, bool finished) : base(x, y, text, finished)
        {
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
        }

        public YuzuNotationGroup(long timestamp, int x, int y, string text, bool finished) : base(timestamp, x, y, text, finished)
        {
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
        }
        
        // TODO: refactor start: 新增 load cleaning notation (1. 尝试从 temp 2. 尝试复制到 temp 3. 记得 Ensure)
        
        // TODO: refactor end

        // Other kinds of notations
    }
}
