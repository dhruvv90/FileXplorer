using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FileXplorer
{
    public abstract class IPropertyNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool RaiseEvent { get; set; }

        public IPropertyNotifier() : base()
        {
            this.RaiseEvent = true;
        }

        public IPropertyNotifier(bool raiseEvent) : base()
        {
            this.RaiseEvent = raiseEvent;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (RaiseEvent)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /**
     * Base Class for every object. Enables to listen for property change events
     */
    public abstract class IBaseClass : IPropertyNotifier
    {
        public IBaseClass() : base()
        {
        }

        public IBaseClass(bool raiseEvent) : base(raiseEvent)
        {
        }

        private IDictionary<string, object> _values = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

        public T GetValue<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }
            var value = GetValue(key);
            if (value is T)
            {
                return (T)value;
            }
            return default(T);
        }

        private object GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            if (this._values.ContainsKey(key))
            {
                return this._values[key];
            }
            return null;
        }

        public void SetValue(string key, object value)
        {
            if (this._values.ContainsKey(key))
            {
                this._values[key] = value;
            }
            else
            {
                this._values.Add(key, value);
            }
            base.OnPropertyChanged(key);
        }
    }
}
