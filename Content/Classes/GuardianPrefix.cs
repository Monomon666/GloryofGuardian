namespace GloryofGuardian.Content.Class
{
    //以下是武器词条
    public abstract class GaurdianWeaponPrefix : ModPrefix
    {
        // 属性预设
        //影响因子
        public virtual float Power => 1f;
        //伤害
        public virtual float DamageMult => 1f;
        //攻速
        public virtual float CoolingReduction => 1f;
        //过载
        public virtual int OverloadChance => 0;

        // 影响哪些物品可以获得此前缀
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public override float RollChance(Item item) {
            return 5f;
        }

        // 决定该前缀是否能刷新
        public override bool CanRoll(Item item) {
            return item.CountsAsClass<GuardianDamageClass>();
        }

        // 前缀修改的变量于此储存
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
            //使用预设量的在此处作用
            damageMult = this.DamageMult;
            useTimeMult = this.CoolingReduction;
            critBonus = this.OverloadChance;
        }

        // 影响物品的稀有度
        public override void ModifyValue(ref float valueMult) {
            valueMult *= 1f + 0.05f * Power;
        }

        public override void Apply(Item item) {
            if (item.CountsAsClass<GuardianDamageClass>())
                item.GetGlobalItem<GOGGlobalItem>().GuardianStrikePrefixBonus = DamageMult;
        }

        // 这个前缀不影响非标准的统计, 所以这些工具行不是必要的, 但是对于其它类似的可以遵循此模式
        //public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        //    yield return new TooltipLine(Mod, "PrefixWeaponAwesome", PowerTooltip.Format(Power)) {
        //        IsModifier = true, // 颜色为正前缀颜色
        //    };
        //    // 这种本地化不能与继承的类共享，需要自己写
        //    yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", AdditionalTooltip.Value) {
        //        IsModifier = true,
        //    };
        //}

        //public static LocalizedText PowerTooltip { get; private set; }

        // 本地化方法
        //public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

        //public override void SetStaticDefaults() {
        //    // 使用共享键
        //    PowerTooltip = Language.GetOrRegister(Mod.GetLocalizationKey($"{LocalizationCategory}.{nameof(PowerTooltip)}"));
        //    // 注册AdditionalTooltip所必要的代码
        //    _ = AdditionalTooltip;
        //}
    }

    #region 词条存储1
    [LegacyName("Peerless")]
    public class Peerless0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1.15f;
        public override float CoolingReduction => 0.9f;
        public override int OverloadChance => 5;

    }

    [LegacyName("Excellent")]
    public class Excellent0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1.15f;
        public override float CoolingReduction => 0.9f;
    }

    [LegacyName("Classic")]
    public class Classic0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1.1f;
        public override float CoolingReduction => 0.9f;
        public override int OverloadChance => 5;
    }

    [LegacyName("Precise")]
    public class Precise0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1.1f;
        public override float CoolingReduction => 0.9f;
        public override int OverloadChance => -10;
    }

    [LegacyName("Overclocked")]
    public class Overclocked0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1f;
        public override int OverloadChance => 10;
    }

    [LegacyName("Stainless")]
    public class Stainless0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1.15f;
        public override float CoolingReduction => 0.85f;
        public override int OverloadChance => -20;
    }

    [LegacyName("Sensitive")]
    public class Sensitive0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1f;
        public override float CoolingReduction => 0.85f;
    }

    [LegacyName("Burdened")]
    public class Burdened0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 1.15f;
        public override float CoolingReduction => 1.1f;
        public override int OverloadChance => 0;
    }

    [LegacyName("Silent")]
    public class Silent0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 0.9f;
        public override float CoolingReduction => 0.85f;
        public override int OverloadChance => 5;
    }

    [LegacyName("Blooey")]
    public class Blooey0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 0.9f;
        public override int OverloadChance => 20;
    }

    [LegacyName("Scrapped")]
    public class Scrapped0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 0.95f;
        public override float CoolingReduction => 1.05f;
        public override int OverloadChance => -5;
    }

    [LegacyName("Damaged")]
    public class Damaged0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 0.9f;

        public override float CoolingReduction => 1.1f;
        public override int OverloadChance => -10;
    }

    [LegacyName("ShortCircuited")]
    public class ShortCircuited0 : GaurdianWeaponPrefix
    {
        public override float DamageMult => 0.85f;
        public override float CoolingReduction => 1.15f;
        public override int OverloadChance => 20;
    }

    #endregion

    //以下是饰品的词条
    public abstract class GaurdianAccessoryPrefix : ModPrefix
    {
        //Todo
    }

    #region 词条存储2

    #endregion
}

