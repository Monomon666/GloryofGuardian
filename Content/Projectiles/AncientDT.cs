using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class AncientDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 32;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 64;

            count0 = 60;

            exdust = DustID.Sand;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -66);
            Lighting.AddLight(Projectile.Center, new Vector3(2.25f, 2.25f, 1.00f));

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 velfire = (target0.Center - AttackPos).SafeNormalize(Vector2.Zero);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, velfire, ModContent.ProjectileType<AncienProj>(), lastdamage, 6, Owner.whoAmI);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 velfire = (target0.Center - AttackPos).SafeNormalize(Vector2.Zero);
                int vec = target0.Center.X > Projectile.Center.X ? 1 : -1;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, velfire, ModContent.ProjectileType<AncienProj2>(), lastdamage, 1, Owner.whoAmI, 0, vec);
            }

            FinishAttack = true;
            return projlist;
        }

        //符文
        int runenul = 0;
        float glow1 = 0;
        float glow2 = 0;
        float glow3 = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientDT2").Value;
            Texture2D textureg1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientDTGlow1").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture0, drawPosition0 + new Vector2(0, -32), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(0, -50) + new Vector2(0, -24), null, lightColor, Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(textureg1, drawPosition0 + new Vector2(0, -32), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
