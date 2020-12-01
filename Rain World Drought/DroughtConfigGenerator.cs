using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Globalization;
using Partiality.Modloader;
using OptionalUI;
using UnityEngine;

namespace Rain_World_Drought
{
    public partial class DroughtMod
    {
        public OptionInterface LoadOI() => DroughtConfigGenerator.LoadOI(this);
    }

    /// <summary>
    /// Generates config
    /// </summary>
    internal static class DroughtConfigGenerator
    {
        private static Type oiType;

        // Create a new instance of the generated OptionInterface type
        public static OptionInterface LoadOI(DroughtMod mod)
        {
            try
            {
                if (oiType == null) oiType = CreateOIType();
                return (OptionInterface)Activator.CreateInstance(oiType, mod);
            } catch(Exception e)
            {
                Debug.LogException(new Exception("Failed to generate option interface!", e));
                throw e;
            }
        }

        // Generate a class that inherits from OptionInterface
        public static Type CreateOIType()
        {
            Type oi = typeof(OptionInterface);
            Type dc = typeof(DroughtConfig);

            // Define an assembly and module to contain the type
            AssemblyName asmName = new AssemblyName($"{nameof(DroughtConfigGenerator)}Dynamic");
            AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder modb = asm.DefineDynamicModule(asmName.Name);

            // Define the type, inheriting from OptionInterface
            TypeBuilder tb = modb.DefineType($"{nameof(DroughtConfig)}Proxy", TypeAttributes.Public | TypeAttributes.Class, oi);

            // CONSTRUCTORS //
            // ctor (PartialityMod) => Ctor (OptionInterface, PartialityMod)
            {
                ConstructorBuilder cb = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(PartialityMod) });
                ILGenerator ilg = cb.GetILGenerator();
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Call, oi.GetConstructor(new Type[] { typeof(PartialityMod) }));
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Call, dc.GetMethod("Ctor"));
                ilg.Emit(OpCodes.Ret);
            }

            // METHODS //

            GenerateOverrideProxy(tb, dc.GetMethod("Initialize"));
            GenerateOverrideProxy(tb, dc.GetMethod("Update"));
            GenerateOverrideProxy(tb, dc.GetMethod("ConfigOnChange"));

            return tb.CreateType();
        }

        private enum BaseCallOrder { Before, After, Never }
        private static void GenerateOverrideProxy(TypeBuilder type, MethodInfo dst, BaseCallOrder baseCallOrder = BaseCallOrder.Before)
        {
            // Get parameter types
            ParameterInfo[] dstParams = dst.GetParameters();
            if (dstParams.Length < 1) throw new ArgumentException("The destination method has no arguments.");
            Type parentClass = type.BaseType;
            if (!dstParams[0].ParameterType.IsAssignableFrom(parentClass)) throw new ArgumentException("The destination method's first parameter must be the base class.");
            
            Type[] srcTypes = new Type[dstParams.Length - 1];
            for (int i = 0; i < srcTypes.Length; i++)
                srcTypes[i] = dstParams[i + 1].ParameterType;

            // Get parent method
            MethodInfo parentMethod = parentClass.GetMethod(dst.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, srcTypes, null);
            if (parentMethod == null) throw new ArgumentException("No suitable override was found for the destination method.");

            // Generate override that calls a proxy method
            MethodBuilder mb = type.DefineMethod(parentMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, parentMethod.ReturnType, srcTypes);
            ILGenerator ilg = mb.GetILGenerator();

            MethodInfo[] callOrder;
            switch (baseCallOrder)
            {
                case BaseCallOrder.Before: callOrder = new MethodInfo[] { parentMethod, dst }; break;
                case BaseCallOrder.After: callOrder = new MethodInfo[] { dst, parentMethod }; break;
                default: callOrder = new MethodInfo[] { dst }; break;
            }

            for(int i = 0; i < callOrder.Length; i++)
            {
                MethodInfo m = callOrder[i];
                for (short arg = 0; arg < dstParams.Length; arg++)
                    ilg.Emit(OpCodes.Ldarg, arg);
                ilg.Emit(OpCodes.Call, m);
            }
            ilg.Emit(OpCodes.Ret);

            type.DefineMethodOverride(mb, parentMethod);
        }

        //public static CultureInfo GetCultureInfo(OptionInterface self) => return self.GetCultureInfo();
    }
}
