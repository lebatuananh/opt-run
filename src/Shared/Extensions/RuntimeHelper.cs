using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Shared.Constants;

namespace Shared.Extensions
{
    public static class RuntimeHelper
    {
        /// <summary>
        /// Get project assemblies, exclude all system assemblies (Microsoft., System., etc.), Nuget download packages
        /// </summary>
        /// <returns></returns>
        public static IList<Assembly> GetAllAssemblies()
        {
            var list = new List<Assembly>();
            var deps = DependencyContext.Default;
            //Exclude all system assemblies, Nuget download packages
            var libs = deps.CompileLibraries.Where(lib => lib.Type == AssembleTypeConsts.Project); //只获取本项目用到的包
            foreach (var lib in libs)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    list.Add(assembly);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return list;
        }

        public static IList<Assembly> GetAllCoreAssemblies()
        {
            var list = new List<Assembly>();
            var deps = DependencyContext.Default;
            //Exclude all system assemblies, Nuget download packages
            var libs = deps.CompileLibraries.Where(lib =>
                lib.Type == AssembleTypeConsts.Project ||
                lib.Name.StartsWith("Shared")); //Get only the packages used in this project
            foreach (var lib in libs)
            {
                try
                {
                    if (lib.Name == "Shared") continue;
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    list.Add(assembly);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            return GetAllCoreAssemblies().FirstOrDefault(assembly =>
                assembly.FullName != null && assembly.FullName.Contains(assemblyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IList<Type> GetAllTypes()
        {
            var list = new List<Type>();
            foreach (var assembly in GetAllAssemblies())
            {
                var typeInfos = assembly.DefinedTypes;
                foreach (var typeInfo in typeInfos)
                {
                    list.Add(typeInfo.AsType());
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static IList<Type> GetTypesByAssembly(string assemblyName)
        {
            var list = new List<Type>();
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            var typeInfos = assembly.DefinedTypes;
            foreach (var typeInfo in typeInfos)
            {
                list.Add(typeInfo.AsType());
            }

            return list;
        }

        /// <summary>
        /// Get the implementation class
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="baseInterfaceType"></param>
        /// <returns></returns>
        public static Type GetImplementType(string typeName, Type baseInterfaceType)
        {
            return GetAllTypes().FirstOrDefault(t =>
            {
                if (t.Name == typeName &&
                    t.GetTypeInfo().GetInterfaces().Any(b => b.Name == baseInterfaceType.Name))
                {
                    var typeInfo = t.GetTypeInfo();
                    return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType;
                }

                return false;
            });
        }
    }
}