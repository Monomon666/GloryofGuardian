using System.Collections.Generic;
using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class Bloody2Proj1 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 45;
            Projectile.height = 69;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            Projectile.scale *= 0.7f;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.5f);

            //临时遵循传入的重力的影响
            if (count < 4) {
                float G = Projectile.ai[0];
                Projectile.velocity += new Vector2(0, G);
            }
            //追踪并加速
            if (count >= 4) {
                if (target0 != null && target0.active) {
                    Projectile.velocity *= 0.9f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * Main.rand.NextFloat(2f, 2.5f);
                    if (Vector2.Distance(Projectile.Center, target0.Center) < 64) {
                        Projectile.velocity *= 0.7f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * 3;
                    }
                }
            }

            if (count >= 4) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), Main.rand.NextBool(16) ? DustID.Ichor : DustID.Crimson, 0f, 0f, 50, Color.White, 1.2f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].velocity -= Projectile.velocity / 4;
                    Main.dust[num].noGravity = true;

                    if (Main.rand.NextBool(2)) {
                        Main.dust[num].scale = 0.5f;
                        if (count >= 16) Main.dust[num].velocity *= 2f;
                    }
                    if (Main.rand.NextBool(8)) {
                        Main.dust[num].noGravity = false;
                    }

                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            //指向的
            Attack(false, target.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //垂直的
            Vector2 vec = Vector2.Zero;
            //碰撞竖直墙壁
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                vec = (oldVelocity.X > 0) ? new Vector2(-1, 0) : new Vector2(1, 0); 
            }
            //碰撞水平地面
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                vec = (oldVelocity.Y > 0) ? new Vector2(0, -1) : new Vector2(0, 1);
            }

            if(vec != Vector2.Zero)Attack(true, vec);
            return base.OnTileCollide(oldVelocity);
        }

        /// <summary>
        /// 生长血荆棘
        /// </summary>
        /// <param name="velorpos">true为垂直型,直接使用注入的vel;false为导向型,从弹幕尾部指向vek</param>
        /// <param name="vel"></param>
        void Attack(bool velorpos, Vector2 vel) {
            //在目标点范围内寻找最近的实心物块,并返回它们的集合
            Vector2 cen = velorpos ? Projectile.Center : vel;
            List<Vector2> tileCoordsList = TileHelper.FindTilesInRectangle(cen, 6, 6, 0, 8, true);
            if (tileCoordsList != null && tileCoordsList.Count != 0) {
                // 随机确定要返回的元素数量(2~3个)，但不超过列表的总元素数
                Random random = new Random();
                int countToReturn = Math.Min(random.Next(2, 4), tileCoordsList.Count);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                for (int i = 0; i < countToReturn; i++) {
                    int randomIndex = random.Next(tileCoordsList.Count);
                    // 获取选中的元素,向下偏移一格多
                    Vector2 selectedTile = tileCoordsList[randomIndex] + new Vector2(0, 16);
                    // 对选中的元素执行A()方法
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), selectedTile,
                    velorpos ? vel : selectedTile.Toz(vel)
                        , ModContent.ProjectileType<Bloody2Proj2>(), Projectile.damage, 8, Owner.whoAmI);
                }
            }
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            for (int j = 0; j < 25; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.Red, 1.7f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
                num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10);
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2Proj1").Value;

            Main.EntitySpriteDraw(texture,Projectile.Center - Main.screenPosition,null,lightColor,Projectile.rotation,texture.Size() / 2,Projectile.scale,SpriteEffects.None,0);

            return false;
        }
    }
}
