#if SOLARENGINE_BYTEDANCE_CLOUD&&(!UNITY_EDITOR||SOLORENGINE_DEVELOPEREDITOR)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SolarEngine.MiniGames.info;
using SolarEngine.MiniGames.Utils;
using StarkSDKSpace;
using UnityEngine;

namespace SolarEngine.Platform
{
    public class ByteDanceStarkSDKAdapter : SEAdapterInterface
    {
        private bool islogin = false;
        string filePath = Application.persistentDataPath + "/SolarEngineData.json";
        Dictionary<string ,object> result = new Dictionary<string, object>();
        public SEDeviceInfo setDeviceInfo()
        {
            var sysInfo = StarkSDK.API.GetSystemInfo();
            LogTool.DebugLog("sysInfo" + JsonConvert.SerializeObject(sysInfo));
            string[] tempSys = (sysInfo.system?.Split(' ')) ?? new string[0]; // 最细粒度的系统版本号
            SEDeviceInfo seDeviceInfo = new SEDeviceInfo
            {
                _manufacturer = sysInfo.brand,
                _device_model = sysInfo.model,
                _device_type = AdapterTool.getDeviceType(sysInfo.platform),
                _platform = AdapterTool.getPlatform(sysInfo.platform),
                _os_version = tempSys.Length > 1 ? tempSys[1] : sysInfo.system,
                _screen_height = sysInfo.screenHeight,
                _screen_width = sysInfo.screenWidth,
                _language = sysInfo.language,
                //App 的版本，小程序下为宿主 App 版本号（宿主指微信、抖音、今日头条等）
                _app_version = sysInfo.hostVersion,
                //小程序客户端基础库版本
                _mp_version = sysInfo.sdkVersion,
                _browser_version = sysInfo.hostVersion,
                _app_name = sysInfo.hostName,
                _browser = ""
            };
            return seDeviceInfo;

        }


        public void CacheDit()
        {
             
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                Dictionary<string, object> loadedDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                if (loadedDic != null)
                {
                    foreach (var entry in loadedDic)
                    {
                        result[entry.Key]= entry.Value;
                    }
                }
               
            }
            else
            {
                Debug.LogWarning("File does not exist: " + filePath);
            }
          
        }

        public void savePath()
        {
            string jsonString = JsonConvert.SerializeObject(result);
            File.WriteAllText(filePath, jsonString);
        }
        public void saveData(string key, object value)
        { 
            result[ key]= value;
          if(islogin)
              savePath();
          
        }
        public bool  hasKey(string key)
        {
          
           return result.ContainsKey(key);
        }

        public void init()
        {
           
           CacheDit();
           savePath();
           islogin = true;
    
        }
      
        public object getData(string key, Type type)
        {
            Debug.LogWarning(filePath);
            if (result.ContainsKey(key))
            {
            
                if (type == typeof(int))
                {
                    return int.Parse(result[key].ToString());
                }
                else if (type == typeof(float))
                {
                    return float.Parse(result[key].ToString());
                }
                else if (type == typeof(string))
                {
                   return result[key].ToString();
                }
            
               
            }
            
      
            return null;

            // if (!string.IsNullOrEmpty(key) && StarkSDK.API.PlayerPrefs.HasKey(key))
            // {
            //
            //     if (type == typeof(int))
            //     {
            //         return StarkSDK.API.PlayerPrefs.GetInt(key);
            //     }
            //     else if (type == typeof(float))
            //     {
            //         return StarkSDK.API.PlayerPrefs.GetFloat(key);
            //     }
            //     else if (type == typeof(string))
            //     {
            //         return StarkSDK.API.PlayerPrefs.GetString(key);
            //     }
            //
            //     StarkSDK.API.PlayerPrefs.Save();
            // }
            //
            // return null;
        }

        public void deleteData(string key)
        {
            
            if (result.ContainsKey(key))
            {
                result.Remove(key);
               
              savePath();
            }
        }

        public void deleteAll()
        {
            StarkSDK.API.PlayerPrefs.DeleteAll();
        }


        public EnterOptionsInfo getEnterOptionsInfo()
        {
         
            LaunchOption launchOptionsSync = StarkSDK.API.GetLaunchOptionsSync();

            string scene = launchOptionsSync.Scene;
            Dictionary<string, object> newRefererInfo = new Dictionary<string, object>();
            Dictionary<string, string> RefererInfo = launchOptionsSync.RefererInfo;

#if UNITY_EDITOR
   RefererInfo = new Dictionary<string, string>()
            {
                { "testappId", "123" },
                { "extraData", "rergh" }

            };
#endif
            foreach (var kvp in RefererInfo)
            {
                newRefererInfo.Add(kvp.Key, kvp.Value);
            }

            EnterOptionsInfo enterOptionsInfo = new EnterOptionsInfo
            {
                _launch_scene = (!string.IsNullOrEmpty(scene)) ? scene : "1000",
                _path = !string.IsNullOrEmpty(launchOptionsSync.Path) ? launchOptionsSync.Path : "",

                _query_info = launchOptionsSync.Query,
                _referrer_info = newRefererInfo
            };
            LogTool.DebugLog(JsonConvert.SerializeObject(enterOptionsInfo));
            return enterOptionsInfo;
        }

        public void login(SEAdapterInterface.OnLoginSuccessCallback successCallback,
            SEAdapterInterface.OnLoginFailedCallback failedCallback, bool forceLogin = true)
        {
           
           
            StarkSDK.API.GetAccountManager(). Login(
                (c1, c2, isLogin) =>
                {
                    successCallback?.Invoke(c1, c2, isLogin);
                  
                  
                },
                    
                
                (errMsg) => failedCallback?.Invoke(errMsg),
                forceLogin
            );
            

        }

        public void triggerOnShow(SEAdapterInterface.OnShowEvent showEvent)
        {
            StarkSDK.API.GetStarkAppLifeCycle().OnShowWithDict += (dic) =>
            {

                string scene = "";
                Dictionary<string, string> query = new Dictionary<string, string>();
                Dictionary<string, object> RefererInfo = new Dictionary<string, object>();
                if (dic.Count != 0)
                {
                    if (dic.ContainsKey("scene"))
                    {
                        scene = dic["scene"].ToString();
                    }

                    if (dic.ContainsKey("query"))
                    {

                        query = (Dictionary<string, string>)dic["query"];
                        
                    }

                    if (dic.ContainsKey("refererInfo"))
                            {
                            Dictionary<string ,string> refererInfo = (Dictionary<string, string>)dic["refererInfo"];
                                foreach (var VARIABLE in refererInfo)
                                {
                                    
                                    RefererInfo.Add(VARIABLE.Key, VARIABLE.Value);
                                    
                                }

                                // 在这里可以继续对获取到的scene、query、refererInfo进行后续的联合操作等

                            }

                            showEvent?.Invoke(scene, query, RefererInfo);

                            // 触发 OnShow 事件

                        }

                    
                
            };
        }


        public void triggerOnHide(SEAdapterInterface.OnHideEvent hideEvent)
        {
            StarkSDK.API.GetStarkAppLifeCycle().OnHide += () =>
            {
                // 触发 OnHide 事件
                hideEvent?.Invoke();
            };
        }

        public string getmptype()
        {
            return "douyin";
        }

        public string getsubmptype()
        {
          
          if(StarkUtils.IsCloudRuntime())
              return "cloud";
          else
              return "native";
        }
    }
}
#endif