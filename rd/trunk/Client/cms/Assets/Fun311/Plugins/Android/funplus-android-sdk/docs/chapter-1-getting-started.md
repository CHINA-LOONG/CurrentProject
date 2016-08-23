# Getting Started

## Obtain Game ID and Game Key

Funplus SDK uniquely identifies your game with the combination of:

- Game ID - Your unique Funplus game ID.
- Game Key - Your unique Funplus developer key.

To get the game ID and game key, please contact the Funplus Team for more information.

## Add the SDK to Your Game

### Eclipse Integration

#### Step 1: Import Funplus SDK to Eclipse

1. Select `File > Import`.
2. Select `Android > Existing Android Code Into Workspace`, and click `Next`.
3. Click `Browse for Root Directory`, and choose the root directory of Funplus SDK, which is `funplus-android-<version>/funplus/`. Then click `Finish`.

#### Step 2: Enable Manifest Merger

Add this line to your project's `project.properties`.

``` shell
manifestmerger.enabled=true
```

#### Step 3: Set Funplus SDK as Dependency of Your Game

1. Select your project in `Package Explorer`.
2. Right click the project name and select `Properties`.
3. Choose `Android` from the left panel.
4. Click `Add` button, choose Funplus SDK, then click `OK`.
5. Click `Apply` and `OK`.

### Android Studio Integration

#### Step 1: Import Funplus SDK As Module

1. Select `File > New > Import Module`.
2. Choose the root directory of Funplus SDK, which is `funplus-android-<version>/funplus/`. Then click `OK`.

#### Step2: Set Funplus SDK as Dependency of Your Game

1. Select `File > Project Structure`.
2. Select your game project from the left panel, then open `dependencies` tab in the right panel.
3. Click `+` and select `Module Dependency`.
4. Add Funplus SDK, then click `OK`.

## Install Funplus SDK

When installing Funplus SDK you should pass the two tokens we've discussed at the beginning. You install the SDK by calling the `install()` method:

``` java
[FunplusSdk]
public static void install(Activity activity,
                           string gameId,
             			   string gameKey,
             			   OnInstallSdkListener listener);
```

Example:

``` java
import com.funplus.sdk.FunplusSdk;
import com.funplus.sdk.FunplusError;
import com.funplus.sdk.listeners.OnInstallSdkListener;

public class MainActivity extends Activity {

    @Override
    public void onCreate() {
        super.onCreate();
        FunplusSdk.install(this, "<YOUR_GAME_ID>", "<YOUR_GAME_KEY>", new OnInstallSdkListener() {
            @Override onSuccess() {
                // Your codes.
            }

            @Override onError(FunplusError error) {
                // Your codes.
            }
        });
    }
}
```

The `FunplusSdk.install()` has an override method which takes an extra map parameter, in which you can put your additional settings.

The following code snippets show how to enable the sandbox environment, which forces the SDK to output comprehensive device logs for the sake of debugging.

``` java
Map<String, String> settings = new HashMap<>();
settings.put("environment", "sandbox");

FunplusSdk.install(activity, "<YOUR_GAME_ID>", "<YOUR_GAME_KEY>", settings, new OnInstallSdkListener() {
    // ...
});
```