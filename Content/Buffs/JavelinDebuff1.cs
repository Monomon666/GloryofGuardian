using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Buffs
{
    //Todo
    public class JavelinDebuff1 : ModBuff
    {
        public override string Texture => GOGConstant.Buffs + Name;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;//Ϊtrueʱ����debuff
            Main.buffNoSave[Type] = true;//Ϊtrueʱ�˳�����ʱbuff��ʧ
            Main.buffNoTimeDisplay[Type] = false;//Ϊtrueʱ����ʾʣ��ʱ��
            Main.pvpBuff[Type] = true; // PVP = true

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;//debuff���ɱ���ʿȥ��
            BuffID.Sets.LongerExpertDebuff[Type] = false;//ר��ģʽDebuff����ʱ���Ƿ��ӳ�
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.GetGlobalNPC<GOGGlobalNPCs>().JavelinDebuffEffect1 = true;
            base.Update(npc, ref buffIndex);
        }
    }
}