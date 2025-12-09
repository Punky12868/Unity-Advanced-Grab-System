# Advanced Grab System

## Overview

This repository contains an advanced grabbing system that allows players to interact with objects using physics, similar to the mechanics in the game **Teardown** or **R.E.P.O**. The system enables players to grab objects with mass and manipulate them using mouse and key controls. 

## Features

- Simple input for grabbing and releasing objects.
- Enable and disable features.
- Connection breaks if the object moves too far from the player.
- Heavier objects are more challenging to maneuver to the grab point.
- Visual feedback includes a line indicating the holding point and the grab point.
- Compatible with any type of mesh.

## Installation

1. Download the latest `Grab System` Unity package from the [Releases](https://github.com/Punky12868/Unity-Advanced-Grab-System/releases/tag/Releases) section.
2. Open your Unity project.
3. Import the package by simply dragging it into the assets folder, or go navigating to `Assets` > `Import Package` > `Custom Package` and selecting the downloaded `.unitypackage` file.

## Setup

- To set up the Grab System just attatch the `Grab System` to you player and place your desired camera on the `Camera Transform` field.
- In case of using object rotation, just add to your camera controller a way to disable mouse inputs with the `IsRotating` label.

## License

This project is licensed under the MIT License. See the [LICENSE](https://choosealicense.com/licenses/mit/) file for details.
