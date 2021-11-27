using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YuzuMarker.Common
{
    public class SelfNotifiableObject : NotifyObject
    {
        public object DummyObject { get; set; }
        
        public override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);
            var attribute = GetType().GetProperty(propertyName)?.GetCustomAttribute<SelfNotifiable>();
            if (attribute != null)            
                base.RaisePropertyChanged(nameof(DummyObject));
        }
    }
}