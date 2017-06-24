using System.Reflection;

namespace SmartEngine.Core
{
    public interface IWrappedPropertyDescriptor
    {
        object GetWrappedOwner();

        PropertyInfo GetWrappedProperty();
    }
}
