namespace WaypointTogetherContinued
{
    using HarmonyLib;
    using Vintagestory.GameContent;
    using Vintagestory.API.Client;
    using Vintagestory.API.Config;

    [HarmonyPatch(typeof(GuiDialogEditWayPoint), "onSave")]
    class Patch_GuiDialogEditWayPoint_onSave
    {
        public static void Postfix(GuiDialogEditWayPoint __instance)
        {
            ICoreClientAPI capi = Traverse.Create(__instance).Field("capi").GetValue<ICoreClientAPI>();
            GuiComposer guiComposer = __instance.SingleComposer;
            if (capi != null && guiComposer.GetSwitch(Settings.ShouldShareSwitchName).On)
            {
                Core mod = capi.ModLoader.GetModSystem<Core>();
                GuiElementTextInput nameInput = guiComposer.GetTextInput("nameInput");
                if (nameInput == null)
                {
                    capi.ShowChatMessage("Failed to parse nameInput");
                    return;
                }

                var currentWaypoint = Traverse.Create(__instance).Field("waypoint").GetValue<Waypoint>();
                if (currentWaypoint == null)
                {
                    capi.ShowChatMessage("Failed to parse waypoint");
                    return;
                }
                currentWaypoint.Title = nameInput.GetText();
                mod.client.network.ShareWaypoint(currentWaypoint);
                string messageToTheUser = Lang.Get("waypointtogethercontinued:waypoint-shared", currentWaypoint.Title);
                capi.ShowChatMessage(messageToTheUser);
            }
        }
    }
}
