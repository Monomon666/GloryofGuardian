namespace GloryofGuardian.Content.Classes {
    public abstract class GOGAccessories : ModItem {
        //定义互斥饰品组

        //戍卫信物
        public static readonly int[] GuardiansBrand = new int[]
        {
            //ModContent.ItemType<WildernessSeed>(),
            //ModContent.ItemType<BloomingLife>(),
            //ModContent.ItemType<HellFlare>(),
            //
            //ModContent.ItemType<HolyBond>(),
            //ModContent.ItemType<Resolute>(),
            //ModContent.ItemType<Syncore>()
        };

        //戍卫信物
        public static readonly int[] GuardiansErodedBrand = new int[]
        {
            //ModContent.ItemType<Dirge>(),
            //ModContent.ItemType<Elegy>(),
            //ModContent.ItemType<Candor>(),
        };

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) {
            //// 如果当前饰品和新饰品都属于戍卫信物
            //if (GuardiansBrand.Contains(equippedItem.type) &&
            //    GuardiansBrand.Contains(incomingItem.type)) {
            //    return false; // 禁止同时装备
            //}
            //
            //// 如果当前饰品和新饰品都属于侵蚀信物
            //if (GuardiansErodedBrand.Contains(equippedItem.type) &&
            //    GuardiansErodedBrand.Contains(incomingItem.type)) {
            //    return false; // 禁止同时装备
            //}

            return true; // 允许同时装备
        }
    }
}
