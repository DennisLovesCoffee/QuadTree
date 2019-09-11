# QuadTree
 - Can be used for faster finding of the closest game objects or positions within a given range.
 
 # Exanples
 - You can use it in a simulated enviroment where each animal wants to find the closest source of water/food.
 
 
 # Why a Quad Tree?
 - It can be used to search a 3D area projected into a 2D area (not using the height).
 - It also does NOT need to compare distances of all objects with each other (2 for loops). 
 - It instead uses a tree structure which subdivides space recursively into exactly 4 children.
 - This makes the search much easier since we can give it a point and a max Distance (for ex. the view distance of an object) 
 and than just search the exact distance and find the closest object, or get null, if we can not find anything.
 - More information here: https://en.wikipedia.org/wiki/Quadtree .
