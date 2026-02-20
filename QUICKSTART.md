# Quick Start Guide

Get the puzzle game running in 10 minutes.

## Step 1: Open Project in Unity (2 min)

1. **Download & Install Unity 2022 LTS**
   - https://unity.com/download
   - Include Android & iOS build support

2. **Open Project**
   - Launch Unity Hub
   - Click "Open" → Select `PuzzleGameUnity/` folder
   - Wait for import (~5 minutes)

3. **Verify Project**
   - Should see Assets folder with Scripts
   - No errors in Console tab (Ctrl+Shift+C)

## Step 2: Test Core Systems (5 min)

1. **Open Console**
   - View → Console (or Ctrl+Shift+C)

2. **Create Test Scene**
   - File → New Scene
   - Right-click Hierarchy → Create Empty → Name "TestGame"
   - Add components:
     - Add: GameInitializer script
     - Add: PuzzleGame script
     - Add: LevelManager script
     - Add: MonetizationManager script
     - Add: Analytics script
     - Add: UIManager script

3. **Run Test**
   - Click Play button (top center)
   - Watch Console for initialization messages
   - Should see "[GameInitializer] All systems initialized successfully"

## Step 3: Next Steps (3 min)

### To See Full Game:
1. Create scenes:
   - Create `Scenes/MainMenu` scene
   - Create `Scenes/Gameplay` scene
   - Create `Scenes/Victory` scene

2. Add Canvas to MainMenu:
   - GameObject → UI → Panel
   - Name it "Canvas"

3. Add UIManager:
   - Drag UIManager script onto Canvas
   - Assign UI elements

4. Build and test

### To Add Your Assets:
1. Drop your artwork in `Assets/Sprites/`
2. Drop audio in `Assets/Audio/`
3. Assign materials in scripts

### To Build for Devices:
1. Follow `BUILD_GUIDE.md` for Android
2. Follow `BUILD_GUIDE.md` for iOS

---

## 10-Minute Quick Test

Want to verify everything works quickly?

```csharp
// Create this test script and run it:

using UnityEngine;

public class QuickTest : MonoBehaviour {
    void Start() {
        // Test core systems exist
        Debug.Assert(PuzzleGame.Instance != null, "PuzzleGame missing!");
        Debug.Assert(LevelManager.Instance != null, "LevelManager missing!");
        Debug.Assert(MonetizationManager.Instance != null, "MonetizationManager missing!");
        Debug.Assert(Analytics.Instance != null, "Analytics missing!");
        
        // Test level loading
        var level = LevelManager.Instance.GetLevel(1);
        Debug.Assert(level != null, "Level 1 not found!");
        Debug.Log("✅ All systems verified!");
    }
}
```

Save as `Assets/Scripts/QuickTest.cs`, add to scene, press Play.

---

## Project Structure at a Glance

```
Assets/
├── Scripts/              ← All game code (7 files)
│   ├── PuzzleGame.cs    ← Core gameplay
│   ├── LevelManager.cs  ← 20 levels
│   ├── UIManager.cs     ← All menus
│   ├── MonetizationManager.cs ← IAP/ads
│   ├── Analytics.cs     ← Tracking
│   ├── CharacterController.cs ← Mascot
│   └── BoardRenderer.cs ← Board drawing
├── Scenes/              ← Scenes (create in editor)
├── Sprites/             ← Graphics (add your assets)
├── Audio/               ← Sounds (add your audio)
└── Materials/           ← Unity materials

Documentation/
├── BUILD_GUIDE.md       ← Build for Android/iOS
├── GAME_DESIGN.md       ← Full game spec
├── MONETIZATION_GUIDE.md ← Revenue setup
└── LEVEL_EDITOR.md      ← Add new levels
```

## Common First Tasks

### Task 1: Create a Simple Tile (30 sec)
```csharp
// In a new scene:
GameObject tile = new GameObject("Tile");
Image img = tile.AddComponent<Image>();
img.color = Color.blue;
```

