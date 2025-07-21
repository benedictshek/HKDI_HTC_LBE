# Image Sequencer
Animate image sequences on sprite renderers
---
## Introduction
This utility can be used to take a sequence of still images and play them as an animation in your Unity project and has been provided free-of-charge from Dauntless XR!  Images can be loaded either in the Unity editor or procedurally at runtime using a single method LoadImageSequence().  
---
## Usage
### Setup
Create a game object with a sprite renderer component that will be the render target for the video playback.  Then attach the ImageSequencer.cs script to a game object in your scene.  Generally speaking it is best practice to attach the ImageSequencer.cs script to the same game object with the sprite renderer though this is not required for functionality.  Just make sure you include your sprite renderer on the "Render Target" field in the inspector for the playback to work properly.

### Inspector Settings
First, drag and drop your image sequence into the "Image Sequence" array in the inspector.  Please note that the images need to be placed in the array in sequential order with the starting frame at index 0 of the array, since playback simply iterates through the array by index and will not parse image names to determine the order.
Next, drag and drop the game object you set up in the previous step with the sprite renderer component into the "Render Target" field.  This informs the script on where to render the image sequence.
Then, choose the playback settings:
Playback Speed: Speed of the video playback in frames-per-second
Playback Mode: There are 3 options
- Play Once: Plays the image sequence one time, stopping playback when it reaches the end of the sequence.
- Loop: Plays the image sequence continuously from end-to-end, starting over from the beginning when it reaches the end of the sequence.
- Ping Pong: Plays the image sequence continuously from end-to-end, reversing playback direction when it reaches the end of the sequence.
Play On Start: Determines if the video playback should start as soon as the component is loaded during runtime.
Is Reversed: Boolean value to reverse the playback.  Generally you do not need to manipulate this setting manually unless playing the video in reverse during runtime is desired.
Starting Frame: Sets the start point for your animation.  Generally you do not need to manipulate this setting manually either unless you are loading an image sequence procedurally that contains unsplit animations.

### Methods
There are several key utility methods that are necessary for processing the image sequence before playback:
LoadImageSequence(List<Texture2D> images): Loads an image sequence procedurally from a list of Texture2D images during runtime.  Multiple overload methods are available for you to choose the precise nature of the playback behavior when loading images.
PrepareSequence(bool playOnComplete):  Prepares an image sequence for playback by converting the image list to a sprite array.  This method is called before playback begins and takes a bool playOnComplete argument that determines whether the playback begins immediately upon preparation.

There are multiple methods to allow you to control playback methods as well:
Play(): Begin playback from current frame
PlayFromStart(): Begin playback from starting frame
PlayFromFrame(int frame): Begin playback from specified frame
Pause(): Stops playback at current frame.
Stop(): Stops playback and resets to the starting frame and playback speed
FastForward(): Doubles playback speed without reversing playback direction
Rewind(): Doubles playback speed and reverses playback direction
---
## Support
Contact us for any issues or feedback regarding this package.
hello@dauntlessxr.com