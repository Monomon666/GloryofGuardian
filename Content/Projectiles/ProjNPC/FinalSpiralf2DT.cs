using System.Buffers.Text;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles.ProjNPC
{
    public class FinalSpiralf2DT : ModNPC
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.width = 74;
            NPC.height = 54;
            NPC.damage = 2000;//普通模式下的碰撞攻击力
            NPC.defense = 200;//防御力(2防=1点免疫)
            NPC.lifeMax = 10000;//最大生命值
            NPC.HitSound = SoundID.NPCHit4;//受伤音效
            NPC.DeathSound = SoundID.NPCDeath14;//死亡音效
            NPC.value = Item.buyPrice(1, 0, 0, 0);//掉落金钱（普通模式为基准）
            NPC.knockBackResist = 0f;//击退接受性（0为彻底免疫击退）
            NPC.alpha = 0;//透明度，史莱姆为105
            NPC.noGravity = false;//无重力
            NPC.noTileCollide = true;//穿墙
            NPC.boss = false;//令其为boss
            NPC.npcSlots = 1f; //占用1个NPC槽位
            NPC.lavaImmune = true;//免疫岩浆伤害
            NPC.friendly = true;//敌对关系

            //旗帜
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EbonianBlightSlimeBanner>();

            NPC.aiStyle = -1;
            AIType = -1;

            NPC.scale = 1.5f;
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

        bool isClimbing = false;   // 是否正在攀爬墙壁
        int direction = 0;
        public override void AI()
        {
            float initialSpeed = 1.5f; // 初始水平速度
            if (NPC.velocity.X != 0) direction = NPC.velocity.X > 0 ? -1 : 1;

            count++;

            if (count % 12 == 0) Attack();
            if (count % 120 == 0) Attack2();

            // 检查下方是否有物块
            if (!HasGroundBelow())
            {
                // 下方没有物块，允许重力作用
                NPC.noGravity = false;
                isClimbing = false; // 坠落时重置攀爬状态
            }
            else
            {
                // 下方有物块，停止垂直移动
                NPC.noGravity = true;
                NPC.velocity.Y = 0;

                // 检查前方是否有墙壁
                if (HasWallInFront())
                {
                    NPC.velocity.Y -= 2f;
                    // 前方有墙壁，开始以45度角向上移动
                    isClimbing = true;
                    NPC.velocity.X = initialSpeed * direction * -0.7f;
                    NPC.velocity.Y = -initialSpeed * 0.7f;
                }
            }

            if (count > 1200)
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

            if (count > 1200) NPC.velocity *= 0;

            base.AI();
        }

        // 检查下方是否有物块
        private bool HasGroundBelow()
        {
            // 检查NPC底部中心点下方1像素处
            int checkWidth = 16; // 检查宽度(防止从窄平台掉落)
            int centerX = (int)(NPC.position.X + (direction < 0 ? 48 : 48)) / 16;
            int bottomY = (int)(NPC.position.Y + NPC.height + 1) / 16;


            for (int i = 0; i <= 3; i++)
            {
                if (WorldGen.SolidOrSlopedTile(centerX + i, bottomY))
                {
                    return true;
                }
            }
            return false;
        }

        // 检查前方是否有墙壁
        bool HasWallInFront()
        {
            int checkHeight = 8;
            int frontX = (int)(NPC.position.X + (direction < 0 ? 120 : 16)) / 16;

            int baseY = (int)(NPC.position.Y + 12) / 16;

            // 分段检查前方碰撞
            for (int y = 0; y < checkHeight; y++)
            {
                int checkY = baseY + (int)(y * (NPC.height / checkHeight / 16f));

                if (WorldGen.SolidOrSlopedTile(frontX, checkY) ||
                    TileID.Sets.Platforms[Main.tile[frontX, checkY].TileType])
                {
                    return true;
                }
            }
            return false;
        }

        void Attack()
        {
            Vector2 firepos = NPC.Center + new Vector2(0, -12);

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item114, NPC.Center);

            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(NPC), firepos + new Vector2(direction * -42f, 0), new Vector2(direction * -12f, 0).RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)), ModContent.ProjectileType<FinalSpiralfProj>(), 120, 8);

            for (int i = 0; i < 12; i++)
            {
                int num1 = Dust.NewDust(firepos + new Vector2(direction * -42f, 0), 0, 0, DustID.GemAmber, 0f, 0f, 10, Color.Pink, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = new Vector2(direction * -12f, 0).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            }
        }

        void Attack2()
        {
            Vector2 firepos = NPC.Center + new Vector2(0, -18);

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item33, NPC.Center);

            for (int i = 0; i < 8; i++)
            {
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(NPC), firepos + new Vector2(direction * 24f, 0), new Vector2(0, -4).RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<FinalSpiralf2Proj>(), 80, 8);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            if(!target.boss && target.knockBackResist < 1) target.knockBackResist = 1f;

            // 自定义击退逻辑
            float knockback = 20f; // 基础击退力
            float direction = NPC.Center.X < target.Center.X ? -1 : 1; // 击退方向

            // 根据目标类型调整击退
            if (target.boss)
            {
                knockback *= 0f; // 对Boss减半击退
            }

            // 应用击退
            target.velocity.X = knockback * direction;
            target.velocity.Y = -MathHelper.Clamp(knockback * 8f, 2f, 6f); // 轻微向上击退

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
        {//濒死判定

            return false;//空血处理
        }

        public override void OnKill()//在确认被杀后执行的一系列操作，例如推进世界进度，开启某些条件等,死亡效果不是用这个完成的
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
            base.OnKill();
        }


        int drawcount = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawcount++;
            if (drawcount >= 18) drawcount = 2;
            float t = (drawcount % 20) / 20f; // t在 [0, 1) 范围内循环
            Color RedColor = WhiteToRedGradient(t); // 生成渐变色

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralf2DT").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralf2DTG").Value;

            SpriteEffects spriteEffects = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(
                texture01,
                NPC.Center + new Vector2(0, 4) - Main.screenPosition,
                null,
                Color.LightGray * 1f,
                NPC.rotation,
                texture0.Size() / 2,
                NPC.scale,
                spriteEffects,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(
                    texture02,
                    NPC.Center + new Vector2(0, 4) - Main.screenPosition,
                    null,
                    RedColor,
                    NPC.rotation,
                    texture0.Size() / 2,
                    NPC.scale,
                    spriteEffects,
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
