# Game Design Document - Puzzle Game

## 1. Game Overview

**Title:** Puzzle Game (Working Title)

**Genre:** Logic Puzzle / Tile-Matching

**Target Audience:** Kids (8+) and Teens (13+)

**Platforms:** Android (7.0+) and iOS (14+)

**Core Mechanic:** Logic + Tile Hybrid - Players solve puzzles by manipulating tiles on a grid while solving logic constraints.

## 2. Game Philosophy

### Core Principles
1. **No Punishment** - Players can retry infinitely without penalties
2. **Progressive Learning** - Early levels teach mechanics gently
3. **Fair Difficulty** - Clear win conditions, no hidden tricks
4. **Feel Good** - Celebrate wins, encourage play
5. **Accessibility** - Simple controls, adjustable text size, colorblind modes

### Design Focus
- Charming character guides players
- Satisfying visual/audio feedback
- No timers, no lives, no stamina
- Play at your own pace
- Non-violent, family-friendly

## 3. Core Gameplay Mechanics

### 3.1 The Board

**Setup:**
- Grid: 5x5 (early) to 6x6 (advanced)
- Each cell contains a **Tile** with a value (1-9) or symbol
- Tiles are manipulated to solve the puzzle

**Tile Interaction:**
- **Tap:** Select/deselect a tile
- **Drag:** Swap two adjacent tiles (optional)
- **Long-press:** View tile details (value, locked status)

### 3.2 Puzzle Types

#### Type 1: Sum to Target
**Goal:** Select tiles that sum to a target value

**Example:** Level 3
- Target: Sum to 10
- Board: [1, 2, 3, 4, 5, 5, 4, 3, 2, 1, ...]
- Solution: Select [1, 4, 5] = 10 ✓

**Difficulty Progression:**
- Level 1: Sum to 5 (obvious)
- Level 5: Sum to 15 (requires swaps)
- Level 10: Sum to 20 (complex grid)

#### Type 2: Connect Patterns
**Goal:** Select tiles that match a specific pattern

**Example:** Level 7
- Pattern: "Find 3 connected tiles of same value"
- Board: [1, 1, 2, 2, 3, 1, 2, 3, 1, 3, ...]
- Solution: Select [1, 1, 1] in connected path ✓

**Difficulty Progression:**
- Level 2: Connect 2 same tiles (easy)
- Level 7: Connect 4 in specific shape
- Level 15: Connect while avoiding others

#### Type 3: Sequence Order
**Goal:** Select tiles in ascending order

**Example:** Level 4
- Goal: Select 5 tiles in order: 1→2→3→4→5
- Board: [5, 1, 4, 2, 3, 1, 2, 3, 4, 5, ...]
- Solution: Select in path [1, 2, 3, 4, 5] ✓

**Difficulty Progression:**
- Level 3: Order 3 tiles (1→2→3)
- Level 13: Order 6 tiles with obstacles
- Level 19: Order 7 tiles, complex grid

### 3.3 Game Rules

**Win Condition:**
- Complete the puzzle objective before running out of moves (if applicable)
- OR solve the puzzle (time-unlimited mode)

**Losing:**
- No losing! Players can retry infinitely
- Can reset level at any time
- Can use hints to progress

**Level Progression:**
- Complete Level 1 to unlock Level 2
- All levels always accessible after unlock (no expiry)

## 4. Level Design

### 4.1 Level Structure (20+ levels)

**Tutorial Block (Levels 1-5):**
- Level 1: Learn tile selection
- Level 2: Learn pattern matching
- Level 3: Learn sum mechanics
- Level 4: Learn sequence order
- Level 5: Mixed simple puzzle

**Standard Block (Levels 6-15):**
- 10 levels of increasing complexity
- Mix of all three puzzle types
- Difficulty: Medium
- Expected playtime: 3-10 min per level

**Challenge Block (Levels 16-20):**
- Hard puzzles requiring strategy
- Larger grids (6x6)
- Complex rules
- Expected playtime: 10-20 min per level

### 4.2 Difficulty Curve

```
Difficulty Rating (1-3 stars):
Level 1-5:    ⭐        (Easy - tutorial)
Level 6-15:   ⭐⭐      (Medium - standard)
Level 16-20:  ⭐⭐⭐    (Hard - challenge)
```

**Metrics:**
- Grid size: 5x5 → 6x6
- Tile count: 25 → 36
- Max value: 5 → 12
- Required selections: 2 → 8

### 4.3 Level Data Format

```csharp
[System.Serializable]
public class PuzzleLevel
{
    public int levelId;
    public int gridWidth = 5;
    public int gridHeight = 5;
    public string puzzleRule = "SumToTen"; // Type
    public int[] initialTiles = { /* 25 values */ };
    public int targetSum = 10;
    public int difficulty = 1; // 1=Easy, 2=Medium, 3=Hard
}
```

### 4.4 Expanding Levels

