using BepInEx.Logging;
using CardDataEditor.Utils.Debug;
using UnboundLib;
using UnboundLib.Utils.UI;
using UnityEngine;

namespace CardDataEditor.UI {
     public class CardDataEditorMenuHandler : MonoBehaviour {
        public static CardDataEditorMenuHandler Instance;
        public bool isOpened = false;


        private void Awake() {
            Instance = this;
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.Escape)) CloseMenu();
        }


        internal void CreateMenu() {
            GameObject modOptionsMenu = (GameObject)ModOptions.instance.GetFieldValue("modOptionsMenu");

            MenuHandler.CreateButton("Card Data Editor", modOptionsMenu, OpenMenu);
        }

        public void OpenMenu() {
            if (!isOpened) {
                if (MainMenuHandler.instance.isOpen) MainMenuHandler.instance.Close();
                Camera mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
                Canvas canvas = CardDataEditorUI.Instance.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = mainCamera;

                CardDataEditorUI.Instance.OpenMenu();
                LoggerUtils.Log(LogLevel.Debug, "Card data edttor menu have been opened.");
                isOpened = true;
            }
        }

        public void CloseMenu() {
            if (isOpened) {
                if (!MainMenuHandler.instance.isOpen) MainMenuHandler.instance.Open();
                CardDataEditorUI.Instance.CloseMenu();

                LoggerUtils.Log(LogLevel.Debug, "Card data edttor menu have been closed.");
                isOpened = false;
            }
        }
    }
}
