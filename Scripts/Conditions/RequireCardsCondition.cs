using System.Collections.Generic;
using System.Linq;

namespace CardDataEditor.Conditions {
    public static class RequireCardsCondition {
        public static Dictionary<CardInfo, List<CardInfo>> RequireCards = new Dictionary<CardInfo, List<CardInfo>>();

        public static bool PlayerAllowedCard(Player player, CardInfo cardInfo) {
            if (RequireCards.TryGetValue(cardInfo, out List<CardInfo> requireCards)) {
                return requireCards.Count == 0 || requireCards.All(player.data.currentCards.Contains);
            } else {
                return true;
            }
        }
    }
}
