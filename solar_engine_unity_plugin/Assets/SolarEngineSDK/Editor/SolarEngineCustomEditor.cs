using System.Reflection;
using SolarEngineSDK.Editor;
using UnityEditor;
using UnityEngine;

namespace SolarEngine
{
    [CustomEditor(typeof(SolarEngineAnalytics))]
    public class SolarEngineCustomEditor : Editor
    {
        [MenuItem(ConstString.MenuItem.createAnalyticsObject, false, 10)]
        public static void CreateAnalyticsObject()
        {
            // 查找已有 SolarEngineAnalytics
            SolarEngineAnalytics existing = Object.FindObjectOfType<SolarEngineAnalytics>();

            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("Already Exists",
                        "A SolarEngineAnalytics already exists in the scene.\nDo you want to create another one (copying its values)?",
                        "Yes", "No"))
                {
                    return;
                }
            }

            // 创建新 GameObject 并挂载组件
            GameObject analyticsGO = new GameObject("SolarEngineAnalytics");
            SolarEngineAnalytics newAnalytics = analyticsGO.AddComponent<SolarEngineAnalytics>();

            // 复制字段值（浅拷贝）
            if (existing != null)
            {
                CopySerializedFields(existing, newAnalytics);
            }

            Undo.RegisterCreatedObjectUndo(analyticsGO, "Create SolarEngineAnalytics");
            Selection.activeGameObject = analyticsGO;

            Debug.Log("Created new SolarEngineAnalytics object in the scene.");
        }

        private static void CopySerializedFields(SolarEngineAnalytics source, SolarEngineAnalytics target)
        {
            var type = typeof(SolarEngineAnalytics);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in type.GetFields(flags))
            {
                // 仅复制非 static 字段
                if (!field.IsStatic && !field.IsNotSerialized)
                {
                    object value = field.GetValue(source);
                    field.SetValue(target, value);
                }
            }
        }
        private Editor settingsEditor;

        public override void OnInspectorGUI()
        {
            var solarEngine = (SolarEngineAnalytics)target;
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal =
                {
                    textColor = EditorUtils.GetColor(EditorUtils.EditorColor.Cyan)
                },
                fontSize = 12
            };

     
            solarEngine.startManually = EditorGUILayout.Toggle(  new GUIContent("Start SDK Manually", "Enable Start SDK Manually"),
                solarEngine.startManually);

            EditorGUILayout.Space();


            using (new EditorGUI.DisabledScope(solarEngine.startManually))
            {
                var lableStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal =
                    {
                        textColor = EditorUtils.GetColor(EditorUtils.EditorColor.SoftGreen)
                    }
                };
                solarEngine.preInitSeSdkManually = EditorGUILayout.Toggle("Pre Init SDK Manually", solarEngine.preInitSeSdkManually);

                EditorGUILayout.LabelField("MULTIPLATFORM SETTINGS", titleStyle);
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();
                // solarEngine.appKey = EditorGUILayout.TextField(new GUIContent("App Key", "SolarEngine  AppKey"),
                //     solarEngine.appKey);
                solarEngine.appKey = EditorGUILayout.TextField(
                    new GUIContent("App Key", "SolarEngine AppKey"),
                    solarEngine.appKey
                );

                if (!string.IsNullOrEmpty(solarEngine.appKey)&&solarEngine.appKey.Contains(" "))
                {
                    EditorGUILayout.HelpBox("App Key Cannot Contain Spaces", MessageType.Error);
                }

                solarEngine.logEnabled = EditorGUILayout.Toggle("Log Enable", solarEngine.logEnabled);
                solarEngine.isDebugModel = EditorGUILayout.Toggle("Debug Model", solarEngine.isDebugModel);

#if UNITY_IOS||UNITY_ANDROID
                if (SolarEngineSettings.isOversea)
                {
                    solarEngine.isGDPRArea = EditorGUILayout.Toggle("Gdpr Area", solarEngine.isGDPRArea);
                    solarEngine.isCoppaEnabled = EditorGUILayout.Toggle("Coppa Enable", solarEngine.isCoppaEnabled);
                    solarEngine.isKidsAppEnabled =
                        EditorGUILayout.Toggle("Kids App Enabled", solarEngine.isKidsAppEnabled);
                }

                solarEngine.deferredDeeplinkenable =
                    EditorGUILayout.Toggle("Deferred Deeplink", solarEngine.deferredDeeplinkenable);
