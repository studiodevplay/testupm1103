using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SolarEngine;
using UnityEngine;

public class XmlModifier
{
    // 配置常量
    private const string SolarEngineNet = "Assets/SolarEngineNet/";
    private const string DEPENDDANCIDES = "Editor/Dependencies.xml";
    private const string SDK_XML_FILE_PATH = SolarEngineNet + DEPENDDANCIDES;
    private const string REMOTECONFIG_PATH = SolarEngineNet + "SolarEnginePlugins/RemoteConfigSDK/";
    private const string ANDROID_REMOTECONFIG_PATH = REMOTECONFIG_PATH + "Android/" + DEPENDDANCIDES;

    private const string IOS_POD_NODE_NAME = "iosPod";
    private const string IOS_POD_NAME_ATTR = "name";
    private const string IOS_POD_VERSION_ATTR = "version";

    // Oversea相关常量
    private const string IOS_POD_OVERSEA_NAME = "SolarEngineSDKiOSInter";
    private const string IOS_POD_OVERSEA_VERSION = "1.2.8.3";
    private const string ANDROID_OVERSEA_VERSION = "1.2.8.3";
    private const string ANDROID_PACKAGE_OVERSEA_SPEC = "com.reyun.solar.engine.oversea:solar-engine-core:" + ANDROID_OVERSEA_VERSION;
    
    private const string ANDROID_REMOTECONFIGE_OVERSEA_SPEC = "com.reyun.solar.engine.oversea:solar-remote-config:" + ANDROID_OVERSEA_VERSION;
    
    // CN相关常量
    private const string IOS_POD_CN_NAME = "SolarEngineSDK";
    private const string IOS_POD_CN_VERSION = "1.2.9.0";
    private const string ANDROID_CN_VERSION = "1.2.9.0";
    private const string ANDROID_PACKAGE_CN_SPEC = "com.reyun.solar.engine.china:solar-engine-core:" + ANDROID_CN_VERSION;
    private const string ANDROID_REMOTECONFIGE_CN_SPEC = "com.reyun.solar.engine.china:solar-remote-config:" + ANDROID_CN_VERSION;


    /// <summary>
    /// 加载XML文档
    /// </summary>
    /// <param name="_filePath">XML文件路径</param>
    /// <returns>加载成功返回XDocument对象，否则返回null</returns>
    private static XDocument LoadXmlDocument(string _filePath)
    {
        string filePath = _filePath;
        XDocument doc;

        // 加载XML文件并处理可能的加载错误
        if (File.Exists(filePath))
        {
            doc = XDocument.Load(filePath);
        }
        else
        {
            Debug.LogError($"文件 {filePath} 不存在");
            return null;
        }

        return doc;
    }

