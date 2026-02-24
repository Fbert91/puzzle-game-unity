# BrainBlast — Complete Setup Guide

> From Zero to Google Play — A Beginner-Friendly Walkthrough
> Unity 2022.3 LTS | Version 1.0 | February 2026

---

## Chapter 1 — Prerequisites

### System Requirements
- OS: Windows 10 64-bit / macOS 11+
- RAM: 8 GB minimum (16 GB recommended)
- Disk: 15 GB free (30 GB+ recommended)
- GPU: DX11 capable

### 1.1 Install Unity Hub
1. Go to https://unity.com/download
2. Download and run the Unity Hub installer
3. Launch Unity Hub, sign in or create a free account

### 1.2 Install Unity 2022.3.62f3 LTS
1. Unity Hub → Installs → Install Editor
2. Select 2022.3.62f3 under LTS tab (or use Archive link)
3. Check modules: **Android Build Support**, Visual Studio
4. Click Install (~5-8 GB download)

### 1.3 Install a Code Editor
- **Visual Studio** (Windows): Selected during Unity install, or download from https://visualstudio.microsoft.com/
- **VS Code** (cross-platform): Download from https://code.visualstudio.com/, install C# and Unity extensions

### 1.4 Install Git
- Windows: https://git-scm.com/download/win
- macOS: `git --version` (installs Command Line Tools if needed)
- Verify: `git --version`

---

## Chapter 2 — Getting the Project

### 2.1 Clone the Repository
```bash
cd ~/Documents
git clone https://github.com/Fbert91/puzzle-game-unity.git
```

### 2.2 Open in Unity Hub
1. Unity Hub → Projects → Open
2. Navigate to the `puzzle-game-unity` folder → Open
3. Select Unity version 2022.3.62f3 if prompted
4. Click to launch (first import takes 5-15 minutes — be patient!)

### 2.3 Safe Mode
If Unity opens in Safe Mode (compilation errors), check:
- Window → Package Manager → Install TextMeshPro
- Import TMP Essentials when prompted
- Close and reopen Unity if needed

---

## Chapter 3 — Project Structure

### Folders
| Folder | Purpose |
|--------|---------|
| Assets/Scripts/ | All C# game logic |
| Assets/Scenes/ | Unity scene files (SplashScreen, Gameplay) |
| Assets/Sprites/ | All 2D images (backgrounds, tiles, icons, mascot, UI) |
| Assets/Editor/ | Editor-only scripts (SceneBuilder, PlayFromStart) |
| Assets/Prefabs/ | Reusable pre-configured GameObjects |
| Assets/Resources/LevelData/ | levels.json — all 200 level definitions |
| Assets/Audio/ | Music and sound effects |
| Assets/Materials/ | Unity materials |
| ProjectSettings/ | Unity project configuration |

### Key Scripts
| Script | Role |
|--------|------|
| GameInitializer.cs | Bootstrap — creates all managers on startup |
| PuzzleGame.cs | Core SumToTen game logic |
| UIManager.cs | Panel system — manages all UI screens |
| BoardRenderer.cs | Creates and renders the tile grid |
| LevelManager.cs | Level data, progression, stars |
| ThemeManager.cs | Light/Dark theme system |
| SceneBuilder.cs (Editor) | Builds both scenes via BrainBlast menu |
| AdManager.cs | Google AdMob (optional, requires define symbol) |

### Architecture
- **2 scenes**: SplashScreen (index 0) → Gameplay (index 1)
- **Single-scene UI**: All panels in Gameplay scene, toggled by UIManager
- **Singleton managers**: Created by GameInitializer, accessed via `.Instance`

---

## Chapter 4 — Building the Scenes

1. In Unity: **BrainBlast → Build All Scenes** (top menu)
2. This creates SplashScreen.unity and Gameplay.unity in Assets/Scenes/
3. **File → Build Settings** (Ctrl+Shift+B):
   - Add SplashScreen at index **0**
   - Add Gameplay at index **1**
   - Both must be checked/enabled

> ⚠️ SplashScreen MUST be at index 0!

**Play From Start**: BrainBlast → Play From Start (plays from splash screen)

---

## Chapter 5 — Understanding the Game

### Core Mechanic: SumToTen
Select tiles whose numbers add up to exactly **10**. Clear all tiles to win.

### Worlds (200 levels total)
| World | Levels | Unlock |
|-------|--------|--------|
| 🌿 Garden | 1–40 | Start |
| 🌊 Ocean | 41–80 | Complete Garden |
| 🏔️ Mountain | 81–120 | Complete Ocean |
| 🚀 Space | 121–160 | Complete Mountain |
| 💎 Crystal | 161–200 | Complete Space |

