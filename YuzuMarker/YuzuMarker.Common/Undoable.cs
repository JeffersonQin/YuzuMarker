using System;

namespace YuzuMarker.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Undoable : Attribute
    {
        public UndoRedoRecord.DelegateActionWithValue DisposeAction;

        public UndoRedoRecord.DelegateActionWithAndReturnValue UndoAction, RedoAction;

        public Undoable()
        {
            
        }
        
        public Undoable(UndoRedoRecord.DelegateActionWithAndReturnValue undoAction = null,
            UndoRedoRecord.DelegateActionWithAndReturnValue redoAction = null,
            UndoRedoRecord.DelegateActionWithValue disposeAction = null)

        {
            UndoAction = undoAction;
            RedoAction = redoAction;
            DisposeAction = disposeAction;
        }
    }
}