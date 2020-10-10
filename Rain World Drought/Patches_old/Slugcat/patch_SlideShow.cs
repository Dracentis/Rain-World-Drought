using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Menu;
using MonoMod;

[MonoModPatch("global::Menu.SlideShow")]
class patch_SlideShow : Menu.SlideShow
{
    [MonoModIgnore]
    public patch_SlideShow(ProcessManager manager, SlideShow.SlideShowID slideShowID) : base(manager, slideShowID)
    {
    }

    public extern void orig_ctor(ProcessManager manager, SlideShow.SlideShowID slideShowID);

    [MonoModConstructor]
    public void ctor(ProcessManager manager, SlideShow.SlideShowID slideShowID)
    {
        Type[] constructorSignature = new Type[2];
        constructorSignature[0] = typeof(ProcessManager);
        constructorSignature[1] = typeof(ProcessManager.ProcessID);
        RuntimeMethodHandle handle = typeof(Menu.Menu).GetConstructor(constructorSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<ProcessManager, ProcessManager.ProcessID> funct = (Action<ProcessManager, ProcessManager.ProcessID>)Activator.CreateInstance(typeof(Action<ProcessManager, ProcessManager.ProcessID>), this, ptr);

        if (slideShowID != (SlideShow.SlideShowID)SlideShowID.WhiteOutro){
            orig_ctor(manager, slideShowID);
        }
        else
        {
            this.current = -1;
            //Delegate to call the base constructor
            funct(manager, ProcessManager.ProcessID.SlideShow);//Menu Constructor

            this.slideShowID = (SlideShow.SlideShowID)slideShowID;
            this.pages.Add(new Page(this, null, "main", 0));
            this.playList = new List<SlideShow.Scene>();
            if (manager.musicPlayer != null)
            {
                this.waitForMusic = "RW_Outro_Theme";
                this.stall = true;
                manager.musicPlayer.MenuRequestsSong(this.waitForMusic, 1.5f, 10f);
            }
            this.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, 0f, 0f, 0f));
            this.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Outro_1_Left_Swim, this.ConvertTime(0, 1, 20), this.ConvertTime(0, 5, 0), this.ConvertTime(0, 17, 0)));
            this.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Outro_2_Up_Swim, this.ConvertTime(0, 21, 0), this.ConvertTime(0, 25, 0), this.ConvertTime(0, 37, 0)));
            this.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, this.ConvertTime(1, 1, 0), this.ConvertTime(1, 1, 0), this.ConvertTime(1, 6, 0)));
            for (int j = 1; j < this.playList.Count; j++)
            {
                this.playList[j].startAt -= 1.1f;
                this.playList[j].fadeInDoneAt -= 1.1f;
                this.playList[j].fadeOutStartAt -= 1.1f;
            }
            this.nextProcess = ProcessManager.ProcessID.Credits;
            this.preloadedScenes = new SlideShowMenuScene[this.playList.Count];
            for (int k = 0; k < this.preloadedScenes.Length; k++)
            {
                this.preloadedScenes[k] = new SlideShowMenuScene(this, this.pages[0], this.playList[k].sceneID);
                this.preloadedScenes[k].Hide();
            }
            manager.RemoveLoadingLabel();
            this.NextScene();
        }
    }
}

