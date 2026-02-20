# Android Build Guide - Beginner-Friendly Setup

**Target**: Install Android development environment, configure Unity for Android, build your first APK, test on device/emulator, and optimize build size.  
**Time**: ~2-3 hours (mostly waiting for downloads/builds)  
**Prerequisites**: Unity project ready, computer with 10GB+ free space  
**Result**: Signed APK built and tested on Android device  

---

## TABLE OF CONTENTS
1. [System Requirements](#system-requirements)
2. [Install Android Development Tools](#install-android-development-tools)
3. [Configure Unity for Android](#configure-unity-for-android)
4. [Create Keystore (for APK Signing)](#create-keystore-for-apk-signing)
5. [Build Your First APK](#build-your-first-apk)
6. [Test APK on Device/Emulator](#test-apk-on-deviceemulator)
7. [Build Size Optimization](#build-size-optimization)
8. [Common Errors & Solutions](#common-errors--solutions)

---

## SYSTEM REQUIREMENTS

### Computer Requirements:

- **OS**: Windows 10+, Mac OS X 10.13+, or Linux (Ubuntu 18.04+)
- **Free Disk Space**: 10-15 GB minimum
  - 2 GB: Android SDK
  - 5 GB: Android NDK
  - 3 GB: Java Development Kit (JDK)
  - 5 GB: Build cache & temporary files
- **RAM**: 4 GB minimum (8 GB recommended)
- **Processor**: Any modern CPU (2+ cores)

### Android Device/Emulator Requirements:

- **Device**: Android 6.0 (API level 23) or higher
- **OR Emulator**: Android Studio emulator with 2GB+ RAM allocated
- **USB Cable**: For device testing
- **USB Debugging**: Enabled on device

---

## INSTALL ANDROID DEVELOPMENT TOOLS

### Step 1: Install Java Development Kit (JDK)

Unity requires JDK to build Android apps.

#### On Windows:

1. **Download JDK 11** (recommended):
   - Go to: https://www.oracle.com/java/technologies/downloads/
   - Download: **JDK 11** (Windows x64 installer)
   - Run installer, accept defaults
   - Note the installation path (usually: `C:\Program Files\Java\jdk-11.x.x`)

2. **Set Java Path in Unity**:
   - Open Unity → Edit → Preferences (on Mac: Unity → Settings)
   - Go to: External Tools → Android
   - Set **JDK Installed Path**: `C:\Program Files\Java\jdk-11.x.x` (or your path)
   - Click "Done"

#### On Mac:

1. **Download JDK 11**:
   - Go to: https://www.oracle.com/java/technologies/downloads/
   - Download: **JDK 11** (macOS installer)
   - Run installer

2. **Set Java Path in Unity**:
   - Unity → Settings → External Tools → Android
   - Set **JDK Installed Path**: `/Library/Java/JavaVirtualMachines/jdk-11.x.x/Contents/Home`
   - Click "Done"

#### On Linux (Ubuntu/Debian):

```bash
sudo apt update
sudo apt install openjdk-11-jdk
java -version  # Verify installation
```

✅ **JDK installed**

---

### Step 2: Install Android SDK (via Unity)

Unity has a built-in Android SDK downloader. This is the easiest method.

#### Method 1: Unity's Built-in Android Setup (RECOMMENDED)

1. **Open Unity** → Edit → Preferences
2. **Go to**: External Tools → Android
3. You should see:
   - [ ] Android SDK Tools
   - [ ] Android NDK
   - [ ] OpenJDK
4. **Check the checkbox** next to each and Unity will download automatically
5. **Wait** for downloads to complete (this can take 5-10 minutes)

✅ **Android SDK, NDK, and OpenJDK installed by Unity**

#### Method 2: Manual Installation (Optional)

If Method 1 doesn't work:

1. **Download Android Studio**: https://developer.android.com/studio
2. **Install Android Studio** (larger download, ~1GB)
3. Open Android Studio → Tools → SDK Manager
4. Install these components:
   - Android SDK Platform 33 (or latest)
   - Android SDK Build-Tools
   - Android Emulator
   - Android SDK Platform-Tools
5. In Unity Preferences, set:
   - **Android SDK Path**: Where you installed Android Studio
   - **Android NDK**: Similar location
   - **JDK Path**: Your JDK installation

---

### Step 3: Verify Installation

In **Unity Preferences** (Edit → Preferences → External Tools → Android):

You should see paths like:
```
✓ JDK: C:\Program Files\Java\jdk-11.x.x
✓ Android SDK: C:\Users\YourName\AppData\Local\Android\Sdk
✓ Android NDK: C:\Users\YourName\AppData\Local\Android\Sdk\ndk\21.x.x
```

All three should have **green checkmarks** ✅

If any show **red X**:
- Check the path exists
- Re-download if corrupted
- Restart Unity

✅ **All Android tools installed and configured**

---

## CONFIGURE UNITY FOR ANDROID

### Step 1: Switch Build Platform to Android

1. **Open File → Build Settings** (or Ctrl+Shift+B)
2. In the list of platforms, select **"Android"**
3. Click **"Switch Platform"** button (right side)
4. **Wait** for Unity to recompile (1-2 minutes)

✅ **Platform switched to Android**

### Step 2: Configure Player Settings

1. Still in **Build Settings**, click **"Player Settings..."** button
2. This opens **Project Settings → Player**

#### Configure Package Name:

1. Go to: **Android → Identification**
2. **Package Name**: Change to your app identifier
   - Format: `com.yourname.appname`
   - Example: `com.bert.puzzlegame`
   - ⚠️ **IMPORTANT**: Match the package name you used in AdMob setup
3. Click "Apply"

#### Configure Minimum API Level:

1. Go to: **Android → Other Settings**
2. **Minimum API Level**: Set to **API 24** (Android 7.0)
   - Or higher if needed
   - ⚠️ AdMob requires API 21+, so API 24 is safe
3. **Target API Level**: Set to **API 33** or latest
4. Click "Apply"

#### Enable 64-bit Architecture:

1. Still in **Android → Other Settings**
2. Scroll down to **"Scripting Backend"**
3. **Architecture**: Select **"ARM64"**
   - Google Play requires 64-bit support
4. Click "Apply"

#### Configure Resolution/Aspect Ratio:

1. Go to: **Android → Resolution and Presentation**
2. **Default Orientation**: Select **"Auto Rotate"** or **"Landscape Left"** (your choice)
3. **Supported Orientations**:
   - For puzzle game, recommend: **Landscape Left** or **Portrait**
   - Choose based on your game design
4. Click "Apply"

✅ **Player settings configured**

### Step 3: Verify Build Settings

In **Build Settings** window:
1. **Platform**: Android (selected)
2. **Scenes In Build**: Your main scene + any other scenes
   - If "Scenes" list is empty, add them:
   - Click "Add Open Scenes"
3. Click "Player Settings..." and verify Package Name is set

✅ **Build settings verified**

---

## CREATE KEYSTORE (for APK Signing)

Every APK must be digitally signed to upload to Google Play. You'll create a keystore (key file) once and use it for all future builds.

### Step 1: Create Keystore in Unity

1. **Open File → Build Settings**
2. Go to: **Android → Publishing Settings**
3. You should see **"Create a new keystore..."** option
4. Click **"Keystore..."** button
5. Select **"Create New"**

### Step 2: Set Keystore Details

A dialog will appear. Fill in:

```
Keystore Path: (auto-filled, usually in your project)
Keystore Password: MyGameKeystore123! (create a STRONG password)
Confirm Password: MyGameKeystore123!
```

⚠️ **IMPORTANT**: 
- Write down this password somewhere safe (you'll need it later)
- Use a strong password (min 6 characters, mix of letters/numbers/symbols)
- Example strong password: `Puzzle2024Game!Key`

Click **"Create"**

✅ **Keystore created**

### Step 3: Set Key Alias

After keystore is created, you'll see another dialog:

```
Key Alias: key0 (or your choice, e.g., "puzzlegame")
Password: (same as keystore password)
Confirm Password: (same as keystore password)
Validity (years): 25 (recommended for app longevity)
```

Fill in and click **"Create"**

✅ **Key alias created**

### Step 4: Save Keystore Password

In **Build Settings**, check that:
- **Keystore**: Path is shown (✓)
- **Key Alias**: Shown (✓)
- **User Key Alias Password**: Password field filled (✓)

⚠️ **CRITICAL**: Save this information:
```
Keystore Path: _____________
Keystore Password: _____________
Key Alias: _____________
Key Password: _____________
Validity: 25 years
```

**You'll need these to publish updates** to Google Play!

✅ **Keystore ready for signing**

---

## BUILD YOUR FIRST APK

### Step 1: Prepare Build Settings

1. **Open File → Build Settings**
2. Verify:
   - [ ] Platform is "Android"
   - [ ] All scenes are in "Scenes In Build"
   - [ ] Player Settings configured
   - [ ] Keystore created
   - [ ] Architecture is ARM64

### Step 2: Start Build

1. In **Build Settings**, click **"Build"** (bottom right)
2. **Choose location** where to save APK:
   - Recommend: Create "Builds" folder in your project
   - Save as: `PuzzleGame.apk`
3. Click **"Save"**

### Step 3: Wait for Build

Unity will now build your APK. This takes **5-15 minutes** depending on:
- Game complexity
- Computer speed
- First build is slower (subsequent builds faster)

**Progress indicators**:
- Console shows progress messages
- Bottom right shows percentage
- **Don't close Unity during build**

### Step 4: Build Complete

When finished, you'll see message:
```
Build complete!
```

Or if errors occurred:
```
Build failed with X error(s)
```

✅ **APK successfully built**

If errors occurred, see **Common Errors & Solutions** section below.

---

## TEST APK ON DEVICE/EMULATOR

### Option A: Test on Android Device (RECOMMENDED)

**Prerequisites**:
- Android phone (6.0+)
- USB cable
- USB Debugging enabled

#### Enable USB Debugging:

1. **On Android phone**:
   - Go to: Settings → About Phone
   - Tap **"Build Number"** 7 times (hidden menu)
   - Go back to Settings → Developer Options
   - Enable **"USB Debugging"**
2. **Connect to computer** via USB cable

#### Install APK on Device:

**Option 1: Using Android Debug Bridge (ADB)**

```bash
# Connect device via USB
# Open terminal/command prompt

adb devices  # Should show your device

adb install ~/Builds/PuzzleGame.apk
# Or full path: adb install "C:\Users\YourName\PuzzleGameUnity\Builds\PuzzleGame.apk"

# Wait for installation
# Should show: Success
```

**Option 2: Using File Manager**

1. Copy `PuzzleGame.apk` to your phone (USB connection)
2. On phone, open File Manager → Find `PuzzleGame.apk`
3. Tap to install
4. Grant permissions
5. App installs

#### Test on Device:

1. **Launch app** on your phone
2. **Test each feature**:
   - Game loads without crashes
   - Sprites display correctly
   - Buttons work
   - **Banner ads appear** (at bottom)
   - Try watching a rewarded ad (if implemented)
   - Try level completion (interstitial ad should show)
3. **Check for errors** in Android Logcat:
   - Window → TextMesh Pro → Logcat
   - Look for red error messages
   - Note any ad loading errors

#### Example Logcat Output (Good):

```
D/GoogleAds: Initializing Google Mobile Ads SDK
I/Ads: Google Mobile Ads SDK is on version 20.6.0
D/AdRequest: Banner ad requested
I/AdRequest: Banner ad loaded successfully
```

#### Example Logcat Output (Problem):

```
E/Ads: Failed to initialize Google Mobile Ads SDK
E/AdRequest: Could not load banner ad - No ad unit ID set
```

✅ **APK tested on device**

### Option B: Test on Emulator

If you don't have a physical device:

1. **Download Android Studio**: https://developer.android.com/studio
2. **Create emulator** through Android Studio:
   - Tools → Device Manager
   - Create new virtual device
   - Select Android 12 or later
   - Give 2GB+ RAM
3. **Install APK on emulator**:
   ```bash
   adb install ~/Builds/PuzzleGame.apk
   ```
4. **Launch and test** same as above

⚠️ **Note**: Emulator is slower and ads may not load (internet issues). Physical device is better.

✅ **APK tested successfully**

---

## BUILD SIZE OPTIMIZATION

If your APK is larger than 100 MB, optimize:

### Check APK Size:

1. **Build Settings** → Click **"Build"**
2. After build finishes:
   - Right-click on APK file
   - Properties/Info → See file size
3. Or in terminal:
   ```bash
   ls -lh ~/Builds/PuzzleGame.apk
   # Shows size
   ```

### Optimization Tips:

#### 1. **Enable Compression**

In **Build Settings**:
1. Go to: **Android → Publishing Settings**
2. Enable **"Compress (LZ4)"**
3. Rebuild APK

This reduces size by 20-30%.

#### 2. **Disable Unused Modules**

In **Project Settings → Player → Android**:
1. Go to: **Android → Other Settings**
2. Under "Scripting Backend", find "Strip Engine Code"
3. Enable **"Strip Engine Code"**
4. Rebuild

This removes unused Unity code.

#### 3. **Use ARMv7 Only (if possible)**

⚠️ **Note**: Google Play requires 64-bit (ARM64). Only use if you have alternatives.

#### 4. **Remove Unused Assets**

1. Check your Sprites folder - only keep what you use
2. Delete unused scenes
3. Remove test/debug code

#### 5. **Optimize Sprites**

1. Ensure sprites are **not larger** than needed
2. Use PNG compression (Unity does this automatically)
3. Delete duplicate sprite files

### Example Optimization Results:

```
Before optimization: 150 MB
After compression: 90 MB
After stripping code: 70 MB
Final APK: 65-70 MB (good size for Google Play)
```

✅ **Build optimized**

---

## COMMON ERRORS & SOLUTIONS

### Error: "JDK not found"

**Cause**: JDK path not set in Unity

**Solution**:
1. Edit → Preferences → External Tools → Android
2. Click **"Browse"** next to JDK path
3. Navigate to your Java installation:
   - Windows: `C:\Program Files\Java\jdk-11.x.x`
   - Mac: `/Library/Java/JavaVirtualMachines/jdk-11.x.x/Contents/Home`
4. Click "Apply"

### Error: "Android SDK not found"

**Cause**: SDK path not set or not installed

**Solution**:
1. Check if SDK is installed via Android Studio
2. Edit → Preferences → External Tools → Android
3. Set **Android SDK Path**:
   - Windows: `C:\Users\YourName\AppData\Local\Android\Sdk`
   - Mac: `~/Library/Android/sdk`
4. Click "Apply"

### Error: "No connected device"

**Cause**: Phone not in USB Debugging mode or not connected

**Solution**:
1. Enable USB Debugging on phone:
   - Settings → About Phone → Build Number (tap 7x)
   - Settings → Developer Options → USB Debugging (Enable)
2. Connect via USB cable
3. Run: `adb devices` (should list your device)

### Error: "Build failed: APK already exists"

**Cause**: Trying to build where APK already exists

**Solution**:
1. Delete old APK file
2. Or choose different filename
3. Rebuild

### Error: "Package name already exists" (Google Play)

**Cause**: Using same package name as another app

**Solution**:
1. In Build Settings, change package name:
   - Edit → Project Settings → Player → Android → Identification
   - Change: `com.yourname.puzzlegame` → `com.yourname.puzzlegame2`
2. Rebuild APK

### Error: "Out of memory during build"

**Cause**: Computer running out of RAM

**Solution**:
1. Close other programs (browsers, Discord, etc.)
2. Restart Unity
3. Try building again
4. If persistent, increase available RAM

### Error: "Ads not showing in app"

**Cause**: Ad unit IDs not set or network issue

**Solution**:
1. Verify ad unit IDs in AdManager.cs match AdMob dashboard
2. Check device has internet connection
3. Check Logcat for error messages:
   ```
   E/Ads: Failed to load ad - check unit ID
   ```
4. Ensure device is registered as test device in AdMob

### Error: "Gradle build failed"

**Cause**: Build system error (common)

**Solution**:
1. Clean build cache:
   - Edit → Preferences → GvmConfig (Gradle settings)
   - Or delete: `ProjectSettings/Gradle` folder
2. Rebuild
3. If persistent, update Unity to latest version

### Error: "Cannot find module 'com.google.android.gms:play-services-ads'"

**Cause**: Google Mobile Ads SDK not properly installed

**Solution**:
1. Re-import Google Mobile Ads plugin:
   - Delete: Assets/GoogleMobileAds folder
   - Import GoogleMobileAdsPlugin package again
2. Rebuild

### Error: "Invalid keystore password"

**Cause**: Wrong password entered

**Solution**:
1. Delete the keystore file
2. Create new keystore:
   - Build Settings → Android → Publishing Settings
   - Create New Keystore
   - Enter correct password
3. Rebuild

---

## FINAL CHECKLIST Before Publishing

- [ ] APK built successfully
- [ ] APK tested on Android device (no crashes)
- [ ] All game features work
- [ ] Ads load correctly
- [ ] AdMob unit IDs match your real IDs (not test IDs)
- [ ] APK size optimized (under 100 MB)
- [ ] Sprites display correctly
- [ ] Game is playable end-to-end
- [ ] No error messages in Logcat
- [ ] Keystore password saved securely

✅ **Android build complete and tested!**

---

## NEXT STEPS

1. ✅ Android APK built and tested
2. ✅ Ready for Google Play submission
3. Follow: **GOOGLE_PLAY_SETUP_GUIDE.md** to create developer account and publish

---

## QUICK REFERENCE

| Task | Command |
|------|---------|
| Check devices connected | `adb devices` |
| Install APK on device | `adb install filename.apk` |
| Uninstall app from device | `adb uninstall com.yourname.appname` |
| View Logcat output | `adb logcat` |
| Clear device logs | `adb logcat -c` |

---

**✅ Android build guide complete! Your APK is ready for Google Play publication.**

