
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

        private const string storageWarning = "You can only choose either China or Overseas！";
        private const string nostorageWarning = "You must choose either China or Overseas!";
        private const string confirm = "confirm";
        private const string cancel = "cancel";
        private const string SolorEngine = "[SolorEngine]";

        private SerializedProperty chinaProperty;
        private SerializedProperty overseaProperty;
        
        private SerializedProperty useRemoteConfig;
        private SerializedProperty useOaid;
        private SerializedProperty useDeepLink;
        private  SerializedProperty useSpecifyVersion;
        
        
        
        
        private  SerializedProperty iOSRemoteConfig;
        private SerializedProperty androidRemoteConfig;
        private SerializedProperty miniGameRemoteConfig;
        
        
        
        
    
        
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
            useRemoteConfig = serializedObject.FindProperty("_RemoteConfig");
            useOaid= serializedObject.FindProperty("_Oaid");
            useDeepLink= serializedObject.FindProperty("_DeepLink");
            useSpecifyVersion = serializedObject.FindProperty("_SpecifyVersion");
            
            
            
            
            // disAllRemoteConfig= serializedObject.FindProperty("_All");
            iOSRemoteConfig= serializedObject.FindProperty("_iOS");
            androidRemoteConfig= serializedObject.FindProperty("_Android");
            miniGameRemoteConfig= serializedObject.FindProperty("_MiniGame");

            
            oldChinaValue= chinaProperty.boolValue;
            oldOverseaValue= overseaProperty.boolValue;
            // oldDisAllValue= disAllRemoteConfig.boolValue;
            // oldDisiOSValue= disiOSRemoteConfig.boolValue;
            // oldDisAndroidValue= disAndroidRemoteConfig.boolValue;
            // oldDisMiniGameValue= disMiniGameRemoteConfig.boolValue;
            oldDisOaidValue= useOaid.boolValue;
         
        }


        public override void OnInspectorGUI()
        {
          
            this.GUI();
            
        }
     
        private void ChinaOrOversea(  GUIStyle darkerCyanTextFieldStyles)
        {
        
           // EditorGUILayout.LabelField("Please set up your data storage area.", darkerCyanTextFieldStyles);
            EditorGUILayout.HelpBox("Please set up your data storage area.", MessageType.Info);
            
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(chinaProperty);
            EditorGUILayout.PropertyField(overseaProperty);
            EditorGUI.indentLevel -= 1;
            if (chinaProperty.boolValue && overseaProperty.boolValue)
            {
                EditorGUILayout.HelpBox(storageWarning, MessageType.Warning);
            }
           
            if (serializedObject.ApplyModifiedProperties())
            {
                
                // 处理 China 值变化
                ProcessPropertyChange(chinaProperty, ref oldChinaValue, "_China",null, () =>
                {
                    overseaProperty.boolValue = false;
                    oldOverseaValue = overseaProperty.boolValue;
                    
                });
            
                // 处理 Oversea 值变化
                ProcessPropertyChange(overseaProperty, ref oldOverseaValue, "_Oversea",null, () =>
                {
                    chinaProperty.boolValue = false;
                    oldChinaValue = chinaProperty.boolValue;
                    if (overseaProperty.boolValue)
                    {
                        useOaid.boolValue = false;

                    }
                  
                });
            }
        }




        bool changleStorageValue()
        {
            if (chinaProperty.boolValue)
            {
             return   XmlModifier.cnxml(true);
            }
            else if (overseaProperty.boolValue)
            {
                return   XmlModifier.Overseaxml(true);
            }

            return false;

        }
     
        private void RemoteConfig()
        {
          
            
            EditorGUILayout.PropertyField(useRemoteConfig);
            if (!useRemoteConfig.boolValue)
            {  
               
                EditorGUILayout.HelpBox("You can choose which platform's RemoteConfig to cancel", MessageType.Info);
                
                EditorGUI.indentLevel += 1;
                // EditorGUILayout.PropertyField(disAllRemoteConfig);
                EditorGUILayout.PropertyField(iOSRemoteConfig);
                EditorGUILayout.PropertyField(androidRemoteConfig);
                EditorGUILayout.PropertyField(miniGameRemoteConfig);
                EditorGUI.indentLevel -= 1;
                
            }
            else
            {
                iOSRemoteConfig.boolValue = true;
                androidRemoteConfig.boolValue = true;
                miniGameRemoteConfig.boolValue = true;
            }

        }
        private void UseOaid()
        {
            if (chinaProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Oaid cannot be disable in the China storage", MessageType.Info);
                useOaid.boolValue = true;

            }

            if (overseaProperty.boolValue)
            {
              

                if (useOaid.boolValue)
                {
                    EditorGUILayout.HelpBox("Please confirm whether you want to enable Oaid in Oversea", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("Oaid is disable in the Oversea storage", MessageType.Info);
                }
            }
            EditorGUILayout.PropertyField(useOaid);
          
        }

      

        private void UseDeepLink( GUIStyle darkerCyanTextFieldStyles )
        {
            
            EditorGUILayout.PropertyField(useDeepLink, new GUIContent("DeepLink"));
            if (useDeepLink.boolValue)
            {
                EditorGUI.indentLevel += 1;
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
                EditorGUI.indentLevel -= 1;
            }
        }
        
        private void SdkVersion(GUIStyle darkerCyanTextFieldStyles)
        {
            EditorGUILayout.PropertyField(useSpecifyVersion);
            if (useSpecifyVersion.boolValue)
            {
                EditorGUILayout.HelpBox("Please Set iOS and Android Version", MessageType.Info);

                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(iOSVersion,new GUIContent("iOS Version"));
                EditorGUILayout.PropertyField(AndroidVersion);
                EditorGUI.indentLevel -= 1;
                if (!iOSVersion.stringValue.Equals(SolarEngineSettings.iOSVersion))
                    SolarEngineSettings.iOSVersion=iOSVersion.stringValue;
                if( !AndroidVersion.stringValue.Equals(SolarEngineSettings.AndroidVersion))
                    SolarEngineSettings.AndroidVersion = AndroidVersion.stringValue;
            }
         
       
           
        }


       
        
        
        private void ProcessPropertyChange(SerializedProperty property, ref bool oldValue, string propertyName, System.Action<bool> xmlAction, System.Action additionalAction = null)
        {
       
            if (property.boolValue!= oldValue)
            {
         
                oldValue = property.boolValue;
                additionalAction?.Invoke();
            }
        }
       
        public void GUI()
        {
            GUIStyle darkerCyanTextFieldStyles = new GUIStyle(EditorStyles.boldLabel);
            //darkerCyanTextFieldStyles.normal.textColor = Color.white;
         
            DrawH2Title("SDK Setting");

          
            ChinaOrOversea(darkerCyanTextFieldStyles);
       
            DrawH2Title("SDK Plugins");
            RemoteConfig();
            UseOaid();

            UseDeepLink( darkerCyanTextFieldStyles);
            
    
            
            SdkVersion(darkerCyanTextFieldStyles);


            ApplyButton();
            
        
          
            
            serializedObject.ApplyModifiedProperties();
        }


        private void ApplyButton()
        {
            GUIStyle buttonStyle = new GUIStyle();
            buttonStyle.normal.textColor = Color.white;
            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, Color.gray);
            backgroundTexture.Apply();
            buttonStyle.normal.background = backgroundTexture;
            buttonStyle.fixedHeight = 25;
            buttonStyle.fixedWidth = 100;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            
            
            if (GUILayout.Button("Apply", buttonStyle))          
            {
                if (chinaProperty.boolValue && overseaProperty.boolValue)
                {
                    EditorUtility.DisplayDialog("fail", storageWarning, confirm);
                    return;
                    
                }else if (!chinaProperty.boolValue && !overseaProperty.boolValue)
                {
                    EditorUtility.DisplayDialog("fail", nostorageWarning, confirm);
                    return;
                }
                string storage = chinaProperty.boolValue ? "China" : "Oversea";
                
                bool result = EditorUtility.DisplayDialog("Confirm Operation",
                    $"The current data storage area is set to  {storage}.\n " +
                    $"Are you sure you want to perform this operation? ",
                    confirm,
                    cancel);
                if (result)
                {
                    if (Apply())
                    {
                        ShowTips("Successfully",  "SolarEngineSDK Successfully Matching Settings Panel");
                    }
                      
                }
                
                else
                    Debug.Log("You cancelled the operation");
            }
        }

        //用户应用
        public  bool Apply()
        {
        return  iOSRemoteConfigValue()&&
           androidRemoteConfigValue()&&
           miniGameRemoteConfigValue()&&
             
           OaidValue()&&

           changleStorageValue();
         

        }


        private bool OaidValue()
        {
            if (useOaid.boolValue)
            {
              return  PluginsEdtior.showOaid();
            }
            else
            {
              return  PluginsEdtior.disableOaid();
            }
          
        }
   

        bool iOSRemoteConfigValue()
        {
            if (iOSRemoteConfig.boolValue)
            {
              
                DefineSymbolsEditor.delete_DISABLE_REMOTECONFIG(BuildTargetGroup.iOS);
                return  PluginsEdtior.showiOS();
            }
               
            else
            {
               
                DefineSymbolsEditor.add_DISABLE_REMOTECONFIG(BuildTargetGroup.iOS);
                return  PluginsEdtior.disableiOS();


            }
        }
        bool androidRemoteConfigValue()
        {

            if (androidRemoteConfig.boolValue)
            {
              
                DefineSymbolsEditor.delete_DISABLE_REMOTECONFIG(BuildTargetGroup.Android);
              return  PluginsEdtior.showAndroid();
            

            }
              
            else
            {
              
                DefineSymbolsEditor.add_DISABLE_REMOTECONFIG(BuildTargetGroup.Android);
                 return PluginsEdtior.disableAndroid();
               

            }
        }
        bool miniGameRemoteConfigValue()
        {

            if (miniGameRemoteConfig.boolValue)
            {  
               
                DefineSymbolsEditor.delete_DISABLE_REMOTECONFIG(BuildTargetGroup.WebGL);
               return  PluginsEdtior.showMiniGame();
            }
             
            else
            {
                
            
              
                DefineSymbolsEditor.add_DISABLE_REMOTECONFIG(BuildTargetGroup.WebGL);
                return   PluginsEdtior.disableMiniGame();
            }
            
        }
        

       
        
        
        
        
        
    
        // 通用标签间的间距
        private const float COMMON_SPACE = 13f;

        // 最大间距
        private const float MAX_SPACE = 25f;

        protected void DrawH2Title(string title)
        {
            DrawAreaTitle(title, Color.black, TextAnchor.MiddleLeft, 16);
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
        
        
        
        
        
        /// <summary>
        /// 展示提示.
        /// </summary>
        /// <param name="title">标题.</param>
        /// <param name="content">具体内容.</param>
        public static void ShowTips(string title, string content)
        {
            // 展示提示信息.
            EditorUtility.DisplayDialog(title, content, "OK");
        }

        
        
        

    }



}
