
using System;
 using UnityEngine;
using UnityEditor;
using System.Collections;
 using System.IO;
 using System.Linq;
 using System.Reflection;
 using System.Xml.Linq;
using SolarEngine.Build;

namespace SolarEngine
{
    [CustomEditor(typeof(SolarEngineSettings))]
    public class SolarEngineSettingsEditor : Editor
    {
        private const string SolorEngine = "[SolorEngine]";

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
        SerializedProperty iOSVersion;
        
        SerializedProperty AndroidUrlSchemes;
        
        SerializedProperty AndroidVersion;
        
        
        private bool oldChinaValue;
        private bool oldOverseaValue;
        private bool oldDisAllValue;
        private bool oldDisiOSValue;
        private bool oldDisAndroidValue;
        private bool oldDisMiniGameValue;
        private bool oldDisOaidValue;

      

        private object SolarEngineSetting;

        void OnEnable()
        {
            SolarEngineSetting = target as SolarEngineSettings;
            iOSVersion= serializedObject.FindProperty("_iOSVersion");
            AndroidVersion= serializedObject.FindProperty("_AndroidVersion");
            
            iOSUrlIdentifier = serializedObject.FindProperty("_iOSUrlIdentifier");
            iOSUrlSchemes = serializedObject.FindProperty("_iOSUrlSchemes");
            iOSUniversalLinksDomains = serializedObject.FindProperty("_iOSUniversalLinksDomains");
            
            AndroidUrlSchemes= serializedObject.FindProperty("_AndroidUrlSchemes");
            
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
          
            this.GUI();
            
        }

        private void AndroidGUI()
        {
            
        }
        private void SdkVersion()
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(iOSVersion);
            EditorGUILayout.PropertyField(AndroidVersion);
            EditorGUI.indentLevel -= 1;
            if (!iOSVersion.stringValue.Equals(SolarEngineSettings.iOSVersion))
                SolarEngineSettings.iOSVersion=iOSVersion.stringValue;
            if( !AndroidVersion.stringValue.Equals(SolarEngineSettings.AndroidVersion))
                SolarEngineSettings.AndroidVersion = AndroidVersion.stringValue;
       
           
        }

        private void ChinaOrOversea()
        {
            if (string.IsNullOrEmpty(SolarEngineSettings.iOSVersion)||string.IsNullOrEmpty (AndroidVersion.stringValue))
           
                return;
        
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(chinaProperty);
            EditorGUILayout.PropertyField(overseaProperty);
            EditorGUI.indentLevel -= 1;
        
           
            if (serializedObject.ApplyModifiedProperties())
            {
                
                // 处理 China 值变化
                ProcessPropertyChange(chinaProperty, ref oldChinaValue, "_China",changeToCN, () =>
                {
                    overseaProperty.boolValue = false;
                    oldOverseaValue = overseaProperty.boolValue;
                    
                });

                // 处理 Oversea 值变化
                ProcessPropertyChange(overseaProperty, ref oldOverseaValue, "_Oversea",changeToOversea, () =>
                {
                    chinaProperty.boolValue = false;
                    oldChinaValue = chinaProperty.boolValue;
                  
                });
            }
        }


        void changeToCN(bool value)
        {
            if (value)
            {
                if (disOaid.boolValue)
                {
                    PluginsEdtior.showOaid();
                    disOaid.boolValue = false;
                    SolarEngineSettings.isDisOaid = disOaid.boolValue;
                }
              
                XmlModifier.cnxml(value);
                Debug.Log($"{SolorEngine}cn onenable oaid");
            }
            
        }
        void changeToOversea(bool value)
        {
            if (value)
            {
             
                if (!disOaid.boolValue)
                {
                    PluginsEdtior.disableOaid();
                    disOaid.boolValue = true;
                    SolarEngineSettings.isDisOaid = disOaid.boolValue;
                }
             
                XmlModifier.Overseaxml(value);
              
                Debug.Log($"{SolorEngine}oversea disable oaid");
            }
            
        }


