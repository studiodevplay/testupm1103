
using System;
 using UnityEngine;
using UnityEditor;
using System.Collections;
 using System.IO;
 using System.Linq;
 using System.Reflection;
 using System.Xml.Linq;

 namespace SolarEngine
{
    [CustomEditor(typeof(SolarEngineSettings))]
    public class SolarEngineSettingsEditor : Editor
    {
        private SerializedProperty chinaProperty;
        private SerializedProperty overseaProperty;
        
        
        private SerializedProperty disAllRemoteConfig;
        private  SerializedProperty disiOSRemoteConfig;
        private SerializedProperty disAndroidRemoteConfig;
        private SerializedProperty disMiniGameRemoteConfig;
        
        
        
        
        private SerializedProperty disOaid;
        
        SerializedProperty iOSUrlIdentifier;
        SerializedProperty iOSUrlSchemes;
        SerializedProperty iOSUniversalLinksDomains;
        
        
        private bool oldChinaValue;
        private bool oldOverseaValue;
        private bool oldDisAllValue;
        private bool oldDisiOSValue;
        private bool oldDisAndroidValue;
        private bool oldDisMiniGameValue;
        private bool oldDisOaidValue;
        

        private object SolarEngineSettings;

        void OnEnable()
        {
            iOSUrlIdentifier = serializedObject.FindProperty("_iOSUrlIdentifier");
            iOSUrlSchemes = serializedObject.FindProperty("_iOSUrlSchemes");
            iOSUniversalLinksDomains = serializedObject.FindProperty("_iOSUniversalLinksDomains");
            chinaProperty = serializedObject.FindProperty("_China");
            overseaProperty = serializedObject.FindProperty("_Oversea");
            disAllRemoteConfig= serializedObject.FindProperty("_All");
            disiOSRemoteConfig= serializedObject.FindProperty("_iOS");
            disAndroidRemoteConfig= serializedObject.FindProperty("_Android");
            disMiniGameRemoteConfig= serializedObject.FindProperty("_MiniGame");
            disOaid= serializedObject.FindProperty("_DisOaid");
            
            oldChinaValue= chinaProperty.boolValue;
            oldOverseaValue= overseaProperty.boolValue;
            oldDisAllValue= disAllRemoteConfig.boolValue;
            oldDisiOSValue= disiOSRemoteConfig.boolValue;
            oldDisAndroidValue= disAndroidRemoteConfig.boolValue;
            oldDisMiniGameValue= disMiniGameRemoteConfig.boolValue;
            oldDisOaidValue= disOaid.boolValue;
        }


        public override void OnInspectorGUI()
        {
            SolarEngineSettings = target as SolarEngineSettings;
            this.GUI();
            
        }


        private void ChinaOrOversea()
        {  
            EditorGUILayout.PropertyField(chinaProperty);
            EditorGUILayout.PropertyField(overseaProperty);
      
            if (serializedObject.ApplyModifiedProperties())
            {
                // 处理 China 值变化
                ProcessPropertyChange(chinaProperty, ref oldChinaValue, "_China",XmlModifier. cnxml, () =>
                {
                    overseaProperty.boolValue = false;
                    oldOverseaValue = overseaProperty.boolValue;
                });

                // 处理 Oversea 值变化
                ProcessPropertyChange(overseaProperty, ref oldOverseaValue, "_Oversea",XmlModifier. Overseaxml, () =>
                {
                    chinaProperty.boolValue = false;
                    oldChinaValue = chinaProperty.boolValue;
                });
            }
        }


        private void RemoteConfig()
        {
            EditorGUILayout.PropertyField(disAllRemoteConfig);
            EditorGUILayout.PropertyField(disiOSRemoteConfig);
            EditorGUILayout.PropertyField(disAndroidRemoteConfig);
            EditorGUILayout.PropertyField(disMiniGameRemoteConfig);
            if (serializedObject.ApplyModifiedProperties())
            {
                ProcessPropertyChange(disAllRemoteConfig, ref oldDisAllValue, "_disAllRemoteConfig", DisableAll, () =>
                {
                    Debug.LogError("disAllRemoteConfig");

                });
                ProcessPropertyChange(disiOSRemoteConfig, ref oldDisiOSValue, "_disiOSRemoteConfig", DisableiOS);
                ProcessPropertyChange(disAndroidRemoteConfig, ref oldDisAndroidValue, "_disAndroidRemoteConfig", DisableAndroid);
                ProcessPropertyChange(disMiniGameRemoteConfig, ref oldDisMiniGameValue, "_disMiniGameRemoteConfig", DisableMiniGame);
            }
            
         
        }
        private void Oaid()
        {
            EditorGUILayout.PropertyField(disOaid);
            if (serializedObject.ApplyModifiedProperties())
            {
                ProcessPropertyChange( disOaid, ref oldDisOaidValue, "_disOaid", DisableOaid);
            }
        }

