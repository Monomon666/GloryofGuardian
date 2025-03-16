namespace GloryofGuardian.Content.Class

{
    public abstract class GOGCalling : ModItem
    {
        //允许进行当前伤害类型的重铸操作
        public override bool WeaponPrefix() => true;

        /// <summary>
        /// 用来特判和传递来自武器前缀的攻速加成
        /// </summary>
        public static float PrefixCD(Item item) {
            if (item.prefix == ModContent.PrefixType<Stainless0>()
                || item.prefix == ModContent.PrefixType<Sensitive0>()
                || item.prefix == ModContent.PrefixType<Silent0>()
                ) return 0.85f;
            if (item.prefix == ModContent.PrefixType<Peerless0>()
                || item.prefix == ModContent.PrefixType<Excellent0>()
                || item.prefix == ModContent.PrefixType<Classic0>()
                || item.prefix == ModContent.PrefixType<Precise0>()
                ) return 0.9f;
            if (item.prefix == ModContent.PrefixType<Overclocked0>()
                || item.prefix == ModContent.PrefixType<Blooey0>()
                ) return 1f;
            if (item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return 1.05f;
            if (item.prefix == ModContent.PrefixType<Burdened0>()
                || item.prefix == ModContent.PrefixType<Damaged0>()
                ) return 1.1f;
            if (item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 1.15f;
            return 1;
        }

        /// <summary>
        /// 用来特判和传递来自武器前缀的暴击加成
        /// </summary>
        public static float PrefixCrit(Item item) {
            if (item.prefix == ModContent.PrefixType<Excellent0>()
                || item.prefix == ModContent.PrefixType<Sensitive0>()
                ) return 0f;
            if (item.prefix == ModContent.PrefixType<Peerless0>()
                || item.prefix == ModContent.PrefixType<Classic0>()
                || item.prefix == ModContent.PrefixType<Silent0>()
                ) return 5f;
            if (item.prefix == ModContent.PrefixType<Overclocked0>()
                || item.prefix == ModContent.PrefixType<Burdened0>()
                ) return 10f;
            if (item.prefix == ModContent.PrefixType<Blooey0>()
                || item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 20f;
            if (item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return -5f;
            if (item.prefix == ModContent.PrefixType<Precise0>()
                || item.prefix == ModContent.PrefixType<Damaged0>()
                ) return -10f;
            if (item.prefix == ModContent.PrefixType<Stainless0>()
                ) return -20f;
            return 0;
        }
    }
}