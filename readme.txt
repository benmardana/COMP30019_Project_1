The terrain is generated using an implementation of the diamond-square algorithm.
Our algorithm works by computing a 2D-array heightmap.
The indexes correspond to an x and z co-ordinate, which map to a height.
We settled on a starting roughness of 10.0 and dividing by 1.8 through the algorithm loop. 
This provided a good distribution of heights.
The terrain has dimensions 128x128.

// BEN - Can you please explain our implementation of the square edges?

We used this diamond-square algorithm as a guide: https://github.com/rishsharma1/diamondSquareTerrain/blob/master/terrainGeneration/Assets/Scripts/diamondSquare.cs

The water plane is rendered as a flat plane through the x and z axes.
The height of the plane is determined dynamically each render as a function of other terrain heights according to.
This ensures that water is always present on the terrain and that it is always next to sand, giving a beachy look.

// BEN - Can you please explain the PhongShaders? I will talk about the waves.

Waves are present in the water; this was achieved through vertex displacement in the vertex shader.
To achieve better visibility of wave presence, we displaced by the color value of the vertex by the same amount.

// BEN - Camera and collision detection.

We also implememted an orbiting sun-like light source. The sphere orbits diagonally through the plane, rising and setting.
This gives a sense of day and night, as well as demonstrating the different components of our PhongShaders.
The orbit is elliptical, with a 'width' of 64 and 'height' of 96. This ensures the sphere rises and sets exactly on the corners of the plane.
