namespace CardDataEditor.Interfaces {
    public static class ToggleCardsCategorieInterface {
        public static string GetCardSubcategory(CardInfo cardInfo) {
            if (CardDataEditor.Plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.aalund13.rounds.toggle_cards_categories")) {
                ToggleCardsCategories.IToggleCardCategory toggleCardCategory = cardInfo.GetComponent<ToggleCardsCategories.IToggleCardCategory>();
                if (toggleCardCategory != null) {
                    return toggleCardCategory.GetCardCategoryInfo().Name;
                } else {
                    return "";
                }
            } else {
                return "";
            }
        }
    }
}
