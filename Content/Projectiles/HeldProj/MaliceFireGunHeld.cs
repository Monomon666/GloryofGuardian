using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GloryofGuardian.Content.Projectiles.HeldProj
{
    public class MaliceFireGunHeld : GOGProj
    {
        //有目标转移对玩家方向、手持弹幕指向的处理的典例
        public override string Texture => GOGConstant.Weapons + "MaliceFireGun";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SetDefaults() {
            Projectile.width = 78;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10000;//消散时间
            Projectile.aiStyle = -1;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12; // 只打一次
            Projectile.ignoreWater = true;
            Projectile.light = 1;

            Projectile.scale *= 1f;
        }
        Player Owner => Main.player[Projectile.owner];
        Vector2 ToMou => Main.MouseWorld - Owner.Center;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        bool hadatk = false;
        public override void AI() {
            //计时器
            count++;
            // 使用动画延长到使用完
            Owner.itemAnimation = Owner.itemTime = 2;
            //跟着主人
            Projectile.Center = Owner.Center;
            //改变玩家朝向
            Owner.direction = (Main.MouseWorld - Owner.Center).X > 0 ? 1 : -1;
            // （玩家被）打断的效果
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.Kill();
                return;
            }
            // 客户端运行以确保多人联机同步
            if (Projectile.owner == Main.myPlayer) {
                Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                UpdateAim(rrp);
                Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            //弹幕转向
            int dir = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.Center - Projectile.velocity * 24f;
            Projectile.timeLeft = 18000;
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * -Owner.direction).ToRotation();

            //伤害矫正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(100) / 100f;
            int lastdamage = (int)(newDamage * rangedOffset);
            //设置手臂位置
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            //手持弹幕认定
            Owner.heldProj = Projectile.whoAmI;
            //松手则消除
            if (!Owner.PressKey()) {
                Projectile.Kill();
            }

            if (count >= 2) {
                //发射
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item34);

                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 projcen = Projectile.Center + new Vector2(0, 0);
                        Vector2 tomou = projcen.Toz(Main.MouseWorld);

                        float rot = Main.rand.NextFloat(-0.05f, 0.1f);
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 6f;

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + tomou * 68f, tomou.RotatedBy(rot) * vel, ModContent.ProjectileType<MaliceFireGunFireProj>(), lastdamage, 2, Owner.whoAmI, Projectile.ai[1]);

                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateAim(Vector2 source) {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), 0.1f));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        public override bool? CanHitNPC(NPC target) {
            return null;
        }

        public override bool CanHitPlayer(Player target) {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Weapons + "MaliceFireGun").Value;

            Vector2 newOrg = Projectile.spriteDirection > 0 ? new Vector2(0, texture1.Height / 2) : new Vector2(texture1.Width / 2, texture1.Height / 2);
            SpriteEffects spriteEffects = ToMou.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(
            texture1,
            Projectile.Center - Main.screenPosition + new Vector2(0, 8),
            new Rectangle(0, 0, texture1.Width, texture1.Height),
            lightColor,
            Projectile.rotation,
            newOrg,
            Projectile.scale,
            spriteEffects,
            0
            );

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            overPlayers.Add(index);
        }
    }
}