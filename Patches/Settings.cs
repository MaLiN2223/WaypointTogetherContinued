using HarmonyLib;
using System.Reflection;
using System;
using Vintagestory.API.Client;

namespace WaypointTogetherContinued.Patches;
public static class Settings
{
    public static readonly string ShouldShareSwitchName = "shouldShareButton";
    public static readonly MethodInfo SendChatMessageMethod = AccessTools.Method(typeof(ICoreClientAPI), nameof(ICoreClientAPI.SendChatMessage), new Type[2] { typeof(string), typeof(string) });
}
