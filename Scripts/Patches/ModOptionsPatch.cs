using HarmonyLib;
using UnboundLib.Utils.UI;
using CardDataEditor.UI;

namespace CardDataEditor.Patches {
    [HarmonyPatch(typeof(ModOptions), "CreatModOptionsMenu")]
    internal class ModOptionsPatch {
        public static void Postfix(bool pauseMenu) {
            if(pauseMenu) return;
            CardDataEditorMenuHandler.Instance.CreateMenu();
        }
    }
}
