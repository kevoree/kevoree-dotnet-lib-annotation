using System.Linq;
using System.Reflection;

namespace Org.Kevoree.Library.Annotation
{
    public class KevoreeInjector<U>
    {
        public void inject<T>(object target, T value)
        {
            var fields = target.GetType().GetFields().Where(FilterByAnnotation).Where(FilterByFieldType<T>);
            foreach (var field in fields)
            {
                field.SetValue(target, value);
            }
        }

        private static bool FilterByFieldType<T>(FieldInfo y)
        {
            return y.FieldType.IsAssignableFrom(typeof(T));
        }

        private static bool FilterByAnnotation(FieldInfo x)
        {
            return x.GetCustomAttribute(typeof(U)) != null;
        }
    }
}
