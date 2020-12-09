﻿using System;
using System.Linq;
using Cosmos.Reflection;

namespace Cosmos.Finders
{
    /// <summary>
    /// Attribute type finder
    /// </summary>
    /// <typeparam name="TAttributeType"></typeparam>
    public class BaseAttributeFinder<TAttributeType> : BaseFinder<Type>, ITypeFinder
        where TAttributeType : Attribute
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public BaseAttributeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            return _allAssemblyFinder
                   .FindAll(true)
                   .SelectMany(assembly => assembly.GetTypes())
                   .Where(type => type.IsClass && !type.IsAbstract && type.IsAttributeDefined<TAttributeType>())
                   .Distinct()
                   .ToArray();
        }
    }
}