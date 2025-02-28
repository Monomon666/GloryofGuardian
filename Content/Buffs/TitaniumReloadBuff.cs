using GloryofGuardian.Common;
using System.Runtime.InteropServices;
using Terraria.ID;

namespace GloryofGuardian.Content.Buffs
{
    public class TitaniumReloadBuff : ModBuff//继承modbuff类
    {
        public override string Texture => GOGConstant.Buffs + Name;

        public override void SetStaticDefaults() {
            ////////////////////////////////////////////////////////////////////////
            Main.debuff[Type] = false;//为true时就是debuff
            Main.buffNoSave[Type] = false;//为true时退出世界时buff消失
            Main.buffNoTimeDisplay[Type] = false;//为true时不显示剩余时间
            Main.pvpBuff[Type] = true; // PVP = true

            Main.buffNoTimeDisplay[Type] = false;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;//debuff不可被护士去除
            BuffID.Sets.LongerExpertDebuff[Type] = false;//专家模式Debuff持续时间是否延长
        }

        public override void Update(Player player, ref int buffIndex) {
            if (player.HasBuff<TitaniumShieldBuff>()) player.ClearBuff(ModContent.BuffType<TitaniumShieldBuff>());

            for (int i = 0; i <= 1; i++) {
                Dust dust1 = Dust.NewDustDirect(player.position + new Vector2(-8, 16), player.width + 12, player.height - 16, DustID.Titanium, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity *= 0.5f;
                dust1.noGravity = true;
            }

            base.Update(player, ref buffIndex);
        }
    }
}