### Star Rating
- ⭐⭐⭐ = Par or below moves
- ⭐⭐ = Within 1.5× par
- ⭐ = Completed (any moves)

### levels.json Format
```json
{
  "levelId": 1,
  "world": "Garden",
  "difficulty": "Easy",
  "gridWidth": 3,
  "gridHeight": 3,
  "tiles": [5, 5, 3, 7, 2, 8, 1, 9, 4],
  "parMoves": 5,
  "starThresholds": [5, 7, 99]
}
```

---

## Chapter 6 — Playing and Testing

1. Open SplashScreen.unity → Press Play (▶️ or Ctrl+P)
2. Game flow: Splash → MainMenu → LevelSelect → Gameplay → Victory

### Common Issues
| Issue | Fix |
|-------|-----|
| Black screen | Add scenes to Build Settings |
| No UI visible | Run BrainBlast → Build All Scenes |
| Buttons don't work | Rebuild scenes (EventSystem missing) |
| Pink textures | Right-click sprites → Reimport |

---

## Chapter 7 — Customizing

### Change Colors
Edit `ThemeManager.cs` color values or `SceneBuilder.cs` palette at the top, then rebuild scenes.

### Add New Levels
Edit `Assets/Resources/LevelData/levels.json` — add entries following the existing format. Ensure all tiles can be grouped into sums of 10.

### Change Mascot
Drop new PNG (512×512, transparent) into `Assets/Sprites/Mascot/`, set as Sprite type, assign in Hierarchy.

### Change Game Title
1. In-game: Edit "BrainBlast" in SceneBuilder.cs → rebuild scenes
2. App name: Edit → Project Settings → Player → Product Name

---

## Chapter 8 — Adding Google AdMob (Optional)

1. Download plugin: https://github.com/googleads/googleads-mobile-unity/releases
2. Assets → Import Package → Custom Package → select .unitypackage
3. Edit → Project Settings → Player → Other Settings → Scripting Define Symbols → add `GOOGLE_MOBILE_ADS`
4. Configure ad unit IDs in `AdManager.cs`
5. Use test IDs during development:
   - Rewarded: `ca-app-pub-3940256099942544/5224354917`
   - Interstitial: `ca-app-pub-3940256099942544/1033173712`

> 🚨 Never use real ad IDs during testing — your account may be banned!

---

## Chapter 9 — Building for Android

1. Verify Android module installed (Unity Hub → Installs)
2. File → Build Settings → select Android → **Switch Platform**
3. Player Settings:
   - Package Name: `com.pitogames.brainblast`
   - Scripting Backend: **IL2CPP**
   - Target Architecture: **ARM64** ✅
   - Minimum API: 24 (Android 7.0)
   - Orientation: Portrait
4. Publishing Settings → Custom Keystore → Create New
   - **BACK UP YOUR KEYSTORE!**
5. Build:
   - APK for testing: Build → save as .apk
   - AAB for Play Store: check "Build App Bundle" → Build
6. Test on device:
   - Enable Developer Options + USB Debugging on phone
   - `adb install BrainBlast.apk` or use Build and Run

---

## Chapter 10 — Publishing to Google Play

1. Create developer account at https://play.google.com/console ($25 one-time)
2. Create new app → fill in name, language, type (Game), pricing (Free)
3. Upload AAB: Release → Production → Create release → Upload
4. Store Listing:
   - App name, short description, full description
   - App icon (512×512), feature graphic (1024×500)
   - At least 2 screenshots (1080×1920)
5. Content Rating: Complete questionnaire (likely "Everyone")
6. Privacy Policy: Required if using ads
7. Review & publish: Start rollout to Production
8. Wait for Google review (hours to days)

---

## Chapter 11 — Troubleshooting

### Common Errors
| Error | Solution |
|-------|----------|
| TMPro not found | Package Manager → install TextMeshPro |
| GoogleMobileAds not found | Install AdMob plugin or remove GOOGLE_MOBILE_ADS define |
| Safe Mode | Fix compilation errors shown in console |
| No Android module | Unity Hub → Add Modules |
| Gradle build error | Check Android SDK path in Preferences → External Tools |
| Firebase warning | Expected & harmless — ignore it |

### Reset Project
1. Close Unity
2. Delete `Library/` and `Temp/` folders
3. Reopen project (reimports from scratch)

### Console Guide
- ℹ️ White = Info (safe to ignore)
- ⚠️ Yellow = Warning (usually harmless)
- 🔴 Red = Error (must fix!)

### Help Resources
- Unity Docs: https://docs.unity3d.com/2022.3/Documentation/Manual/
- Unity Forums: https://forum.unity.com/
- Stack Overflow: tag [unity3d]
- Unity Learn: https://learn.unity.com/

---

*BrainBlast Setup Guide v1.0 — February 2026*
