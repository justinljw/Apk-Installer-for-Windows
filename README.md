# Apk-Installer-for-Windows
* Prerequisites: Android Debug Bridge - Android Studio/Command line tools (https://developer.android.com)
* Initial Setup: Download and install MSIX package
* How to use: 
    - **Install apk by double click:** If you haven't launched the app before, right click any apk file (local or on remote location) and click "Open with" to select "Apk Installer for Windows" and launch the app. Otherwise, you can double click the .apk file and the Apk Installer app should be launched. Follow the instruction in the app to install apk onto WSA. At the final step, you will see adb log saying "Performing Streamed Install Success" and click done to finish the installation.
    - **Install apk by selecting file:** Launch the app and click "Select" to select an apk file to install. Follow the instruction in the app to install apk onto WSA. At the final step, you will see adb log saying "Performing Streamed Install Success" and click done to finish the installation.
    - **Install apk from AppCenter:** 
        - Get [AppCenter](https://appcenter.ms/apps) API token (login your account -> Account Settings -> User API tokens -> New API token -> Full Access -> copy token). 
        - Launch the app, click :gear: (settings) and paste the token you just copied. If you like, you can also change the download folder from "default" (app's cache folder) to any location you are preferring. Click "Save" to save settings and back to main page. 
        - Type in an app name to download and install (url would be https://appcenter.ms/orgs/appstudio-h0at/apps/Survey123-dotnet_arm64_apk and app name would be Survey123-dotnet_arm64_apk). If you like, you can also update the following textbox, release ID, from "latest" to the release you want to download (e.g., 1205). 
        - Click Install button to start downloading the apk file from AppCenter. 
        - Follow the instruction in the app to install apk onto WSA. At the final step, you will see adb log saying "Performing Streamed Install Success" and click done to finish the installation.
* Features:
    - **WSA port:** default value is "58526".
    - **AppCenter OwnerName:** default value is "appstudio-h0at". (url would be https://appcenter.ms/orgs/appstudio-h0at/applications?os=All&release_type=All and AppCenter OwnerName would be appstudio-h0at)
    - **Download Folder:** default value is "default", which means your downloaded temporary apk files will be saved to app's cache folder (normally Windows users don't have the access). By clicking "...", you can select which folder you want your apk files to be saved. 
    - **Auto Launch (beta):**
        - The feature is disabled by default.
        - Launch the app, click :gear: (settings) and click the checkbox next to "Auto Launch". Click "Save" to save settings and back to main page.
        - This feature applies to all usage methods. After clicking "Done" at the final step, you will see Apk Installer For Windows app shuts down shortly and the app you want to open will be launched in a few seconds. 
        - If you don't want your app to be auto launched, you can uncheck the checkbox next to "Auto Launch" and save the settings. 
