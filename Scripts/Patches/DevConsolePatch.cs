using CardDataEditor.UI;
using HarmonyLib;

namespace CardDataEditor.Patches {
    [HarmonyPatch(typeof(DevConsole), "Update")]
    public class DevConsolePatch {
        private static bool Prefix() {
            if(CardDataEditorMenuHandler.Instance.isOpened) return false;
            return true;
        }
    }
}
