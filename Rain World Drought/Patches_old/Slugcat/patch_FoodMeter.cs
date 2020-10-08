using MonoMod;
using HUD;


[MonoModPatch("global::HUD.FoodMeter")]
class patch_FoodMeter : FoodMeter
{

    [MonoModIgnore]
    public patch_FoodMeter(HUD.HUD hud, int maxFood, int survivalLimit) : base(hud, maxFood, survivalLimit)
    {
    }

    private bool hibernation1 = false;
    private bool hibernation2 = false;

    public extern void orig_ctor(HUD.HUD hud, int maxFood, int survivalLimit);

    [MonoModConstructor]
    public void ctor(HUD.HUD hud, int maxFood, int survivalLimit)
    {
        orig_ctor(hud, maxFood, survivalLimit);
        hibernation1 = false;
        hibernation2 = false;
    }

    private extern void orig_GameUpdate();

    private void GameUpdate()
    {
        if (this.hud.owner is Player)
        {
            if (!hibernation1 && ((this.hud.owner as Player) as patch_Player).uses <= 20)
            {
                if ((this.hud.owner as Player).playerState.foodInStomach >= 1 && (this.hud.owner as Player).abstractCreature.world.game.GetStorySession.saveState.totFood >= 1)
                {
                    if ((this.hud.owner as Player).abstractCreature.world.game.IsStorySession)
                    {
                        (this.hud.owner as Player).AddFood(-1);
                        ((this.hud.owner as Player) as patch_Player).uses += 10;
                    }
                }
                else
                {
                    AddHibernateCost();
                    hibernation1 = true;
                }
            }
            else if (!hibernation2 && ((this.hud.owner as Player) as patch_Player).uses <= 10)
            {
                if ((this.hud.owner as Player).playerState.foodInStomach >= 1 && (this.hud.owner as Player).abstractCreature.world.game.GetStorySession.saveState.totFood >= 1)
                {
                    if ((this.hud.owner as Player).abstractCreature.world.game.IsStorySession)
                    {
                        (this.hud.owner as Player).AddFood(-1);
                        ((this.hud.owner as Player) as patch_Player).uses += 10;
                    }
                }
                else
                {
                    AddHibernateCost();
                    hibernation2 = true;
                }
            }
        }
        orig_GameUpdate();
    }

    public void AddHibernateCost()
    {
        this.survivalLimit++;
        this.RefuseFood();
    }
    private void UpdateShowCount()
    {
        if (this.showCount < this.hud.owner.CurrentFood)
        {
            if (this.showCountDelay == 0)
            {
                this.showCountDelay = 10;
                if (this.showCount >= 0 && this.showCount < this.circles.Count && !this.circles[this.showCount].foodPlopped)
                {
                    this.circles[this.showCount].FoodPlop();
                }
                this.showCount++;
                if (this.quarterPipShower != null)
                {
                    this.quarterPipShower.Reset();
                }
            }
            else
            {
                this.showCountDelay--;
            }
        }
        else if (this.showCount > this.hud.owner.CurrentFood)
        {
            this.circles[this.showCount - 1].EatFade();
            this.showCount--;
        }
    }
    

}

