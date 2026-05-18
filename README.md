# 🏂 Ice Quest

> A 2D snowboarding game built with Unity and C# — race down icy slopes, pull off spins, collect power-ups, and reach the finish line with the highest score possible.

---

## 📖 About the Game

**Ice Quest** is a 2D physics-based snowboarding game developed with Unity and C# as a lab assignment for **PRU212 — Game Development with Unity** at FPT University. Players control a snowboarder sliding down snow-covered slopes across 3 progressively harder levels, collecting coins, picking up power-ups, avoiding obstacles, and performing aerial spins to earn bonus points.

The game features a full star rating system, per-level high score persistence, and level unlock progression.

---

## 🎮 Gameplay

- Slide down the slope — speed builds naturally based on terrain angle
- Reach the **Finish Line** to complete the level and earn a star rating
- Fall off the map or crash head-first into the ground → **Game Over**
- Hit a **wood obstacle** → speed reduced (unless shielded)
- Perform **aerial spins** while jumping to earn bonus score points

### Scoring System

| Source | Points |
|---|---|
| Distance traveled (furthest X position) | +1 per unit |
| 1 full spin in the air | +100 |
| Each additional spin | +150 per extra spin |

### Star Rating Thresholds

| Level | 1 ⭐ | 2 ⭐ | 3 ⭐ |
|---|---|---|---|
| Level 1 | 2,000 | 3,000 | 3,600 |
| Level 2 | 5,400 | 6,400 | 7,200 |
| Level 3 | 5,400 | 6,900 | 8,500 |

---

## ✨ Features

### Player Movement
- **Physics-based slope movement** — speed and torque scale with terrain angle via Rigidbody2D
- **Jump** — press Space to jump; aerial rotation controlled with A/D keys
- **Balance system** — auto-corrects player angle to match slope normal on the ground
- **Stability system** — prevents excessive tipping when grounded
- **Spin detection** — tracks full 360° rotations while airborne (minimum 0.5s in air); awards bonus score on landing

### Power-Up Items

| Item | Effect |
|---|---|
| Speed Boost | Smoothly accelerates the player to a boosted velocity for a set duration, then gradually decelerates back |
| Helmet (Shield) | Activates a timed shield (5 seconds); absorbs one wood obstacle hit; plays appear/idle/fade animations |
| Coin | Collectible item |

### Obstacles

| Obstacle | Effect |
|---|---|
| Wood log | Reduces speed by 50% for 3 seconds; if shielded, consumes the shield instead |
| Head crash (ground collision) | Triggers Game Over |
| Fall zone (off-map) | Triggers Game Over |

### Level System
- **3 playable levels** with unique terrain layouts and increasing difficulty
- **Level unlock progression** — Level 2 unlocks after completing Level 1; Level 3 unlocks after Level 2
- **Star display** on level select screen shows best star rating per level
- **Next Level** button on win screen advances to the next scene

### Score Persistence
- Per-level **high score** saved to JSON file (desktop) or PlayerPrefs (WebGL)
- **Level completion status** also persisted — used for unlock checks and star display
- Scores only saved on level complete or game over (not mid-run)

### UI & Menus
- **Main Menu** — Play, Options (3-page), Exit
- **Level Select Panel** — shows 3 levels with star ratings; locked levels are non-interactable
- **Pause Panel** — press **P** to pause; Resume, Restart, Settings, Exit to Menu
- **Game Over Panel** — Restart, Settings, Exit to Menu
- **Win Panel** — shows star rating image, Next Level, Exit to Menu
- **Settings Panel** — accessible from Pause and Game Over panels; returns to calling panel on close
- **Spin Bonus Popup** — briefly shows "+bonus" text on screen when a spin is scored

### Audio
- Background music on main menu (random track selection)
- In-game music (random track selection)
- Win music and lose music on level complete / game over
- SFX: button clicks, shield pickup, shield disappear, wood hit
- AudioManager singleton persists across scenes

### Visual
- **Parallax scrolling background** — multi-layer depth effect
- **Camera follow** — smooth camera tracking of the player
- **Background follow** — background layers scroll relative to camera
- **Dust trail** particle effect while sliding
- **Snow particle generator** on the slope
- **Shield animation** — appear, idle loop, fade out states via Animator
- **Wood obstacle animation** — triggered on player collision
- Custom **Ice Kingdom** font for UI
- URP (Universal Render Pipeline) for 2D lighting

