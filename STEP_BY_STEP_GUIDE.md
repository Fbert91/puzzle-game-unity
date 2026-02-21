# Step-by-Step Guide: Opening PuzzleGame in Unity

## Prerequisites
- Unity Hub installed
- Unity 2022.3.22f1 (or any 2022.3 LTS) installed

## Steps

### 1. Clone the Repository
```bash
git clone https://github.com/Fbert91/puzzle-game-unity.git
```

### 2. Open in Unity Hub
1. Open Unity Hub
2. Click **"Add"** → **"Add project from disk"**
3. Navigate to the cloned `puzzle-game-unity` folder (the one containing `Assets/` and `ProjectSettings/`)
4. Click **Select Folder**

### 3. First Launch
- Unity will import all assets (may take 2-5 minutes first time)
- TextMeshPro may prompt to import TMP Essential Resources → Click **"Import TMP Essentials"**
- If prompted about API update, click **"Yes"**

### 4. Open the Main Scene
1. In Project window: `Assets/Scenes/MainMenu.unity`
2. Double-click to open
3. Press **Play** ▶ to test

### 5. Scene Overview
| Scene | Purpose |
|-------|---------|
| MainMenu | Title screen with Play, Settings, Levels buttons |
| Gameplay | Puzzle grid with HUD, timer, hint/undo buttons |
| Settings | Music/SFX volume sliders, about section |

### 6. Build for Android
1. **File → Build Settings**
2. Select **Android**
3. Click **Switch Platform**
4. Click **Build** or **Build and Run**

### 7. Important Notes
- Scripts reference `AudioManager.Instance` - the AudioManager uses singleton pattern with DontDestroyOnLoad
- Button click handlers are set up via code in `MainMenuUI.cs` and `GameplayUI.cs` (assign buttons in Inspector)
- The GameBoard in Gameplay scene is populated at runtime by `BoardRenderer.cs`
- Tile prefab is at `Assets/Prefabs/TilePrefab.prefab`

### 8. Customization
- Change colors: Edit button Image colors in the Inspector
- Change grid size: Modify `BoardRenderer.cs` or `LevelManager.cs`
- Add levels: Edit `LevelManager.cs` level data
- Audio: Replace files in `Assets/Audio/Music/` and `Assets/Audio/SFX/`

## Project Structure
```
PuzzleGameUnity/
├── Assets/
│   ├── Audio/Music/        # Background music
│   ├── Audio/SFX/          # Sound effects
│   ├── Prefabs/            # Tile prefab
│   ├── Scenes/             # 3 scenes (MainMenu, Gameplay, Settings)
│   └── Scripts/            # All C# scripts
├── Packages/manifest.json  # Package dependencies
└── ProjectSettings/        # Unity project configuration
```
