using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using GuiComposerHelpers = Vintagestory.API.Client.GuiComposerHelpers;

static class WaypointAdd {
    [HarmonyPatch(typeof(GuiDialogAddWayPoint), "ComposeDialog")]
    static class AddWaypointPatch {
        static void OnShareSwitch(bool on) {}

        public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds) {
            if (GuiComposerHelpers.GetSwitch(composer, "shouldShare") == null) {
                composer = composer.AddStaticText("Share", CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 9, 0, 0));

                return GuiComposerHelpers.AddSwitch(composer, OnShareSwitch, toggleBounds = toggleBounds.BelowCopy(0, 5, 0, 0).WithFixedWidth(200), "shouldShare");
            }

            return composer;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var found = false;

            foreach (var instruction in instructions) {
                if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "waypoint-color") {
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(AddWaypointPatch).GetMethod("AddShareComponent", BindingFlags.Static | BindingFlags.Public));

                    found = true;
                }

                yield return instruction;
            }

            if (found is false) {
                throw new ArgumentException("Cannot find `waypoint-color` in GuiDialogAddWayPoint.ComposeDialog");
            }
        }
    }

    [HarmonyPatch(typeof(GuiDialogAddWayPoint), "onSave")]
    static class SaveWaypointPatch {
        public static void BroadcastWaypoint(
            string message,
            bool shouldShare,
            ICoreClientAPI capi
        ) {
            if (!shouldShare) {
                return;
            }

            WaypointTogetherContinued.Core mod = capi.ModLoader.GetModSystem<WaypointTogetherContinued.Core>();

            mod.client.network.ShareWaypoint(message);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var found = false;

            foreach (var instruction in instructions) {
                if (instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == typeof(ICoreClientAPI).GetMethod("SendChatMessage", new Type[2]{ typeof(string), typeof(string) })) {
                    // Remove the ldnull call, add back at end
                    yield return new CodeInstruction(OpCodes.Pop, null);
                    // Duplicate message
                    yield return new CodeInstruction(OpCodes.Dup, null);
                    // Get "shouldShare" switch value from patched ComposeDialog
                    yield return new CodeInstruction(OpCodes.Ldarg_0, null);
                    yield return new CodeInstruction(OpCodes.Call, typeof(GuiDialog).GetMethod("get_SingleComposer", BindingFlags.Instance | BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Ldstr, "shouldShare");
                    yield return new CodeInstruction(OpCodes.Call, typeof(GuiComposerHelpers).GetMethod("GetSwitch", BindingFlags.Static | BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(GuiElementSwitch).GetField("On", BindingFlags.Instance | BindingFlags.Public));
                    // Load capi
                    yield return new CodeInstruction(OpCodes.Ldarg_0, null);
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(GuiDialogAddWayPoint).GetField("capi", BindingFlags.Instance | BindingFlags.NonPublic));
                    yield return new CodeInstruction(OpCodes.Call, typeof(SaveWaypointPatch).GetMethod("BroadcastWaypoint", BindingFlags.Static | BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Ldnull, null);

                    found = true;
                }

                yield return instruction;
            }

            if (found is false) {
                throw new ArgumentException("Cannot find `SendChatMessage` in OriginalType.OriginalMethod");
            }
        }
    }
}
