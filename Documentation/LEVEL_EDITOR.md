# Level Editor Guide - Puzzle Game

Quick guide to adding new levels to the Puzzle Game.

## Quick Start

### Add a Level in 30 Seconds

1. Open `Assets/Scripts/LevelManager.cs`
2. Find the `InitializeLevels()` method
3. Add this line:
```csharp
CreateLevel(21, "SumToTen", 5, 5, 
    new int[] { 1,2,3,4,5, 1,1,1,2,2, 3,3,4,4,5, 1,2,3,4,5, 1,1,1,1,1 }, 
    15, 2);
```
4. Save and test!

## Level Format

### CreateLevel Parameters

```csharp
CreateLevel(
    levelId,        // int - Level number (1, 2, 3, ...)
    puzzleRule,     // string - "SumToTen", "ConnectPatterns", "SequenceOrder"
    width,          // int - Grid width (5 or 6)
    height,         // int - Grid height (5 or 6)
    initialTiles,   // int[] - Tile values (size = width × height)
    targetSum,      // int - Target value (for SumToTen)
    difficulty      // int - 1=Easy, 2=Medium, 3=Hard
)
```

### Puzzle Types Explained

#### 1. SumToTen
**Goal:** Select tiles that sum to target value

```csharp
// Example: Sum to 10
CreateLevel(1, "SumToTen", 5, 5, 
    new int[] { 1,2,3,4,5, 2,3,4,5,1, 1,1,2,2,3, 3,4,5,1,2, 1,1,1,1,1 }, 
    10, 1);

// Solution: Player selects 4 tiles = [1,4,5] or [2,3,5] or [1,2,3,4], etc.
```

**Creating SumToTen Levels:**
1. Decide target sum (8-25)
2. Create tile grid with variety
3. Ensure multiple valid solutions exist
4. Test that puzzle is solvable

#### 2. ConnectPatterns
**Goal:** Select tiles that form a connected pattern (same value or shape)

```csharp
// Example: Connect 3 tiles of value 1
CreateLevel(2, "ConnectPatterns", 5, 5, 
    new int[] { 1,2,3,4,5, 1,1,2,3,4, 5,1,2,2,3, 4,5,1,3,2, 1,2,3,4,5 }, 
    10, 1);

// Solution: Player selects connected 1s: positions (0,0), (0,1), (1,1)
```

**Creating ConnectPatterns Levels:**
1. Choose a value to connect (1-9)
2. Place it in 3+ locations on grid
3. Ensure they can form a path
4. Difficulty: More values = harder

#### 3. SequenceOrder
**Goal:** Select tiles in ascending numerical order (1, 2, 3, ...)

```csharp
// Example: Order 4 tiles as 1→2→3→4
CreateLevel(3, "SequenceOrder", 5, 5, 
    new int[] { 1,2,3,4,5, 5,4,3,2,1, 1,2,3,4,5, 2,3,1,4,5, 1,2,3,4,5 }, 
    10, 1);

// Solution: Player selects path: 1→2→3→4 (any 4 in ascending order)
```

**Creating SequenceOrder Levels:**
1. Decide sequence length (3-7 tiles)
2. Place values scattered on grid
3. Ensure clear paths exist
4. Test difficulty (long sequences = harder)

## Grid Sizes

### 5×5 Grid (25 tiles)
- Early/medium levels
- Easier to solve
- Faster gameplay

**Create array:**
```csharp
int[] tiles = new int[25];
// Fill with 25 values (0-indexed: 0 to 24)
```

### 6×6 Grid (36 tiles)
- Medium/hard levels
- More complexity
- Longer gameplay

**Create array:**
```csharp
int[] tiles = new int[36];
// Fill with 36 values (0-indexed: 0 to 35)
```

## Difficulty Ratings

### Easy (Difficulty = 1)
- Small grids (5×5)
- Low tile values (1-5)
- Obvious solutions
- Few tiles to manipulate
- **When:** Levels 1-5 (tutorial)

