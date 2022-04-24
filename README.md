# TRIK-Studio-3DVizualization
A tool for visualizing programs created in TRIK Studio (https://trikset.com/products/trik-studio ).
Use Unity to create beautiful scenes and enjoy!

## Quick start
1. Install Unity (version no lower than 2019.4.3f1)
2. Install this package 
3. Create a copy of the "Template Scene", you can do this using the file manager
4. Decorate your scene using various objects from the package or purchase others from the Unity Asset Store.

Now you have a scene and you have to choose the vizualization method:

a) Use a trajectory from a file
or
b) Play animation in real time

### If you selected a), then you need to:
5. Run your program in TRIK Studio, wait until it stops, and then find the file "trajectory.json" in your folder.
6. Add the file to your Unity project folder "/Trajectories".
7. Click the "Run" button in Unity, and then click the "Run from file" button.

### If you selected b): 
5. Set an IP address of the computer with the Unity project to the TRIK Studio "Visualize IP" panel.
6. Click the "Run" button in Unity, and then click the "Run in realtime" button.


##Adding new objects

### Robot
To add robot object on scene, go to Window/TRIK Studio Interface and click "Add robot".

### Balls and skittles
You can find in Assets/Physic Objects/ template objects: ball, skittle and wall. To add object to scene just drag it with mouse. 

###Adding walls

If you want to add simple wall, drag it from  Assets/Physic Objects/ to scene and click on it. But if you have some complicated objects, which are acting like walls (for example, sofas),
you should create bounds around it using template walls. To make these walls transparent, choose them, go to Inspector and remove tick sfrom Mesh Renderer.

### Changing object appearance

If you want to change object material/color, find it in Assets/Materials/ and drag it to object.
If you want to change object shape, click on it and look at Inspector panel. At Mesh Filter/Mesh choose new mesh. This procedure can change object size. Don't be afraid, just go to Inspector and fix it in Transfom/Scale.