---

## 🛠️ Built With

| Technology | Details |
|---|---|
| Engine | Unity (2D) |
| Render Pipeline | Universal Render Pipeline (URP) |
| Language | C# |
| Physics | Unity 2D Rigidbody + custom PhysicsMaterial2D |
| UI Text | TextMeshPro |
| Animation | Unity Animator + DOTween |
| Persistence | JSON file (desktop) / PlayerPrefs (WebGL) |
| Version Control | Git |

---

## 🏗️ Project Structure

```
Assets/
├── Scenes/
│   ├── GameMenu.unity          # Main menu scene
│   ├── Level1.unity            # Level 1
│   ├── Level2.unity            # Level 2
│   └── Level3.unity            # Level 3
├── Scrips/
│   ├── PlayerController.cs     # Movement, jump, spin detection, shield, speed boost
│   ├── GameManager.cs          # Pause, game over, win, restart, next level
│   ├── ScoreManager.cs         # Distance-based score + spin bonus tracking
│   ├── GlobalScoreManager.cs   # Per-level high score & completion persistence (JSON/PlayerPrefs)
│   ├── LevelCompletionChecker.cs # Star threshold logic and level unlock checks
│   ├── GameMenuController.cs   # Main menu, level select, options panel
│   ├── CrashDetector.cs        # Head crash → Game Over trigger
│   ├── FallZone.cs             # Off-map fall → Game Over trigger
│   ├── FinishLine.cs           # Finish line → Win trigger
│   ├── SpeedItem.cs            # Speed boost power-up
│   ├── HelmetItem.cs           # Shield power-up (helmet)
│   ├── PlayerShield.cs         # Shield visual + invulnerability logic
│   ├── WoodItem.cs             # Wood obstacle — slows player or consumes shield
│   ├── CoinItem.cs             # Coin collectible
│   ├── CameraFollow.cs         # Smooth camera tracking
│   ├── BackgroundFollow.cs     # Background layer follow
│   ├── ParallaxBackground.cs   # Parallax scrolling effect
│   ├── Parallax.cs             # Per-layer parallax helper
│   ├── DustTrail.cs            # Dust particle effect while sliding
│   ├── ItemTrigger.cs          # Generic item trigger handler
│   ├── DestroyItem.cs          # Auto-destroy items off screen
│   ├── SpeedDisplay.cs         # Real-time speed display UI
│   ├── ScoreItem.cs            # Score pickup item
│   ├── HighScoreDisplay.cs     # High score UI display
│   └── IgnoreOnceIfShielded.cs # Collision ignore helper for shielded player
├── Prefabs/                    # Player, items, obstacles, UI panels, star ratings
├── Animations/                 # Player, shield, item, and obstacle animations
├── Audios/                     # Music and SFX clips
├── Sprites/                    # Player, background, GUI, map, and tile sprites
├── Physic2d/                   # Custom PhysicsMaterial2D for player
├── Fonts/                      # Ice Kingdom custom font
└── BackGrounds/                # Background layer images
```

---

## 🚀 Getting Started

### Prerequisites
- Unity Hub installed
- Unity Editor (check `ProjectSettings/ProjectVersion.txt` for exact version)
- Git with long path support (recommended on Windows):
  ```bash
  git config --global core.longpaths true
  ```

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```

2. Open **Unity Hub** → **Add project from disk** → select the cloned folder

3. Open the project in Unity Editor

4. Open `Assets/Scenes/GameMenu.unity` and press **Play**

### Play Online
🎮 [Play on itch.io](https://hauhuynh2k4.itch.io/ice-quest)

---

## 🎮 Controls

| Key | Action |
|---|---|
| Automatic | Slide down slope |
| `Space` | Jump |
| `A` / `←` | Rotate left (in air) |
| `D` / `→` | Rotate right (in air) |
| `P` | Pause / Resume |

---

## 📚 Course Information

- **Course**: PRU212 — Game Development with Unity
- **Semester**: Semester 7
- **University**: FPT University

---

## 📝 License

This project is developed for educational purposes as part of the FPT University curriculum.