**Example:**
```csharp
CreateLevel(5, "SumToTen", 5, 5,
    new int[] { 1,2,3,4,5, 1,2,3,4,5, 1,2,3,4,5, 1,2,3,4,5, 1,2,3,4,5 }, 
    10, 1);
```

### Medium (Difficulty = 2)
- 5×5 or 6×6 grids
- Medium tile values (1-9)
- Multiple solution paths
- Requires some strategy
- **When:** Levels 6-15 (standard)

**Example:**
```csharp
CreateLevel(10, "ConnectPatterns", 6, 6,
    GenerateRandomBoard(36, 5), 
    15, 2);
```

### Hard (Difficulty = 3)
- 6×6 grids
- High tile values (1-15)
- Complex puzzle rules
- Requires planning
- **When:** Levels 16-20+ (challenge)

**Example:**
```csharp
CreateLevel(20, "SequenceOrder", 6, 6,
    GenerateRandomBoard(36, 15), 
    20, 3);
```

## Creating Tile Arrays

### Method 1: Manual Entry (5×5)
```csharp
int[] tiles = new int[] { 
    1, 2, 3, 4, 5,      // Row 0
    1, 1, 2, 2, 3,      // Row 1
    3, 4, 4, 5, 1,      // Row 2
    2, 3, 4, 5, 1,      // Row 3
    1, 2, 3, 4, 5       // Row 4
};
```

### Method 2: Random Generation (6×6)
```csharp
CreateLevel(15, "SumToTen", 6, 6,
    GenerateRandomBoard(36, 9),  // 36 tiles, values 1-9
    18, 2);
```

**What is `GenerateRandomBoard`?**
```csharp
private int[] GenerateRandomBoard(int tileCount, int maxValue)
{
    int[] board = new int[tileCount];
    for (int i = 0; i < tileCount; i++)
    {
        board[i] = Random.Range(1, maxValue + 1);
    }
    return board;
}
```

### Method 3: Balanced Mix (for consistency)
```csharp
int[] CreateBalancedBoard(int size)
{
    int[] board = new int[size];
    int value = 1;
    
    for (int i = 0; i < size; i++)
    {
        board[i] = (value % 9) + 1;  // Cycle through 1-9
        value++;
    }
    
    return board;
}
```

## Testing Your Level

### In Unity Editor

1. Open the level:
```csharp
// In PuzzleGame.cs Start():
currentLevelId = 21;  // Your new level
```

2. Play the scene
3. Try to solve it
4. Verify:
   - [ ] Puzzle is solvable
   - [ ] Difficulty matches rating
   - [ ] Plays in reasonable time
   - [ ] No UI issues
   - [ ] Score calculates correctly

### Difficulty Checklist

**Too Easy?**
- Increase grid size (5×5 → 6×6)
- Increase value range (1-5 → 1-9)
- Increase target (10 → 20)
- Reduce hints given

**Too Hard?**
- Decrease grid size
- Decrease value range
- Lower target number
- Simplify puzzle rule

## Recommended Level Progression

### Levels 1-5: Tutorial Block
```
Level 1: SumToTen,       5×5, target=5,   difficulty=1
Level 2: ConnectPatterns, 5×5, target=2,   difficulty=1
Level 3: SumToTen,       5×5, target=10,  difficulty=1
Level 4: SequenceOrder,  5×5, sequence=3, difficulty=1
Level 5: Mixed,          5×5, target=10,  difficulty=1
```

### Levels 6-15: Standard Block
```
Level 6:  SumToTen,      6×6, target=15, difficulty=2
Level 7:  ConnectPatterns, 6×6, pattern=3, difficulty=2
Level 8:  SumToTen,      6×6, target=20, difficulty=2
Level 9:  SequenceOrder, 5×5, sequence=5, difficulty=2
Level 10: SumToTen,      6×6, target=15, difficulty=2
... and so on
```

