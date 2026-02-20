# SPRITE INTEGRATION GUIDE
## How to Wire Sprites into Unity Game

**Game**: Puzzle Logic + Tiles Hybrid  
**Engine**: Unity (2021+)  
**Target**: Kids-Teenage audience  

---

## 📚 TABLE OF CONTENTS

1. [Import Sprites into Unity](#import-sprites)
2. [Configure Sprite Importer Settings](#configure-sprites)
3. [Mascot Character Integration](#mascot-integration)
4. [Tile System Integration](#tile-integration)
5. [UI Button Setup](#ui-buttons)
6. [Icon Integration](#icon-integration)
7. [Background Setup](#background-setup)
8. [Effect & Particle Integration](#effects)
9. [Common Issues & Solutions](#troubleshooting)

---

## 🎨 IMPORT SPRITES INTO UNITY {#import-sprites}

### Step 1: Copy Sprite Files
```
1. Download all sprite packs (see SPRITE_MANIFEST.md)
2. Extract ZIP files locally
3. Copy PNG files to: Assets/Sprites/[Category]/
4. Maintain folder structure from manifest
```

### Step 2: Unity Project Structure
Your project should have:
```
Assets/
├── Sprites/
│   ├── Mascot/
│   ├── Tiles/
│   ├── UI/
│   ├── Icons/
│   ├── Backgrounds/
│   └── Effects/
├── Scripts/
│   ├── GameBoard/
│   ├── Character/
│   ├── UI/
│   └── Effects/
├── Scenes/
│   ├── MainMenu.unity
│   ├── Gameplay.unity
│   └── GameOver.unity
└── Prefabs/
```

### Step 3: Verify Import
- Unity auto-detects PNG files
- Check Assets folder in Unity Editor
- Verify transparent backgrounds are preserved
- All sprites should appear in Project panel

---

## ⚙️ CONFIGURE SPRITE IMPORTER SETTINGS {#configure-sprites}

### Recommended Settings for All Sprites

**For Pixel Art Sprites** (Mascot, Tiles, Effects):

```
Texture Type:              Sprite (2D and UI)
Sprite Mode:              Single (or Multiple for spritesheets)
Mesh Type:                Tight
Pixels Per Unit (PPU):     32 (adjust based on your game scale)
Filter Mode:              Point (no filter) - for crisp pixels
Compression:              None (or Lossless for file size)
```

**For UI & Background Sprites**:

```
Texture Type:              Sprite (2D and UI)
Sprite Mode:              Single
Mesh Type:                Full Rect
Pixels Per Unit (PPU):     100
Filter Mode:              Bilinear
Compression:              Lossy (to reduce file size)
```

### Apply Settings

1. **Select sprite** in Project panel
2. **Inspector → Texture Importer**
3. **Change settings** as above
4. **Click "Apply"**
5. **Repeat for each category**

### Batch Import Script (Optional)

If you have many sprites, use this script to auto-configure:

```csharp
using UnityEditor;
using UnityEngine;

public class SpriteImporter : MonoBehaviour
{
    [MenuItem("Tools/Configure All Sprites")]
    public static void ConfigureAllSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.meshType = SpriteMeshType.Tight;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        
        EditorUtility.DisplayDialog("Success", "All sprites configured!", "OK");
    }
}
```

**Usage**:
1. Create file: `Assets/Editor/SpriteImporter.cs`
2. Paste code above
3. Menu → Tools → Configure All Sprites
4. Automatically applies settings to all sprites

---

## 🎭 MASCOT CHARACTER INTEGRATION {#mascot-integration}

### Overview
- One main character displayed in game
- Multiple expression states
- Positioned at side or bottom of screen
- Provides feedback and encouragement

### Step 1: Create Character Controller

**Script**: `Assets/Scripts/Character/MascotController.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class MascotController : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite celebratingSprite;
    [SerializeField] private Sprite encouragingSprite;
    [SerializeField] private Sprite thinkingSprite;
    [SerializeField] private Sprite confusedSprite;
    
    [SerializeField] private Image mascotImage;
    [SerializeField] private float expressionDuration = 2f;
    
    private Sprite currentSprite;
    private float expressionTimer;

    private void Start()
    {
        // Start with idle expression
        ShowExpression(idleSprite);
    }

    private void Update()
    {
        // Auto-return to idle after expression duration
        if (expressionTimer > 0)
        {
            expressionTimer -= Time.deltaTime;
            if (expressionTimer <= 0)
            {
                ShowExpression(idleSprite);
            }
        }
    }

    public void ShowExpression(Sprite sprite)
    {
        mascotImage.sprite = sprite;
        currentSprite = sprite;
        expressionTimer = expressionDuration;
    }

    // Public methods for game events
    public void OnLevelStart()
    {
        ShowExpression(happySprite);
    }

    public void OnHint()
    {
        ShowExpression(encouragingSprite);
    }

    public void OnWrongMove()
    {
        ShowExpression(confusedSprite);
    }

    public void OnLevelComplete()
    {
        ShowExpression(celebratingSprite);
    }

    public void OnGameOver()
    {
        ShowExpression(confusedSprite);
    }
}
```

### Step 2: Setup in Scene

1. **Create GameObject**
   - Right-click in Hierarchy → Create empty → Name: "Mascot"
   - Position: Bottom-right corner (or preferred location)

2. **Add Components**
   - Add Image component (from UI)
   - Add MascotController script

3. **Configure Image**
   - Set size to match sprite (e.g., 128x128)
   - Set sprite to: Mascot/character_idle.png
   - Match image size in Inspector

4. **Assign Sprites in Inspector**
   - Drag mascot sprites to script fields:
     - Idle Sprite → character_idle.png
     - Happy Sprite → character_happy.png
     - Celebrating Sprite → character_celebrating.png
     - Encouraging Sprite → character_encouraging.png
     - Thinking Sprite → character_thinking.png
     - Confused Sprite → character_confused.png

5. **Test**
   - Play scene
   - Call expressions from GameManager to test

### Alternative: Simple Display (No Script)

If you want just static mascot without expressions:

1. Create Image in Canvas
2. Assign character_idle.png sprite
3. Position and scale as desired
4. Done!

---

## 🧩 TILE SYSTEM INTEGRATION {#tile-integration}

### Overview
- Board is made of tiles
- Each tile shows number (1-9) or symbol
- Tiles have interaction states (selected, highlighted, disabled)
- Tiles can be matched/removed

### Step 1: Create Tile Prefab

**Script**: `Assets/Scripts/GameBoard/Tile.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image tileImage;
    [SerializeField] private Text numberText;
    
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite[] numberSprites; // tiles 1-9
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private Sprite lockedSprite;
    
    private int value;
    private TileState state;
    private Button tileButton;

    public enum TileState { Normal, Selected, Highlighted, Locked, Disabled }

    private void Start()
    {
        tileButton = GetComponent<Button>();
        if (tileButton != null)
        {
            tileButton.onClick.AddListener(OnTileClicked);
        }
    }

    public void Initialize(int tileValue)
    {
        value = tileValue;
        SetState(TileState.Normal);
        
        // Display number on tile
        if (numberText != null)
        {
            numberText.text = value.ToString();
        }
        
        // Set tile sprite
        if (tileValue >= 1 && tileValue <= 9)
        {
            tileImage.sprite = numberSprites[tileValue - 1];
        }
        else
        {
            tileImage.sprite = baseSprite;
        }
    }

    public void SetState(TileState newState)
    {
        state = newState;
        
        switch (state)
        {
            case TileState.Normal:
                tileImage.sprite = numberSprites[value - 1];
                GetComponent<Button>().interactable = true;
                break;
            case TileState.Selected:
                tileImage.sprite = selectedSprite;
                break;
            case TileState.Highlighted:
                tileImage.sprite = highlightedSprite;
                break;
            case TileState.Locked:
                tileImage.sprite = lockedSprite;
                GetComponent<Button>().interactable = false;
                break;
            case TileState.Disabled:
                tileImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                GetComponent<Button>().interactable = false;
                break;
        }
    }

    private void OnTileClicked()
    {
        // Send click event to GameBoard
        GameBoard.Instance.OnTileClicked(this);
    }

    public int GetValue() => value;
    public TileState GetState() => state;
}
```

### Step 2: Create Board Layout

**Script**: `Assets/Scripts/GameBoard/GameBoard.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get; private set; }

    [SerializeField] private GridLayoutGroup boardGrid;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 4;
    
    private Tile[,] tiles;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        tiles = new Tile[gridWidth, gridHeight];
        
        // Configure grid
        boardGrid.constraintCount = gridWidth;
        
        // Create tiles
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject tileObj = Instantiate(tilePrefab, boardGrid.transform);
                Tile tile = tileObj.GetComponent<Tile>();
                
                // Random value 1-9
                int value = Random.Range(1, 10);
                tile.Initialize(value);
                
                tiles[x, y] = tile;
            }
        }
    }

    public void OnTileClicked(Tile clickedTile)
    {
        // Game logic for tile interaction
        // Example: Check for matches, combinations, etc.
        Debug.Log($"Tile clicked with value: {clickedTile.GetValue()}");
    }
}
```

### Step 3: Setup Prefab

1. **Create Tile Prefab**
   - Create empty GameObject: "Tile"
   - Add Image component (set size to 64x64)
   - Add Button component
   - Add Tile.cs script
   - Add Text for number display (optional)

2. **Configure in Inspector**
   - Drag tile sprites to Inspector fields
   - Assign number sprites (1-9) array

3. **Create Board**
   - Create empty GameObject: "GameBoard"
   - Add GridLayoutGroup component
   - Set Preferred Cell Size: 64x64
   - Add GameBoard.cs script
   - Assign tilePrefab to script

4. **Test**
   - Play scene
   - Board should generate with random tiles
   - Tiles clickable, display values

---

## 🔘 UI BUTTONS SETUP {#ui-buttons}

### Overview
- Menu buttons (Play, Settings, etc.)
- Game buttons (Pause, Hint, etc.)
- Button states (default, hover, pressed)

### Step 1: Create Button Prefab

**Script**: `Assets/Scripts/UI/MenuButton.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;
    
    private Image buttonImage;
    private Button button;
    private Sprite originalSprite;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        originalSprite = defaultSprite;
        
        // Button state listeners
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.yellow;
        colors.pressedColor = Color.gray;
        button.colors = colors;
    }

    public void SetSprites(Sprite normal, Sprite hover, Sprite pressed)
    {
        defaultSprite = normal;
        hoverSprite = hover;
        pressedSprite = pressed;
        buttonImage.sprite = normal;
    }
}
```

### Step 2: Create UI Canvas

1. **Create Canvas**
   - Right-click in Hierarchy → UI → Canvas
   - Set Canvas Scaler: Scale with Screen Size

2. **Add Buttons**
   - Right-click Canvas → UI → Button - Legacy
   - Rename: "PlayButton", "PauseButton", "SettingsButton", etc.
   - Position on canvas

3. **Configure Buttons**
   - Select button → Inspector
   - Find Image component
   - Drag button sprite (e.g., button_play.png)
   - Adjust size and position

4. **Add Click Events**
   - Select button
   - Inspector → Button → On Click (+ button)
   - Drag script/object with handler
   - Select method to call

### Example: Main Menu Setup

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OnSettingsClicked()
    {
        // Open settings panel
        SettingsPanel.Instance.Open();
    }

    public void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
```

---

## 🎯 ICON INTEGRATION {#icon-integration}

### Overview
- Small icons for UI elements
- Coin, gem, star, hint, health, etc.
- Used in HUD and menus

### Step 1: Create Icon Display

**Script**: `Assets/Scripts/UI/HUDDisplay.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HUDDisplay : MonoBehaviour
{
    [SerializeField] private Image coinIcon;
    [SerializeField] private Text coinText;
    
    [SerializeField] private Image gemIcon;
    [SerializeField] private Text gemText;
    
    [SerializeField] private Image starsText;
    [SerializeField] private Text starsCount;
    
    [SerializeField] private Button hintButton;
    [SerializeField] private Image hintIcon;
    [SerializeField] private Text hintsRemaining;

    private void Start()
    {
        // Initialize icons
        coinIcon.sprite = Resources.Load<Sprite>("Sprites/Icons/icon_coin");
        gemIcon.sprite = Resources.Load<Sprite>("Sprites/Icons/icon_gem");
        hintIcon.sprite = Resources.Load<Sprite>("Sprites/Icons/icon_hint");
        
        hintButton.onClick.AddListener(OnHintClicked);
    }

    public void UpdateCoins(int amount)
    {
        coinText.text = amount.ToString();
    }

    public void UpdateGems(int amount)
    {
        gemText.text = amount.ToString();
    }

    public void UpdateStars(int amount)
    {
        starsCount.text = amount.ToString();
    }

    private void OnHintClicked()
    {
        // Game logic for hint
        GameManager.Instance.UseHint();
    }
}
```

### Step 2: Setup in Scene

1. **Create HUD Panel**
   - Right-click Canvas → Create empty → "HUD"
   - Position at top-right corner

2. **Add Icon Displays**
   - For each icon (coin, gem, star, hint):
     - Create Image (icon)
     - Create Text (value)
     - Parent to HUD

3. **Assign Icons**
   - Select Image
   - Drag sprite to Inspector
   - Set Native Size (64x64)

4. **Connect to Script**
   - Select HUD GameObject
   - Drag HUDDisplay.cs
   - Assign icon images and text fields

---

## 🎨 BACKGROUND SETUP {#background-setup}

### Overview
- Full-screen or scene backgrounds
- Different backgrounds for different screens

### Step 1: Create Background Manager

**Script**: `Assets/Scripts/Background/BackgroundManager.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance { get; private set; }

    [SerializeField] private Image backgroundImage;
    
    [SerializeField] private Sprite menuBackground;
    [SerializeField] private Sprite gameplayBackground;
    [SerializeField] private Sprite successBackground;
    [SerializeField] private Sprite gameoverBackground;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetBackground(BackgroundType type)
    {
        switch (type)
        {
            case BackgroundType.Menu:
                backgroundImage.sprite = menuBackground;
                break;
            case BackgroundType.Gameplay:
                backgroundImage.sprite = gameplayBackground;
                break;
            case BackgroundType.Success:
                backgroundImage.sprite = successBackground;
                break;
            case BackgroundType.GameOver:
                backgroundImage.sprite = gameoverBackground;
                break;
        }
    }

    public enum BackgroundType { Menu, Gameplay, Success, GameOver }
}
```

### Step 2: Setup in Scene

1. **Create Background Image**
   - Right-click Canvas → UI → Image
   - Name: "Background"
   - Position: 0,0 (top-left) with full size
   - Set to back (move to bottom in Hierarchy)

2. **Configure Image**
   - Set Sprite to background image
   - Set Size: Match screen resolution
   - Set Anchors: Stretch (full screen)

3. **Add Script**
   - Add BackgroundManager.cs to GameObject
   - Assign background sprites in Inspector

---

## ✨ EFFECT & PARTICLE INTEGRATION {#effects}

### Overview
- Confetti on level complete
- Sparkles on tile interactions
- Explosions on power-ups

### Step 1: Create Particle System

**Option A: Built-in Particle System**

```csharp
using UnityEngine;

public class ConfettiEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem confettiParticles;
    [SerializeField] private Sprite confettiSprite;
    
    public void PlayConfetti(Vector3 position)
    {
        // Position and play particles
        confettiParticles.transform.position = position;
        confettiParticles.Play();
    }

    private void ConfigureParticles()
    {
        var renderer = confettiParticles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        var main = confettiParticles.main;
        main.duration = 2f;
    }
}
```

**Option B: Sprite-Based Particles**

```csharp
using UnityEngine;
using System.Collections;

public class SpriteParticle : MonoBehaviour
{
    private Image image;
    private float duration;
    private float elapsedTime;

    public void Initialize(Sprite sprite, float lifetime, Vector3 startPos)
    {
        image = GetComponent<Image>();
        image.sprite = sprite;
        duration = lifetime;
        transform.position = startPos;
        elapsedTime = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        
        // Fade out
        Color color = image.color;
        color.a = 1 - (elapsedTime / duration);
        image.color = color;
        
        // Move up
        transform.Translate(Vector3.up * Time.deltaTime * 50f);
        
        if (elapsedTime >= duration)
            Destroy(gameObject);
    }
}
```

### Step 2: Use in Game

```csharp
public void OnLevelComplete()
{
    // Play confetti effect
    confettiEffect.PlayConfetti(transform.position);
    
    // Call mascot
    mascot.OnLevelComplete();
}
```

---

## 🔧 TROUBLESHOOTING {#troubleshooting}

### Issue: Sprites appear blurry

**Solution**:
- Texture Importer → Filter Mode: Point (no filter)
- Pixels Per Unit: 32 or match your game scale
- Ensure you're not upscaling tiny sprites

### Issue: Transparency not showing

**Solution**:
- File is PNG with alpha channel (not JPG)
- Texture Type: Sprite (2D and UI)
- Alpha is 1.0 in Inspector

### Issue: Buttons not clickable

**Solution**:
- Add Button component (not just Image)
- Add GraphicRaycaster to Canvas
- Check Raycast Target is enabled
- Ensure button is not behind another UI element

### Issue: Sprite bleeding/pixelation on edges

**Solution**:
- Use Point (no filter) in Texture Importer
- Set Pixels Per Unit appropriately
- Ensure sprite padding in Sprite Editor

### Issue: Performance issues with many sprites

**Solution**:
- Compress textures (lossy for non-pixel art)
- Combine static sprites into atlases
- Use object pooling for particles
- Profile in Unity Profiler

### Issue: Wrong resolution display

**Solution**:
- Canvas Scaler → Scale with Screen Size
- Set Reference Resolution to your target (e.g., 1920x1080)
- Set Image size to match screen dimensions

---

## 📋 QUICK REFERENCE

### Common Script Patterns

**Assign Sprite from Resources**:
```csharp
image.sprite = Resources.Load<Sprite>("Sprites/UI/button_play");
```

**Assign Sprite from Field**:
```csharp
[SerializeField] private Sprite buttonSprite;
image.sprite = buttonSprite;
```

**Change Sprite at Runtime**:
```csharp
image.sprite = newSprite;
```

**Create Animation from Sprites**:
```csharp
// Use Unity Animator with sprite transitions
// Or manually cycle through sprites in Update()
sprites[currentFrame].enabled = true;
currentFrame++;
```

**Handle Button Click**:
```csharp
button.onClick.AddListener(() => {
    // Do something
});
```

---

## 🚀 NEXT STEPS

1. **Download all sprites** per SPRITE_MANIFEST.md
2. **Import into Assets/Sprites/**
3. **Configure Sprite Importer settings** per guide above
4. **Create scripts** for each system (Mascot, Tiles, UI, etc.)
5. **Setup prefabs** and scene layouts
6. **Connect game logic** to sprite displays
7. **Test and iterate** on visual feedback

---

**Integration Guide Created**: 2026-02-20  
**Ready for**: Unity 2021+ project  
**Sprites**: All CC0 licensed, free to use  

