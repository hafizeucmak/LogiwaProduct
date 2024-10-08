using System.Reflection;

namespace Logiwa.Common.Utils
{
    public static class PropertySetter
    {
        public static void SetPropertyValue<T>(T objectToSet, string propertyName, object value)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(objectToSet, value);
            }
            else
            {
                throw new ArgumentException($"Property '{propertyName}' not found or not writable on type '{typeof(T).Name}'.");
            }
        }

        public static void SetPrivateProperty<T>(T objectToSet, string propertyName, object value)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(objectToSet, value);
            }
            else
            {
                throw new ArgumentException($"Private property '{propertyName}' not found or not writable on type '{typeof(T).Name}'.");
            }
        }
    }
}
