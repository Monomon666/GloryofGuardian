using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles.ProjNPC
{
    public class FinalSpiralf3DT : ModNPC
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 48;
            NPC.damage = 100;//普通模式下的碰撞攻击力
            NPC.defense = 50;//防御力(2防=1点免疫)
            NPC.lifeMax = 500;//最大生命值
            NPC.HitSound = SoundID.NPCHit29;//受伤音效
            NPC.DeathSound = SoundID.NPCDeath32;//死亡音效
            NPC.value = Item.buyPrice(1, 0, 0, 0);//掉落金钱（普通模式为基准）
            NPC.knockBackResist = 1.5f;//击退接受性（0为彻底免疫击退）
            NPC.alpha = 0;//透明度，史莱姆为105
            NPC.noGravity = false;//无重力
            NPC.noTileCollide = false;//穿墙
            NPC.boss = false;//令其为boss
            NPC.npcSlots = 1f; //占用1个NPC槽位
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.friendly = true;//敌对关系

            //旗帜
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EbonianBlightSlimeBanner>();

            NPC.aiStyle = -1;
            AIType = -1;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return base.GetAlpha(drawColor);
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        int count = 0;
        public override void AI()
        {
            count++;
            //摩擦力


            base.AI();
        }

        void Attack()
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            base.OnHitNPC(target, hit);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        public override bool CheckDead()
        {//濒死判定,

            return false;//空血处理
        }

        public override void OnKill()//在确认被杀后执行的一系列操作，例如推进世界进度，开启某些条件等,死亡效果不是用这个完成的
        {
            //击杀记录
            //GOGDownedBossSystem.downedNightWatcher = true;
            base.OnKill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralf3DT").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralf3DTG").Value;

            return true;
        }

        //Todo要写联机同步哟
    }
}
