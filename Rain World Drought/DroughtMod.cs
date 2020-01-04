using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Partiality.Modloader;
using UnityEngine;


public class DroughtMod : PartialityMod
{
    public DroughtMod()
    {
        ModID = "Rain World Drought";
        Version = "0100";
        author = "Rain World Drought Team";
    }

    //public static DroughtScript script;
    public override void OnLoad()
    {
        base.OnLoad();
        //GameObject obj = new GameObject();
        //script = obj.AddComponent<DroughtScript>();
        //DroughtScript.mod = this;

    }
}
/*
public class DroughtScript : MonoBehaviour
{
    public void ApplyPatch()
    {
        StaticWorldPatch.AddCreatureTemplate();
        StaticWorldPatch.ModifyRelationship();

    }

    public RainWorld rw;
    public static DroughtMod mod;
    public void Update()
    {
        if(rw == null)
        {
            rw = FindObjectOfType<RainWorld>();

            if (rw != null)
            { //Rain World has been Initialized
                ApplyPatch();
            }
        }
    }
    
}
*/
