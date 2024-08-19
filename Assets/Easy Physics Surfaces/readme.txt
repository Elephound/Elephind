Hi, my name is Maxim and I'll going to tell you how to use this asset

It is very simple:

1. Create>Physics>Surface
    Set audioClips and and as you wish

2. Add PhysicsSurfaceData.cs to you surface ( plane, box, mesh, whatever with collider )

3. Set in PhysicsSurfaceData properties desired surface.

4. Do raycasts while your player moving and get PhysicsSurfaceData from hit

5. Use GetFootstepSound() from your raycasted PhysicsSurfaceData

6. Play this clip

7. ???

8. Profit!