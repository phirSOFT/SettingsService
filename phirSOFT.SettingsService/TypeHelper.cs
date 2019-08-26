// <copyright file="TypeHelper.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Provides some helper method for <see cref="Type"/>s.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        ///     Gets whether to one type is assignable to an other.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The result of this function is symmetrical.
        ///     </para>
        /// </remarks>
        /// <param name="typeA">
        ///     The <see cref="Type"/> to check, whether it is assignable to <paramref name="typeB"/> or
        ///     <paramref name="typeB"/> can assigned to it.
        /// </param>
        /// <param name="typeB">
        ///     The <see cref="Type"/> to check, whether it is assignable to <paramref name="typeA"/> or
        ///     <paramref name="typeA"/> can assigned to it.
        /// </param>
        /// <param name="type">The <see cref="Type"/>, that can be assigned from both types.</param>
        /// <returns>
        ///     <see langword="true"/>, if <paramref name="typeA"/> can be assigned to <paramref name="typeB"/> or the other
        ///     way around.
        /// </returns>
        [ContractAnnotation("=> true,type:notnull; => false,type:null")]
        internal static bool AreAssignable([NotNull] TypeInfo typeA, [NotNull] TypeInfo typeB, out Type type)
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

        /// <summary>
        ///     Gets the default value of a type, that is not known a compile time.
        /// </summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <remarks>
        ///     <para>
        ///         This code yield the equivalent of <c>default(type)</c>.
        ///     </para>
        /// </remarks>
        /// <returns>The default value of the type.</returns>
        [CanBeNull]
        internal static object GetDefaultValue([NotNull] Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        ///     Gets whether to types share a common base class other than <see cref="object"/>.
        /// </summary>
        /// <param name="typeA">The first type to check.</param>
        /// <param name="typeB">The second type to check.</param>
        /// <param name="type">The common base type of <paramref name="typeA"/> and <paramref name="typeB"/>.</param>
        /// <returns>
        ///     <see langword="true"/>, if <paramref name="typeA"/> and <paramref name="typeB"/> share a common base class
        ///     other than <see cref="object"/>.
        /// </returns>
        [ContractAnnotation("=> true,type:notnull; => false,type:null")]
        internal static bool HaveCommonBaseType([NotNull] TypeInfo typeA, [NotNull] TypeInfo typeB, out Type type)
        {
            Queue<TypeInfo> typeChainA = GetTypeChain(typeA);
            Queue<TypeInfo> typeChainB = GetTypeChain(typeB);
            type = null;

            while ((typeChainA.Count > 1) && (typeChainB.Count > 1))
            {
                if (AreAssignable(typeChainA.Peek(), typeChainB.Peek(), out type))
                    return type != typeof(object);

                if (typeChainA.Count > typeChainB.Count)
                    typeChainA.Dequeue();
                else
                    typeChainB.Dequeue();
            }

            // This line will never be executed but static code analysis cannot infer that
            return false;
        }

        [NotNull]
        [ItemNotNull]
        private static Queue<TypeInfo> GetTypeChain([NotNull] TypeInfo type)
        {
            var chain = new Queue<TypeInfo>();

            do
            {
                chain.Enqueue(type);
                type = type.BaseType?.GetTypeInfo();
            }
            while (type != null);

            return chain;
        }
    }
}
