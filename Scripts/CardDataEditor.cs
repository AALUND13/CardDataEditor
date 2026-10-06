using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CardDataEditor.Conditions;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.DataEditting.Registries;
using CardDataEditor.UI;
using CardDataEditor.UI.ContextMenu.Cards;
using CardDataEditor.UI.Menus;
using CardDataEditor.Utils.Debug;
using HarmonyLib;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;

namespace CardDataEditor {
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]

    [BepInDependency("com.willuwontu.rounds.managers", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInProcess("Rounds.exe")]
    [BepInPlugin(ModId, ModName, Version)]
    public class CardDataEditor : BaseUnityPlugin {
        public const string ModId = "AALUND13.Card.Data.Editor";
        public const string ModName = "Card Data Editor";
        public const string ModInitials = "CDE";
        public const string Version = "1.3.7"; // What version are we on (major.minor.patch)?

        public static CardDataEditor Instance { get; private set; }
        public static ManualLogSource ModLogger { get; private set; }

        public static bool IsFirstRun { get; internal set; } = true;
        public static AssetBundle Assets;

        public static List<BaseUnityPlugin> Plugins;
        public static ConfigFile ModConfig;

        internal static bool ClassesInitialized = false;

        void Awake() {
            Instance = this;
            ModLogger = Logger;

            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            gameObject.AddComponent<Profiler>();
            gameObject.AddComponent<CardDataEditorMenuHandler>();

            Assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("card_data_editor", typeof(CardDataEditor).Assembly);
            ModConfig = new ConfigFile(Path.Combine(Paths.ConfigPath, "CardDataEditor", "CardDataEditor.cfg"), true);
        }

        void Start() {
            Plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            CardDataEditorConfig.RegisterMenu(ModConfig);
            PropertyRegistry.RegisterDefaultProperties();
            UIRegsitry.Init();

            StartCoroutine(InitializeData());

            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);

            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(RequireCardsCondition.PlayerAllowedCard);

            CreateCardDataEditorCanvas();
        }

        private IEnumerator InitializeData() {
            // Wait for 60 frames to ensure that all other mods have loaded and initialized their cards, otherwise we will get issues with missing cards.
            for (int i = 0; i < 60; i++) {
                yield return null;
            }
            // We have to wait until the classes are initialized before we can register our card options and config, otherwise we will get issus.
            yield return new WaitUntil(() => ClassesInitialized);

            CardOptionRegistry.RegisterAllCardOptions();
            CardOptionsConfigManager.RegisterConfig();

            CardDataEditorMenu.Instance.Init(CardOptionsConfigManager.Config);
            CardsListContextMenu.Instance.Init(CardOptionsConfigManager.Config);
        }

        private void CreateCardDataEditorCanvas() {
            if (CardDataEditorMenu.Instance != null) return;

            GameObject UIManagerCanvas = Instantiate(Assets.LoadAsset<GameObject>("Card Data Editor Canvas"));
            Camera mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            UIManagerCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            UIManagerCanvas.GetComponent<Canvas>().worldCamera = mainCamera;

            DontDestroyOnLoad(UIManagerCanvas);

            LoggerUtils.LogInfo("Created Card Data Editor Canvas.");
        }

        private void OnHandShakeCompleted() {
            if (PhotonNetwork.IsMasterClient) {
                CardOptionsConfigManager.Config.SyncWithConfig();
                byte[] data = CardOptionRegistry.Serialize();
                NetworkingManager.RPC_Others(typeof(CardDataEditor), nameof(SyncCardsOptions), data);
            }
        }

        [UnboundRPC]
        private static void SyncCardsOptions(byte[] data) {
            CardOptionRegistry.Deserialize(data);
        }
    }
}