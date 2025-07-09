//ÊùÎÀÉËº¦ÀàÐÍµÄ×¢²á
namespace GloryofGuardian.Content.Class {
    public class GuardianDamageClass : DamageClass {
        internal static GuardianDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
            if (damageClass == Generic) return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
    }
}