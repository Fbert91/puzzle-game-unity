# Build Guide - Puzzle Game Unity

Complete guide to building and deploying the Puzzle Game for Android and iOS.

## Prerequisites

- **Unity 2022 LTS** (or compatible version)
- **Android Studio** (for Android builds)
- **Xcode** (for iOS builds, macOS only)
- **Git** (for version control)
- **JDK 11+** (for Android)

### Installation Steps

1. Install Unity 2022 LTS from https://unity.com/download
2. Include **Android Build Support** and **iOS Build Support** in installation
3. Clone/download the project

## Project Structure

```
PuzzleGameUnity/
├── Assets/
│   ├── Scripts/          # All C# game logic
│   ├── Scenes/           # Unity scenes
│   ├── Prefabs/          # Reusable components
│   ├── Sprites/          # Graphics assets
│   ├── Audio/            # Sound & music files
│   └── Materials/        # Unity materials
├── ProjectSettings/      # Unity project config
├── Packages/            # Dependencies
└── Documentation/       # Guides & docs
```

## Building for Android

### Step 1: Configure Android Settings

1. Open Unity → File → Build Settings
2. Select **Android** platform
3. Switch to Android platform
4. Click "Player Settings"

**Player Settings Configuration:**
- **Product Name:** Puzzle Game
- **Company Name:** Your Company
- **Bundle Identifier:** com.yourcompany.puzzlegame
- **Version:** 1.0
- **Target API Level:** 34 (latest stable)
- **Minimum API Level:** 24 (Android 7.0)
- **Graphics API:** OpenGL ES 3.0 + Vulkan

### Step 2: Configure Keystore (For Release Builds)

1. In Player Settings → Publishing Settings
2. Click "Create New Keystore"
3. Set password (save this securely!)
4. Create new key alias
5. Set key password

**Important:** Keep keystore file and passwords safe for future updates!

### Step 3: Build APK

1. File → Build Settings
2. Add scenes:
   - Scenes/SplashScreen
   - Scenes/MainMenu
   - Scenes/LevelSelect
   - Scenes/Gameplay
   - Scenes/Victory
3. Click "Build APK"
4. Choose output location
5. Wait for build completion

**Output:** `PuzzleGame.apk` (ready to test on devices)

### Step 4: Test on Device

```bash
# Connect Android device with USB debugging enabled
adb install PuzzleGame.apk
adb logcat | grep "PuzzleGame"  # View logs
```

## Building for iOS

### Prerequisites (macOS Only)

1. Install Xcode from App Store
2. Accept Xcode license: `sudo xcode-select --install`
3. Install CocoaPods: `sudo gem install cocoapods`

### Step 1: Configure iOS Settings

1. Open Unity → File → Build Settings
2. Select **iOS** platform
3. Switch to iOS platform
4. Click "Player Settings"

**Player Settings Configuration:**
- **Product Name:** Puzzle Game
- **Bundle Identifier:** com.yourcompany.puzzlegame
- **Version:** 1.0.0
- **Target Minimum iOS Version:** 14.0
- **Target SDK:** Latest iOS

### Step 2: Build Xcode Project

1. File → Build Settings
2. Add all scenes (same as Android)
3. Click "Build"
4. Choose output folder (e.g., `iOS_Build/`)
5. Wait for build completion

**Output:** Xcode project in `iOS_Build/` folder

### Step 3: Open in Xcode

```bash
cd iOS_Build
open Unity-iPhone.xcodeproj
```

### Step 4: Configure in Xcode

1. Select "Unity-iPhone" project
2. Select "Unity-iPhone" target
3. Go to "Signing & Capabilities"
4. **For Testing:**
   - Development Team: Personal Team (free)
   - Provisioning Profile: Automatic
5. **For App Store:**
   - Development Team: Your organization
   - Code signing: Apple Development Certificate

### Step 5: Build & Run

- **For Simulator:** Product → Run (or Cmd+R)
- **For Device:** Connect iPhone, select device in Xcode, Product → Run

## Building for Different Release Types

### Development Build

```bash
# Unity command line build
unity -quit -batchmode -projectPath ./ \
  -executeMethod BuildScript.BuildAndroidDev
```

### Release Build (Signed)

```bash
# Android release with signing
unity -quit -batchmode -projectPath ./ \
  -executeMethod BuildScript.BuildAndroidRelease
```

