import {
    booleanItem,
    ConfigItem,
    jsonItem, numberItem,
    RemoteConfigManager,
    safeBoxingConfigItem, stringItem
} from '@solarengine/remoteconfig';
import {SEOpenHarmonyProxy, _log, _SolarEngineLog} from './SEOpenHarmonyProxy';


export class SERCOpenHarmonyProxy {


    static setRemoteDefaultConfig(defaultConfig: string): void {


        const remoteDefaultConfigJsonObject = SEOpenHarmonyProxy.parseJsonStrict(defaultConfig);

        let _defaultConfig: Array<ConfigItem> = new Array();
        Object.keys(remoteDefaultConfigJsonObject).forEach(key => {
            const value = remoteDefaultConfigJsonObject[key];

            let nameItem = value[SERCConstant.NAME];
            let valueItem = value[SERCConstant.VALUE];

            let type = value[SERCConstant.TYPE];
            if (_log) {
                console.log(`${_SolarEngineLog} setRemoteDefaultConfig: Item  ${valueItem}: ${nameItem} :${type}`);
            }

            switch (type) {
                case 1 :
                    let item1 = stringItem(nameItem, valueItem);
                    safeBoxingConfigItem(_defaultConfig, item1);

                    break;
                case 2 :
                    let item2 = numberItem(nameItem, valueItem);
                    safeBoxingConfigItem(_defaultConfig, item2);
                    break;
                case 3 :
                    let item3 = booleanItem(nameItem, valueItem);
                    safeBoxingConfigItem(_defaultConfig, item3);
                    break;
                case 4 :
                    let item4 = jsonItem(nameItem, valueItem);
                    safeBoxingConfigItem(_defaultConfig, item4);
                    break;
            }


        });


        RemoteConfigManager.sharedInstance().setDefaultConfigs(_defaultConfig);
    }

    static setRemoteConfigEventProperties(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "setRemoteConfigEventProperties", properties);
        }
        const eventProperties = SEOpenHarmonyProxy.parseJsonStrict(properties) as Record<string, string | number | boolean> || {};
        RemoteConfigManager.sharedInstance().setRemoteConfigEventProperties(eventProperties);

    }

    static setRemoteConfigUserProperties(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "setRemoteConfigUserProperties", properties);
        }
        const userProperties = SEOpenHarmonyProxy.parseJsonStrict(properties) as Record<string, string | number | boolean> || {};
        ;
        RemoteConfigManager.sharedInstance().setRemoteConfigUserProperties(userProperties);

    }


    static fastFetchRemoteConfig(key: string, callback: (value: string) => void) {
        if (_log) {
            console.log(_SolarEngineLog, "fastFetchRemoteConfig", key);
        }
        RemoteConfigManager.sharedInstance().fastFetchRemoteConfig(key, (configItem: ConfigItem | null) => {
            if (configItem == null) {
                if (_log)
                    console.log(_SolarEngineLog, "RemoteConfig", "key_string type is:  null");
                callback?.(null);
            } else {
                var type=typeof configItem?.value;
                var value=configItem?.value;

                if (_log)
                    console.log(_SolarEngineLog, "RemoteConfig", "key_string type is: " + type + " value: " + value);
                if (typeof configItem?.value === "object" && configItem?.value !== null) {
                    callback?.(JSON.stringify(value));
                }else{
                    callback?.(value.toString());
                }



            }
        })
    }


    static asyncFetchRemoteConfig(key: string, callback: (value: string) => void) {
        if (_log) {
            console.log(_SolarEngineLog, "asyncFetchRemoteConfig", key);
        }
        RemoteConfigManager.sharedInstance().asyncFetchRemoteConfig(key, (configItem: ConfigItem | null) => {
            if (configItem == null) {
                if (_log)
                    console.log(_SolarEngineLog, "RemoteConfig", "key_string type is:  null");
                callback?.(null);
            } else {
                var type=typeof configItem?.value;
                var value=configItem?.value;

                if (_log)
                    console.log(_SolarEngineLog, "RemoteConfig", "key_string type is: " + type + " value: " + value);
                if (typeof configItem?.value === "object" && configItem?.value !== null) {
                    callback?.(JSON.stringify(value));
                }else{
                    callback?.(value.toString());
                }



            }
        })
    }

    static fastAllFetchRemoteConfig(callback: (value: string) => void): void {
        if (_log) {
            console.log(_SolarEngineLog, "fastAllFetchRemoteConfig");
        }
        RemoteConfigManager.sharedInstance().fastAllFetchRemoteConfig((configItemsMap: Map<string, ConfigItem>) => {

            const configArray = Array.from(configItemsMap.values());

            const config: Record<string, object> = {};
            const jsonString = JSON.stringify(configArray, null, 2);
            if (_log) {
                console.log(_SolarEngineLog, 'fastAllFetchRemoteConfig:', jsonString);
            }
            for (const item of configArray) {

                let nameItem=item.key;
                let valueItem=item.value;
                if(_log)
                    console.log(_SolarEngineLog,`Key: ${item.key}, Value: ${item.value}， ${ item.type}`);
                config[nameItem] = { valueItem };

            }
            if(_log)
                console.log(_SolarEngineLog,JSON.stringify(config));
            callback?.(JSON.stringify(config));
        });
    }

    static asyncAllFetchRemoteConfig(callback: (value: string) => void): void {
        if (_log) {
            console.log(_SolarEngineLog, "asyncAllFetchRemoteConfig");
        }
        RemoteConfigManager.sharedInstance().asyncAllFetchRemoteConfig((configItemsMap: Map<string, ConfigItem>) => {

            const configArray = Array.from(configItemsMap.values());

            const config: Record<string, object> = {};
            const jsonString = JSON.stringify(configArray, null, 2);
            if (_log) {
                console.log(_SolarEngineLog, 'asyncAllFetchRemoteConfig:', jsonString);
            }
            for (const item of configArray) {

                let nameItem=item.key;
                let valueItem=item.value;
                if(_log)
                    console.log(_SolarEngineLog,`Key: ${item.key}, Value: ${item.value}， ${ item.type}`);
                config[nameItem] = { valueItem };

            }
            if(_log)
                console.log(_SolarEngineLog,JSON.stringify(config));
            callback?.(JSON.stringify(config));
        });
    }


}

export class SERCConstant {
    // 应用内广告相关常量
    public static readonly NAME: string = "name";
    public static readonly VALUE: string = "value";
    public static readonly TYPE: string = "type";
}