        private void DisableOaid(bool value)
        {
            if (value)
            {
                PluginsEdtior.disableOaid();
            }
          
        }

        void DisableAll(bool value)
        {
            if (value)
            {
                disiOSRemoteConfig.boolValue = true;
                disAndroidRemoteConfig.boolValue = true;
                disMiniGameRemoteConfig.boolValue = true;
               PluginsEdtior.disableAll();
            }
          
        }

        void DisableiOS(bool value)
        {
            if(value)
                PluginsEdtior.disableiOS(); 
        }
        void DisableAndroid(bool value)
        {
            if(value)
                PluginsEdtior.disableAndroid();
        }
        void DisableMiniGame(bool value)
        {
            if(value)
                PluginsEdtior.disableMiniGame();
        }
        

       
        
        
        private void ProcessPropertyChange(SerializedProperty property, ref bool oldValue, string propertyName, System.Action<bool> xmlAction, System.Action additionalAction = null)
        {
            if (property.boolValue!= oldValue)
            {
                Debug.Log($"{propertyName} value changed.");
                oldValue = property.boolValue;

                if (oldValue)
                {
                    xmlAction(oldValue);
                }

                additionalAction?.Invoke();
            }
        }
       
        public void GUI()
        {
            GUIStyle darkerCyanTextFieldStyles = new GUIStyle(EditorStyles.boldLabel);
            darkerCyanTextFieldStyles.normal.textColor = new Color(0f / 255f, 190f / 255f, 190f / 255f);
            
            DrawH2Title("SDK Setting");
            EditorGUILayout.LabelField("Please choose China or overseas:(require)", darkerCyanTextFieldStyles);
          
            ChinaOrOversea();
       
            DrawH2Title("SDK Plugins");
            EditorGUILayout.LabelField("Disable Remote Config:", darkerCyanTextFieldStyles);
            RemoteConfig();
            EditorGUILayout.LabelField("Dis Oaid:", darkerCyanTextFieldStyles);
            Oaid();
            
            
            EditorGUILayout.Space();
            
            DrawH2Title("iOS");
            EditorGUILayout.LabelField("DEEP LINKING:", darkerCyanTextFieldStyles);
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(iOSUrlIdentifier,
                new GUIContent("iOS URL Identifier",
                    "Value of CFBundleURLName property of the root CFBundleURLTypes element. " +
                    "If not needed otherwise, value should be your bundle ID."),
                true);
            EditorGUILayout.PropertyField(iOSUrlSchemes,
                new GUIContent("iOS URL Schemes",
                    "URL schemes handled by your app. " +
                    "Make sure to enter just the scheme name without :// part at the end."),
                true);
            EditorGUILayout.PropertyField(iOSUniversalLinksDomains,
                new GUIContent("iOS Universal Links Domains",
                    "Associated domains handled by your app. State just the domain part without applinks: part in front."),
                true);

            EditorGUI.indentLevel -= 1;
            serializedObject.ApplyModifiedProperties();
        }




    
        // 通用标签间的间距
        private const float COMMON_SPACE = 13f;

        // 最大间距
        private const float MAX_SPACE = 25f;

        protected void DrawH2Title(string title)
        {
            DrawAreaTitle(title, new Color(128, 138, 135), TextAnchor.MiddleCenter, 16);
        }

        /// <summary>
        /// 绘制标题区域.
        /// </summary>
        /// <param name="title">标题.</param>
        /// <param name="color">字体颜色.</param>
        /// <param name="textAnchor">对齐方式.</param>
        /// <param name="fontSize">字体大小.</param>
        private static void DrawAreaTitle(string title, Color color, TextAnchor textAnchor, int fontSize)
        {
            EditorGUILayout.BeginVertical();
            DrawVerticalSpace(MAX_SPACE);

            var guiStyle = new GUIStyle();
            guiStyle.fontSize = fontSize;
            guiStyle.normal.textColor = color;
            guiStyle.fontStyle = FontStyle.BoldAndItalic;
            guiStyle.alignment = textAnchor;
            EditorGUILayout.TextArea(title, guiStyle);
            EditorGUILayout.EndVertical();
            DrawVerticalSpace(COMMON_SPACE);
        }

        /// <summary>
        /// 绘制垂直方向间距.
        /// </summary>
        /// <param name="pixels">间距.</param>
        private static void DrawVerticalSpace(float pixels)
        {
            GUILayout.Space(pixels);
        }

    }



}