        private void RemoteConfig()
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(disAllRemoteConfig);
            EditorGUILayout.PropertyField(disiOSRemoteConfig);
            EditorGUILayout.PropertyField(disAndroidRemoteConfig);
            EditorGUILayout.PropertyField(disMiniGameRemoteConfig);
            EditorGUI.indentLevel -= 1;
            if (serializedObject.ApplyModifiedProperties())
            {
                ProcessPropertyChange(disAllRemoteConfig, ref oldDisAllValue, "_disAllRemoteConfig", DisableAll, () =>
                {
                   // Debug.Log("disAllRemoteConfig");

                });
                ProcessPropertyChange(disiOSRemoteConfig, ref oldDisiOSValue, "_disiOSRemoteConfig", DisableiOS);
                ProcessPropertyChange(disAndroidRemoteConfig, ref oldDisAndroidValue, "_disAndroidRemoteConfig", DisableAndroid);
                ProcessPropertyChange(disMiniGameRemoteConfig, ref oldDisMiniGameValue, "_disMiniGameRemoteConfig", DisableMiniGame);
            }
            
         
        }
        private void Oaid()
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(disOaid);
            EditorGUI.indentLevel -= 1;
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
            else
            {
                PluginsEdtior.showOaid();
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
             
            }else
                {
                disiOSRemoteConfig.boolValue = false;
                disAndroidRemoteConfig.boolValue = false;
                disMiniGameRemoteConfig.boolValue = false;
                PluginsEdtior.showAll();
                
            }
            
          
        }

        void DisableiOS(bool value)
        {
            if (value)
            {
                PluginsEdtior.disableiOS();
                if(!disAllRemoteConfig.boolValue)
                DefineSymbolsEditor.add_DISABLE_REMOTECONFIG(BuildTargetGroup.iOS);

            }
               
            else
            {
                PluginsEdtior.showiOS();
                DefineSymbolsEditor.delete_DISABLE_REMOTECONFIG(BuildTargetGroup.iOS);

            }
        }
        void DisableAndroid(bool value)
        {

            if (value)
            {
                PluginsEdtior.disableAndroid();
                if(!disAllRemoteConfig.boolValue)
                DefineSymbolsEditor.add_DISABLE_REMOTECONFIG(BuildTargetGroup.Android);

            }
              
            else
            {
                PluginsEdtior.showAndroid();
                DefineSymbolsEditor.delete_DISABLE_REMOTECONFIG(BuildTargetGroup.Android);

            }
        }
        void DisableMiniGame(bool value)
        {

            if (value)
            {   PluginsEdtior.disableMiniGame();
                if(!disAllRemoteConfig.boolValue)
                DefineSymbolsEditor.add_DISABLE_REMOTECONFIG(BuildTargetGroup.WebGL);
            }
             
            else
            {
                
                PluginsEdtior.showMiniGame();
                DefineSymbolsEditor.delete_DISABLE_REMOTECONFIG(BuildTargetGroup.WebGL);
            }
        }
        

       
        
        
        private void ProcessPropertyChange(SerializedProperty property, ref bool oldValue, string propertyName, System.Action<bool> xmlAction, System.Action additionalAction = null)
        {
            if (property.boolValue!= oldValue)
            {
                Debug.Log($"{SolorEngine}{propertyName} value changed.");
                oldValue = property.boolValue;

                xmlAction(oldValue);
                additionalAction?.Invoke();
            }
        }
       
        public void GUI()
        {
            GUIStyle darkerCyanTextFieldStyles = new GUIStyle(EditorStyles.boldLabel);
            darkerCyanTextFieldStyles.normal.textColor = new Color(0f / 255f, 190f / 255f, 190f / 255f);
            
            DrawH2Title("SDK Setting");

            EditorGUILayout.LabelField("Please Set iOS and Android Version",darkerCyanTextFieldStyles);
            SdkVersion();
            EditorGUILayout.LabelField("Please choose China or overseas:(require)", darkerCyanTextFieldStyles);
          
            ChinaOrOversea();
       
            DrawH2Title("SDK Plugins");
            EditorGUILayout.LabelField("Disable Remote Config:", darkerCyanTextFieldStyles);
            RemoteConfig();
            EditorGUILayout.LabelField("Dis Oaid:", darkerCyanTextFieldStyles);
            Oaid();
            
            
            EditorGUILayout.Space();
            
            DrawH2Title("DEEP LINKING");
            EditorGUILayout.LabelField("iOS:", darkerCyanTextFieldStyles);
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
            
          
            EditorGUILayout.LabelField("Android:", darkerCyanTextFieldStyles);
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(AndroidUrlSchemes);
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
