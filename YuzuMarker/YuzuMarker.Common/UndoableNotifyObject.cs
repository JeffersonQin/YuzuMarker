using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YuzuMarker.Common
{
    public partial class NotifyObject
    {
        public bool SetProperty<T>(T value, [CallerMemberName] string propertyName = "", 
            BackingNamingStyle backingNamingStyle = BackingNamingStyle.UnderscoreAndLowerCase,
            Action beforeChanged = null, Action onChanged = null,
            UndoRedoRecord.DelegateActionWithAndReturnValue undoAction = null,
            UndoRedoRecord.DelegateActionWithAndReturnValue redoAction = null,
            UndoRedoRecord.DelegateActionWithValue disposeAction = null)
        {
            var backingName = "";
            switch (backingNamingStyle)
            {
                case BackingNamingStyle.LowerCase:
                    backingName = propertyName[0].ToString().ToLower() + propertyName.Substring(1);
                    break;
                case BackingNamingStyle.UnderscoreAndLowerCase:
                    backingName = "_" + propertyName[0].ToString().ToLower() + propertyName.Substring(1);
                    break;
            }
            
            Type type = null;
            FieldInfo backingInstance = null;
            do
            {
                type = type == null ? GetType() : type.BaseType;
                if (type == null) return false;
                backingInstance = type.GetField(backingName, BindingFlags.NonPublic | BindingFlags.Instance);
            } while (backingInstance == null);
            
            if (EqualityComparer<T>.Default.Equals((T)backingInstance.GetValue(this), value))
                return false;
            
            var attribute = GetType().GetProperty(propertyName)?.GetCustomAttribute<Undoable>();
            if (attribute == null)
            {
                beforeChanged?.Invoke();
                backingInstance.SetValue(this, value);
                onChanged?.Invoke();
                RaisePropertyChanged(propertyName);
            }
            else
            {
                UndoRedoManager.PushAndPerformRecord(undoAction ?? (o =>
                {
                    var nowValue = backingInstance.GetValue(this);
                    backingInstance.SetValue(this, o);
                    RaisePropertyChanged(propertyName);
                    return nowValue;
                }), redoAction ?? (o =>
                {
                    var nowValue = backingInstance.GetValue(this);
                    o ??= value;
                    backingInstance.SetValue(this, o);
                    RaisePropertyChanged(propertyName);
                    return nowValue;
                }), disposeAction);
            }
            
            return true;
        }
    }
}