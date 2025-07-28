#if UNITY_EDITOR
using SolarEngineSDK.Editor;
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;


namespace SolarEngine
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class SolarEngineSettings : ScriptableObject
    {
        private const string ASSET_NAME = "SolarEngineSettings";
        private const string ASSET_PATH = "Resources";
        private const string ASSET_EXT = ".asset";

        private static SolarEngineSettings instance;

        #region 数据存储区域

        [SerializeField] private bool _China;
        [SerializeField] private bool _Oversea;

        #endregion

        //可选功能
        [HideInInspector] public bool _OptionalFeatures;

        #region RC

        //默认开启
        [SerializeField] private bool _RemoteConfig = true;
        [SerializeField] private bool _All = true;
        [SerializeField] private bool _iOS = true;
        [SerializeField] private bool _Android = true;
        [SerializeField] private bool _MiniGame = true;
        [SerializeField] private bool _OpenHarmony = true;
        [SerializeField] private bool _MacOS = true;

        #endregion

        #region _Oaid、_ODMInfo、_RemoveAndroidSDK

        [SerializeField] private bool _Oaid = true;
        [SerializeField] private bool _ODMInfo = false;


        [SerializeField] private bool _RemoveAndroidSDK;

        #endregion


        #region deeplink

        [SerializeField] private bool _DeepLink;
        [SerializeField] private string _iOSUrlIdentifier;
        [SerializeField] private string[] _iOSUrlSchemes = new string[0];
        [SerializeField] private string[] _iOSUniversalLinksDomains = new string[0];


        [SerializeField] private string[] _AndroidUrlSchemes = new string[0];

        #endregion


        #region Version;

        [SerializeField] private bool _SpecifyVersion;


        [SerializeField] private string _iOSVersion;
        [SerializeField] private string _AndroidVersion;
        [SerializeField] private string _OpenHarmonyVersion;
        // [SerializeField] private string _MacOSVersion;

        #endregion


        public static SolarEngineSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(ASSET_NAME) as SolarEngineSettings;
                    if (instance == null)
                    {
                        // If not found, autocreate the asset object.
                        instance = ScriptableObject.CreateInstance<SolarEngineSettings>();
#if UNITY_EDITOR
                        string properPath = Path.Combine(Application.dataPath, ASSET_PATH);
                        if (!Directory.Exists(properPath))
                        {
                            Directory.CreateDirectory(properPath);
                        }

                        string fullPath = Path.Combine(Path.Combine("Assets", ASSET_PATH), ASSET_NAME + ASSET_EXT);
                        AssetDatabase.CreateAsset(instance, fullPath);
#endif
                    }
                }

                return instance;
            }
        }

        public static string iOSUrlIdentifier
        {
            get { return Instance._iOSUrlIdentifier; }
            set { Instance._iOSUrlIdentifier = value; }
        }

        public static string[] iOSUrlSchemes
        {
            get { return Instance._iOSUrlSchemes; }
            set { Instance._iOSUrlSchemes = value; }
        }

        public static string[] iOSUniversalLinksDomains
        {
            get { return Instance._iOSUniversalLinksDomains; }
            set { Instance._iOSUniversalLinksDomains = value; }
        }

        public static string[] AndroidUrlSchemes
        {
            get { return Instance._AndroidUrlSchemes; }
            set { Instance._AndroidUrlSchemes = value; }
        }

        public static bool isCN
        {
            get { return Instance._China; }
        }


        public static bool isOversea
        {
            get { return Instance._Oversea; }
        }

    
        public static bool removeAndroidSDK
        {
            get
            {
                return Instance._RemoveAndroidSDK; 
                
            }
            set { Instance._RemoveAndroidSDK = value; }
        }


        public static bool isUseMiniGame
        {
            get { return Instance._MiniGame; }
            set { Instance._MiniGame = value; }
        }

        public static bool isUseOpenHarmony
        {
            get
            {
                return Instance._OpenHarmony;
                
            }
            set { Instance._OpenHarmony = value; }
        }

        public static bool isUseMacOS
        {
            get
            {
                return Instance._MacOS; 
                
            }
            set { Instance._MacOS = value; }
        }

        public static bool isUseAndroid
        {
            get
            {
                return Instance._Android;
                
            }
            set { Instance._Android = value; }
        }

        public static bool isUseiOS
        {
            get
            {
                return Instance._iOS; 
                
            }
            set { Instance._iOS = value; }
        }

        public static bool isUseAll
        {
            get { return Instance._All; }
        }

        public static bool isUseOaid
        {
            get { return Instance._Oaid; }
            set { Instance._Oaid = value; }
        }

        public static bool isUseODMInfo
        {
            get { return Instance._ODMInfo; }
            set { Instance._ODMInfo = value; }
        }

        public static bool isSpecifyVersio
        {
            get { return Instance._SpecifyVersion; }
            set { Instance._SpecifyVersion = value; }
        }

        public static string iOSVersion
        {
            get { return Instance._iOSVersion; }
            set { Instance._iOSVersion = value; }
        }

        public static string AndroidVersion
        {
            get { return Instance._AndroidVersion; }
            set { Instance._AndroidVersion = value; }
        }

        public static string OpenHarmonyVersion
        {
            get { return Instance._OpenHarmonyVersion; }
            set { Instance._OpenHarmonyVersion = value; }
        }

        public static string MacOSVersion
        {
            get { return Instance._iOSVersion; }
          //  set { Instance._iOSVersion = value; }
        }
#if UNITY_EDITOR
        [MenuItem(ConstString.MenuItem.sdkEditSettings, false, 0)]
        public static void EditSettings()
        {
            Selection.activeObject = Instance;
        }
#endif
    }
}