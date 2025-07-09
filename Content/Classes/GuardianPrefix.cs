using GloryofGuardian.Content.Class;

namespace GloryofGuardian.Content.Classes
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
            damageMult = DamageMult;
            useTimeMult = CoolingReduction;
            critBonus = OverloadChance;
        }

        // 影响物品的稀有度
        public override void ModifyValue(ref float valueMult) {
            valueMult *= 1f + 0.05f * Power;
        }

        public override void Apply(Item item) {
            if (item.CountsAsClass<GuardianDamageClass>())
                item.GetGlobalItem<GOGGlobalItem>().GuardianStrikePrefixBonus = DamageMult;
        }
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

