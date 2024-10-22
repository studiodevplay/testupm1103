using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
// using SolarEngine.Sample;
using AOT;

namespace SolarEngine
{
    public partial class SESDKRemoteConfig : MonoBehaviour
    {

        private FetchRemoteConfigCallback fetchRemoteConfigCallback_private = null;
        public delegate void FetchRemoteConfigCallback(string result);

        private FetchAllRemoteConfigCallback fetchAllRemoteConfigCallback_private = null;
        public delegate void FetchAllRemoteConfigCallback(Dictionary<string, object> result);

        private static List<Action> waitingTaskList = new List<Action>();
        private static List<Action> executingTaskList = new List<Action>();

        private static SESDKRemoteConfig _instance = null;

        public static SESDKRemoteConfig Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType(typeof(SESDKRemoteConfig)) as SESDKRemoteConfig;
                    if (!_instance)
                    {
                        GameObject am = new GameObject("SESDKRemoteConfig");
                        _instance = am.AddComponent(typeof(SESDKRemoteConfig)) as SESDKRemoteConfig;
                    }
                }
                return _instance;
            }
        }

        public static void PostTask(Action task)
        {
            lock (waitingTaskList)
            {
                waitingTaskList.Add(task);
            }
        }

        private void Update()
        {
            lock (waitingTaskList)
            {
                if (waitingTaskList.Count > 0)
                {
                    executingTaskList.AddRange(waitingTaskList);

                    waitingTaskList.Clear();
                }
            }

            for (int i = 0; i < executingTaskList.Count; ++i)
            {
                Action task = executingTaskList[i];
                try
                {
                    task();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message, this);
                }
            }

            executingTaskList.Clear();
        }

        /// <summary>
        /// 设置默认配置，在线参数SDK需要开发者预置一份默认配置到用户app中，方便在线参数SDK使用此默认配置进行兜底操作。
        /// </summary>
        /// <param name="defaultConfig">默认配置/param>
        public void SetRemoteDefaultConfig(Dictionary<string, object>[] defaultConfig)
        {
            SESDKSetRemoteDefaultConfig(defaultConfig);
        }

        /// <summary>
        /// 设置自定义事件属性，请求参数配置时后端会使用该属性匹配
        /// </summary>
        /// <param name="properties">跟在后台页面设置的事件属性对应</param>
        public void SetRemoteConfigEventProperties(Dictionary<string, object> properties)
        {
            SESDKSetRemoteConfigEventProperties(properties);
        }

        /// <summary>
        /// 设置自定义用户属性，请求参数配置时后端会使用该属性匹配
        /// </summary>
        /// <param name="properties">跟在后台页面设置的用户属性对应</param>
        public void SetRemoteConfigUserProperties(Dictionary<string, object> properties)
        {
            SESDKSetRemoteConfigUserProperties(properties);
        }

        /// <summary>
        /// 同步获取参数配置
        /// 优先从缓存配置查询，查询不到则从默认配置查询，都查询不到则返回nil
        /// </summary>
        /// <param name="key">在后台页面设置的参数key，命中则返回对应值value</param>
        public string FastFetchRemoteConfig(string key)
        {
            return SESDKFastFetchRemoteConfig(key);
        }

        /// <summary>
        /// 同步获取所有参数配置
        /// 优先从缓存配置查询，查询不到则从默认配置查询，都查询不到则返回nil
        /// <returns>Dictionary 字典，代表所有参数配置</returns>
        /// </summary>
        public Dictionary<string, object> FastFetchRemoteConfig()
        {
            return SESDKFastFetchAllRemoteConfig();
        }

        /// <summary>
        /// 异步获取参数配置，回调方法：OnRemoteConfigFetchCompletion
        /// 请求服务端配置后与本地缓存配置合并，然后从缓存配置查询，查询不到则从默认配置中查询，都查询不到则返回nil 
        /// </summary>
        /// <param name="key">在后台页面设置的参数key，命中则返回对应值value</param>
        /// <param name="callback">参数配置异步回调</param>
        public void AsyncFetchRemoteConfig(string key, FetchRemoteConfigCallback callback)
        {
            SESDKRemoteConfig.Instance.fetchRemoteConfigCallback_private = callback;
            SESDKAsyncFetchRemoteConfig(key);
        }

        /// <summary>
        /// 异步获取所有参数配置
        /// 请求服务端配置后与本地缓存配置合并，然后从缓存配置查询，查询不到则从默认配置中查询，都查询不到则返回nil 
        /// </summary>
        /// <param name="callback">参数配置异步回调</param>
        public void AsyncFetchRemoteConfig(FetchAllRemoteConfigCallback callback)
        {
            SESDKRemoteConfig.Instance.fetchAllRemoteConfigCallback_private = callback;
            SESDKAsyncFetchAllRemoteConfig();
        }
        
        #region 新增set方法

        public Dictionary<string, object> stringItem(string name, string value)
        {
            Dictionary<string, object> configDict = new Dictionary<string, object>();
            configDict.Add("name", name);
            configDict.Add("value", value);
            configDict.Add("type", 1);
            return configDict;
        }

        public Dictionary<string, object> intItem(string name, int value)
        {
            Dictionary<string, object> configDict = new Dictionary<string, object>();
            configDict.Add("name", name);
            configDict.Add("value", value);
            configDict.Add("type", 2);
            return configDict;
        }


        public Dictionary<string, object> boolItem(string name, bool value)
        {
            Dictionary<string, object> configDict = new Dictionary<string, object>();
            configDict.Add("name", name);
            configDict.Add("value", value);
            configDict.Add("type", 3);
            return configDict;
        }

        public Dictionary<string, object> jsonItem(string name, string value)
        {
            Dictionary<string, object> configDict = new Dictionary<string, object>();
            configDict.Add("name", name);
            configDict.Add("value", value);
            configDict.Add("type", 4);
            return configDict;
        }

        #endregion

    private static void OnFetchAllRemoteConfigCallback(Dictionary<string, object> result)
    {

        SESDKRemoteConfig.PostTask(() =>
        {
            if (SESDKRemoteConfig.Instance.fetchAllRemoteConfigCallback_private != null)
            {
                SESDKRemoteConfig.Instance.fetchAllRemoteConfigCallback_private.Invoke(result);
            }
        });

    }

  private static void OnFetchRemoteConfigCallback(String result)
     {

         SESDKRemoteConfig.PostTask(() =>
         {
             if (SESDKRemoteConfig.Instance.fetchRemoteConfigCallback_private != null)
             {
                 SESDKRemoteConfig.Instance.fetchRemoteConfigCallback_private.Invoke(result);
             }
         });

     }

    }
}
        
    
