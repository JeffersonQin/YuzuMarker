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

        public static void PushRecord(UndoRedoRecord record)
        {
            if (ContinuousRecording)
                UndoStack[_head].Add(record);
            else PushRecord(new List<UndoRedoRecord> { record });
        }

        public static void PushRecord(object value,
            UndoRedoRecord.DelegateActionWithValue setValueAction,
            UndoRedoRecord.DelegateActionReturnValue getValueAction, 
            UndoRedoRecord.DelegateActionWithValue undoAction = null, 
            UndoRedoRecord.DelegateActionWithValue redoAction = null, 
            UndoRedoRecord.DelegateActionWithValue disposeAction = null)
        {
            PushRecord(new UndoRedoRecord(value, setValueAction, getValueAction, undoAction, redoAction, disposeAction));
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
            IgnoreOtherRecording = true;
            for (var i = UndoStack[_head].Count - 1; i >= 0; i --)
            {
                var record = UndoStack[_head][i];
                var value = record.GetValueAction?.Invoke();
                record.SetValueAction(record.Value);
                record.UndoAction?.Invoke(record.Value);
                record.Value = value;
            }
            _head --;
            IgnoreOtherRecording = false;
        }

        public static void Redo()
        {
            if (_head == _max) return;
            IgnoreOtherRecording = true;
            _head ++;
            foreach (var record in UndoStack[_head])
            {
                var value = record.GetValueAction?.Invoke();
                record.SetValueAction(record.Value);
                record.RedoAction?.Invoke(record.Value);
                record.Value = value;
            }
            IgnoreOtherRecording = false;
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