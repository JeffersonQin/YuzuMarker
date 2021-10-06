using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace YuzuMarker.Common
{
    /// <summary>
    /// Add Proxy &lt;ut:BindingProxy x:Key="Proxy" Data="{Binding}" /&gt; to Resources <br/>
    /// Bind like &lt;Element Property="{Binding Data.MyValue, Source={StaticResource Proxy}}" /&gt; <br/>
    /// From: https://stackoverflow.com/questions/6575180/how-to-access-parents-datacontext-from-a-usercontrol
    /// </summary>
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy));
    }
}
