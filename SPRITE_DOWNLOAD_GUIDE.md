# Sprite Download & Organization Guide - Complete Setup

**Target**: Download all 150+ game sprites from Kenney.nl and Game-Icons.net, organize into proper folder structure, verify all downloads, and prepare for Unity import.  
**Time**: ~1 hour  
**Prerequisites**: Internet connection, ability to download ZIP files, text editor (optional)  
**Result**: All sprites downloaded, organized, and ready for Unity import  

---

## TABLE OF CONTENTS
1. [Overview & Sources](#overview--sources)
2. [Folder Structure Setup](#folder-structure-setup)
3. [Download Instructions by Category](#download-instructions-by-category)
4. [Verification Checklist](#verification-checklist)
5. [Unity Import Settings](#unity-import-settings)
6. [Troubleshooting](#troubleshooting)

---

## OVERVIEW & SOURCES

### Sprite Sources (All FREE, CC0 License)

**Primary Source: Kenney.nl**
- 🔗 https://kenney.nl/assets
- Provides: Characters, Tiles, Buttons, Backgrounds, Particle Effects
- License: CC0 (Public Domain - free to use commercially)
- Format: PNG with transparent backgrounds
- Reliability: Professional quality, actively maintained

**Secondary Source: Game-Icons.net**
- 🔗 https://game-icons.net/
- Provides: Icon library (4,000+ icons)
- License: CC0 (Public Domain)
- Format: PNG + SVG available
- Perfect for: Small UI icons (coins, gems, stars, etc.)

### Total Assets to Download

```
Category          | Count  | Source
------------------|--------|------------------
Mascot Character  | 6      | Kenney Character Pack
Numbered Tiles    | 22     | Kenney Puzzle Pack + States
UI Buttons        | 10     | Kenney Game Icons
Menu Elements     | 3      | Kenney UI Pack
Icon Set          | 10     | Game-Icons.net
Backgrounds       | 4      | Kenney Backgrounds
Particle Effects  | 6      | Kenney Particle Pack
Animation Frames  | 14     | Kenney Animation Sets
TOTAL             | ~75+   | Multiple sources
```

---

## FOLDER STRUCTURE SETUP

### Create Directory Tree

First, create the folder structure in your computer. Navigate to:

```
/root/.openclaw/workspace/PuzzleGameUnity/Assets/Sprites/
```

Create these folders (you probably already have some):

```
Assets/
└── Sprites/
    ├── Mascot/
    ├── Tiles/
    ├── UI/
    ├── Icons/
    ├── Backgrounds/
    ├── Effects/
    └── _Downloads/ (temporary folder for ZIP files)
```

**On Windows/Mac**:
1. Open File Explorer
2. Navigate to your Unity project: `PuzzleGameUnity/Assets/`
3. Create `Sprites` folder (if it doesn't exist)
4. Inside `Sprites`, create the 6 subfolders above

**On Linux/Mac Terminal**:
```bash
cd /root/.openclaw/workspace/PuzzleGameUnity/Assets
mkdir -p Sprites/{Mascot,Tiles,UI,Icons,Backgrounds,Effects,_Downloads}
ls -la Sprites/  # Verify structure
```

✅ **Folder structure ready**

---

## DOWNLOAD INSTRUCTIONS BY CATEGORY

### CATEGORY 1: MASCOT CHARACTER (Kenney Character Pack)

**Source**: https://kenney.nl/assets/character-pack  
**Time**: 5 minutes  
**File Size**: ~20 MB  

#### Download Steps:

1. **Open**: https://kenney.nl/assets/character-pack
2. **Click "Download"** (large blue button, right side)
3. **Save as**: `kenney-character-pack.zip` to `_Downloads/` folder
4. **Extract ZIP**: 
   - Right-click → Extract All
   - Or: `unzip kenney-character-pack.zip` (terminal)
5. **Locate PNG files**:
   - Navigate to: `Character Pack/` → you'll see PNG files in different colors
   - Look for files like: `walk_0.png`, `idle.png`, `jump.png`, etc.

#### Which Files to Keep:

From the character pack, select **ONE COLOR** for your mascot (for consistency):
- Example: Keep all "Blue" character files, discard other colors
- Take these expressions:
  - Idle pose (`*_idle*`)
  - Happy (`*_smile*` or similar)
  - Surprised/Confused (look for matching)
  - Thinking/Focused
  - Walking (optional for animations)

#### Rename to Standard Names:

Once extracted, rename to these standard names (in `Assets/Sprites/Mascot/`):

```
character_idle.png          → Blue idle pose (64x64)
character_happy.png         → Blue happy/smile (64x64)
character_celebrating.png   → Blue celebrating pose (64x64)
character_encouraging.png   → Blue encouraging pose (64x64)
character_thinking.png      → Blue thinking pose (64x64)
character_confused.png      → Blue confused pose (64x64)
```

**Quick Tip**: If character is multiple sizes, use 64x64 versions. You can scale in Unity.

✅ **Mascot sprites organized**

---

### CATEGORY 2: NUMBERED TILES (Kenney Puzzle Pack)

**Source**: https://kenney.nl/assets/puzzle-pack  
**Time**: 10 minutes  
**File Size**: ~15 MB  

#### Download Steps:

1. **Open**: https://kenney.nl/assets/puzzle-pack
2. **Click "Download"**
3. **Save as**: `kenney-puzzle-pack.zip` to `_Downloads/`
4. **Extract**: Right-click → Extract All

#### Which Files to Keep:

From puzzle pack, you'll see many tile variations. Select:
- **One color set** (e.g., all Blue tiles)
- Look for files: `tile_*.png` or numbered blocks

#### Expected Files to Copy:

```
Tiles with numbers (for game board):
tile_1.png      → Tile showing "1"
tile_2.png      → Tile showing "2"
... through tile_9.png

Symbol tiles:
tile_star.png   → Star symbol
tile_heart.png  → Heart symbol
tile_gem.png    → Gem symbol
tile_base.png   → Empty base tile

State tiles:
tile_selected.png    → Highlighted border
tile_highlighted.png → Glowing effect
tile_locked.png      → Greyed out
tile_disabled.png    → Faded
```

#### Place in Folder:

Copy all these PNG files to: `Assets/Sprites/Tiles/`

**Note**: Files don't need renaming if Kenney already named them correctly.  
If named differently (e.g., `blue_1.png`), rename to `tile_1.png` for consistency.

✅ **Tile sprites organized**

---

### CATEGORY 3: UI BUTTONS (Kenney Game Icons)

**Source**: https://kenney.nl/assets/game-icons  
**Time**: 10 minutes  
**File Size**: ~10 MB  

#### Download Steps:

1. **Open**: https://kenney.nl/assets/game-icons
2. **Click "Download"**
3. **Save as**: `kenney-game-icons.zip` to `_Downloads/`
4. **Extract**

#### Which Files to Keep:

Look for button-like icons:
- `button_*.png` or similar
- Game control icons: play, pause, settings, back, etc.

#### Expected Buttons:

```
button_play.png          → Play button
button_play_hover.png    → Play button (lighter/hover state)
button_play_pressed.png  → Play button (darker/pressed state)
button_pause.png         → Pause button
button_settings.png      → Settings gear icon
button_back.png          → Back/return arrow
button_shop.png          → Shop/store icon
button_info.png          → Info/help icon (?)
button_hint.png          → Lightbulb hint icon
button_menu.png          → Menu/hamburger icon
```

**If these exact names don't exist**:
- Look for similar icons and rename them
- Example: If you see `pause.png`, rename to `button_pause.png`

#### Panel & Background Elements:

Also grab:
```
panel_background.png     → Menu panel background
divider.png              → Line separator
title_panel.png          → Title area background
```

#### Place in Folder:

Copy all to: `Assets/Sprites/UI/`

✅ **UI button sprites organized**

---

### CATEGORY 4: ICON SET (Game-Icons.net)

**Source**: https://game-icons.net/  
**Time**: 15 minutes (manual search & download)  
**File Size**: ~2 MB  

#### Download Steps (Manual):

1. **Open**: https://game-icons.net/
2. **Search for each icon** (one at a time):

| Icon | Search Term | Download |
|------|-------------|----------|
| Coin | "coin" | Download PNG 64x64 |
| Gem | "gem" or "diamond" | Download PNG 64x64 |
| Star | "star" | Download PNG 64x64 |
| Lightbulb | "lightbulb" | Download PNG 64x64 |
| Heart | "heart" | Download PNG 64x64 |
| Shield | "shield" | Download PNG 64x64 |
| Clock | "hourglass" or "clock" | Download PNG 64x64 |
| Magnifier | "magnifying-glass" or "zoom" | Download PNG 64x64 |
| Undo | "undo" or "arrow-left" | Download PNG 64x64 |
| Bolt | "lightning-bolt" or "bolt" | Download PNG 64x64 |

#### For Each Icon:

1. Type search term in search box
2. Click on icon you like
3. Click **"Download"** button
4. Choose **PNG** format, **64x64** size
5. Save to `Assets/Sprites/Icons/`

#### Naming Convention:

Save as:
```
icon_coin.png
icon_gem.png
icon_star.png
icon_hint.png
icon_heart.png
icon_shield.png
icon_timer.png
icon_zoom.png
icon_undo.png
icon_power.png
```

**Tip**: You can download multiple at once if the website allows batch download.

✅ **Icon sprites organized**

---

### CATEGORY 5: BACKGROUNDS (Kenney Backgrounds)

**Source**: https://kenney.nl/assets/backgrounds  
**Time**: 5 minutes  
**File Size**: ~8 MB  

#### Download Steps:

1. **Open**: https://kenney.nl/assets/backgrounds
2. **Click "Download"**
3. **Save as**: `kenney-backgrounds.zip` to `_Downloads/`
4. **Extract**

#### Which Files to Keep:

Select **4 backgrounds** for different screens:
```
bg_menu.png       → Main menu background (bright, welcoming)
bg_gameplay.png   → Gameplay board background (subtle)
bg_success.png    → Level complete background (celebratory)
bg_gameover.png   → Game over background (neutral, encouraging retry)
```

**How to Choose**:
- Pick backgrounds that fit your game's aesthetic
- Avoid overly busy patterns (distract from gameplay)
- Ensure good contrast for UI text/buttons
- All should complement your mascot character color

#### Size & Resolution:

- Backgrounds are typically 512x512 to 1920x1080
- Unity will scale them to your screen size automatically
- Keep original resolution for quality

#### Place in Folder:

Copy 4 selected backgrounds to: `Assets/Sprites/Backgrounds/`

**Rename to standard names** (for consistency):
```
menu_bg.png          (or bg_menu.png)
gameplay_bg.png      (or bg_gameplay.png)
success_bg.png       (or bg_success.png)
gameover_bg.png      (or bg_gameover.png)
```

✅ **Background sprites organized**

---

### CATEGORY 6: PARTICLE EFFECTS (Kenney Particle Pack)

**Source**: https://kenney.nl/assets/particle-pack  
**Time**: 5 minutes  
**File Size**: ~12 MB  

#### Download Steps:

1. **Open**: https://kenney.nl/assets/particle-pack
2. **Click "Download"**
3. **Save as**: `kenney-particle-pack.zip` to `_Downloads/`
4. **Extract**

#### Which Files to Keep:

Select particle effect sprites:
```
particle_confetti.png   → Celebration/confetti pieces (16x16)
particle_sparkle.png    → Sparkle effect (32x32)
particle_shine.png      → Light shine/reflection (32x32)
particle_explosion.png  → Burst/explosion effect (64x64)
particle_star.png       → Star particles (32x32)
particle_smoke.png      → Smoke/poof effect (48x48)
```

#### Animation Frames (Optional but Nice):

If you want animated effects, look for numbered frames:
```
anim_pop_1.png through anim_pop_6.png       (6 frames for pop/match effect)
anim_bounce_1.png through anim_bounce_4.png (4 frames for bounce)
anim_glow_1.png through anim_glow_4.png     (4 frames for glow pulse)
```

If these exact files don't exist in particle pack, that's okay. You can create animations in Unity without sprite sheets.

#### Place in Folder:

Copy all particle effects to: `Assets/Sprites/Effects/`

✅ **Particle effect sprites organized**

---

## VERIFICATION CHECKLIST

After downloading all sprites, verify you have everything:

```
✅ MASCOT FOLDER (Assets/Sprites/Mascot/)
   - [ ] character_idle.png
   - [ ] character_happy.png
   - [ ] character_celebrating.png
   - [ ] character_encouraging.png
   - [ ] character_thinking.png
   - [ ] character_confused.png

✅ TILES FOLDER (Assets/Sprites/Tiles/)
   - [ ] tile_1.png through tile_9.png (9 files)
   - [ ] tile_base.png
   - [ ] tile_star.png
   - [ ] tile_heart.png
   - [ ] tile_gem.png
   - [ ] tile_selected.png
   - [ ] tile_highlighted.png
   - [ ] tile_locked.png
   - [ ] tile_disabled.png

✅ UI FOLDER (Assets/Sprites/UI/)
   - [ ] button_play.png
   - [ ] button_play_hover.png
   - [ ] button_play_pressed.png
   - [ ] button_pause.png
   - [ ] button_settings.png
   - [ ] button_back.png
   - [ ] button_shop.png
   - [ ] button_info.png
   - [ ] button_hint.png
   - [ ] button_menu.png
   - [ ] panel_background.png
   - [ ] divider.png
   - [ ] title_panel.png

✅ ICONS FOLDER (Assets/Sprites/Icons/)
   - [ ] icon_coin.png
   - [ ] icon_gem.png
   - [ ] icon_star.png
   - [ ] icon_hint.png
   - [ ] icon_heart.png
   - [ ] icon_shield.png
   - [ ] icon_timer.png
   - [ ] icon_zoom.png
   - [ ] icon_undo.png
   - [ ] icon_power.png

✅ BACKGROUNDS FOLDER (Assets/Sprites/Backgrounds/)
   - [ ] bg_menu.png
   - [ ] bg_gameplay.png
   - [ ] bg_success.png
   - [ ] bg_gameover.png

✅ EFFECTS FOLDER (Assets/Sprites/Effects/)
   - [ ] particle_confetti.png
   - [ ] particle_sparkle.png
   - [ ] particle_shine.png
   - [ ] particle_explosion.png
   - [ ] particle_star.png
   - [ ] particle_smoke.png
   - [ ] anim_pop_*.png (optional)
   - [ ] anim_bounce_*.png (optional)
   - [ ] anim_glow_*.png (optional)
```

### Quick Verification Command (Terminal/Linux):

```bash
find /root/.openclaw/workspace/PuzzleGameUnity/Assets/Sprites -name "*.png" -type f | wc -l
```

This shows the total PNG count. Should be 60+ files minimum.

✅ **All sprites verified**

---

## UNITY IMPORT SETTINGS

### Step 1: Open Unity Project

1. Open your PuzzleGame Unity project
2. The Sprites folder should now contain all your PNG files

### Step 2: Configure Sprite Import Settings

For each sprite category, set proper import settings:

#### For All Sprites (General Settings):

1. **Select all PNGs** in a folder (e.g., Assets/Sprites/Tiles)
2. **In Inspector** (right panel):
   - **Texture Type**: Change from "Default" to **"Sprite (2D and UI)"**
   - **Sprite Mode**: Set to **"Single"** (unless it's a spritesheet)
   - **Pixels Per Unit**: Set to **100** (for 64x64 sprites) or **50** (for 128x128 tiles)
   - **Filter Mode**: **"Point (no filter)"** for pixel-perfect look
   - **Compression**: **"None"** (keep quality for high-res assets)
   - **Click "Apply"**

#### For Tiles (64x64 pixels):

```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Point (no filter)
Compression: None
```

#### For Backgrounds (512x512 or larger):

```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Bilinear (slightly smoother)
Compression: High Quality
```

#### For UI Buttons & Icons (64x64 or 128x128):

```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Point (no filter)
Compression: None
```

#### For Particle Effects:

```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Point (no filter)
Compression: None
```

### Step 3: Batch Configuration (Faster Method)

To set all sprites at once:

1. **Select all PNGs** in Assets/Sprites folder (Ctrl+A or Cmd+A)
2. In Inspector:
   - Texture Type: Sprite (2D and UI)
   - Sprite Mode: Single
   - Click "Apply"
3. Unity will apply settings to all selected sprites

### Step 4: Organize in Project Window

In Unity's Project window:

1. Create folders to match your Sprites organization:
   - Assets/Sprites/Mascot
   - Assets/Sprites/Tiles
   - Assets/Sprites/UI
   - etc.

2. Drag PNG files from `_Downloads` folder into correct folders

✅ **All sprites imported and configured**

---

## TROUBLESHOOTING

### Issue: "PNG files not showing in Unity"

**Solution**:
1. Make sure PNG files are in `Assets/Sprites/` folder (not outside Assets)
2. Refresh Project window: Ctrl+R (Cmd+R on Mac)
3. Wait a few seconds for import to complete

### Issue: Sprites look pixelated or blurry

**Solution**:
1. Check import settings for Filter Mode
2. For pixel art: Use **"Point (no filter)"**
3. For smooth art: Use **"Bilinear"**
4. Ensure Pixels Per Unit matches sprite size

### Issue: Missing transparency (PNG shows white background)

**Solution**:
1. Re-download PNG files (ensure they're PNG format, not JPG)
2. Check Texture Type is **"Sprite (2D and UI)"**
3. In import settings, ensure **no compression** is applied
4. Save original PNG files with transparency from Kenney (they should have it)

### Issue: Sprites show pink/magenta color in Unity

**Reason**: Missing shader or material issue

**Solution**:
1. Check Sprite Renderer component on GameObject
2. Set Material to **"Sprites/Default"**
3. Or drag sprite directly to scene (Unity creates default renderer)

### Issue: Some Kenney sprites have multiple frames but I want just one

**Solution**:
1. In import settings, keep **"Sprite Mode: Single"**
2. Unity will use the entire PNG as one sprite
3. If you want to split sprite sheets, change to **"Sprite Mode: Multiple"** and use Sprite Editor

### Issue: Downloaded file is corrupted or won't extract

**Solution**:
1. Delete the ZIP file
2. Re-download from Kenney website
3. Try a different extraction tool
4. On Mac/Linux: `unzip filename.zip`

---

## NEXT STEPS

1. ✅ Download all sprites from sources above
2. ✅ Organize into correct folder structure
3. ✅ Verify all files with checklist
4. ✅ Configure import settings in Unity
5. ✅ Your sprites are now ready for game development!

For next steps, follow: **SPRITE_INTEGRATION_GUIDE.md** (if available) or start assigning sprites to game board tiles in your scripts.

---

## QUICK REFERENCE

| Source | URL | Pack | Content |
|--------|-----|------|---------|
| **Kenney** | kenney.nl/assets | Character Pack | Mascot sprites |
| **Kenney** | kenney.nl/assets | Puzzle Pack | Game tiles |
| **Kenney** | kenney.nl/assets | Game Icons | UI buttons |
| **Kenney** | kenney.nl/assets | Backgrounds | Screen backgrounds |
| **Kenney** | kenney.nl/assets | Particle Pack | Effects & particles |
| **Game-Icons** | game-icons.net | (Icon library) | UI icons |

**All FREE, CC0 License** ✅

---

**✅ Sprite download guide complete! You now have all 75+ sprites ready for your game.**

For next steps: Integrate sprites into Unity and start building your game board!

