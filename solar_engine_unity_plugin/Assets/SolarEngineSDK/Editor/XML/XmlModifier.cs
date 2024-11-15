using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SolarEngine;
using Debug = UnityEngine.Debug;

public class XmlModifier
{
    enum StrongType
    {
        CN,
        Oversea,
        Default,
    }

    private const string SolorEngine = "[SolorEngine]";

    // 配置常量
    private const string SolarEngineNet = "Assets/SolarEngineNet/";
    private const string DEPENDDANCIDES = "Editor/Dependencies.xml";
    private const string SDK_XML_FILE_PATH = SolarEngineNet + DEPENDDANCIDES;
    private const string REMOTECONFIG_PATH = SolarEngineNet + "SolarEnginePlugins/RemoteConfigSDK/";
    private const string ANDROID_REMOTECONFIG_PATH = REMOTECONFIG_PATH + "Android/" + DEPENDDANCIDES;
    private const string IOS_REMOTECONFIG_PATH = REMOTECONFIG_PATH + "iOS/" + DEPENDDANCIDES;
    private const string ANDROID_OAID_PATH = SolarEngineNet + "SolarEnginePlugins/Oaid/" + DEPENDDANCIDES;

    private const string IOS_POD_NODE_NAME = "iosPod";
    private const string IOS_POD_NAME_ATTR = "name";
    private const string IOS_POD_VERSION_ATTR = "version";

    // Oversea
    private const string IOS_POD_OVERSEA_NAME = "SolarEngineSDKiOSInter";

    private const string ANDROID_PACKAGE_OVERSEA_SPEC = "com.reyun.solar.engine.oversea:solar-engine-core:";

    //
    private const string ANDROID_REMOTECONFIGE_OVERSEA_SPEC = "com.reyun.solar.engine.oversea:solar-remote-config:";


    // CN
    private const string IOS_POD_CN_NAME = "SolarEngineSDK";

    private static string ANDROID_PACKAGE_CN_SPEC = "com.reyun.solar.engine.china:solar-engine-core:";
    private static string ANDROID_REMOTECONFIGE_CN_SPEC = "com.reyun.solar.engine.china:solar-remote-config:";


    private const string IOS_REMOTECONFIGE_SPEC = "SESDKRemoteConfig";
    private const string ANDROID_OAID_SPEC = "com.reyun.solar.engine:se-plugin-oaid:";


    private const string ANDROID_PACKAGE_Default = "";
    private const string ANDROID_PACKAGE_Default_VERSION = "+";
    private const string IOS_POD_Default = "";
    private const string IOS_POD_Default_VERSION = ">=0";


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
            Debug.LogError($"{SolorEngine}文件 {filePath} 不存在");
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
    /// 修改iOS相关节点的属性
    /// </summary>
    /// <param name="doc">要修改的XDocument对象</param>
    private static bool ModifyIOSNodes(XDocument doc, string _name,StrongType type=StrongType.CN)
    {
        bool name = false;
        bool version = false;
        var iosPod = doc.Descendants(IOS_POD_NODE_NAME).FirstOrDefault();
        if (iosPod != null)
        {
            var nameAttribute = iosPod.Attribute(IOS_POD_NAME_ATTR);
            if (nameAttribute != null)
            {
                nameAttribute.Value = _name;
                name = true;
            }

            var versionAttribute = iosPod.Attribute(IOS_POD_VERSION_ATTR);
            if (versionAttribute != null)
            {
                if (type == StrongType.Default)
                {
                    versionAttribute.Value = IOS_POD_Default_VERSION;
                 
                }
                else
                {
                    versionAttribute.Value = string.IsNullOrEmpty(SolarEngineSettings.iOSVersion)
                        ?IOS_POD_Default_VERSION
                        : SolarEngineSettings.iOSVersion;
                }

                Debug.LogError( versionAttribute.Value);
                version = true;
            }
        }

        return name && version;
    }

    private static bool iOSRC(StrongType strongType=StrongType.CN)
    {
        bool isModified = false;

        if (!SolarEngineSettings.isUseiOS)
            return true;
        else
        {
            XDocument docRemote = LoadXmlDocument(IOS_REMOTECONFIG_PATH);
            if (docRemote != null)
            {
                if (strongType == StrongType.Default)
                {
                    isModified = ModifyIOSNodes(docRemote, IOS_POD_Default);

                }
                else
                {
                    isModified = ModifyIOSNodes(docRemote, IOS_REMOTECONFIGE_SPEC);

                }
                SaveXmlDocument(docRemote, IOS_REMOTECONFIG_PATH);
            }
        }

        return isModified;
    }

    private static bool AndroidRC(StrongType strongType)
    {
        bool isModified = false;

        if (!SolarEngineSettings.isUseAndroid)
            return true;
        else
        {
            XDocument docRemote = LoadXmlDocument(ANDROID_REMOTECONFIG_PATH);
            if (docRemote != null)
            {
                
                string version = string.IsNullOrEmpty(SolarEngineSettings.AndroidVersion)
                    ? ANDROID_PACKAGE_Default_VERSION
                    : SolarEngineSettings.AndroidVersion;
                switch (strongType)
                {
                    case StrongType.Oversea:
                        isModified = ModifyAndroidNode(docRemote, ANDROID_REMOTECONFIGE_OVERSEA_SPEC + version);
                        break;
                    case StrongType.CN:
                        isModified = ModifyAndroidNode(docRemote, ANDROID_REMOTECONFIGE_CN_SPEC + version);
                        break;
                    case StrongType.Default:
                        isModified = ModifyAndroidNode(docRemote, ANDROID_PACKAGE_Default);


                        break;
                }


                SaveXmlDocument(docRemote, ANDROID_REMOTECONFIG_PATH);
            }

            return isModified;
        }
    }

