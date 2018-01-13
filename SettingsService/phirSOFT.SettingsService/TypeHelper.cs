using System;
using System.Collections.Generic;
using System.Reflection;

namespace phirSOFT.SettingsService
{
    internal static class TypeHelper
    {
        internal static bool AreAssignable(TypeInfo typeA, TypeInfo typeB, out Type type)
        {
            type = null;
            // initialType : default Type
            if (typeA.IsAssignableFrom(typeB))
                type = typeA.AsType();

            // defaultType : initialType
            else if (typeB.IsAssignableFrom(typeA))
                type = typeB.AsType();

            return type != null;
        }

        internal static bool HaveCommonBaseType(TypeInfo typeA, TypeInfo typeB, out Type type)
        {
            var typeChainA = GetTypeChain(typeA);
            var typeChainB = GetTypeChain(typeB);
            type = null;

            while (typeChainA.Count > 1 && typeChainB.Count > 1)
            {
                if (typeChainA.Count > typeChainB.Count)
                    typeChainA.Dequeue();
                else
                    typeChainB.Dequeue();

                if (AreAssignable(typeChainA.Peek(), typeChainB.Peek(), out type))
                    return type != typeof(object);
            }

            // This line will never be executed but static code analysis cannot infer that
            return false;
        }

        private static Queue<TypeInfo> GetTypeChain(TypeInfo type)
        {
            var chain = new Queue<TypeInfo>();
            if (type == null)
                return chain;

            do
            {
                chain.Enqueue(type);
                type = type.BaseType?.GetTypeInfo();
            } while (type != null);

            return chain;
        }

        internal static object GetDefaultValue(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}