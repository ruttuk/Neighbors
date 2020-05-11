# Neighbors
A bite-sized example of a recursive pathfinding algorithm.
#
![NeighborsClip](/Media/NeighborsClipGif.gif)
#
Given a 2D-Grid, find the shortest path from a given point to any other cell. This Grid data structure is composed of a 2-dimensional array of Cells, which are either shaded (cannot be moved onto) or not. Each Cell has a reference to 4 other Cells which represent it’s cardinal neighbors. To find the shortest possible path from any given cell, the current cell’s neighbors are traversed recursively. Each traversal decrements a movement point.

