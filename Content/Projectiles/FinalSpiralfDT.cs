using GloryofGuardian.Common;
using GloryofGuardian.Content.Classes;
using GloryofGuardian.Content.Dusts;
using GloryofGuardian.Content.Items.Weapon;
using GloryofGuardian.Content.Projectiles.ProjNPC;
using GloryofGuardian.Skies;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace GloryofGuardian.Content.Projectiles
{
    public class FinalSpiralfDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 132;
            Projectile.height = 70;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 6;//默认发射间隔
            interval = 5;//多重攻击间隔长度
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int interval = 0;

        int countank = 0;
        int countUAV = 0;

        bool hereboss = false;
        int countfinal = 0;
        int final = 0;
        bool shield = false;
        //重力
        bool drop = true;
        //炮台转动
        float wrotation = 0;
        float projRot = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //重置判定
        bool firstatk = false;
        //帧
        int frame1 = 0;
        int frame2 = 0;
        //状态机
        int mode = 0;
        public override void AI() {
            count++;
            countank++;
            countUAV++;

            shield = false;

            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);

            if (target1 != null && !target1.active) target1 = null;

            if (target1 != null && final == 0) {
                if (mode == 0) {
                    Attack(target1);
                }

                Turn(target1);
                Turn(target1);
            }

            //boss检测
            hereboss = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].boss)
                {
                    hereboss = true;
                }
            }

            //无boss重置
            if(!hereboss) {
            countfinal = 0;
            if(final > 0)
            {
            
            }
            }

            //boss存活
            if (target1 != null && target1.boss)
            {
                countfinal++;

                //粒子展现
                for (int i = 0; i <= 54; i++)
                {
                    Dust dust1 = Dust.NewDustDirect(firepos + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 1200, 8, 8, DustID.PinkFairy, 1f, 1f, 100, Color.Pink, 0.8f);
                    dust1.velocity = dust1.position.Toz(firepos) * 4f;
                    dust1.noGravity = true;

                    Dust dust2 = Dust.NewDustDirect(firepos + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 1200, 8, 8, DustID.PinkFairy, 1f, 1f, 100, Color.Pink, 0.8f);
                    dust2.velocity = dust2.position.Toz(firepos) * 0.5f;
                    dust2.noGravity = true;
                }

                if (Vector2.Distance(Owner.Center, Projectile.Center) < 1200)
                {
                    Owner.statDefense += 100;
                    shield = true;
                }

                if (final == 0 && countfinal > 300)
                {
                    countfinal = 0;
                    final = 1;
                }
            }

            //蓄能
            //动画
            if (final == 1)
            {
                Turn2(firepos + new Vector2(1, -600));
                if (countfinal > 520)
                {
                    final = 2;
                }
            }

            //发射0
            if (final == 2)
            {
                if (Owner.HeldItem.type == ModContent.ItemType<FinalSpiralfCalling>() && Owner.PressKey(true, true))
                {
                    final = 3;
                    countfinal = 0;
                }
            }

            //发射
            if (final == 3)
            {
                if (countfinal == 2) Terraria.Audio.SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);

                //加载天空
                if (countfinal > 2)
                {
                    //加载天空
                    SkyManager.Instance.Activate("LightPinkSky");//激活我们注册的天空
                    if (!SkyManager.Instance["LightPinkSky"].IsActive())//如果这个天空没激活
                        SkyManager.Instance.Activate("LightPinkSky");
                    ((LightPinkSky)SkyManager.Instance["LightPinkSky"]).Timeleft = 2;//之后每帧赋予这个倒计时2，如果npc不在了，天空自动关闭
                    //天空逐渐开启
                    if (countfinal <= 20) GloryofGuardianMod.Instance.skycount = (20 - countfinal) / 20f;
                    if (countfinal > 20 && countfinal <= 100) GloryofGuardianMod.Instance.skycount = 0;
                    if (countfinal > 100 && countfinal < 120) GloryofGuardianMod.Instance.skycount = (countfinal - 100) / 20f;
                }

                for (int j = 0; j < 500; j++)
                {
                    int num2 = Dust.NewDust(firepos + new Vector2(0, -68) + new Vector2(0, Main.rand.NextFloat(0, -1200)), 0, 0, ModContent.DustType<FireLightDust>(), 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f));
                }

                for (int j = 0; j < 50; j++)
                {
                    int num2 = Dust.NewDust(firepos + new Vector2(0, -68) + new Vector2(Main.rand.NextFloat(-16, 16), Main.rand.NextFloat(0, -1200)), 0, 0, DustID.FireworkFountain_Red, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2(0, -16).RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f));
                }

                if (countfinal > 120)
                {
                    final = 4;
                    countfinal = 0;
                }
            }

            //收回
            if (final == 4)
            {
                if (countfinal >= 160)
                {
                    final = 0;
                    countfinal = 0;
                    drawcircount = 0;
                }
            }

            if (countank >= 480 && target1 != null)
            {
                int numtank = 0;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == ModContent.NPCType<FinalSpiralf2DT>())
                    {
                        numtank += 1;
                    }
                }

                if (numtank == 1)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.type == ModContent.NPCType<FinalSpiralf2DT>())
                        {
                            npc.life = 0;
                            npc.active = false;
                        }
                    }
                    numtank -= 1;
                }

                if (numtank == 0)
                {
                    for (int i = -1; i < 2; i+=2)
                    {
                        int type0 = ModContent.NPCType<SpiderNPC>();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int x = (int)Projectile.Center.X;
                            int y = (int)Projectile.Center.Y;
                            int npc1 = NPC.NewNPC(new EntitySource_ItemUse(Owner, Item), x, y, ModContent.NPCType<FinalSpiralf2DT>());
                            Main.npc[npc1].velocity = new Vector2(1.5f * i, 0);
                        }
                        else
                        {
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Owner.whoAmI, number2: type0);//发包，用来联机同步
                        }
                    }
                }

                countank = 0;
            }

            if (countUAV >= 60 && target1 != null)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    int type0 = ModContent.NPCType<SpiderNPC>();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int x = (int)Projectile.Center.X;
                        int y = (int)Projectile.Center.Y;
                        int npc1 = NPC.NewNPC(new EntitySource_ItemUse(Owner, Item), x, y, ModContent.NPCType<FinalSpiralf3DT>());
                        Main.npc[npc1].velocity = new Vector2(1.5f * i, 0);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Owner.whoAmI, number2: type0);//发包，用来联机同步
                    }
                }

                countUAV = 0;
            }

            //帧图
            Projectile.frameCounter++;
            frame1 = (int)MathHelper.Min(Projectile.frameCounter / 8, 8);
            if (final == 1 && countfinal > 180)
            {
                frame2 = (int)MathHelper.Min((countfinal - 180) / 8, 16);
            }
            if (final == 2 || final == 3)
            {
                frame2 = 16;
            }
            if (final == 4)
            {
                frame2 = (int)MathHelper.Max(16 - countfinal / 8, 0);
            }

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
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = firepos;
            //发射
            if (count + interval >= Gcount && !firstatk) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item33, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 68f, nowvel * 48f, ModContent.ProjectileType<FinalSpiralfProj>(), lastdamage, 8, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item33, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 68f, nowvel * 48f, ModContent.ProjectileType<FinalSpiralfProj>(), lastdamage, 8, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //前置攻击完成
                firstatk = true;
            }

            //发射
            if (count >= Gcount) {

                //计时重置,通过更改这个值来重置攻击
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                firstatk = false;
            }
        }

        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 3);
            Vector2 projcen = firepos;

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.04f;

            //转头
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
                    wrotation = tarrot;//那么直接让方向与到目标方向防止抖动
                                       //tarrot = wrotation;
                    return;
                } else {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
                                                                                     //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//如果顺时针的差值更小
                    {
                        wrotation += rspeed;
                    } else {
                        wrotation -= rspeed;
                    }
                }
            }
        }

        /// <summary>
        /// 炮台旋转2
        /// </summary>
        void Turn2(Vector2 vec)
        {
            Vector2 tarpos = vec;
            Vector2 projcen = firepos;

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.04f;

            //转头
            if (wrotation != tarrot)
            {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed)
                {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
                    wrotation = tarrot;//那么直接让方向与到目标方向防止抖动
                                       //tarrot = wrotation;
                    return;
                }
                else
                {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
                                                                                     //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//如果顺时针的差值更小
                    {
                        wrotation += rspeed;
                    }
                    else
                    {
                        wrotation -= rspeed;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (final == 3)
            {
                return true;
            }
            else return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        int drawcount = 0;
        int drawcount2 = 0;
        int drawcircount = 0;
        float float2 = 0;
        Vector2 firepos = Vector2.Zero;
        float breath = 0;
        public override bool PreDraw(ref Color lightColor) {
            GloryofGuardianMod.Instance.skycount += 0.02f;

            drawcount++;
            if (drawcount < 30 || drawcount >= 60) drawcount++;
            if (drawcount >= 80) drawcount = 10;
            float t = (drawcount % 90) / 90f; // t在 [0, 1) 范围内循环
            Color RedColor = WhiteToRedGradient(t); // 生成渐变色

            drawcount2++;
            float t2 = (drawcount2 % 60) / 60f; // t在 [0, 1) 范围内循环
            Color RedColor2 = WhiteToRedGradient(t2); // 生成渐变色

            breath = (float)Math.Sin((int)Main.GameUpdateCount / 18f);

            float2 = (float)Math.Sin(drawcount / 12f + MathHelper.PiOver2) + 1;

            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT").Value;
            Texture2D texture012 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDTG").Value;
            Texture2D texture013 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDTG2").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2").Value;
            Texture2D texture022 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2G").Value;
            Texture2D texture023 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2G2").Value;
            Texture2D texture024 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2G3").Value;

            Texture2D shieldtexture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "LightPulse").Value;

            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusProj01").Value;

            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition;

            int singleFrameY = texture01.Height / 9;
            Vector2 drawPos1 = Projectile.Center - Main.screenPosition + new Vector2(98, 36);

            Main.spriteBatch.Draw(
                texture01,
                drawPos1,
                new Rectangle(0, singleFrameY * frame1, texture01.Width, singleFrameY),//动图读帧
                lightColor,
                Projectile.rotation,
                new Vector2(132, 70),
                Projectile.scale * 1.5f,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < 2; i++) {
                if (Projectile.frameCounter > 64) {
                    Main.spriteBatch.Draw(
                    texture013,
                    drawPos1,
                    null,
                    RedColor,
                    Projectile.rotation,
                    texture013.Size(),
                    Projectile.scale * 1.5f,
                    SpriteEffects.None,
                    0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            int singleFrameY2 = texture02.Height / 18;
            Vector2 drawPos2 = Projectile.Center - Main.screenPosition + new Vector2(-12, 48);

            firepos = Projectile.Center + new Vector2(-5, -19);
            Lighting.AddLight(firepos, 255 * 0.05f, 20 * 0.05f, 20 * 0.05f);
            Lighting.AddLight(firepos, 255 * 0.01f, 255 * 0.01f, 255 * 0.01f);

            Main.spriteBatch.Draw(
                texture02,
                drawPos2 + new Vector2(10, -68),
                new Rectangle(0, singleFrameY2 * frame2, texture02.Width, singleFrameY2),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                wrotation,
                new Vector2(28, 18),
                Projectile.scale * 1.5f,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < 2; i++) {
                if (Projectile.frameCounter > 64) {
                    Main.spriteBatch.Draw(
                        texture022,
                        drawPos2 + new Vector2(10, -68),
                        new Rectangle(0, singleFrameY2 * frame2, texture02.Width, singleFrameY2),//动图读帧
                        RedColor * 0.4f,
                        wrotation,
                        new Vector2(28, 18),
                        Projectile.scale * 1.5f,
                        SpriteEffects.None,
                        0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (shield)
            {
                Vector2 shieldpos = Owner.Center + new Vector2(0, 0) - Main.screenPosition;
                Main.EntitySpriteDraw(shieldtexture, shieldpos, null, Color.Red, Projectile.rotation, shieldtexture.Size() * 0.5f, 1.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shieldtexture, shieldpos, null, RedColor2, Projectile.rotation, shieldtexture.Size() * 0.5f, 1.6f, SpriteEffects.None, 0);
            }

            /////////////////////////////////////////////////////////

            float rs = drawcount / 30f;
            List<VertexInfo2> vertices = new List<VertexInfo2>();
            float FlipRotation = 0f;
            float rotcount = drawcount / 30f;

            if ((final == 1 && countfinal > 340))
            {
                if(drawcircount < 100) drawcircount++;
                float v0 = 4 - drawcircount / 33f;

                //大环
                Vector2 v2 = 
                    new Vector2(-300, -300).RotatedBy(rotcount * 2) * v0;
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 0, 1), Color.Red * (drawcircount / 100f)));
                //记录纹理左上角的顶点坐标
                v2 = new Vector2(300, -300).RotatedBy(rotcount * 2) * v0;
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 0, 1), Color.Red * (drawcircount / 100f)));
                //记录纹理右上角的顶点坐标
                v2 = new Vector2(-300, 300).RotatedBy(rotcount * 2) * v0;
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 1, 1), Color.Red * (drawcircount / 100f)));
                //记录纹理左下角的顶点坐标
                v2 = new Vector2(300, 300).RotatedBy(rotcount * 2) * v0;
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 1, 1), Color.Red * (drawcircount / 100f)));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                if (vertices.Count >= 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Main.graphics.GraphicsDevice.Textures[0] = texture2;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                    }
                }
                vertices.Clear();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            if (final == 2 || final == 3)
            {
                drawcircount = 100;

                //大环
                Vector2 v2 = 
                    new Vector2(-300, -300).RotatedBy(rotcount * 2);
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 0, 1), Color.Red * (drawcircount / 100f)));
                //记录纹理左上角的顶点坐标
                v2 = new Vector2(300, -300).RotatedBy(rotcount * 2);
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 0, 1), Color.Red * (drawcircount / 100f)));
                //记录纹理右上角的顶点坐标                                                         
                v2 = new Vector2(-300, 300).RotatedBy(rotcount * 2);                           
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 1, 1), Color.Red * (drawcircount / 100f)));
                //记录纹理左下角的顶点坐标                                                         
                v2 = new Vector2(300, 300).RotatedBy(rotcount * 2);                            
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 1, 1), Color.Red * (drawcircount / 100f)));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                if (vertices.Count >= 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Main.graphics.GraphicsDevice.Textures[0] = texture2;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                    }
                }
                vertices.Clear();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            }

            return false;
        }

        Vector2 Drwapos2(Vector2 v1, float FlipRotation)
        {
            return Projectile.Center - Main.screenPosition + new Vector2(0, -96) + new Vector2(0, -2) * breath + new Vector2(v1.X / 6, v1.Y / 16).RotatedBy(FlipRotation);
        }

        //Shader结构体
        public struct VertexInfo2 : IVertexType
        {
            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
            });
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;
            public VertexInfo2(Vector2 position, Vector3 texCoord, Color color)
            {
                Position = position;
                TexCoord = texCoord;
                Color = color;
            }
            public VertexDeclaration VertexDeclaration
            {
                get => _vertexDeclaration;
            }
        }

        // 生成白色和红色之间的渐变色
        Color WhiteToRedGradient(float t) {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            if (t < 0.333f) {
                // 阶段1：黑色(0,0,0) → 红色(1,0,0)
                float r = t * 3f;    // 红色通道从0升到1
                return new Color(r, 0f, 0f);
            } else if (t < 0.666f) {
                // 阶段2：红色(1,0,0) → 白色(1,1,1)
                float gb = (t - 0.333f) * 3f; // 绿蓝通道从0升到1
                return new Color(1f, gb, gb);
            } else {
                // 阶段3：白色(1,1,1) → 黑色(0,0,0)
                float rgb = 1f - (t - 0.666f) * 3f; // 所有通道从1降到0
                return new Color(rgb, rgb, rgb);
            }
        }
    }
}