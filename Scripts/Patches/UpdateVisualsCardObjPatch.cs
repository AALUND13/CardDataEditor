using CardDataEditor.DataEditting;
using CardDataEditor.DataEditting.Registries;
using HarmonyLib;
using UnboundLib.Utils.UI;
using UnityEngine;

namespace CardDataEditor.Scripts.Patches {
    [HarmonyPatch(typeof(ToggleCardsMenuHandler), nameof(ToggleCardsMenuHandler.UpdateVisualsCardObj))]
    public class UpdateVisualsCardObjPatch {
        public static void Postfix(ToggleCardsMenuHandler __instance, GameObject cardObject) {
            if (ToggleCardsMenuHandler.cardMenuCanvas.gameObject.activeSelf) {
                CardInfo cardInfo = cardObject.GetComponentInChildren<CardInfo>(false);
                if (cardInfo == null) return;

                string name = cardObject.GetComponentInChildren<CardInfo>().name.Substring(0, cardObject.GetComponentInChildren<CardInfo>().name.Length - 7);
                CardInfo sourceCardInfo = ModdingUtils.Utils.Cards.instance.GetCardWithObjectName(name);
                CardOptions cardOptions = CardOptionRegistry.GetCardOptions(sourceCardInfo);
                if (cardOptions == null) return;

                foreach (var property in cardOptions.Properties.Values) {
                    property.ApplyPropertyToPreviewCard(cardObject, cardInfo);
                }
            }
        }
    }
}
