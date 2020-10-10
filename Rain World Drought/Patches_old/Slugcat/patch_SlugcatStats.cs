using MonoMod;

class patch_SlugcatStats : SlugcatStats
{
    [MonoModIgnore]
    public patch_SlugcatStats(int slugcatNumber, bool malnourished) : base(slugcatNumber, malnourished)
    {
    }

    public void AddHibernationCost()
    {
        this.foodToHibernate++;
    }
}
