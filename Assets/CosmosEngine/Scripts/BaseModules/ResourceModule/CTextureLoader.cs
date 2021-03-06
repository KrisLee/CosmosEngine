﻿//------------------------------------------------------------------------------
//
//      CosmosEngine - The Lightweight Unity3D Game Develop Framework
// 
//                     Version 0.8 (20140904)
//                     Copyright © 2011-2014
//                   MrKelly <23110388@qq.com>
//              https://github.com/mr-kelly/CosmosEngine
//
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[CDependencyClass(typeof(CAssetFileBridge))]
public class CTextureLoader
{
	public bool IsFinished = false;

    public Texture Asset { get; private set; }

    public delegate void CTextureLoaderDelegate(bool isOk, Texture tex, object[] args);

    public CTextureLoaderDelegate Callback;
    public object[] CallbackArgs;

    public static void Load(string path, CTextureLoaderDelegate callback = null, params object[] args)
    {
        new CTextureLoader(path, callback, args);
    }

    public CTextureLoader(string path, CTextureLoaderDelegate callback = null, params object[] args)
    {
        Callback = callback;
        CallbackArgs = args;
        new CAssetFileBridge(path, OnAssetLoaded);
    }

    void OnAssetLoaded(bool isOk, UnityEngine.Object obj)
    {
        Asset = obj as Texture;
        IsFinished = true;

        if (Callback != null)
            Callback(isOk, Asset, CallbackArgs);

    }

}
