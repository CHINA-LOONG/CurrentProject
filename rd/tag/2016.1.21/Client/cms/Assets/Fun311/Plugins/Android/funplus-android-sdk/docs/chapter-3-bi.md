# BI Guide

After integrated Funplus SDK for Unity to your game project, you can now use the functions provided by the BI module.

## Event Groups

The BI module enables game developers to log valuable data to Funplus Data System. Basic types of data can be traced:

- KPI events: `session_start`, `session_end`, `new_user` and `payment`.
- Finance events: `payment` and `transaction`.


- Custom events.

You don't need to manually trace the KPI events, SDK will take care of them automatically.

## Trace a Custom Event

There is only one API to do this:

``` java
[FunplusBi]
public void traceEvent(String eventName, JSONObject properties);
```

Each event contains two parts: the metadata and the properties.

Metadata is the basic information of an event, it is needed for BI backend to identify the event's context. SDK has already taken care of event's metadata, don't bother building it by hand in your application.

Properties are what you should focus your attention on. The structure of properties is a JSON object. It is your responsibility to make sure every item in this JSON object is correct, SDK will not do any check on it.

We can now trace the `payment` event in such a form like this:

``` java
// Compose the properties object
JSONObject properties = new JSONObject();

properties.put("app_version", "xxx");
properties.put("os", "xxx");
properties.put("os_version", "xxx");
// ... any other required fields

JSONArray itemsReceived = new JSONArray();
// Compose this itemsReceived array
properties.put("c_items_received", itemsReceived);

// Trace it
FunplusBi.getInstance().traceEvent("payment", properties);
```

If anything remains unclear about the events, please consult the BI team for further information.

## Update Current User's Information

Whenever the current user's information changes, such as the user's level goes up, you need to notify Funplus SDK about that.

You notify the SDK by calling the `logUserInfoUpdate()` method, please pass `null` to blank fields:

``` java
[FunplusSdk]
public static void logUserInfoUpdate(string serverId,
                                     string userId,
                                     string userName,
                                     string userLevel,
                                     string userVipLevel,
                                     bool isPaidUser);
```

- `serverId`: The server ID where the player is in.
- `userId`: Player's in-game ID. Don't confuse this with Funplus ID.
- `userName`: Players's in-game name.
- `userLevel`: Player's current in-game level.
- `userVipLevel`: Player's current VIP level.
- `isPaidUser`: This value indicates whether the player is a paid user or not.