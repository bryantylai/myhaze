using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace HazeMY.ViewModels
{
    public abstract class BindableObjectBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("PropertyExpression is null.");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("The body must be a member expression");

            RaisePropertyChanged(memberExpression.Member.Name);
        }
    }
}