To add more levels, create new PuzzleLevel instances in `LevelManager.InitializeLevels()`:

```csharp
CreateLevel(21, "SumToTen", 6, 6, 
    new int[] { /* 36 values */ }, 25, 3);
```

## 5. User Interface

### 5.1 Main Screens

#### Splash Screen
- Shows game logo + character
- Company branding
- Auto-transitions to main menu (3 sec)

#### Main Menu
- **Play Button** → Level Select
- **Settings Button** → Settings Screen
- **About Button** → Credits
- Character mascot waving

**Visual:**
- Bright background (gradient: blue-purple)
- Title text (large, playful font)
- Buttons (big, colorful, rounded)

#### Level Select
- Grid of level buttons (5 columns)
- Shows:
  - Level number
  - Difficulty stars
  - Completion stars (✓✓✓ or blank)
  - Lock icon (if locked)
- Progress bar at top (% complete)
- Back button to main menu

#### Gameplay Screen
- **Top:** Level name, move counter
- **Center:** Game board (5x5 or 6x6 tiles)
- **Bottom:** Buttons (Hint, Pause, Settings)
- **HUD:** Coins/gems counter

#### Victory Screen
- Celebration animation
- Score display
- Stars earned (1-3)
- Buttons: Next Level, Replay, Menu
- Character celebrating

#### Settings Screen
- Music toggle
- Sound effects toggle
- Language selection (EN, ES, FR, DE, etc.)
- Colorblind mode
- Text size
- Credits
- Privacy policy link
- Back button

### 5.2 UI Colors & Aesthetics

**Color Palette:**
```
Primary:    #6366F1 (Indigo)
Secondary:  #EC4899 (Pink)
Accent:     #F59E0B (Amber)
Success:    #10B981 (Green)
Warning:    #EF4444 (Red)
Background: #F8FAFC (Light gray)
Text:       #1E293B (Dark gray)
```

**Typography:**
- Headlines: Rounded Sans (Poppins, Raleway)
- Body: Clean Sans (Inter, Roboto)
- Sizes: 14sp (small), 16sp (body), 20sp (title), 28sp+ (heading)

**Buttons:**
- Rounded corners (8-12pt radius)
- Gradient fills (subtle)
- Hover: Scale up 5%
- Press: Scale down 5%
- Disabled: Opacity 50%

**Transitions:**
- Fade: 300ms (screen changes)
- Slide: 200ms (UI elements)
- Bounce: 500ms (celebration)
- No jarring movements

## 6. Character System

### 6.1 Mascot Character

**Name:** Luna (or customizable)

**Personality:**
- Cheerful, helpful, non-condescending
- Gives tips when player struggles
- Celebrates wins enthusiastically
- Evolves as player progresses

**Expressions:**
- Neutral (default, idle animation)
- Happy (level start)
- Thinking (hint mode)
- Celebrating (level win)
- Encouraging (after struggles)
- Sad (never - never makes player feel bad!)

### 6.2 Character Progression

**Unlockable Skins:**
- Default Luna
- Astronaut Luna ($2.99)
- Pirate Luna ($1.99)
- Medieval Luna ($1.99)
- Neon Luna ($2.99)

**Customization:**
- Player can dress Luna in earned/purchased outfits
- Cosmetic only, no gameplay effect

## 7. Progression System

### 7.1 Level Unlocking

**Rules:**
- Level 1 always unlocked
- Level N+1 unlocks after completing Level N
- All unlocked levels accessible (no expiration)

**Tracking:**
- PlayerPrefs stores: completed, starsEarned, bestScore
- Auto-save after level complete
- Sync to cloud (optional, future feature)

### 7.2 Scoring System

**Per-Level Score:**
```
Base Score:        1000 points
Move Bonus:        (50 - moveCount) × 10
Time Bonus:        (300 - secondsUsed) × 2
Star Calculation:
  ≤20 moves    → 3 stars ⭐⭐⭐
  ≤35 moves    → 2 stars ⭐⭐
  >35 moves    → 1 star  ⭐
```

**Total Score = Base + Move Bonus + Time Bonus**

