using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.Classes;
using Terraria;
using Terraria.ID;

namespace GloryofGuardian.Content.ParentClasses {
    public abstract class GOGGuardNPC : ModNPC {
        //预设变量
        /// <summary>
        /// 行为计时器
        /// </summary>
        protected int count = 0;

        /// <summary>
        /// 永久计时器
        /// </summary>
        protected int drawcount = 0;

        /// <summary>
        /// 攻击间隔的初始值
        /// </summary>
        protected int count0 = 60;
        /// <summary>
        /// 计算得出的最终攻击间隔
        /// </summary>
        protected int Gcount = 0;
        /// <summary>
        /// 核心位置
        /// </summary>
        protected Vector2 AttackPos = Vector2.Zero;

        /// <summary>
        /// 主人玩家索引,如果找不到主人,此时会自动寻找最近的玩家作为主人
        /// </summary>
        public int OwnerWhoAmI = -1;
        public Player Owner = null;
        public int OwnerCrit = 0;

        public override void SetStaticDefaults() {
            //Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetDefaults() {
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = false;
            NPC.boss = false;
            NPC.npcSlots = 1f;
            NPC.friendly = true;

            SetProperty();
        }

        /// <summary>
        /// 用于设置额外的基础属性，在<see cref="SetDefaults"/>中被最后调用,能够覆盖靠前的设置
        /// </summary>
        public virtual void SetProperty() {
        }

        public override void AI() {
            count++;
            drawcount++;

            if (OwnerWhoAmI != -1) {
                Owner = Main.player[OwnerWhoAmI];
            }
            else Owner = Main.player[NPC.FindClosestPlayer()];

            if (OwnerCrit == 0 && Owner != null) OwnerCrit = (int)Owner.GetCritChance<GenericDamageClass>();
            DTDrop();
            Attack0();

            base.AI();
        }

        public bool HasDropped = false;
        /// <summary>
        /// 坠落
        /// </summary>
        void DTDrop() {
            NPC.velocity.Y += 0.2f;
            if (NPC.velocity.Y > 8f) {
                NPC.velocity.Y = 8f;
            }

            Vector2 droppos = NPC.Bottom;
            if (!HasDropped) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 4));
                    if (tile0.HasTile) {
                        NPC.Bottom = (droppos + new Vector2(0, y - 2) * 4);
                        break;
                    }
                }
                HasDropped = true;
            }
        }

        protected bool FinishAttack = false;//是否完成攻击

        /// <summary>
        /// 预设的攻击行为
        /// </summary>
        /// <param name="target1"></param>
        void Attack0() {
            //发射
            if (count >= Gcount) {
                //万一忘了在具体子类里重置,重置一下
                //普通攻击
                Attack1();

                if (FinishAttack) {
                    count = 0;
                    FinishAttack = false;
                }
            }
        }

        /// <summary>
        /// 在此补充普通攻击逻辑
        /// 注意令FinishAttack为true来结束
        /// </summary>
        protected virtual void Attack1() {
        }

        /// <summary>
        /// 在此补充过载攻击逻辑
        /// 注意令FinishAttack为true来结束
        /// </summary>
        protected virtual void Attack2() {
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnKill() {
            ExplosionDust(exdust);
            base.OnKill();
        }

        //粒子
        protected int exdust = 0;
        protected Vector2 exdpos = Vector2.Zero;
        protected int exdx = 0;
        protected int exdy = 0;

        /// <summary>
        /// 收回炮台的爆炸粒子特效
        /// </summary>
        protected virtual void ExplosionDust(int dustid) {
            if (dustid == 0) dustid = DustID.Wraith;

            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustid, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustid, 0f, 0f, 10, Color.White,0.8f);
            Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) {
            return 0;
        }

        public override bool CheckDead() {
            return false;
        }
    }
}
