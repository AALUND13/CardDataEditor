using ToggleCardsCategories;

namespace CardDataEditor.Interfaces {
    public static class ToggleCardsCategorieInterface {
        public static string GetCardSubcategory(CardInfo cardInfo) {
            IToggleCardCategory toggleCardCategory = cardInfo.GetComponent<IToggleCardCategory>();
            if (toggleCardCategory != null) {
                return toggleCardCategory.GetCardCategoryInfo().Name;
            } else {
                return "";
            }
        }
    }
}
