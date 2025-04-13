using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using static System.Net.Mime.MediaTypeNames;

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
            NPC.width = 46;
            NPC.height = 46;
            NPC.damage = 500;//普通模式下的碰撞攻击力
            NPC.defense = 50;//防御力(2防=1点免疫)
            NPC.lifeMax = 2000;//最大生命值
            NPC.HitSound = SoundID.NPCHit4;//受伤音效
            NPC.DeathSound = SoundID.NPCDeath14;//死亡音效
            NPC.value = Item.buyPrice(1, 0, 0, 0);//掉落金钱（普通模式为基准）
            NPC.knockBackResist = 1.5f;//击退接受性（0为彻底免疫击退）
            NPC.alpha = 0;//透明度，史莱姆为105
            NPC.noGravity = true;//无重力
            NPC.noTileCollide = true;//穿墙
            NPC.boss = false;//令其为boss
            NPC.npcSlots = 1f; //占用1个NPC槽位
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.friendly = true;//敌对关系

            //旗帜
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EbonianBlightSlimeBanner>();

            NPC.aiStyle = -1;
            AIType = -1;

            NPC.scale *= 1.2f;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return base.GetAlpha(drawColor);
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        int countleft = 0;
        int count = 0;
        int mode = 0;
        NPC tar0 = null;
        public override void AI()
        {
            countleft++;
            count++;
            NPC.rotation = -MathHelper.PiOver4;

            //上升,连续攻击并前往一个位置,散弹,向上飞出脱战
            //如果是boss,会锁定与boss的相对位置

            //上升
            if (mode == 0)
            {
                NPC.velocity = new Vector2(0, -2f);
                if (count >= 60)
                {
                    NPC target1 = NPC.Center.InPosClosestNPC(3000, true, true);
                    if (target1 != null && target1.active)
                    {
                        tar0 = target1;
                        count = 0;
                        mode = 1;
                        if(tar0.boss) mode = -1;
                    }

                    if (target1 == null)
                    {
                        NPC.velocity *= 0;
                    }
                }
            }

            //巡逻
            if (mode == 1)
            {
                if (tar0 != null && tar0.active)
                {
                    if (Vector2.Distance(NPC.Center, tar0.Center) > 400) NPC.velocity = NPC.Center.Toz(tar0.Center + new Vector2(0, -180)) * 12f;
                    if (Vector2.Distance(NPC.Center, tar0.Center) <= 240) NPC.velocity *= 0f;
                }

                if (tar0 == null || !tar0.active)
                {
                    NPC.velocity *= 0.1f;

                    NPC target1 = NPC.Center.InPosClosestNPC(3000, true, true);
                    if (target1 != null && target1.active)
                    {
                        tar0 = target1;
                        count = 0;
                        mode = 1;
                        if (tar0.boss) mode = -1;
                    }
                }
            }

            //应对
            if (mode == -1 || mode == -2)
            {
                if (mode == -1 && tar0 != null && tar0.active)
                {
                    if (Vector2.Distance(NPC.Center, tar0.Center + new Vector2(0, -tar0.height / 2 - 180)) > 16) NPC.velocity = NPC.Center.Toz(tar0.Center + new Vector2(0, -tar0.height / 2 - 180)) * 12f;
                    if (Vector2.Distance(NPC.Center, tar0.Center + new Vector2(0, -tar0.height / 2 - 180)) <= 16)
                    {
                        NPC.Center = tar0.Center + new Vector2(0, -tar0.height / 2 - 180);
                        mode = -2;
                    }
                }

                if(mode == -2) NPC.Center = tar0.Center + new Vector2(0, -tar0.height / 2 - 180);

                if (tar0 == null || !tar0.active)
                {
                    NPC.velocity *= 0.1f;

                    NPC target1 = NPC.Center.InPosClosestNPC(3000, true, true);
                    if (target1 != null && target1.active)
                    {
                        tar0 = target1;
                        count = 0;
                        mode = 1;
                        if (tar0.boss) mode = -1;
                    }
                }
            }

            if (mode != 0 && tar0 != null && tar0.active)
            {
                if(countleft % 12 == 0)
                Attack(tar0);
            }

            //粒子
            for (int j = 0; j < 2; j++)
            {
                int num1 = Dust.NewDust(NPC.Center + new Vector2(10, 6), 0, 0, DustID.PinkFairy, 0f, 0f, 10, RedColor, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = new Vector2(0, 2f);

                int num2 = Dust.NewDust(NPC.Center + new Vector2(-18, 6), 0, 0, DustID.PinkFairy, 0f, 0f, 10, RedColor, 1f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = new Vector2(0, 2f);
            }

            if (countleft > 300)
            {
                //爆炸粒子
                for (int j = 0; j < 20; j++)
                {
                    int num1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity *= 4f;
                }
                for (int j = 0; j < 60; j++)
                {
                    int num2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 3f;
                }
                NPC.active = false;
            }

            base.AI();
        }

        void Attack(NPC target)
        {
            Vector2 firepos1 = NPC.Center + new Vector2(10, 6);
            Vector2 firepos2 = NPC.Center + new Vector2(-18, 6);

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item114, NPC.Center);

            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(NPC), firepos1, firepos1.Toz(target.Center) * 24f, ModContent.ProjectileType<FinalSpiralfProj>(), 120, 8);
            Projectile proj2 = Projectile.NewProjectileDirect(new EntitySource_Parent(NPC), firepos2, firepos2.Toz(target.Center) * 24f, ModContent.ProjectileType<FinalSpiralfProj>(), 120, 8);
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
            base.OnKill();
        }

        int drawcount = 0;
        Color RedColor;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawcount++;
            if (drawcount >= 18) drawcount = 2;
            float t = (drawcount % 20) / 20f; // t在 [0, 1) 范围内循环
            RedColor = WhiteToRedGradient(t); // 生成渐变色

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralf3DT").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralf3DTG").Value;

            Main.spriteBatch.Draw(
                texture01,
                NPC.Center + new Vector2(0, 0) - Main.screenPosition,
                null,
                Color.LightGray * 1f,
                NPC.rotation,
                texture0.Size() / 2,
                NPC.scale,
                SpriteEffects.None,
                0);

            for (int i = 1; i < NPCID.Sets.TrailCacheLength[Type]; i++)//循环上限小于轨迹长度
            {
                //定义一个从新到旧由1逐渐减少到0的变量，比如i = 0时，factor = 1
                float factor = 1 - (float)i / NPCID.Sets.TrailCacheLength[Type];
                //由于轨迹只能记录弹幕碰撞箱左上角位置，我们要手动加上宽高一半来获取中心
                Vector2 oldcenter = NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition;

                Main.EntitySpriteDraw(
                    texture01,
                    oldcenter,
                    new Rectangle(0, 0, texture01.Width, texture01.Height),
                    Color.Red * factor * 0.5f,
                    NPC.oldRot[i],
                    texture0.Size() / 2,
                    NPC.scale,
                    SpriteEffects.None,
                    0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(
                    texture02,
                    NPC.Center + new Vector2(0, 0) - Main.screenPosition,
                    null,
                    RedColor,
                    NPC.rotation,
                    texture0.Size() / 2,
                    NPC.scale,
                    SpriteEffects.None,
                    0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        //Todo要写联机同步哟

        // 生成白色和红色之间的渐变色
        Color WhiteToRedGradient(float t)
        {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            if (t < 0.333f)
            {
                // 阶段1：黑色(0,0,0) → 红色(1,0,0)
                float r = t * 3f;    // 红色通道从0升到1
                return new Color(r, 0f, 0f);
            }
            else if (t < 0.666f)
            {
                // 阶段2：红色(1,0,0) → 白色(1,1,1)
                float gb = (t - 0.333f) * 3f; // 绿蓝通道从0升到1
                return new Color(1f, gb, gb);
            }
            else
            {
                // 阶段3：白色(1,1,1) → 黑色(0,0,0)
                float rgb = 1f - (t - 0.666f) * 3f; // 所有通道从1降到0
                return new Color(rgb, rgb, rgb);
            }
        }
    }
}
