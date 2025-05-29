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
            const value:Record<string, Object >= remoteDefaultConfigJsonObject[key];

            let nameItem:string = value[SERCConstant.NAME].toString();
            let valueItem:object = value[SERCConstant.VALUE];

            let type = value[SERCConstant.TYPE];
            if (_log) {
                console.log(`${_SolarEngineLog} setRemoteDefaultConfig: Item  ${valueItem}: ${nameItem} :${type}`);
            }

            switch (type) {
                case 1 :
                    let item1 = stringItem(nameItem, valueItem.toString());
                    safeBoxingConfigItem(_defaultConfig, item1);

                    break;
                case 2 :
                    let item2 = numberItem(nameItem, Number(valueItem) );
                    safeBoxingConfigItem(_defaultConfig, item2);
                    break;
                case 3 :
                    let item3 = booleanItem(nameItem,Boolean(valueItem));
                    safeBoxingConfigItem(_defaultConfig, item3);
                    break;
                case 4 :
                    let item4 = jsonItem(nameItem, valueItem as Record<string, Object>);
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

        RemoteConfigManager.sharedInstance().setRemoteConfigUserProperties(userProperties);

    }


    static fastFetchRemoteConfig(key: string, callback: (value: string) => void) {
        if (_log) {
            console.log(_SolarEngineLog, "fastFetchRemoteConfig", key);
        }
        RemoteConfigManager.sharedInstance().fastFetchRemoteConfig(key, (configItem: ConfigItem | null) => {
            if (configItem == null) {
                if (_log)
                    console.log(_SolarEngineLog, "fastFetchRemoteConfig", key , ": null");
                callback?.("");
            } else {
                const type=typeof configItem?.value;
                const value=configItem?.value;

                if (_log)
                    console.log(_SolarEngineLog, "fastFetchRemoteConfig", key ," type:", type + " value: " + value);
                if(configItem?.value !== null){
                   if(typeof configItem?.value === "object")
                       callback?.(JSON.stringify(value));
                   else
                       callback?.(configItem?.value?.toString());

                }else
                    callback?.("");



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
                    console.log(_SolarEngineLog, "asyncFetchRemoteConfig", key , ": null");
                callback?.("");
            } else {
                const type=typeof configItem?.value;
                const value=configItem?.value;

                if (_log)
                    console.log(_SolarEngineLog, "asyncFetchRemoteConfig", key ," type:", type + " value: " + value);
                if(configItem?.value !== null){
                    if(typeof configItem?.value === "object")
                        callback?.(JSON.stringify(value));
                    else
                        callback?.(configItem?.value?.toString());

                }else
                    callback?.("");



            }
        })
    }

    static fastAllFetchRemoteConfig(callback: (value: string) => void): void {
        if (_log) {
            console.log(_SolarEngineLog, "fastAllFetchRemoteConfig");
        }
        RemoteConfigManager.sharedInstance().fastAllFetchRemoteConfig((configItemsMap: Map<string, ConfigItem>) => {

            const configArray = Array.from(configItemsMap.values());

            const config: Record<string, string | boolean | number | Object | null> = {};
            const jsonString = JSON.stringify(configArray, null, 2);
            if (_log) {
                console.log(_SolarEngineLog, 'fastAllFetchRemoteConfig:', jsonString);
            }
            for (const item of configArray) {

                let nameItem=item.key;
                let valueItem=item.value;
                if(_log)
                    console.log(_SolarEngineLog,`Key: ${item.key}, Value: ${item.value}， ${ item.type}`);

                if (nameItem !== undefined && nameItem !== null) {
                    config[nameItem.toString()] = valueItem;
                } else {
                    console.error(_SolarEngineLog,"Invalid or missing key:", item);
                }

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

            const config: Record<string, string | boolean | number | Object | null> = {};
            const jsonString = JSON.stringify(configArray, null, 2);
            if (_log) {
                console.log(_SolarEngineLog, 'asyncAllFetchRemoteConfig:', jsonString);
            }
            for (const item of configArray) {

                let nameItem=item.key;
                let valueItem:string | boolean | number | Object | null=item.value;
                if(_log)
                    console.log(_SolarEngineLog,`Key: ${item.key}, Value: ${item.value}， ${ item.type}`);
                if (nameItem !== undefined && nameItem !== null) {
                    config[nameItem.toString()] =valueItem;
                } else {
                    console.error(_SolarEngineLog,"Invalid or missing key:", item);
                }

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