    private static bool AndroidOaid(StrongType strongType=StrongType.CN)
    {
        bool isModified = false;

        if (!SolarEngineSettings.isUseOaid&&strongType != StrongType.Default)
            return true;
        else
        {
            XDocument docOaid = LoadXmlDocument(ANDROID_OAID_PATH);

            if (docOaid != null)
            {
                if (strongType == StrongType.Default)
                {
                    isModified = ModifyAndroidNode(docOaid, ANDROID_PACKAGE_Default + ANDROID_PACKAGE_Default_VERSION);
                }
                else
                {
                    string version = string.IsNullOrEmpty(SolarEngineSettings.AndroidVersion)
                        ? ANDROID_PACKAGE_Default_VERSION
                        : SolarEngineSettings.AndroidVersion;
                    isModified = ModifyAndroidNode(docOaid, ANDROID_OAID_SPEC + version);
 
                }
               

                SaveXmlDocument(docOaid, ANDROID_OAID_PATH);
                
                
                
            }

            return isModified;
        }
    }


    /// <summary>
    /// 修改安卓相关节点的性能（针对CN情况）
    /// </summary>
    /// <param name="doc">要修改的XDocument对象</param>
    private static bool ModifyAndroidNode(XDocument doc, string _spec)
    {
        // 修改androidPackages下节点的spec属性
        var firstAndroidPackages = doc.Descendants("androidPackages").FirstOrDefault();

        if (firstAndroidPackages != null)
        {
            // 查找第一个androidPackages元素下包含spec属性的子元素androidPackage
            var androidPackagesToModify = from androidPackage in firstAndroidPackages.Elements("androidPackage")
                where androidPackage.Attribute("spec") != null
                select androidPackage;


            foreach (var androidPackage in androidPackagesToModify)
            {
                var specAttr = androidPackage.Attribute("spec");
                if (specAttr != null)
                {
                    specAttr.Value = _spec;
                    return true;
                }
            }
        }

        return false;
    }

    public static void defaultXml()
    {
        sdkSetting(StrongType.Default);
        AndroidRC(StrongType.Default);
        AndroidOaid(StrongType.Default);
        iOSRC(StrongType.Default);
    }
    /// <summary>
    /// 整体执行修改XML文件操作（针对CN情况），包括加载、省略号)
    /// </summary>
    /// <param name="boolVale">控制是否执行修改操作的布尔值</param>
    public static bool cnxml(bool boolVale)
    {
      

        try
        {
            if (sdkSetting(StrongType.CN) && AndroidRC(StrongType.CN) && AndroidOaid() && iOSRC())
            {
                return  true;
            }
            else
            {
                return  false;
            }
                
        }
        catch (Exception ex)
        {
          
            Debug.LogError(SolorEngine + "Error modifying XML file (CN) ex.Message");
            return  false;
        }
    }

    /// <summary>
    /// 整体执行修改XML文件操作（针对Oversea情况），包括加载、修改和保存
    /// </summary>
    /// <param name="boolVale">控制是否执行修改操作的布尔值</param>
    public static bool Overseaxml(bool boolVale)
    {
     
        try
        {
            if (sdkSetting(StrongType.Oversea) && AndroidRC(StrongType.Oversea) && AndroidOaid() && iOSRC())
            {
                Debug.Log($"{SolorEngine}set SDK to Oversea");
                return true;
            }
            else
                return false;

        }
        catch (Exception ex)
        {
            
            Debug.LogError(SolorEngine + "Error modifying XML file (oversea) ex.Message");
            return false;
        }
        return false;
    }

    static bool sdkSetting(StrongType type)
    {
        bool ios = false;
        bool android = false;
        XDocument doc = LoadXmlDocument(SDK_XML_FILE_PATH);

        if (doc != null)
        {
            string androidversion = string.IsNullOrEmpty(SolarEngineSettings.AndroidVersion)
                ? ANDROID_PACKAGE_Default_VERSION
                : SolarEngineSettings.AndroidVersion;
            switch (type)
            {
                case StrongType.CN:
                    ios = ModifyIOSNodes(doc, IOS_POD_CN_NAME);
                    android = ModifyAndroidNode(doc, ANDROID_PACKAGE_CN_SPEC + androidversion);


                    break;
                case StrongType.Oversea:
                    ios = ModifyIOSNodes(doc, IOS_POD_OVERSEA_NAME);


                    android = ModifyAndroidNode(doc, ANDROID_PACKAGE_OVERSEA_SPEC + androidversion);

                    break;
                case StrongType.Default:
                    ios = ModifyIOSNodes(doc, IOS_POD_Default);


                    android = ModifyAndroidNode(doc, ANDROID_PACKAGE_Default);

                    break;
            }


            SaveXmlDocument(doc, SDK_XML_FILE_PATH);
        }

        return ios && android;
    }
    
    
    private static string ConvertVersionFormat(string inputVersion)
    {
        // 这里简单的替换逻辑，实际中可以根据更复杂的规则来验证和转换版本格式
        return inputVersion.Replace("&gt;", ">"); 
    }
}