**Minimum Score:** 1000 (can't go below base)

### 7.3 Achievements (Optional Expansion)

Future achievements (in-game badges):
- "First Win" - Complete level 1
- "Speedster" - Complete level in <2 min
- "Perfect" - Complete level with 3 stars
- "Hint Master" - Collect 10 hints
- "Puzzle Expert" - Complete all levels

## 8. Hint System

### 8.1 Hint Types

**Hint 1: Show Next Valid Move**
- Shows one correct tile selection
- Doesn't reveal full solution
- Resets selection (can undo manually)

**Hint 2: Highlight Possible Moves**
- Highlights 2-3 valid tiles
- Lets player choose
- Still requires player skill

**Hint 3: Reveal Tile Value**
- Reveals one locked/hidden tile
- Useful for complex grids

### 8.2 Hint Economy

**Free Hints:**
- Start with 3 hints per game
- Refill daily (5 hints)
- Watch ad for +1 hint (optional)

**Paid Hints:**
- IAP: Hint Pack (5 hints) for $0.99
- IAP: Unlimited hints (3 days) for $2.99

## 9. Animations & Feedback

### 9.1 Tile Feedback

- **Selection:** Pop animation + sound ding
- **Deselection:** Shrink animation
- **Swap:** Slide animation (200ms)
- **Valid Match:** Shine effect, confetti
- **Invalid:** Shake animation, soft error sound

### 9.2 Screen Transitions

- Menu → Level Select: Fade + slide left
- Level Select → Gameplay: Zoom in
- Gameplay → Victory: Confetti + fade
- Victory → Menu: Fade + zoom out

### 9.3 Audio

**Music:**
- Ambient background (looping, non-intrusive)
- 60-80 BPM (calm, puzzle-appropriate)

**SFX:**
- Tile select: Soft "pop" (220Hz, 100ms)
- Valid match: "chime" (uplifting)
- Level win: Triumphant "fanfare"
- Button press: "click" (subtle)
- Hint used: "sparkle" sound

## 10. Technical Specs

### 10.1 Performance Targets

- **FPS:** 60 (minimum 30 on budget devices)
- **Load Time:** < 2 seconds per level
- **Memory:** < 256 MB
- **APK Size:** < 100 MB

### 10.2 Supported Devices

**Android:**
- API 24+ (Android 7.0+)
- Min RAM: 1 GB
- Min storage: 100 MB

**iOS:**
- iOS 14+
- Min RAM: 1 GB (iPhone 6S+)
- Min storage: 150 MB

### 10.3 Orientation

- Portrait mode primarily
- Support both orientations
- Handle safe areas (notch, home indicator)

## 11. Monetization

### 11.1 Monetization Model

**Free-to-Play (F2P)**
- Game free to download
- No level paywall
- Optional cosmetics & convenience
- Light ads (can be dismissed)

### 11.2 Revenue Streams

See `MONETIZATION_GUIDE.md` for details:
1. **In-App Purchases** ($0.99-19.99)
   - Power-ups: Hints, tile reveals
   - Cosmetics: Character skins, themes
   - Boosters: 2x score, unlimited hints

2. **Ads** (Optional viewing)
   - Rewarded video: Watch for 50 coins
   - Banner: Passive (toggleable)

3. **No Pay-to-Win**
   - All levels solvable without purchases
   - Cosmetics only visual
   - Purchased hints optional

## 12. Analytics Goals

### 12.1 Metrics to Track

**Engagement:**
- DAU (Daily Active Users)
- Session length (avg)
- Levels played per session

**Retention:**
- D1, D7, D30 retention
- Churn rate

**Progression:**
- Level completion rates (funnel)
- Where players drop off
- Average attempts per level

**Monetization:**
- Conversion rate (% who purchase)
- ARPU (avg revenue per user)
- Revenue by IAP product

**Technical:**
- Crash rates
- Load times
- Device performance breakdown

## 13. Quality Assurance

### 13.1 Testing Checklist

- [ ] All levels playable
- [ ] All puzzle types work
- [ ] Score calculation correct
- [ ] Stars awarded correctly
- [ ] Hints work properly
- [ ] Pause/resume works
- [ ] Settings persist
- [ ] No crashes
- [ ] Performance acceptable
- [ ] UI responsive on all devices
- [ ] Colorblind modes work
- [ ] Audio/music toggle works

### 13.2 Platform Testing

- [ ] Android 7.0+ compatibility
- [ ] iOS 14+ compatibility
- [ ] Landscape orientation
- [ ] Notch handling
- [ ] Back button (Android)

## 14. Future Expansions

### 14.1 Phase 2 Content

- 20+ more levels (40 total)
- New puzzle types
- Seasonal themes
- Limited-time events
- Leaderboards (optional)

### 14.2 Phase 3 Features

- Multiplayer (async)
- Daily challenges
- Level editor (user-generated)
- Tournaments
- Battle pass

## 15. Success Metrics

### Post-Launch (30 Days)

✅ **Minimum:**
- 5,000+ installs
- D1 retention: 30%+
- D7 retention: 10%+
- 0.02+ ARPU

✅ **Good:**
- 20,000+ installs
- D1 retention: 45%+
- D7 retention: 20%+
- 0.05+ ARPU

✅ **Excellent:**
- 50,000+ installs
- D1 retention: 60%+
- D7 retention: 30%+
- 0.10+ ARPU

---

## Document Info

**Version:** 1.0
**Last Updated:** 2026-02-20
**Status:** Approved for Development
**Next Review:** Post-launch (Day 30)

For implementation details, see:
- `BUILD_GUIDE.md` - Technical build instructions
- `MONETIZATION_GUIDE.md` - Revenue implementation
- `LEVEL_EDITOR.md` - Adding new levels
