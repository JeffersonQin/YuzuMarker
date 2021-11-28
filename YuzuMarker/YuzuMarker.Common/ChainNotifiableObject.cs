using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YuzuMarker.Common
{
    public class ChainNotifiableObject : NotifyObject
    {
        public object DummyObject { get; set; }
        
        public override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);
            
            var attribute = GetType().GetProperty(propertyName)?.GetCustomAttribute<ChainNotifiable>();
            if (attribute == null) return;
            
            base.RaisePropertyChanged(nameof(DummyObject));

            var type = GetType();
            var obj = this as object;
            while (true)
            {
                var ancestorMarker = type.GetCustomAttribute<AncestorNotifiableMarker>();
                if (ancestorMarker == null) return;
                var field = type.GetProperty(ancestorMarker.AncestorName);
                obj = field.GetValue(obj);
                type = field.GetType();
                ((NotifyObject)obj).RaisePropertyChanged("DummyObject");
            }
        }
    }
}