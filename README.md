# fetch_urp

Here is our code! There are 3 main pieces in this folder.

## HTML

* `html_submission` and `index.html` hold the code that serves a static website showcasing our project
* This is the link to the website: https://taasinsaquib.github.io/fetch_urp/

## Pytorch

* `retina.ipynb` holds code that implements various neural networks for use with our retina. They are implemented using pytorch

## Unity

* Here we have the majority of our code, implemented as a scene in Unity
* To run the code locally, do the following:
    * Clone the repo
    * Import this folder into Unity as a new project, or [add](https://docs.unity3d.com/Manual/ImportingAssets.html) the Assets files to an existing project
    * There will be a path error, to solve that [re-add](https://docs.unity3d.com/Manual/upm-ui-local.html) the ML-Agents package with your local path
    * Press play and interact with the scene (scene1_no_terrain)!

* Some controls
    * WASD moves the head around. Views 2 and 3 show you the left and right eyes, respectively
    * The arrow keys move the entire dog
    * Space bar launches the bone, and a second press resets the bone and randomly chooses a new place to launch it to
    
