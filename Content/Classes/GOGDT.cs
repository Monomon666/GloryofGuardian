using GloryofGuardian.Content.Projectiles;
using InnoVault.GameContent.BaseEntity;
using Terraria.DataStructures;

namespace GloryofGuardian.Content.Class
{
    public abstract class GOGDT : BaseHeldProj
    {
        //绝对时间
        public int globalcount = 0;
        //最早召唤物的标记
        bool earliest = true;
        public override void OnSpawn(IEntitySource source) {
            //被召唤时记录的绝对时间，用于排序和挤占栏位
            globalcount = (int)Main.GameUpdateCount;
        }

        //山铜强化标记
        public bool OrichalcumMarkDT;
        public bool OrichalcumMarkDT2;
        public bool OrichalcumMarkCrit;
        public int OrichalcumMarkDTcount;
        public override void AI() {
            //栏位检查:
            //查询当前玩家的栏位，如果已经超出上限，则依据编号挤掉自己
            {
                //重置标记
                earliest = true;
                //编号
                int MaxIndex = 0;
                int index = 0;//定义序号为0
                foreach (var proj in Main.projectile)//遍历所有弹幕
                {
                    if (proj.active //活跃状态
                        && proj.ModProjectile is GOGDT gogdt1//遍历
                        && proj.owner == Owner.whoAmI //同属同一个主人
                        ) {

                        //总数
                        MaxIndex++;

                        //特判占用更多栏位的炮台
                        if (proj.type == ModContent.ProjectileType<ShurikenDT>()) MaxIndex += 1;
                        if (proj.type == ModContent.ProjectileType<CobaltDT>()) MaxIndex += 1;
                        if (proj.type == ModContent.ProjectileType<SRMeteorDT>()) MaxIndex += 2;
                        if (proj.type == ModContent.ProjectileType<HarpyDT>()) MaxIndex += 1;
                        if (proj.type == ModContent.ProjectileType<AdamantiteDT>()) MaxIndex += 2;
                        if (proj.type == ModContent.ProjectileType<PlanteraDT>()) MaxIndex += 2;
                        if (proj.type == ModContent.ProjectileType<SirenDT>()) MaxIndex += 2;
                        //特判不占用栏位的炮台
                        if (proj.type == ModContent.ProjectileType<SlimeProj0>()) MaxIndex -= 1;

                        //序列号
                        if (gogdt1.globalcount < globalcount) {
                            //检测是否有比自己先生成的炮塔,如果有说明自己的序号要后移一位
                            index++;
                            earliest = false;
                        }
                    }
                }

                //如果召唤物超出上限，则
                if (MaxIndex > Owner.GetModPlayer<GOGModPlayer>().Gslot && earliest) {
                    Projectile.Kill();
                }
            }

            //山铜印记强化
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
