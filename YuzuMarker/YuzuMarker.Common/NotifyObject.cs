using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace YuzuMarker.Common
{
    public class NotifyObject : INotifyPropertyChanged
    {
        public bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action beforeChanged = null,
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            beforeChanged?.Invoke();
            backingStore = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }

        public bool SetProperty<T>(T value, [CallerMemberName] string propertyName = "",
            Action beforeChanged = null, Action onChanged = null)
        {
            var backingName = "_" + propertyName[0].ToString().ToLower() + propertyName.Substring(1);
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
                UndoRedoManager.PushAndPerformRecord(attribute.UndoAction ?? (o =>
                {
                    var nowValue = backingInstance.GetValue(this);
                    backingInstance.SetValue(this, o);
                    RaisePropertyChanged(propertyName);
                    return nowValue;
                }), attribute.RedoAction ?? (o =>
                {
                    var nowValue = backingInstance.GetValue(this);
                    o ??= value;
                    backingInstance.SetValue(this, o);
                    RaisePropertyChanged(propertyName);
                    return nowValue;
                }), attribute.DisposeAction);
            }
            
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
