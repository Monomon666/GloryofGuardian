using System;
using System.Collections.Generic;
using System.Linq;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using static GloryofGuardian.Common.GOGUtils;

namespace GloryofGuardian.Content.Projectiles {
    public class CobaltDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 60;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;
            Projectile.light = 2;

            OtherHeight = 34;

            count0 = 120;
            exdust = DustID.FrostHydra;

            TurnCenter = new Vector2(-4, -44);

            //过近筛除,枪管设计,不攻击过近的敌人
            closeignoredistance = 64;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 AttackPos2 = Vector2.Zero;
        int Recoil = 0;
        bool crit = false;

        int modecount = 0;
        float projrot = 0;
        float projrotvel = 0;

        int hitnum = 0;
        NPC target2 = null;
        NPC target3 = null;
        List<int> ignore1 = [];
        List<int> ignore2 = [];
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -56);
            AttackPos2 = Projectile.Center + new Vector2(-6, -38);

            Lighting.AddLight(AttackPos, new Vector3(1.32f, 2.34f, 2.55f));

            projrot += projrotvel;

            hitnum = 1;

            target2 = null;
            target3 = null;
            ignore1.Clear();
            ignore2.Clear();
            if (target0 != null && target0.active) {
                ignore1.Add(target0.whoAmI);
                target2 = AttackPos.InPosClosestNPC(Attackrange, closeignoredistance, ignoretile, findboss, ignore1);
                hitnum = 1;
            }
            if (target2 != null && target2.active) {
                ignore2.Add(target0.whoAmI);
                ignore2.Add(target2.whoAmI);
                target3 = AttackPos.InPosClosestNPC(Attackrange, closeignoredistance, ignoretile, findboss, ignore2);
                hitnum = 2;
            }
            if (target3 != null && target3.active) hitnum = 3;

            //蓄力状态，刚召唤或攻击后来到这个状态
            //在该状态下，刃叶旋转重新加速
            if (mode == 0) {
                modecount++;
                projrotvel = ((float)modecount / Gcount) * 0.2f;
                if (projrotvel >= 0.05) Projectile.localNPCHitCooldown = 44;
                if (projrotvel >= 0.1) Projectile.localNPCHitCooldown = 36;
                if (projrotvel >= 0.15) Projectile.localNPCHitCooldown = 26;
                if (projrotvel >= 0.2) Projectile.localNPCHitCooldown = 14;

                if (modecount >= Gcount) {
                    mode = 1;
                    projrotvel = 0.2f;
                    count = 0;
                    modecount = 0;
                    Projectile.localNPCHitCooldown = 12;
                }
            }

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < hitnum; i++) {
                if (i == 0) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, AttackPos.Toz(target0.Center) * Main.rand.NextFloat(8, 12), ModContent.ProjectileType<CobaltProj>(), lastdamage, 2, Owner.whoAmI, 0, target0.whoAmI);
                                                                                                                                                                          
                    projlist.Add(proj1);                                                                                                                                  
                }                                                                                                                                                         
                if (i == 1) {                                                                                                                                             
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);                                                                              
                    Projectile proj2 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, AttackPos.Toz(target2.Center) * Main.rand.NextFloat(8, 12), ModContent.ProjectileType<CobaltProj>(), lastdamage, 2, Owner.whoAmI, 0, target2.whoAmI);
                                                                                                                                                                          
                    projlist.Add(proj2);                                                                                                                                  
                }                                                                                                                                                         
                if (i == 2) {                                                                                                                                             
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);                                                                              
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, AttackPos.Toz(target3.Center) * Main.rand.NextFloat(8, 12), ModContent.ProjectileType<CobaltProj>(), lastdamage, 2, Owner.whoAmI, 0, target3.whoAmI);

                    projlist.Add(proj3);
                }
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, AttackPos.Toz(target0.Center) * 16f, ModContent.ProjectileType<CobaltProj>(), lastdamage, 2, Owner.whoAmI, 1, target0.whoAmI);

                projlist.Add(proj1);
            }

            mode = 0;
            FinishAttack = true;
            return projlist;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -25);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj").Value;
            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProjBlackShadow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -53);

            Main.EntitySpriteDraw(textures, drawPosition, null, new Color(132, 234, 255, 0) * (projrotvel * 6 - 0.2f), projrot, textures.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor * (projrotvel * 6 - 0.2f), projrot, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            return false;
        }
    }

    public class CobaltProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;

            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.localNPCHitCooldown = 12;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.light = 2f;

            Projectile.tileCollide = false;

            if (Projectile.ai[0] == 0) Projectile.width = Projectile.height = 16;
            if (Projectile.ai[0] == 1) Projectile.width = Projectile.height = 96;

            firenum = 6;

            ignorenpc.Add((int)Projectile.ai[2]);

            base.SetProperty();
        }

        NPC targetnext = null;
        List<NPC> ignore = [];
        List<int> ignoreint = [];

        int firenum = 0;
        public override void AI() {
            if (Projectile.ai[0] == 0) {
                Projectile.rotation += 0.1f;

                if (targetnext == null || !targetnext.active) targetnext = Projectile.Center.InPosClosestNPC(800, 0, false, true, ignoreint);

                if (mode == 0) {
                    targetnext = Projectile.Center.InPosClosestNPC(800, 0, false, true, ignoreint);

                    Projectile.localNPCHitCooldown = 30;
                    Projectile.penetrate = 5;
                    Projectile.extraUpdates = 2;
                    Projectile.width = 16;
                    Projectile.height = 16;
                }

                if (mode == 1) {
                    if (drawcount >= hitcount && hitcount != 0) {
                        hitcount = 0;
                        mode = 2;
                    }
                }

                if (mode == 2) {
                    if (targetnext != null && targetnext.active) {
                        Projectile.velocity = Projectile.Center.Toz(targetnext.Center) * Main.rand.NextFloat(8, 12);
                        mode = 3;
                    }
                }

                if (mode == 3) {
                    if (targetnext == null || !targetnext.active) {
                        mode = 2;
                    }
                }
            }

            if (Projectile.ai[0] == 1) {
                if (drawcount > 180) Projectile.Kill();
                Projectile.width = Projectile.height = 96;

                if(!ignorenpc.Contains((int)Projectile.ai[2])) ignorenpc.Add((int)Projectile.ai[2]);

                if (firenum > 0 && target0 != null && target0.active && drawcount % 30 == 0) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(-6, -12), (Projectile.Center + new Vector2(-6, -12)).Toz(target0.Center) * Main.rand.NextFloat(8, 12), ModContent.ProjectileType<CobaltProj>(), Projectile.damage, 2, Owner.whoAmI, 0, target0.whoAmI);
                    firenum -= 1;
                }

                if (mode == 0) {
                    Projectile.rotation += 0.2f;

                    //粒子效果
                    for (int i = 0; i <= 32; i++) {
                        float cirrot = (i % 4) * MathHelper.PiOver2;

                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -12)
                            + new Vector2((float)Math.Cos(Projectile.rotation + cirrot), (float)Math.Sin(Projectile.rotation + cirrot)) * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 0.8f);
                        dust1.velocity = Projectile.velocity;
                        dust1.noGravity = true;
                    }
                }

                if (mode == 1) {
                    Projectile.rotation += 0.3f;
                    Projectile.velocity *= 0.8f;

                    if (Projectile.ai[0] == 0) {
                        if (drawcount % 40 == 0 && Main.rand.NextBool(2)) {
                            //Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(-6, -12), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero) * 4f, ModContent.ProjectileType<CobaltProj2>(), Projectile.damage, 2, Owner.whoAmI, 0);
                        }

                        if (drawcount > 120) Projectile.Kill();
                    }
                }

                //粒子效果
                if (mode == 0) {
                    for (int i = 0; i <= 12; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -4)
                            + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 1f);
                        dust1.velocity = Projectile.velocity;
                        dust1.noGravity = true;
                    }
                }

                if (mode == 1) {
                    for (int i = 0; i <= 2; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -4)
                            + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 0.6f);
                        dust1.velocity *= 0;
                        dust1.noGravity = true;
                    }

                    for (int i = 0; i <= 12; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -4)
                            + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 1f);
                        dust1.velocity = (dust1.position - (Projectile.Center + new Vector2(-6, -4))).SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver2) * 4f;
                        dust1.noGravity = true;
                    }
                }
            }
            
            base.AI();
        }

        int hitcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 0) {
                ignoreint.Add(target.whoAmI);
                targetnext = null;

                if (hitcount == 0) {
                    mode = 1;
                    hitcount = drawcount + 10;
                    Projectile.velocity *= 0.01f;
                }
            }

            if (Projectile.ai[0] == 1) mode = 1;
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.ai[0] == 0) {
                for (int i = 0; i <= 40; i++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.FrostHydra, 0f, 0f, 50, Color.White, 0.5f);
                    Main.dust[num].velocity *= 2f;
                    Main.dust[num].noGravity = true;
                    if (Main.rand.NextBool(2)) {
                        Main.dust[num].scale = 1f;
                        Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj2").Value;
            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProjBlackShadow").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj2BlackShadow").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            if (Projectile.ai[0] == 0) {
                Main.EntitySpriteDraw(textures2, drawPos, null, new Color(132, 234, 255, 0), Projectile.rotation, textures2.Size() * 0.5f, Projectile.scale * 1f * 1.2f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture2, drawPos, null, Color.White, Projectile.rotation, textures2.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0);

                //////////////////拖尾
                {
                    //注入
                    List<VertexInfo2> vertices3 = new List<VertexInfo2>();
                    for (int i = 0; i < Projectile.oldPos.Length; i++) {
                        if (Projectile.oldPos[i] != Vector2.Zero) {
                            vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(8, 8) + new Vector2((30 - 1 - i) / 2, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)),
                            new Vector3(i / 8f, 0, 1 - (i / 8f)), new Color(132, 234, 255) * (1 - 0.06f * i)));
                            vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(8, 8) + new Vector2((30 - 1 - i) / 2, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(90)),
                            new Vector3(i / 8f, 1, 1 - (i / 8f)), new Color(132, 234, 255) * (1 - 0.06f * i)));
                        }
                    }

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                    if (vertices3.Count >= 3) {
                        for (int i = 0; i < 4; i++) {
                            Main.graphics.GraphicsDevice.Textures[0] = texture3;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
                        }
                    }

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    //切回原画布
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    vertices3.Clear();
                }
            }

            if (Projectile.ai[0] == 1) {
                Main.EntitySpriteDraw(textures, drawPos, null, new Color(132, 234, 255, 0) * 0.6f, Projectile.rotation, textures.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, textures.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}
