namespace YuzuMarker.Common
{
    public class UndoRedoRecord
    {
        public object Value;

        public delegate void DelegateActionWithValue(object value);
        
        public delegate object DelegateActionWithAndReturnValue(object value);

        public DelegateActionWithValue DisposeAction;

        public DelegateActionWithAndReturnValue UndoAction, RedoAction;

        public UndoRedoRecord(
            DelegateActionWithAndReturnValue undoAction, 
            DelegateActionWithAndReturnValue redoAction, 
            DelegateActionWithValue disposeAction = null)
        {
            UndoAction = undoAction;
            RedoAction = redoAction;
            DisposeAction = disposeAction;
        }
    }
}