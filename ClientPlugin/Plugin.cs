using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmptyKeys.UserInterface.Generated;
using HarmonyLib;
using NLog.Fluent;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Screens.ViewModels;
using Sandbox.Graphics.GUI;
using SpaceEngineers.Game.Entities.UseObjects;
using VRage.Game.Entity.UseObject;
using VRage.Game.ModAPI.Ingame;
using VRage.Plugins;

namespace ClientPlugin
{
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "UltrawideGuiFix";
        public static Plugin Instance { get; private set; }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;

            // TODO: Put your one time initialization code here.
            Harmony harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Dispose()
        {
            // TODO: Save state and close resources here, called when the game exits (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.

            Instance = null;
        }

        public void Update()
        {
            // TODO: Put your update code here. It is called on every simulation frame!
        }

        // TODO: Uncomment and use this method to create a plugin configuration dialog
        // ReSharper disable once UnusedMember.Global
        /*public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new MyPluginConfigDialog());
        }*/
    }
    
    [HarmonyPatch(typeof(MyUseObjectAtmBlock))]
    public static class MyUseObjectAtmBlock_Patch
    {
        [HarmonyPatch(nameof(MyUseObjectAtmBlock.Use))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.Debug("Patching MyUseObjectAtmBlock.Use()");
            var codes = new List<CodeInstruction>(instructions);
            var startIndex = findInstructionIndex(codes, "call", "EmptyKeys.UserInterface.Mvvm.ServiceManager get_Instance()");
            var endIndex = findInstructionIndex(codes, "call", "Void AddScreen(Sandbox.Graphics.GUI.MyGuiScreenBase)");

            if (startIndex > -1 && endIndex > -1)
            {
                codes[startIndex].opcode = OpCodes.Nop;
                codes[startIndex].operand = null;
                codes.RemoveRange(startIndex + 1, endIndex - startIndex);
                IEnumerable<CodeInstruction> Inst()
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return CodeInstruction.Call(typeof(ATM), "Open", new []{typeof(MyStoreBlock)});
                }
                codes.InsertRange(startIndex+1, Inst());
            }

            return codes.AsEnumerable();
        }
        
        static int findInstructionIndex(IEnumerable<CodeInstruction> instructions, string opcode, string operand)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                var strOpcode = codes[i].opcode.ToString();
                var strOperand = codes[i].operand != null ? codes[i].operand.ToString() : null;
                if (strOpcode == opcode && strOperand == operand)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}