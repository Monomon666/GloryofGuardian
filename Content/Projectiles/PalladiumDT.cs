using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class PalladiumDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 44;
            Projectile.height = 48;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;

            //本体具有伤害的炮塔，需要设置无敌帧
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.light = 3f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 240;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int Acount = 0;
        int lastdamage = 0;
        int atknum = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //常态下即攻击
            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 1250 * ringscale) {
                    findplayer = true;
                } else findplayer = false;
            }
            Attack();

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 6) * 16);
                        break;
                    }
                }
                drop = false;
            }
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = (int)(Projectile.originalDamage);
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack() {
            Vector2 projcen = DTcenter;
            //发射
            Vector2 velfire = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero) * 16f;

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 1250 * ringscale * 0.62f) {
                    findplayer = true;
                    player.AddBuff(BuffID.RapidHealing, 300);
                    player.AddBuff(BuffID.Lovestruck, 300);
                }
            }
            if (count >= Gcount) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    foreach (Player player in Main.ActivePlayers) {
                        if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 1250 * ringscale * 0.62f) {
                            player.statLife += 5;

                            CombatText.NewText(player.Hitbox,//跳字生成的矩形范围
                                    Color.LightGreen,//跳字的颜色
                                    "5",//这里是你需要展示的文字
                                    false,//dramatic为true可以使得字体闪烁，
                                    false //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                                    );
                        }
                    }

                    //计时重置
                    if (count >= Gcount && atknum == 0) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    foreach (Player player in Main.ActivePlayers) {
                        if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 1250 * ringscale * 0.62f) {
                            if (atknum < 4) {
                                atknum++;
                                player.statLife += 5;

                                CombatText.NewText(player.Hitbox,//跳字生成的矩形范围
                                        Color.LightGreen,//跳字的颜色
                                        "5",//这里是你需要展示的文字
                                        false,//dramatic为true可以使得字体闪烁，
                                        false //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                                        );

                                count -= 5;
                            }
                        }
                    }

                    //计时重置
                    if (count >= Gcount && atknum == 4) {
                        count = Owner.GetModPlayer<GOGModPlayer>().GcountEx + 20;
                        atknum = 0;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        //浮动因子
        int floatcount = 0;
        float float1 = 0;
        float float2 = 0;
        float float3 = 0;
        float float4 = 0;
        float ringscale = 0;
        float ringlastscale = 0;
        Vector2 DTcenter = Vector2.Zero;
        //向绘制的数据传输
        bool findplayer = false;
        public override bool PreDraw(ref Color lightColor) {
            floatcount++;
            float1 = (float)Math.Sin(floatcount / 12f) + 1;
            float2 = (float)Math.Sin(floatcount / 12f + MathHelper.PiOver2) + 1;
            float3 = (float)Math.Cos(floatcount / ((float)Gcount / 2) * MathHelper.Pi - MathHelper.PiOver2);
            float4 = (float)Math.Cos(floatcount / ((float)Gcount / 2) * MathHelper.Pi);

            if (floatcount == 0) ringscale = 0.02f;
            if (floatcount >= 0 && floatcount <= 120) ringscale += 0.002f;
            if (floatcount >= 120 && floatcount <= 240) ringscale += 0.001f;
            if (floatcount >= 240 && floatcount <= 360) ringscale += 0.0003f;
            if (floatcount >= 360 && ringscale < 0.4f) ringscale += 0.0001f;

            ringlastscale = 0.5f;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PalladiumDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PalladiumDT2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PalladiumDT3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PalladiumDT4").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(-2, -8) + new Vector2(0, -2 * float2), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(4, -36) + new Vector2(0, -4 * float1), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale * (float1 * 0.05f + 1), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture3, drawPosition0 + new Vector2(-2, -8) + new Vector2(0, -2 * float2), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;
            DTcenter = drawPosition0 + new Vector2(-0, -48) + new Vector2(0, -4 * float1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(226, 116, 5) * 0.8f, Projectile.rotation, textures.Size() * 0.5f, Projectile.scale * ringscale * ringlastscale * (float1 * 0.01f + 1), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textures2, DTcenter, null, new Color(226, 116, 5) * 0.7f, Projectile.rotation, textures2.Size() * 0.5f, Projectile.scale * ringscale * ringlastscale * (float1 * 0.005f + 1), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            if (findplayer == true) {
                foreach (Player player in Main.ActivePlayers) {
                    if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 1250 * ringscale * 0.62f) {
                        Main.EntitySpriteDraw(texture2, player.Center - Main.screenPosition + new Vector2(4, -36) + new Vector2(0, -12 * float3) + new Vector2(0, -4 * float1), null, lightColor * float4 * 0.8f, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale * (float3 * 0.05f + 1) * 0.8f, SpriteEffects.None, 0);
                    }
                }
            }

            return false;
        }
    }
}