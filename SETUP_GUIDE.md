# BrainBlast — Scene Setup Guide

## Quick Start

1. **Pull latest changes:**
   ```
   git pull
   ```

2. **Open the project in Unity** (2021.3+ recommended)

3. **Run the scene builder:**
   - Go to menu: **BrainBlast > Build All Scenes**
   - Click "Build" when prompted
   - This creates all 4 scenes (SplashScreen, MainMenu, Gameplay, Settings) with proper UI, wiring, and build settings

4. **Play!** Hit Play in the SplashScreen scene.

## What the builder does

- Creates all GameObjects, Canvas, UI elements (TextMeshPro) for each scene
- Wires all `[SerializeField]` references on scripts (buttons, text, sliders, panels)
- Sets up CanvasScaler for mobile portrait (1080×1920)
- Fixes EditorBuildSettings with correct scene GUIDs
- Attaches all game scripts to the right objects

## Individual scenes

You can also rebuild individual scenes via **BrainBlast > Build Scenes > [scene name]**.

## Colors & Theme

- Background: Dark blue (#1a1a2e)
- Accent: Yellow (#e6b800)  
- Text: White

## Notes

- All text uses TextMeshPro (TMP). If you see missing font errors, import TMP Essentials via Window > TextMeshPro > Import TMP Essential Resources.
- Emoji characters have been replaced with text alternatives for font compatibility.
