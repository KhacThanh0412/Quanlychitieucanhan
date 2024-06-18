using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public static class ReflectionExtensions
    {
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var property = obj?.GetType().GetRuntimeProperty(propertyName);
            return property?.GetValue(obj);
        }

        public static object GetRuntimeFieldValue(this object obj, string fieldName)
        {
            var field = obj?.GetType().GetRuntimeField(fieldName);
            return field?.GetValue(obj);
        }

        public static object InvokeMethod(this object self, string methodName, object[] parameters = null)
        {
            var type = self?.GetType();
            var method = type?.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                Console.WriteLine($"Method '{methodName}' not found in type '{type}'.");
            }

            return method?.Invoke(self, parameters);
        }

        public static object GetFieldValue(this object obj, string fieldName)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                Console.WriteLine($"Field '{fieldName}' not found in type '{type}'.");
            }

            return field.GetValue(obj);
        }
    }
}
