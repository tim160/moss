using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace EC.Framework.Dynamic
{
    public delegate object GetHandler(object persistable);
    public delegate void SetHandler(object persistable, object value);
    public delegate object InstantiateObjectHandler();

    public sealed class DynamicMethodCompiler
    {
        #region Constructor(s)
        private DynamicMethodCompiler()
        {
        }
        #endregion

        #region Method(s)
        public static InstantiateObjectHandler CreateInstantiateObjectHandler(Type type)
        {
            lock (AppInfo.EntityDictionary)
            {
                if (AppInfo.EntityDictionary.ContainsKey(type))
                    return AppInfo.EntityDictionary[type];
                else
                {
                    ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);

                    if (constructorInfo == null)
                        throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));

                    DynamicMethod dynamicMethod = new DynamicMethod("InstantiateObject", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(object), null, type, true);
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Newobj, constructorInfo);
                    generator.Emit(OpCodes.Ret);

                    InstantiateObjectHandler instantiateObjectHandler = (InstantiateObjectHandler)dynamicMethod.CreateDelegate(typeof(InstantiateObjectHandler));
                    AppInfo.EntityDictionary.Add(type, instantiateObjectHandler);
                    return instantiateObjectHandler;
                }
            }
        }

        public static GetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
        {
            lock (AppInfo.GetDictionary)
            {
                if (AppInfo.GetDictionary.ContainsKey(propertyInfo))
                    return AppInfo.GetDictionary[propertyInfo];
                else
                {
                    MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
                    DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
                    ILGenerator getGenerator = dynamicGet.GetILGenerator();

                    getGenerator.Emit(OpCodes.Ldarg_0);
                    getGenerator.Emit(OpCodes.Call, getMethodInfo);
                    Box(getMethodInfo.ReturnType, getGenerator);
                    getGenerator.Emit(OpCodes.Ret);

                    GetHandler getHandler = (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
                    AppInfo.GetDictionary.Add(propertyInfo, getHandler);
                    return getHandler;
                }
            }
        }

        public static SetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
        {
            lock (AppInfo.SetDictionary)
            {
                if (AppInfo.SetDictionary.ContainsKey(propertyInfo))
                    return AppInfo.SetDictionary[propertyInfo];
                else
                {
                    MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
                    DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
                    ILGenerator setGenerator = dynamicSet.GetILGenerator();

                    setGenerator.Emit(OpCodes.Ldarg_0);
                    setGenerator.Emit(OpCodes.Ldarg_1);
                    Unbox(setMethodInfo.GetParameters()[0].ParameterType, setGenerator);
                    setGenerator.Emit(OpCodes.Call, setMethodInfo);
                    setGenerator.Emit(OpCodes.Ret);

                    SetHandler setHandler = (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
                    AppInfo.SetDictionary.Add(propertyInfo, setHandler);
                    return setHandler;
                }
            }
        }
        #endregion

        #region DynamicMethods
        private static DynamicMethod CreateGetDynamicMethod(Type type)
        {
            return new DynamicMethod("DynamicGet", typeof(object), new Type[] { typeof(object) }, type, true);
        }

        private static DynamicMethod CreateSetDynamicMethod(Type type)
        {
            return new DynamicMethod("DynamicSet", typeof(void), new Type[] { typeof(object), typeof(object) }, type, true);
        }
        #endregion

        #region Boxing
        private static void Box(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
                generator.Emit(OpCodes.Box, type);
        }

        private static void Unbox(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
                generator.Emit(OpCodes.Unbox_Any, type);
        }
        #endregion
    }
}
