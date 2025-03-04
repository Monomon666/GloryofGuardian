namespace GloryofGuardian.Content.Class

{
    public abstract class GOGCalling : ModItem
    {
        //允许进行当前伤害类型的重铸操作
        public override bool WeaponPrefix() => true;
    }
}