    /// <summary>
    /// 保存XML文档
    /// </summary>
    /// <param name="doc">要保存的XDocument对象</param>
    /// <param name="_filePath">XML文件路径</param>
    private static void SaveXmlDocument(XDocument doc, string _filePath)
    {
        string filePath = _filePath;

        // 保存修改后的XML文件并处理可能的保存错误
        try
        {
            doc.Save(filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"保存XML文件时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 修改iOS相关节点的属性（针对Oversea情况）
    /// </summary>
    /// <param name="doc">要修改的XDocument对象</param>
    private static void ModifyIOSNodesForOversea(XDocument doc)
    {
        // 修改iosPod节点的name和version属性
        var iosPod = doc.Descendants(IOS_POD_NODE_NAME).FirstOrDefault();
        if (iosPod!= null)
        {
            var nameAttribute = iosPod.Attribute(IOS_POD_NAME_ATTR);
            if (nameAttribute!= null)
            {
                nameAttribute.Value = IOS_POD_OVERSEA_NAME;
            }

            var versionAttribute = iosPod.Attribute(IOS_POD_VERSION_ATTR);
            if (versionAttribute!= null)
            {
                versionAttribute.Value = IOS_POD_OVERSEA_VERSION;
            }

            Debug.Log("changle iOS to Oversea");
        }
    }

    /// <summary>
    /// 修改安卓相关节点的属性（针对Oversea情况）
    /// </summary>
    /// <param name="doc">要修改的XDocument对象</param>
    private static void ModifyAndroidNodesForOversea(XDocument doc,string _spec)
    {
        // 修改androidPackages下节点的spec属性
        var androidPackages = doc.Descendants("androidPackages");
        var androidPackagesToModify = from androidPackage in androidPackages.Elements("androidPackage")
                                       where androidPackage.Attribute("spec")!= null
                                       select androidPackage;

        foreach (var androidPackage in androidPackagesToModify)
        {
            var specAttr = androidPackage.Attribute("spec");
            if (specAttr!= null)
            {
                specAttr.Value = _spec;
                Debug.Log("changle Android to Oversea");
            }
        }
    }

    /// <summary>
    /// 整体执行修改XML文件操作（针对Oversea情况），包括加载、修改和保存
    /// </summary>
    /// <param name="boolVale">控制是否执行修改操作的布尔值</param>
    public static void Overseaxml(bool boolVale)
    {
    
        try
        {
            XDocument doc = LoadXmlDocument(SDK_XML_FILE_PATH);
       
         
            if (doc!= null)
            {
                ModifyIOSNodesForOversea(doc);
                ModifyAndroidNodesForOversea(doc, ANDROID_PACKAGE_OVERSEA_SPEC);
                SaveXmlDocument(doc, SDK_XML_FILE_PATH);
                
               
            }

            if (!SolarEngineSettings.isDisAndroid)
            {
                XDocument docRemote = LoadXmlDocument(ANDROID_REMOTECONFIG_PATH);
                if (docRemote != null)
                {
                    ModifyAndroidNodesForOversea(docRemote, ANDROID_REMOTECONFIGE_OVERSEA_SPEC);
                    SaveXmlDocument(docRemote, ANDROID_REMOTECONFIG_PATH);
                }
                    
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"修改XML文件（Oversea）时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 修改iOS相关节点的属性（针对CN情况）
    /// </summary>
    /// <param name="doc">要修改的XDocument对象</param>
    private static void ModifyIOSNodesForCN(XDocument doc)
    {
        // 修改iosPod节点的name和version属性
        var iosPod = doc.Descendants(IOS_POD_NODE_NAME).FirstOrDefault();
        if (iosPod!= null)
        {
            var nameAttribute = iosPod.Attribute(IOS_POD_NAME_ATTR);
            if (nameAttribute!= null)
            {
            nameAttribute.Value = IOS_POD_CN_NAME;
            }

            var versionAttribute = iosPod.Attribute(IOS_POD_VERSION_ATTR);
            if (versionAttribute!= null)
            {
                versionAttribute.Value = IOS_POD_CN_VERSION;
            }

            Debug.Log("changle iOS to CN");
        }
    }

    /// <summary>
    /// 修改安卓相关节点的性能（针对CN情况）
    /// </summary>
    /// <param name="doc">要修改的XDocument对象</param>
    private static void ModifyAndroidNodesForCN(XDocument doc,string  _spec)
    {
        // 修改androidPackages下节点的spec属性
        var androidPackages = doc.Descendants("androidPackages");
        var androidPackagesToModify = from androidPackage in androidPackages.Elements("androidPackage")
                                       where androidPackage.Attribute("spec")!= null
                                       select androidPackage;

        foreach (var androidPackage in androidPackagesToModify)
        {
            var specAttr = androidPackage.Attribute("spec");
            if (specAttr!= null)
            {
                specAttr.Value = _spec;
                Debug.Log("changle Android to CN");
            }
        }
    }

    /// <summary>
    /// 整体执行修改XML文件操作（针对CN情况），包括加载、省略号)
    /// </summary>
    /// <param name="boolVale">控制是否执行修改操作的布尔值</param>
    public static void cnxml(bool boolVale)
    {
        try
        {
            XDocument doc = LoadXmlDocument(SDK_XML_FILE_PATH);
           
            if (doc!= null)
            {
                ModifyIOSNodesForCN(doc);
                ModifyAndroidNodesForCN(doc,ANDROID_PACKAGE_CN_SPEC);
                SaveXmlDocument(doc, SDK_XML_FILE_PATH);
                
              
            }

            if (!SolarEngineSettings.isDisAndroid)
            {
                XDocument docRemote = LoadXmlDocument(ANDROID_REMOTECONFIG_PATH);
                if (docRemote != null)
                {
                    ModifyAndroidNodesForOversea(docRemote, ANDROID_REMOTECONFIGE_CN_SPEC);
                    SaveXmlDocument(docRemote, ANDROID_REMOTECONFIG_PATH);
                }
                    
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"修改XML文件（CN）时出错: ex.Message");
        }
    }
}