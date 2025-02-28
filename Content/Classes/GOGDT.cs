using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;

namespace GloryofGuardian.Content.Class
{
    public abstract class GOGDT : ModProjectile
    {
        //����������������

        //����
        Player Owner => Main.player[Projectile.owner];

        //����ʱ��
        public int globalcount = 0;
        //�����ٻ���ı��
        bool earliest = true;
        public override void OnSpawn(IEntitySource source) {
            //���ٻ�ʱ��¼�ľ���ʱ�䣬��������ͼ�ռ��λ
            globalcount = (int)Main.GameUpdateCount;
        }

        //ɽͭǿ�����
        public bool OrichalcumMarkDT;
        public bool OrichalcumMarkDT2;
        public bool OrichalcumMarkCrit;
        public int OrichalcumMarkDTcount;
        public override void AI() {
            //��λ���:
            //��ѯ��ǰ��ҵ���λ������Ѿ��������ޣ������ݱ�ż����Լ�
            {
                //���ñ��
                earliest = true;
                //���
                int MaxIndex = 0;
                int index = 0;//�������Ϊ0
                foreach (var proj in Main.projectile)//�������е�Ļ
                {
                    if (proj.active //��Ծ״̬
                        && proj.ModProjectile is GOGDT gogdt1//����
                        && proj.owner == Owner.whoAmI //ͬ��ͬһ������
                        ) {

                        //����
                        MaxIndex++;

                        //����ռ�ø�����λ����̨
                        if (proj.type == ModContent.ProjectileType<ShurikenDT>()) MaxIndex += 1;
                        if (proj.type == ModContent.ProjectileType<SRMeteorProj>()) MaxIndex += 2;
                        //���в�ռ����λ����̨
                        if (proj.type == ModContent.ProjectileType<SlimeProj0>()) MaxIndex -= 1;

                        //���к�
                        if (gogdt1.globalcount < globalcount) {
                            //����Ƿ��б��Լ������ɵ�����,�����˵���Լ������Ҫ����һλ
                            index++;
                            earliest = false;
                        }
                    }
                }

                //����ٻ��ﳬ�����ޣ���
                if (MaxIndex > Owner.GetModPlayer<GOGModPlayer>().Gslot && earliest) {
                    Projectile.Kill();
                }
            }

            //ɽͭӡ��ǿ��
            if (OrichalcumMarkDT2 == false) OrichalcumMarkDT = false;
            if (OrichalcumMarkDT2 == true) OrichalcumMarkDT2 = false;

            if (OrichalcumMarkDT) {
                if (OrichalcumMarkCrit) {
                    OrichalcumMarkCrit = true;
                    Projectile.CritChance += 20;
                }
            }

            if (OrichalcumMarkDT && OrichalcumMarkCrit) Projectile.CritChance -= 20;
        }

        public override bool? CanHitNPC(NPC target) {
            return false;
        }
    }
}
