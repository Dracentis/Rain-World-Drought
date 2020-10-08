using HUD;
using RWCustom;
using UnityEngine;
using MonoMod;


[MonoModPatch("global::HUD.HUDCircle")]
class patch_HUDCircle : HUDCircle
{
    [MonoModIgnore]
    public patch_HUDCircle(HUD.HUD hud, SnapToGraphic snapGraphic, FContainer container, int color) : base(hud, snapGraphic, container, color)
    {
    }

    public bool danger = false;

    public void Draw(float timeStacker)
    {
        Vector2 vector = Vector2.Lerp(this.lastPos, this.pos, timeStacker);
        float num = Mathf.Lerp(this.lastRad, this.rad, timeStacker);
        float num2 = Mathf.Lerp(this.lastThickness, this.thickness, timeStacker);
        if (num <= 0f || !this.visible || (this.lastFade == 0f && this.fade == 0f))
        {
            this.sprite.isVisible = false;
            return;
        }
        this.sprite.isVisible = true;
        if (num2 > num)
        {
            num2 = num;
        }
        this.sprite.x = vector.x;
        this.sprite.y = vector.y;
        if (num == this.snapRad && num2 == this.snapThickness)
        {
            this.sprite.element = Futile.atlasManager.GetElementWithName(this.snapGraphic.ToString());
            this.sprite.scale = 1f;
            this.sprite.alpha = 1f;
            this.sprite.shader = this.basicShader;
            this.sprite.alpha = Mathf.Lerp(this.lastFade, this.fade, timeStacker);
            this.sprite.color = Custom.FadableVectorCircleColors[this.color];
            if (danger)
            {
                this.sprite.color = new Color(1f, 0f, 0);
            }
        }
        else
        {
            this.sprite.element = Futile.atlasManager.GetElementWithName("Futile_White");
            this.sprite.scale = num / 8f;
            if (num2 == -1f)
            {
                this.sprite.alpha = 1f;
            }
            else if (num > 0f)
            {
                this.sprite.alpha = num2 / num;
            }
            else
            {
                this.sprite.alpha = 0f;
            }
            this.sprite.shader = this.circleShader;
            this.sprite.color = new Color((float)this.color / 255f, 0f, Mathf.Lerp(this.lastFade, this.fade, timeStacker));
        }
    }
}
