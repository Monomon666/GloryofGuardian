using System.Collections.Generic;
using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.NPCs.GuardNPCs {
    public class TitaniumDTN : GOGGuardNPC {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetProperty() {
            NPC.lifeMax = 400;

            if (Main.masterMode)
                NPC.lifeMax *= 3;
            else if (Main.expertMode)
                NPC.lifeMax *= 2;

            NPC.width = 30;
            NPC.height = 50;

            // NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.aiStyle = -1;

            NPC.noGravity = false;//无重力
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;//死亡音效

            count0 = 30;
        }

        public override void AI() {
            AttackPos = NPC.Center + new Vector2(0, 0);

            base.AI();
        }

        protected override void Attack1() {


            FinishAttack = true;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
            base.OnHitByItem(player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN4").Value;
            return false;
        }
    }
}
