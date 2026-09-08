using BepInEx.Configuration;
using TMPro;
using UnboundLib;
using UnboundLib.Utils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor {
    public static class CardDataEditorConfig {
        public static ConfigEntry<bool> DangerMode;

        public static void RegisterMenu(ConfigFile config) {
            DangerMode = config.Bind(CardDataEditor.ModName, "DangerMode", false, "Enable or disable \"Danger Mode\" which will allow you to detech ANY class cards from thier classes.");
            Unbound.RegisterMenu($"{CardDataEditor.ModName} Options", () => { }, CreateMenu, null, false);
        }

        public static void CreateMenu(GameObject menuObject) {
            MenuHandler.CreateText("<b>Card Data Editor</b>", menuObject, out TextMeshProUGUI _, 70);
            AddBlank(menuObject, 50);

            GameObject toggle = null;

            toggle = MenuHandler.CreateToggle(
                DangerMode.Value,
                "<color=#8b0000>Danger Mode</color>",
                menuObject,
                value => {
                    Toggle toggleComponent = toggle.GetComponentInChildren<Toggle>();

                    if (!value) {
                        DangerMode.Value = false;
                        return;
                    }

                    if (DangerMode.Value)
                        return;

                    toggleComponent.isOn = false;

                    Unbound.BuildModal()
                        .Title("Are you sure?")
                        .Message(
                            "Are you sure you want to enable <color=#8b0000>Danger Mode</color>?\n" +
                            "<color=#ff0000>THIS COULD BREAK SOME CARDS IF YOU DO NOT KNOW WHAT YOU ARE DOING.</color>"
                        )
                        .CancelButton("No", () => {
                            toggleComponent.isOn = false;
                        })
                        .ConfirmButton("Yes", () => {
                            DangerMode.Value = true;
                            toggleComponent.isOn = true;
                        })
                        .Show();
                }
            );
        }

        private static void AddBlank(GameObject menu, int size = 30) {
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, size);
        }
    }
}
