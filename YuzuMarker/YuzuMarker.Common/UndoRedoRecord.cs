namespace YuzuMarker.Common
{
    public class UndoRedoRecord
    {
        public object Value;

        public delegate void DelegateActionWithValue(object value);

        public delegate object DelegateActionReturnValue();

        public delegate object DelegateActionWithAndReturnValue(object value);

        public DelegateActionWithValue DisposeAction, SetValueAction, UndoAction, RedoAction;

        public DelegateActionReturnValue GetValueAction;

        public UndoRedoRecord(object value, 
            DelegateActionWithValue setValueAction,
            DelegateActionReturnValue getValueAction, 
            DelegateActionWithValue undoAction = null, 
            DelegateActionWithValue redoAction = null, 
            DelegateActionWithValue disposeAction = null)
        {
            Value = value;
            SetValueAction = setValueAction;
            GetValueAction = getValueAction;
            UndoAction = undoAction;
            RedoAction = redoAction;
            DisposeAction = disposeAction;
        }
    }
}