#if UNITY_IOS
                solarEngine.attAuthorizationWaitingInterval = Mathf.Clamp(
                    EditorGUILayout.IntField("ATT Wait Interval", solarEngine.attAuthorizationWaitingInterval),
                    0, 120);
#endif
              
#if UNITY_ANDROID
                solarEngine.fbAppID = EditorGUILayout.TextField("Facebook App ID", solarEngine.fbAppID);

#endif
#if UNITY_IOS
                if (SolarEngineSettings.isCN)
                    solarEngine.caid = EditorGUILayout.TextField("Caid", solarEngine.caid);

#endif

#endif


                EditorGUILayout.LabelField("REMOTE CONFIG", lableStyle);

                solarEngine.isUseRemoteConfig = EditorGUILayout.Toggle("Enable", solarEngine.isUseRemoteConfig);
                if (solarEngine.isUseRemoteConfig)
                    solarEngine.mergeType = (RCMergeType)EditorGUILayout.EnumPopup("Merge Type", solarEngine.mergeType);
#if UNITY_IOS||UNITY_ANDROID
                EditorGUILayout.LabelField("CUSTOM DOMAIN", lableStyle);
                solarEngine.isUseCustomDomain = EditorGUILayout.Toggle("Enable", solarEngine.isUseCustomDomain);
                if (solarEngine.isUseCustomDomain)
                {
                    solarEngine.receiverDomain =
                        EditorGUILayout.TextField("Receiver Domain", solarEngine.receiverDomain);
                    solarEngine.ruleDomain = EditorGUILayout.TextField("Rule Domain", solarEngine.ruleDomain);
                    solarEngine.receiverTcpHost =
                        EditorGUILayout.TextField("Receiver TCP Host", solarEngine.receiverTcpHost);
                    solarEngine.ruleTcpHost = EditorGUILayout.TextField("Rule TCP Host", solarEngine.ruleTcpHost);
                    solarEngine.gatewayTcpHost =
                        EditorGUILayout.TextField("Gateway TCP Host", solarEngine.gatewayTcpHost);
                }

#endif

                drawMiniGameInspector(solarEngine, lableStyle);
                EditorGUI.indentLevel -= 1;
            }


            EditorGUILayout.Space(10f);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = EditorUtils.GetColor(EditorUtils.EditorColor.SoftPurple);
            EditorGUILayout.LabelField("SOLARENGINESETTINGS PANEL", titleStyle);

            if (settingsEditor == null) settingsEditor = CreateEditor(SolarEngineSettings.Instance);

            settingsEditor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(solarEngine);

            EditorGUILayout.Space();
        }


        private void drawMiniGameInspector(SolarEngineAnalytics target, GUIStyle lableStyle)
        {
#if SOLARENGINE_WECHAT||SOLARENGINE_BYTEDANCE_CLOUD||SOLARENGINE_BYTEDANCE_STARK||SOLARENGINE_BYTEDANCE||SOLARENGINE_KUAISHOU
             EditorGUILayout.LabelField("MINI GAME SETTINGS", lableStyle);


            target.unionid = EditorGUILayout.TextField("UnionID", target.unionid);
            target.openid = EditorGUILayout.TextField("OpenID", target.openid);
            target.anonymous_openid = EditorGUILayout.TextField("Anonymous OpenID", target.anonymous_openid);
#if SOLARENGINE_WECHAT
             target.isInitTencentAdvertisingGameSDK = EditorGUILayout.Toggle("Tencent Advertising Game SDK",
                target.isInitTencentAdvertisingGameSDK);
            if (target.isInitTencentAdvertisingGameSDK)
            {
                EditorGUILayout.LabelField("Tencent Advertising Game SDK Settings".ToUpper(), lableStyle);

                target.user_action_set_id = EditorGUILayout.IntField("User Action Set ID", target.user_action_set_id);
                target.secret_key = EditorGUILayout.TextField("Secret Key", target.secret_key);
                target.appid = EditorGUILayout.TextField("AppID", target.appid);
                target.tencentSdkIsAutoTrack =
                    EditorGUILayout.Toggle("Tencent SDK Auto Track", target.tencentSdkIsAutoTrack);
            }
#endif
#endif
        }
    }
}