using System.Linq;
using System.Reflection;

namespace Org.Kevoree.Library.Annotation
{
    public class KevoreeInjector<U>
    {
        public void inject<T>(object target, T value)
        {
            var fields = GetFields(target).Where(FilterByAnnotation).Where(FilterByFieldType<T>);
            foreach (var field in fields)
            {
                field.SetValue(target, value);
            }
        }

        public void injectByName<T>(object target, T value, string fieldName)
        {
            var field = GetFields(target).Where(FilterByAnnotation).Where(FilterByFieldType<T>).Where(x => x.Name == fieldName).First();
            field.SetValue(target, value);
        }

        public void callByName(object target, string methodName, string value)
        {
            var method = GetMethods(target).Where(x => x.GetCustomAttribute(typeof(U)) != null).Where(y => y.Name == methodName).First();
            method.Invoke(target, new object[1] {value});
        }

        public void call(object target, string value)
        {
            var method = GetMethods(target).Where(x => x.GetCustomAttribute(typeof(U)) != null).First();
            object[] paramz;
            if (method.GetParameters().Length == 1)
            {
                paramz = new object[1] {value};
            }
            else
            {
                paramz = new object[2] {value, null};
            }
            method.Invoke(target, paramz);
        }


        public FieldInfo[] GetFields(object target)
        {
            return target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public MethodInfo[] GetMethods(object target)
        {
            return target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }


        private static bool FilterByFieldType<T>(FieldInfo y)
        {
            return y.FieldType.IsAssignableFrom(typeof(T));
        }

        private static bool FilterByAnnotation(FieldInfo x)
        {
            return x.GetCustomAttribute(typeof(U)) != null;
        }

        private static bool FilterByName(FieldInfo x, string fieldName)
        {
            return x.Name == fieldName;
        }

        /**
         * Inject a value p to the targeted field.
         * Deal with the parsing operation by itself.
         */
        public bool smartInject<T>(object target, string fieldName, string dataType, string value)
        {
            var listPotentialFields = GetFields(target).Where(FilterByAnnotation).Where(e => FilterByName(e, fieldName));
            bool ret;
            if (listPotentialFields.Count() != 1)
            {
                ret =  false;
            }
            else
            {
                var targetField = listPotentialFields.First();
                var typeExpected = this.getTypeByDatatype(dataType);
                if (typeExpected != null)
                {
                    if(targetField.FieldType.IsAssignableFrom(typeExpected))
                    {
                        targetField.SetValue(target, convertObject(dataType, value));
                        ret = true;
                    } else
                    {
                        ret = false;
                    }
                }
                else
                {
                    ret = false;
                }

            }
            return ret;
        }

        private object convertObject(string dataType, string value)
        {
            switch (dataType)
            {
                case "INT":
                    return int.Parse(value);
                case "STRING":
                    return value;
                case "FLOAT":
                    return float.Parse(value);
                case "DOUBLE":
                    return double.Parse(value);
                case "BOOLEAN":
                    return bool.Parse(value);
                case "LONG":
                    return long.Parse(value);
                case "BYTE":
                    return byte.Parse(value);
                case "CHAR":
                    return char.Parse(value);
                case "SHORT":
                    return short.Parse(value);
                default:
                    return null;
            }
        }

        private System.Type getTypeByDatatype(string dataType)
        {
            switch (dataType)
            {
                case "INT":
                    return typeof(int);
                case "STRING":
                    return typeof (string);
                case "FLOAT":
                    return typeof (float);
                case "DOUBLE":
                    return typeof (double);
                case "BOOLEAN":
                    return typeof (bool);
                case "LONG":
                    return typeof (long);
                case "BYTE":
                    return typeof (byte);
                case "CHAR":
                    return typeof (char);
                case "SHORT":
                    return typeof (short);
                default:
                    return null;
            }
        }
    }
}