### Task 2: Load Level 1 (1 min)
```csharp
// Add to a script:
PuzzleGame.Instance.LoadLevel(1);
Debug.Log("Level loaded!");
```

### Task 3: Complete a Level (2 min)
```csharp
// Simulate level complete:
LevelManager.Instance.CompleteLevel(1, 1000, 3);
Debug.Log("Level 1 complete with 3 stars!");
```

## Testing on Device

### Android Device
```bash
# Build and test
unity -quit -batchmode -projectPath ./ \
  -executeMethod BuildScript.BuildAndroidDev

# Install
adb install output.apk

# View logs
adb logcat | grep "PuzzleGame"
```

### iOS Device
```bash
# Open in Xcode
open iOS_Build/Unity-iPhone.xcodeproj

# Run on device (in Xcode)
# Cmd+R or Product > Run
```

## Troubleshooting

### "Can't find PuzzleGame.Instance"
→ Make sure GameInitializer is in scene

### "Scripts won't compile"
→ Save all files (Ctrl+S)
→ Restart Unity
→ Check Console for errors

### "No levels showing"
→ Verify LevelManager.Initialize() called
→ Check level IDs in LevelManager.cs

### "Buttons don't work"
→ Make sure EventSystem exists in scene
→ Verify Button components assigned
→ Check Canvas setup

## Reading Order

1. **This file** (Quick Start) - 5 min
2. **README.md** (Project Overview) - 10 min
3. **GAME_DESIGN.md** (Full Mechanics) - 30 min
4. **BUILD_GUIDE.md** (Platform Setup) - 20 min
5. **DEVELOPMENT.md** (Deep Dive) - 30 min

## Key Files to Know

- `PuzzleGame.cs` - What happens when you play a level
- `LevelManager.cs` - All 20 levels defined here
- `UIManager.cs` - Every menu screen
- `MonetizationManager.cs` - Buying stuff / ads
- `Analytics.cs` - Tracking user behavior

## Getting Help

1. **Game doesn't work?**
   → Check Console tab for errors
   → Verify GameInitializer script exists
   → Restart Unity

2. **Level won't load?**
   → Check LevelManager.InitializeLevels()
   → Verify level ID (1-20)
   → Check puzzle rule name (exact case!)

3. **UI buttons don't work?**
   → Verify Canvas → EventSystem exists
   → Check Button component onclick assigned
   → Verify method name exact (case-sensitive!)

4. **Want to add a level?**
   → Open LEVEL_EDITOR.md
   → Add one line to LevelManager.cs
   → Done!

---

## Success Checklist

After this quick start, you should:

- [ ] Project opens in Unity without errors
- [ ] Console shows initialization success
- [ ] Can load and view LevelManager
- [ ] Understand the 7 core scripts
- [ ] Know where to find documentation
- [ ] Know next steps for your workflow

---

## Next: What to Build

### Minimum Viable Game (1 day)
- [ ] Create MainMenu, Gameplay, Victory scenes
- [ ] Create simple tile graphics
- [ ] Link UIManager to scenes
- [ ] Test game flow end-to-end
- [ ] Build APK/IPA

### Polish (1 day)
- [ ] Add better graphics
- [ ] Add music & sound
- [ ] Animate character
- [ ] Particle effects
- [ ] Smooth transitions

### Launch Ready (5 days)
- [ ] Test on real devices
- [ ] Optimize performance
- [ ] Create app store listings
- [ ] Setup analytics
- [ ] Submit to stores

---

**Time to playable:** ~1 day (with assets)
**Time to launch:** ~1 week
**Time to profitability:** 2-4 weeks (depends on marketing)

Good luck! 🚀

---

**For questions:** See Documentation/ folder
**Need to expand?** See LEVEL_EDITOR.md
**Building for devices?** See BUILD_GUIDE.md