### Levels 16-20: Challenge Block
```
Level 16: SumToTen,      6×6, target=30, difficulty=3
Level 17: ConnectPatterns, 6×6, pattern=6, difficulty=3
Level 18: SequenceOrder, 6×6, sequence=7, difficulty=3
Level 19: SumToTen,      6×6, target=35, difficulty=3
Level 20: ConnectPatterns, 6×6, pattern=8, difficulty=3
```

## Advanced: Custom Puzzle Rules

Want to add a new puzzle type? (E.g., "EvenNumbers")

1. **Add to `PuzzleGame.cs`:**
```csharp
case "EvenNumbers":
    solved = CheckEvenNumbers();
    break;

private bool CheckEvenNumbers()
{
    foreach (var tile in selectedTiles)
    {
        if (tile.value % 2 != 0) return false;  // Not even
    }
    return selectedTiles.Count > 0;
}
```

2. **Create levels using it:**
```csharp
CreateLevel(25, "EvenNumbers", 5, 5, 
    new int[] { /* even numbers */ }, 
    10, 1);
```

## Best Practices

✅ **DO:**
- Balance difficulty across levels
- Test every level before release
- Use variety of puzzle types
- Provide clear feedback (visual/audio)
- Ensure solvability (solve it yourself!)
- Document target score/time
- Create gradual difficulty curve

❌ **DON'T:**
- Make unsolvable puzzles
- Jump difficulty too fast
- Reuse same puzzle layout repeatedly
- Create overly complex grids (confusing)
- Make targets too high (frustrating)
- Ignore colorblind players
- Add levels without testing

## Templates

### Copy-Paste: Easy SumToTen
```csharp
CreateLevel(XX, "SumToTen", 5, 5,
    new int[] { 1,2,3,4,5, 1,2,3,4,5, 1,2,3,4,5, 1,2,3,4,5, 1,2,3,4,5 },
    10, 1);
```

### Copy-Paste: Medium ConnectPatterns
```csharp
CreateLevel(XX, "ConnectPatterns", 6, 6,
    GenerateRandomBoard(36, 5),
    15, 2);
```

### Copy-Paste: Hard SequenceOrder
```csharp
CreateLevel(XX, "SequenceOrder", 6, 6,
    GenerateRandomBoard(36, 12),
    20, 3);
```

## Automation Script

To generate a batch of random levels:

```csharp
// In LevelManager.cs
private void GenerateLevels(int startId, int count)
{
    for (int i = 0; i < count; i++)
    {
        int levelId = startId + i;
        string rule = ((i % 3) == 0) ? "SumToTen" : 
                      ((i % 3) == 1) ? "ConnectPatterns" : "SequenceOrder";
        int difficulty = (i / 5) + 1;  // Increase every 5 levels
        int gridSize = (difficulty > 1) ? 36 : 25;
        int maxValue = difficulty * 3;
        
        CreateLevel(levelId, rule, 
            (gridSize == 36) ? 6 : 5, 
            (gridSize == 36) ? 6 : 5,
            GenerateRandomBoard(gridSize, maxValue),
            (difficulty * 5), difficulty);
    }
}
```

## Troubleshooting

### "Puzzle is unsolvable"
- Verify your target is achievable with given tiles
- Check tile values manually
- Test in editor before release

### "Level too easy"
- Increase target value
- Use larger grid
- Increase difficulty rating
- Add more tiles to sort through

### "Level too hard"
- Decrease target
- Reduce grid size
- Use simpler values (1-5 instead of 1-15)
- Lower difficulty rating

---

## Summary

1. Open `LevelManager.cs`
2. Find `InitializeLevels()`
3. Add `CreateLevel()` call
4. Test in editor
5. Adjust difficulty if needed
6. Publish!

**Questions?** See `GAME_DESIGN.md` for full mechanics explanation.

---

**Last Updated:** 2026-02-20
**Version:** 1.0
