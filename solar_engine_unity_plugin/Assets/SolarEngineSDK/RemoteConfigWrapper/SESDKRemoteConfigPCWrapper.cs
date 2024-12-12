#if (!SOLORENGINE_DEVELOPEREDITOR)||SOLORENGINE_DISABLE_REMOTECONFIG
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using AOT;

namespace SolarEngine
{
    public partial class SESDKRemoteConfig : MonoBehaviour
    {

        private void SESDKSetRemoteDefaultConfig(Dictionary<string, object>[] defaultConfig)
        {
            Debug.Log("Unity Editor: SESDKSetRemoteConfigEventProperties");


        }


        private void SESDKSetRemoteConfigEventProperties(Dictionary<string, object> properties)
        {

            Debug.Log("Unity Editor: SESDKSetRemoteConfigEventProperties");
        }

        private void SESDKSetRemoteConfigUserProperties(Dictionary<string, object> properties)
        {

            Debug.Log("Unity Editor: SESDKSetRemoteConfigUserProperties");

        }


        private string SESDKFastFetchRemoteConfig(string key)
        {
            Debug.Log("Unity Editor: SESDKFastFetchRemoteConfig");
            return  null;
        }

        private Dictionary<string, object> SESDKFastFetchAllRemoteConfig()
        {

            Debug.Log("Unity Editor: SESDKFastFetchAllRemoteConfig");
            return null;

        }

        private void SESDKAsyncFetchAllRemoteConfig()
        {

            Debug.Log("Unity Editor: SESDKAsyncFetchAllRemoteConfig");

        }

        private void SESDKAsyncFetchRemoteConfig(string key)
        {

            Debug.Log("Unity Editor: SESDKAsyncFetchRemoteConfig ");
        }

    }


}
#endif