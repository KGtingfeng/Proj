using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAttr : Attribute
{
    public ViewAttr(string packagePath, string packageName, string componmentName,bool animationView=false)
    {
        PackagePath = packagePath;
        PackageName = packageName;
        ComponmentName = componmentName;
        AnimationView = animationView;
    }
    public string PackagePath { get; set; }
    public string PackageName { get; set; }
    public string ComponmentName { get; set; }
    public bool AnimationView { get; set; }
}



