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
using System.Collections;

/// <summary>
/// 根據不同模式，從AssetBundle中獲取Asset或從Resources中獲取,一個橋接類
/// </summary>
public class CAssetFileBridge
{
    public delegate void CAssetFileBridgeDelegate(bool isOk, UnityEngine.Object assetObject);

    CAssetFileBridgeDelegate AssetFileLoadedCallback;
    string AssetInBundleName;  // AssetBundle里的名字, Resources時不用
    
    public bool IsFinished { get; private set; }
    public bool IsError { get; private set; }

    public CAssetFileBridge(string path, CAssetFileBridgeDelegate assetFileLoadedCallback)
    {
        _Init(path, null, assetFileLoadedCallback);
    }

    // AssetBundle或Resource文件夾的資源文件
    public CAssetFileBridge(string path, string assetName, CAssetFileBridgeDelegate assetFileLoadedCallback)
    {
        _Init(path, assetName, assetFileLoadedCallback);
    }

    void _Init(string path, string assetName, CAssetFileBridgeDelegate assetFileLoadedCallback)
    {
        AssetFileLoadedCallback = assetFileLoadedCallback;
        AssetInBundleName = assetName;

        if (CCosmosEngine.GetConfig("IsLoadAssetBundle").ToInt32() == 0)
        {
            CResourceModule.Instance.StartCoroutine(LoadInResourceFolder(path));
        }
        else
        {
            new CAssetBundleLoader(path, OnAssetBundleLoaded);
        }
    }

    IEnumerator LoadInResourceFolder(string path)
    {
        yield return null; // 延遲1幀

        string extension = System.IO.Path.GetExtension(path);
        path = path.Substring(0, path.Length - extension.Length);  // remove extensions

        UnityEngine.Object asset = Resources.Load<UnityEngine.Object>(path);
        if (asset == null)
        {
            CDebug.LogError("Asset is NULL(from Resources Folder): {0}", path);
        }
        IsFinished = true;
        IsError = asset == null;
        if (AssetFileLoadedCallback != null)
            AssetFileLoadedCallback(asset != null, asset);
    }

    void OnAssetBundleLoaded(bool isOk, string url, AssetBundle assetBundle, params object[] args)
    {
        Object asset = null;
        System.DateTime beginTime = System.DateTime.Now;
        if (AssetInBundleName == null)
        {
            // 经过AddWatch调试，.mainAsset这个getter第一次执行时特别久，要做序列化
            try
            {
                asset = assetBundle.mainAsset;
            }
            catch
            {
                CDebug.LogError("[OnAssetBundleLoaded:mainAsset]{0}", url);
            }
        }
        else
        {
            AssetBundleRequest request = assetBundle.LoadAsync(AssetInBundleName, typeof(Object));
            asset = request.asset;
        }

        CResourceModule.LogLoadTime("AssetFileBridge", url, beginTime);

        if (asset == null)
        {
            CDebug.LogError("Asset is NULL: {0}", url);
        }

        IsFinished = true;
        IsError = asset == null;
        if (AssetFileLoadedCallback != null)
            AssetFileLoadedCallback(asset != null, asset);
    }
}
