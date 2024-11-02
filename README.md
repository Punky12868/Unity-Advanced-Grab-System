# Advanced Grab System

## Overview

This repository contains an advanced grabbing system that allows players to interact with objects using physics, similar to the mechanics in the game **Teardown**. The system enables players to grab objects with mass and manipulate them using mouse controls. 

## Features

- Simple input for grabbing and releasing objects.
- Connection breaks if the object moves too far from the player.
- Player movement is influenced by the mass of the carried object.
- Heavier objects are more challenging to maneuver to the grab point.
- Visual feedback includes a line indicating the holding point and the grab point.
- Compatible with any type of mesh.
- Objects can be moved closer or further away using the mouse scroll wheel.

## Installation

1. Download the latest `Grab System` Unity package from the [Releases](https://github.com/Punky12868/Unity-Advanced-Grab-System/releases/tag/Releases) section.
2. Open your Unity project.
3. Import the package by simply dragging it into the assets folder, or go navigating to `Assets` > `Import Package` > `Custom Package` and selecting the downloaded `.unitypackage` file.

## Setup

To set up the Grab System, follow these steps:

#### Components

Ensure your player character is set up according to the intended configuration of this repository:

- Add a **Collider** component to your player.
- Attach a **Rigidbody** component to your player for physics interactions.
- Include the following scripts on your player:
  - `Camera Controller`
  - `First Person Character Controller`
  - `Grab System`
- Optionally, you can add the `Draw Grab Line` script if you want to visualize the grabbing mechanics through the inspector.

#### Scene Configuration

1. **Camera Setup**:
   - Create an empty GameObject to serve as the camera holder. Place this GameObject inside the player object.
   - Attach the `Set Pos Rot` script to this camera holder GameObject.

2. **Grab Point Setup**:
   - Create another empty GameObject to act as the grab point, positioning it inside the camera holder GameObject.

3. **Ground Check**:
   - Add an empty GameObject as a child of the player to serve as the ground check.

#### References

- In the `Camera Controller` script attached to your player, set the empty GameObject (the camera holder) as the **Camera Target**, and assign the player GameObject as the **Body Target**.
  
- In the `First Person Character Controller` script, assign the empty GameObject designated for the ground check to the **Ground Check** field.

- In the `Grab System` script, set the empty GameObject for the grab point in the **Grab Point** field, and define the appropriate **LayerMask** for the objects that can be grabbed.


## License

This project is licensed under the MIT License. See the [LICENSE](https://choosealicense.com/licenses/mit/) file for details.
