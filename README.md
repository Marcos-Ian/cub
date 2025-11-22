Collision Detection System

Overview

This exploration game using 3D implements as well a circle-vs-OBB ( Oriented Bounding Box ) collision detection system to allow the player to not clip through walls and furniture. The collision system works on the XZ-plane (at ground level) and the footprints of the entire fixed geometry of the lab (the walls, door, bed, desk, wardrobe, and sidetable) are represented by oriented bounding boxes. An OBB contains its position in the world (center of the world), half-extents (width and depth), the angle of rotation (yaw), and the vertical span (Y-range). The CircleOverlapsObb algorithm adjusts the size of the circular collision radius of the player to each OBB frame, then calculates the nearest point on the box and computes a push-out vector when penetration occurs. This solution manages rotated colliders gracefully, and offers smooth collisions with edges that slide smoothly without sticking on corners.
Combination with Movement and Rendering.

The collision system is fully embedded in the game update loop and render pipeline. In OnUpdateFrame, the camera position should have been updated based on input and the ResolveCircleVsScene method is used to loop through all registered OBBs and execute minimal push-out vectors to correct any overlaps. This makes sure that the player is pushed aside softly and can slide down sides automatically. CollisionManager The CollisionManager is where all collision logic is done, and it keeps a read-only list of OBBs, which is rebuilt when models are loaded or scenes are reset. To debug, press B to also show the visualization of wireframe of all the collision boxes as part of the DrawCollisionDebug, drawing the green outlines in the same coordinate space as the game world. The collision boxes are correctly scaled and rotated to correspond to the transforms of their corresponding models and thus it is simple to ensure colliders are correct with the visual geometry.

Challenges and Solutions

The initial problem that needed to be solved was how to scale collision boxes properly in models with very different sizes, i.e. the bed model models a scale of 0.0015, the desk models a scale of 21 and the wardrobe models a scale of 0.009. To start with, the collision sizes were defined as world-space quantities that resulted in either physically invisible colliders or gigantic, imaginary boxes. The answer was to make all collision sizes model-local and to use the AddCompoundModelCollider method to automatically scale them by model transformation. The other problem was in making sure that the debug visualization corresponded to the actual collision logic. The first wireframe sketch relied on hard coded height value rather than actual MinY and MaxY range of the OBB, which made boxes to appear in the wrong location. This was resolved by dynamically calculating the height based on the vertical span of the OBB and centering the visual box on this. Lastly, a remnant of the previous development of a giant collision box at (-50, 0, 50) was generating undesirable collisions until the code was eliminated. Instrumenting the console with detailed output when a collision is being set up was very useful in finding which models were null or of wrong size, during debugging.
<img width="1277" height="715" alt="image" src="https://github.com/user-attachments/assets/bf5bd749-540c-4835-ba85-8c1c037422dc" />
<img width="1259" height="722" alt="image" src="https://github.com/user-attachments/assets/800400c1-82f2-4b75-b12e-ab1e3b1cacad" />

assets : Sketchfab: Bedside Table – Download Free 3D model by Melon Polygons (@Melonpolygons) [b25de0e].
https://sketchfab.com/3d-models/bedside-table-b25de0eb97c64a00a538f09fc7072f67

Sketchfab: Low Poly Computer Desk – Download Free 3D model by Glowbox 3D (@glowbox3d) [c67f61f].
https://sketchfab.com/3d-models/low-poly-computer-desk-c67f61fa444044bcb88ec3e28f0ca7ac

Sketchfab: Wardrobe – Download Free 3D model by khanhnguyen1189 (@khanhnguyen1189) [bc15229].
https://sketchfab.com/3d-models/wardrobe-bc15229716ac48b6865d4f80c78b1f6f

Sketchfab: Bed – Download Free 3D model by fredmilk (@fredmilk3) [ed70318].
https://sketchfab.com/3d-models/bed-ed70318df3b549f6bc4544d87867287b

Sketchfab: Door – Download Free 3D model by Arnau Rocher Alcayde (@ArnauRocherAlcayde) [d6b9b84].
https://sketchfab.com/3d-models/door-d6b9b84b78ae4462a6cf021b412085fe

Poly Haven: Laminate Floor 02 — Free texture by Dario Barresi & Dimitrios Savva (CC0) [“laminate_floor_02”]. https://polyhaven.com/a/laminate_floor_02
