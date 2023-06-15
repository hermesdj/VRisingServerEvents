#nullable enable
using System;
using System.IO;
using System.Reflection;
using Iced.Intel;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime.Runtime;

namespace VRisingServerEvents.Utils;

public static class Il2CppMethodResolver
{
    public static unsafe IntPtr ResolveFromMethodInfo(MethodInfo? method)
    {
        var methodInfoField = Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);
        if (methodInfoField == null)
            throw new Exception($"Couldn't obtain method info for {method}");

        var il2CPPMethod =
            UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)(methodInfoField.GetValue(null) ?? IntPtr.Zero));
        if (il2CPPMethod == null)
            throw new Exception($"Method info for {method} is invalid");

        return ResolveMethodPointer(il2CPPMethod.MethodPointer);
    }

    private static unsafe IntPtr ResolveMethodPointer(IntPtr methodPointer)
    {
        var stream = new UnmanagedMemoryStream((byte*)methodPointer, 256, 256, FileAccess.Read);
        var codeReader = new StreamCodeReader(stream);

        var decoder = Decoder.Create(IntPtr.Size == 8 ? 64 : 32, codeReader);
        decoder.IP = (ulong)methodPointer.ToInt64();

        Instruction instr = default;
        while (instr.Mnemonic != Mnemonic.Int3)
        {
            decoder.Decode(out instr);

            if (instr.Mnemonic != Mnemonic.Jmp && instr.Mnemonic != Mnemonic.Add)
            {
                return methodPointer;
            }

            if (instr.Mnemonic == Mnemonic.Add)
            {
                if (instr.Immediate32 != 0x10)
                {
                    return methodPointer;
                }
            }

            if (instr.Mnemonic == Mnemonic.Jmp)
                return new IntPtr((long)ExtractTargetAddress(instr));
        }

        return methodPointer;
    }

    private static ulong ExtractTargetAddress(in Instruction instruction)
    {
        return instruction.Op0Kind switch
        {
            OpKind.FarBranch16 => instruction.FarBranch16,
            OpKind.FarBranch32 => instruction.FarBranch32,
            _ => instruction.NearBranchTarget,
        };
    }
}