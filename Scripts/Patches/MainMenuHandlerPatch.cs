using CardDataEditor.DataEditting.Config;
using HarmonyLib;

namespace CardDataEditor.Scripts.Patches {
    [HarmonyPatch(typeof(MainMenuHandler), "Awake")]
    public class MainMenuHandlerPatch {
        public static void Postfix(MainMenuHandler __instance) {
            if (CardDataEditor.IsFirstRun) {
                CardDataEditor.IsFirstRun = false;
                return;
            }

            CardOptionsConfigManager.Config.SyncWithConfig();
        }
    }
}
