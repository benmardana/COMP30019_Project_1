# Project Description

## Diamond Square Algorithm

The terrain is generated using an implementation of the diamond-square algorithm. (PlaneScript.cs:70)

Our algorithm works by computing a 2D-array heightmap which is then turned into a 1D array of vertices

Summary:
```
While the length of the side of the squares is greater than zero {
	Pass through the array and perform the diamond step for each square present.
	Pass through the array and perform the square step for each diamond present.
	Reduce the random number range.
	Half the side length.
}
```

Our algorithm sets a fixed value for all edges for aesthetics and so that they could feasibly be 'tiled' together. 
(PlaneScript.cs:209, :243)

We settled on a starting 'roughness' of 15.0 and multiplied the random number range by a factor of 1.6 in each loop.
This provided a good distribution of realistic looking heights.
The terrain has dimensions 128x128 vertices.
Pressing the 'space bar' with the game running will generate a new terrain. This helps to visualize the degree of randomness in our terrain generation.

We used this well documented java implementation of the diamond-square algorithm as a guide:
https://stackoverflow.com/questions/2755750/diamond-square-algorithm

## Water

The water plane is generated as a flat mesh on the x and z axes.
The height of the plane is determined dynamically each render as a function of other terrain heights 
(PlaneScript.cs:101)
This ensures that water is always present on the terrain and that it is always next to sand, giving a beachy look.

Waves are present in the water; this was achieved through vertex displacement in the vertex shader.
To achieve better visibility of wave presence, we displaced the color value of the vertex by the same amount.
(WaterPhongShader.Shader:36-38)

## Shaders

The terrain and water elements both use Phong Shaders implemented following guidelines from this subject.

We created a custom light object and passed it's location and light color to the shaders each frame. 
(LightScript.cs:33-35)	

In both implementations we opted for the classical reflection calculation over the approximation as we were able to get more
'natural' looking light given the setting. The approximation gave too tight a specular reflection for this implementation.

## Camera and Collisions

The camera is generated at run time on a game object.
We utilised the standard Unity control method, GetAxis(), which automatically maps to wasd, and mouse movement.
We also added roll on 'q' and 'e' to make orientation a little easier.
(CameraScript.cs:31-35)

The collision detection was done by adding a RigidBody to the Camera object and MeshColliders to the plane and water meshes.
We turned off gravity for the RigidBody and set normal drag and angular drag to infinity so the object would collide but not
experience any physical effects from it.
(CameraScript.cs:18-20)

## Sunlight

We also implememted an orbiting sun-like light source. The sphere orbits diagonally through the plane, rising and setting.
This gives a sense of day and night, as well as demonstrating the different components of our PhongShaders.
The orbit is elliptical, with a 'width' of 64 and 'height' of 96. This ensures the sphere rises and sets exactly on the corners of the plane.
