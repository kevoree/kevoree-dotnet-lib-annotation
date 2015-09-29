using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Org.Kevoree.Library.Annotation
{
    public class AnnotationHelper
    {

        private List<Tuple<Type, object>> filterByTypes(object[] types, Type[] pars)
        {
            var gootypes = new List<Tuple<Type, object>>();
            foreach (object type in types)
            {
                foreach (var par in pars)
                {
                    if (type.GetType().Equals(par))
                    {
                        gootypes.Add(Tuple.Create(par, type));
                    }
                }
            }
            return gootypes;
        }

        private List<Tuple<Type, object>> GetTypeDefinitionSub(Type t, Type[] types)
        {
            return filterByTypes(t.GetCustomAttributes(true), types);
        }

        public Type GetTypeDefinition(Type filteredAssemblyTypes, Type[] expectedTypes)
        {
            return GetTypeDefinitionSub(filteredAssemblyTypes, expectedTypes)[0].Item1;
        }

        public bool FilterByAttribute(Type t, Type[] types)
        {
            return GetTypeDefinitionSub(t, types).Count > 0;
        }

        public IEnumerable<FieldInfo> filterFieldsByAttribute(Type target, Type typeOutput)
        {
            return target.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.GetCustomAttribute(typeOutput) != null);
        }

        public IEnumerable<MethodInfo> filterMethodsByAttribute(Type target, Type type)
        {
            return target.GetMethods().Where(x => x.GetCustomAttribute(type) != null);
        }
    }
}
