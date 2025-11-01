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

/cub
├─ Game.cs
├─ Program.cs
├─ Managers/
│  ├─ ModelManager.cs
│  └─ CollisionManager.cs
├─ Entities/
│  ├─ StaticInstance.cs
│  ├─ SecurityDoor.cs
│  ├─ ExitTrigger.cs
│  └─ IInteractable.cs
├─ Rendering/
│  ├─ Shader.cs
│  ├─ Mesh.cs
│  ├─ Texture.cs
│  └─ Camera.cs
├─ Geometry/
│  ├─ Aabb.cs
│  └─ Obb2D.cs
├─ Assets/
│  ├─ floor.jpg
│  ├─ wall.jpg
│  ├─ door.jpg
│  └─ keycard.jpg
└─ Shaders/
   ├─ vertex.glsl
   └─ fragment.glsl



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
Asset	Source from SketchFab / Author	License / Notes
Microscope - Download Free 3D model by Eugen Vahrushin (@eugen_vahrushin) [9562226]
Desk Low-Poly - Download Free 3D model by Pedro Belthori (@pedrobelthori) [ed62a64]
Chemistry Lab Table - Download Free 3D model by Jawahar Yokesh (@Jawahar_Yokesh) [fc5951d]
ApertureVR:TWP - Lab Chair - Download Free 3D model by nyctomatic (@nyctomatic) [90549fd]
Bar Stool - Download Free 3D model by Saandy (@Saandy) [aaf556f]
Water Bath - Download Free 3D model by VeeRuby Technologies Pvt Ltd (@veerubyinc) [7567889]
Digital timer programmer - Download Free 3D model by Sabowsla (@sabowsla) [e6814a0]
Safety Goggles - Download Free 3D model by C (@C44t) [57fdbe1]
Fire Extinguisher - Download Free 3D model by Loïc (@loichuet1) [5676b17]
Modern Fridge - Download Free 3D model by dylanheyes (@dylanheyes) [366f7df]
LowPoly - Flask - Download Free 3D model by BerserkerBroon (@BerserkerBroon) [dcf232c]
Magic flask - Download Free 3D model by OlegPopka (@OlegPopka) [60e1635]
Conical Flask - Download Free 3D model by VeeRuby Technologies Pvt Ltd (@veerubyinc) [21f6d8e]
Ceiling Light - Download Free 3D model by Heliona (@Heliona) [3e65ceb]
PCR machine - Download Free 3D model by orphic_oasis8 (@orphic_oasis8) [a76bd38]
Test Tube (Mutations) - Download Free 3D model by Michael V (@bossdeff) [96785b6]
Keycard Model - Download Free 3D model by SemB (@SemBoekenoogen) [528fff6]
Card Security Reader - Download Free 3D model by Anom Purple Modelling (@Anom404) [9ff3c81]
door door metal - Download Free 3D model by Mehdi Shahsavan (@ahmagh2e) [b21ec27]
Lab Counter W/ Sink (UV Wrapped, No textures) - Download Free 3D model by Kimbell Whatley (@KimbellWhatley) [19159db]

<img width="1272" height="752" alt="image" src="https://github.com/user-attachments/assets/36757315-330d-41da-b665-8bbdec1d0f8e" />
<img width="1276" height="748" alt="image" src="https://github.com/user-attachments/assets/24d7c7f2-fe43-4689-83ee-2b6a8152e301" />
<img width="1272" height="756" alt="image" src="https://github.com/user-attachments/assets/2074405b-0d71-49bc-9094-b5ab574646a5" />



All assets are used strictly for academic demonstration under fair educational use.