## Firebase Setup

### Enable Analytics

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Create new project (if needed)
3. Enable Google Analytics
4. Download `google-services.json` (Android) and `GoogleService-Info.plist` (iOS)
5. Place files in `Assets/` folder

### Initialize in Code

```csharp
// Already integrated in Analytics.cs
FirebaseApp.CheckAndFixAsync().ContinueWith(task => {
    Debug.Log("Firebase initialized");
});
```

## Google Play & App Store Submission

### Google Play (Android)

1. Create Google Play Developer account ($25 one-time)
2. Complete app information:
   - App name, description, screenshots
   - Content rating questionnaire
   - Pricing & distribution
3. Upload signed APK
4. Review and publish

**Store Listing Requirements:**
- App icon (512x512 PNG)
- 2-8 screenshots (1440x2560 or 1080x1920)
- Feature graphic (1024x500)
- 80-character short description
- Full description (4000 chars max)

### Apple App Store (iOS)

1. Create Apple Developer account ($99/year)
2. Create App ID in Apple Developer Console
3. Create provisioning profiles
4. Build and Archive in Xcode
5. Upload via Xcode → Product → Archive
6. Submit for review in TestFlight/App Store Connect

**Store Listing Requirements:**
- App icon (1024x1024 PNG, no transparency)
- 2-5 screenshots (various device sizes)
- App preview video (15-30 seconds, optional)
- Keywords (100 chars max)
- Description (4000 chars max)
- Support URL
- Privacy Policy URL

## Performance Optimization

### Mobile Optimization Checklist

- [ ] Reduce texture sizes (2048x2048 max)
- [ ] Use JPEG for backgrounds, PNG for tiles
- [ ] Enable GPU instancing on materials
- [ ] Use object pooling for tiles
- [ ] Limit particle effects (< 500 particles)
- [ ] Profile with Profiler (Window → Analysis → Profiler)

### Target Performance

- **Target FPS:** 60 FPS (30 minimum)
- **APK Size:** < 100 MB
- **Memory Usage:** < 256 MB
- **Load Time:** < 3 seconds per level

## Testing Checklist

### Functional Testing
- [ ] All levels playable
- [ ] Win conditions trigger correctly
- [ ] Hints work properly
- [ ] Pause/resume works
- [ ] Settings persist

### Monetization Testing
- [ ] IAP dialog appears
- [ ] Ad rewarded video works
- [ ] Coins/gems tracked correctly

### Platform Testing
- [ ] Works on Android 7.0+ devices
- [ ] Works on iOS 14+ devices
- [ ] Landscape & portrait orientation
- [ ] Notch/safe area handled correctly
- [ ] Back button works (Android)

### Analytics Testing
- [ ] Events logged to Firebase
- [ ] DAU/retention tracked
- [ ] Level completion analytics work

## Troubleshooting

### Android Build Errors

**Error: "targetSdkVersion must be at least 31"**
- Solution: Player Settings → Target API Level: 33+

**Error: "Gradle build failed"**
- Solution: Clear Library folder, reimport assets

### iOS Build Errors

**Error: "Module not found"**
- Solution: Run `pod install` in iOS_Build folder

**Error: "Signing identity not found"**
- Solution: Xcode → Preferences → Accounts, add Apple ID

## Version Updates

### Increment Version Numbers

**Android:**
```
version = 1.0 (versionCode)
versionName = "1.0.0"
```

**iOS:**
```
Version = 1.0.0
Build = 1
```

### Update Changelog

Maintain `CHANGELOG.md` with:
- Version number
- Release date
- Bug fixes
- New features
- Known issues

## Release Checklist

- [ ] All bugs fixed and tested
- [ ] Analytics verified
- [ ] IAP/ads tested in sandbox
- [ ] Screenshots & descriptions ready
- [ ] Privacy policy & terms updated
- [ ] Version numbers incremented
- [ ] Changelog updated
- [ ] Both Android & iOS builds signed and tested
- [ ] Firebase configured
- [ ] App icons & splash screens finalized

## Support

For issues, see:
- `GAME_DESIGN.md` - Game mechanics
- `MONETIZATION_GUIDE.md` - IAP & ads setup
- `LEVEL_EDITOR.md` - Adding new levels
- Unity docs: https://docs.unity.com/

---

**Last Updated:** 2026-02-20
**Version:** 1.0
