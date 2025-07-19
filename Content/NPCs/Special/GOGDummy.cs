using Terraria.ID;

namespace GloryofGuardian.Content.NPCs.Special {
    public class GOGDummy : ModNPC {
        public override void SetStaticDefaults() {
            //Main.npcFrameCount[NPC.type] = 1;//帧图未加载
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetDefaults() {
            NPC.width = 36;
            NPC.height = 50;
            NPC.damage = 0;//普通模式下的碰撞攻击力
            NPC.defense = 0;//防御力(2防=1点免疫)
            NPC.lifeMax = 9999999;//最大生命值
            NPC.HitSound = SoundID.NPCHit1;//受伤音效
            NPC.DeathSound = SoundID.NPCDeath1;//死亡音效
            NPC.value = Item.buyPrice(1, 0, 0, 0);//掉落金钱（普通模式为基准）
            NPC.knockBackResist = 0f;//击退接受性（0为彻底免疫击退）
            NPC.alpha = 55;//透明度，史莱姆为105
            NPC.noGravity = false;//无重力
            NPC.noTileCollide = false;//穿墙
            NPC.boss = false;//令其为boss
            NPC.npcSlots = 1f; //占用1个NPC槽位
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.friendly = false;//敌对关系

            //帧图
            //AnimationType = NPCID.TargetDummy;

            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
        }

        public static int owner = 0;

        public override void AI() {
            NPC.boss = true;
            NPC.life = (int)(NPC.lifeMax / 3f);

            base.AI();
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
            base.OnHitByItem(player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }
    }
}
