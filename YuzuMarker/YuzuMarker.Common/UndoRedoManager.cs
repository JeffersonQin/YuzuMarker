using System.Collections.Generic;

namespace YuzuMarker.Common
{
    public static class UndoRedoManager
    {
        private static int _head = -1;

        private static int _max = -1;
        
        public static bool IgnoreOtherRecording = false;
        
        public static bool ContinuousRecording = false;
        
        public static List<List<UndoRedoRecord>> UndoStack = new List<List<UndoRedoRecord>>();

        public static void StartRecording()
        {
            IgnoreOtherRecording = false;
        }

        public static void StopRecording()
        {
            IgnoreOtherRecording = true;
        }
        
        public static void PushRecord(List<UndoRedoRecord> records)
        {
            if (IgnoreOtherRecording) return;
            var tempHead = _head;
            while (tempHead < _max)
            {
                tempHead ++;
                foreach (var redoRecord in UndoStack[tempHead])
                    redoRecord.DisposeAction?.Invoke(redoRecord.Value);
            }
            _max = ++ _head;
            if (_head >= UndoStack.Count)
                UndoStack.Add(records);
            else UndoStack[_head] = records;
        }

        public static void PushAndPerformRecord(List<UndoRedoRecord> records)
        {
            PushRecord(records);
            foreach (var record in records)
                record.Value = record.RedoAction(null);
        }

        public static void PushRecord(UndoRedoRecord record)
        {
            if (IgnoreOtherRecording) return;
            if (ContinuousRecording)
                UndoStack[_head].Add(record);
            else PushRecord(new List<UndoRedoRecord> { record });
        }

        public static void PushAndPerformRecord(UndoRedoRecord record)
        {
            PushRecord(record);
            record.Value = record.RedoAction(null);
        }

        public static void PushRecord(
            UndoRedoRecord.DelegateActionWithAndReturnValue undoAction, 
            UndoRedoRecord.DelegateActionWithAndReturnValue redoAction, 
            UndoRedoRecord.DelegateActionWithValue disposeAction = null)
        {
            PushRecord(new UndoRedoRecord(undoAction, redoAction, disposeAction));
        }

        public static void PushAndPerformRecord(
            UndoRedoRecord.DelegateActionWithAndReturnValue undoAction, 
            UndoRedoRecord.DelegateActionWithAndReturnValue redoAction, 
            UndoRedoRecord.DelegateActionWithValue disposeAction = null)
        {
            PushAndPerformRecord(new UndoRedoRecord(undoAction, redoAction, disposeAction));
        }
        
        public static void StartContinuousRecording()
        {
            ContinuousRecording = true;
            PushRecord(new List<UndoRedoRecord>());
        }

        public static void StopContinuousRecording()
        {
            ContinuousRecording = false;
        }

        public static void Undo()
        {
            if (_head < 0) return;
            var lastIgnoreStatus = IgnoreOtherRecording;
            IgnoreOtherRecording = true;
            for (var i = UndoStack[_head].Count - 1; i >= 0; i --)
                UndoStack[_head][i].Value = UndoStack[_head][i].UndoAction?.Invoke(UndoStack[_head][i].Value);
            _head --;
            IgnoreOtherRecording = lastIgnoreStatus;
        }

        public static void Redo()
        {
            if (_head == _max) return;
            var lastIgnoreStatus = IgnoreOtherRecording;
            IgnoreOtherRecording = true;
            _head ++;
            foreach (var record in UndoStack[_head])
                record.Value = record.RedoAction?.Invoke(record.Value);
            IgnoreOtherRecording = lastIgnoreStatus;
        }

        public static void Clear()
        {
            if (IgnoreOtherRecording) return;
            for (var i = 0; i < _max; i ++)
                foreach (var record in UndoStack[i])
                    record.DisposeAction?.Invoke(record.Value);
            _head = _max = -1;
        }
    }
}