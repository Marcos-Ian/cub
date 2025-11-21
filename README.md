# 🧪 Mini 3D Explorer

A small **first-person 3D lab exploration game** built with **C# and OpenTK** and OpenGL.  
The player explores a science lab, finds a **keycard**, interacts with a **card reader** to open a **security door**, and can pick up **colorful potion flasks** that temporarily alter gameplay effects.

---

## 🎮 Gameplay Instructions

### Movement
| Action | Key |
|---------|-----|
| Move Forward / Backward | **W / S** |
| Strafe Left / Right | **A / D** |
| Look Around | **Mouse Movement** |
| Toggle 3D flight mode | **F** |
| Toggle Collision Detection | **C** |
| Toggle Collision Debug Boxes | **B** |
| Toggle Cursor Lock | **Esc** |
| Reset Scene | **R** |
| Interact (Pick up / Use)** | **E** |

**Interactions:**
- Pick up the **keycard** to unlock the lab door.
- Use the **card reader** beside the door to gain access.
- Pick up **flask potions** to trigger short visual effects (color inversion, camera flip, inverted controls).

---

## ✨ Features Implemented

- **Phong lighting model** for realistic highlights and shading  
- **First-person 3D camera** with mouse look and optional free-fly mode  
- **Physics-style collisions** with static lab geometry (walls, desks, props)
- **Interactive objects**:
  - Keycard and card reader system (unlocks door)
  - Door with smooth animation
  - Three potion types with visual effects:
    - Inverted colors
    - Inverted controls
    - Flipped camera
- **Lighting and Phong shading**
- **Transparent rendering** for glass and liquids
- **Debug collision boxes** toggle
- **Scene reset system**

---

## 🧱 Project Structure

<img width="377" height="737" alt="image" src="https://github.com/user-attachments/assets/8ac848a5-aabe-4214-84f2-87cdf021f3e0" />



---

## 🛠 How to Build and Run

### Requirements
- **.NET 8.0 SDK** (or compatible version)
- **OpenTK 4.x**
- **NuGet packages:**
  - `OpenTK`  
  - `System.Drawing.Common` (for texture loading)
  - `StbImageSharp` 

🧩 External Credits
Sketchfab models (titles · author · ID):

Microscope — Eugen Vahrushin — 9562226

Desk Low-Poly — Pedro Belthori — ed62a64

Chemistry Lab Table — Jawahar Yokesh — fc5951d

ApertureVR:TWP – Lab Chair — nyctomatic — 90549fd

Bar Stool — Saandy — aaf556f

Water Bath — VeeRuby Technologies Pvt Ltd — 7567889

Digital timer programmer — Sabowsla — e6814a0

Safety Goggles — C — 57fdbe1

Fire Extinguisher — Loïc — 5676b17

Modern Fridge — dylanheyes — 366f7df

LowPoly – Flask — BerserkerBroon — dcf232c

Magic flask — OlegPopka — 60e1635

Conical Flask — VeeRuby Technologies Pvt Ltd — 21f6d8e

Ceiling Light — Heliona — 3e65ceb

PCR machine — orphic_oasis8 — a76bd38

Test Tube (Mutations) — Michael V — 96785b6

Keycard Model — SemB — 528fff6

Card Security Reader — Anom Purple Modelling — 9ff3c81

Door (metal) — Mehdi Shahsavan — b21ec27

Assets are used under their respective Sketchfab licenses for educational/demo purposes.

<img width="1272" height="752" alt="image" src="https://github.com/user-attachments/assets/36757315-330d-41da-b665-8bbdec1d0f8e" />
<img width="1276" height="748" alt="image" src="https://github.com/user-attachments/assets/24d7c7f2-fe43-4689-83ee-2b6a8152e301" />
<img width="1272" height="756" alt="image" src="https://github.com/user-attachments/assets/2074405b-0d71-49bc-9094-b5ab574646a5" />
