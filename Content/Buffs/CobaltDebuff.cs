using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Buffs
{
    public class CobaltDebuff : ModBuff
    {
        public override string Texture => GOGConstant.Buffs + Name;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;//为true时就是debuff
            Main.buffNoSave[Type] = true;//为true时退出世界时buff消失
            Main.buffNoTimeDisplay[Type] = false;//为true时不显示剩余时间
            Main.pvpBuff[Type] = true; // PVP = true

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;//debuff不可被护士去除
            BuffID.Sets.LongerExpertDebuff[Type] = false;//专家模式Debuff持续时间是否延长
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.GetGlobalNPC<GOGGlobalNPCs>().CobaltDebuff = true;
            npc.GetGlobalNPC<GOGGlobalNPCs>().CobaltDebuff2 = true;
            base.Update(npc, ref buffIndex);
        }
    }
}