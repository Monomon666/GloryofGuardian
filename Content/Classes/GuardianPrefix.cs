using System.Collections.Generic;
using Terraria.Localization;

namespace GloryofGuardian.Content.Class
{
    //��������������
    public abstract class GaurdianWeaponPrefix : ModPrefix
    {
        // ����Ԥ��
        //Ӱ������
        public virtual float Power => 1f;
        //�˺�
        public virtual float DamageMult => 1f;
        //����
        public virtual float CoolingReduction => 1f;
        //����
        public virtual int OverloadChance => 0;

        // Ӱ����Щ��Ʒ���Ի�ô�ǰ׺
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public override float RollChance(Item item) {
            return 5f;
        }

        // ������ǰ׺�Ƿ���ˢ��
        public override bool CanRoll(Item item) {
            return item.CountsAsClass<GuardianDamageClass>();
        }

        // ǰ׺�޸ĵı����ڴ˴���
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
            //ʹ��Ԥ�������ڴ˴�����
            damageMult = this.DamageMult;
            useTimeMult = this.CoolingReduction;
            critBonus = this.OverloadChance;
        }

        // Ӱ����Ʒ��ϡ�ж�
        public override void ModifyValue(ref float valueMult) {
            valueMult *= 1f + 0.05f * Power;
        }

        public override void Apply(Item item) {
            if (item.CountsAsClass<GuardianDamageClass>())
                item.GetGlobalItem<GOGGlobalItem>().GuardianStrikePrefixBonus = DamageMult;
        }

        // ���ǰ׺��Ӱ��Ǳ�׼��ͳ��, ������Щ�����в��Ǳ�Ҫ��, ���Ƕ����������ƵĿ�����ѭ��ģʽ
        //public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        //    yield return new TooltipLine(Mod, "PrefixWeaponAwesome", PowerTooltip.Format(Power)) {
        //        IsModifier = true, // ��ɫΪ��ǰ׺��ɫ
        //    };
        //    // ���ֱ��ػ�������̳е��๲����Ҫ�Լ�д
        //    yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", AdditionalTooltip.Value) {
        //        IsModifier = true,
        //    };
        //}

        //public static LocalizedText PowerTooltip { get; private set; }

        // ���ػ�����
        //public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

        //public override void SetStaticDefaults() {
        //    // ʹ�ù����
        //    PowerTooltip = Language.GetOrRegister(Mod.GetLocalizationKey($"{LocalizationCategory}.{nameof(PowerTooltip)}"));
        //    // ע��AdditionalTooltip����Ҫ�Ĵ���
        //    _ = AdditionalTooltip;
        //}
    }

    #region �����洢1
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
        public override int OverloadChance =>   0;
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
        public override float CoolingReduction =>1.05f;
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

    //��������Ʒ�Ĵ���
    public abstract class GaurdianAccessoryPrefix : ModPrefix
    {
        //Todo
    }

    #region �����洢2

    #endregion
}

