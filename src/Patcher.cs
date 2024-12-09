using HarmonyLib;
using System;

public class Patcher {
    private readonly Harmony instance;

    public Patcher(string id) {
        instance = new Harmony(id);
    }

    public void PatchAll() {
        instance.PatchAll();
    }

    public void Dispose() {
        instance.UnpatchAll();
    }
}
