# RecreatedSnake
This is my try to recreate Snake in a Windows Shell in C#. 

# Gameplay
"The player controls a dot, square, or object on a bordered plane. As it moves forward, it leaves a trail behind, resembling a moving snake. In another common scheme the snake has a specific length, so there is a moving tail a fixed number of units away from the head. The player loses when the snake runs into the screen border, a trail, or another obstacle.
...
In the second variant, a sole player attempts to eat objects by running into them with the head of the snake. Each object eaten makes the snake longer, so maneuvering is progressively more difficult. Examples: Nibbler, Snake Byte."
- http://en.wikipedia.org/wiki/Snake_(video_game)#Gameplay

# Controls  
- W - Changes direction to North.
- A - Changes direction to West.
- S - Changes direction to South.
- D -  Changes direction to East.

- P - Pause.
- O - Changes FPS.
- I - Toggle Debug Informations (May cause slowdowns).
- Space - Toggle Skip mode True or False.

#Download
I still didn't set up mit Git...
But you can still download it here! 
http://www.mediafire.com/download/xnkgqymyjbf9jj2/RecreatedSnake.zip

# Modes
If the Skip mode is on False you can queue up Input, which will be executed in the next available frame.
On Skip mode True you can not queue any Input, after each frame the Inputstream will be cleared (May also cause slowdowns).

# ToDo
- More Room Layouts.
- Scoreboard.
- Create a Versions History File.

# Known Issues
- occasionally the screen begins to flicker.
