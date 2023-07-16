using System;
using System.Reflection;
using EmptyKeys.UserInterface.Generated;
using HarmonyLib;
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
    internal static class PatchAtm
    {
        [HarmonyPatch(nameof(MyUseObjectAtmBlock.Use))]
        [HarmonyPostfix]
        public static void Patch(MyUseObjectAtmBlock __instance, UseActionEnum actionEnum, IMyEntity userEntity)
        {
            if (actionEnum == UseActionEnum.Manipulate)
            {
                ATM.Open(__instance.Owner as MyStoreBlock);
            }
        }
    }
}