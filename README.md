# Minesweeper in Unity
#### Video Demo:  [Link](https://youtu.be/G5TW2uCUHPI)
#### Description
Remake of the classic game minesweeper made with Unity. 

There is a build of the project for macOS silicon

#### Board.cs
This script has several methods, such as `setup()`, `generateBoardMask()`, `placeBombs()`, `calculateBombCount()`, `generateBoardBomb()`, `toggleFlag()`, `showTile()`, and `checkWin()`. These methods handle the game's various functions, including setting up the game, placing bombs, calculating the number of bombs adjacent to each tile, generating the board's masks, toggling flags on tiles, showing tiles, and checking for a win.

All the main functionality of the game itself is stored within this class to make interactions between different components easier and isolates it from other components such as UI to make maintenance easier.

#### GameManager.cs
The GameManager class in this Unity Minesweeper game is responsible for managing the game state, UI elements, and high scores. It has a `LevelType` enum for defining the game difficulty, `Camera`, `Board`, and `Highscore` to store data and reference other game components, and UI elements.

The class includes sound effects and music, it uses the `SoundManager` class to play sounds. It has a boolean variable `inGame` to track whether the player is currently playing or not, and a timer variable to track the time elapsed during gameplay.

The `Start()` function sets the default music to be played using the SoundManager class. The `Update()` function increments the timer and updates the flags count text on the UI.

The `startGame()` function sets up the board based on the selected difficulty level and displays the game panel while hiding the start panel. It also sets the inGame boolean to true.

The `endGame()` function ends the game by setting inGame to false, updates the high score if the player has won and their time is better than the previous best time, and displays the end panel. It also plays a sound effect depending on whether the player has won or lost.

The `restartGame()` function resets the timer, hides the end panel and game panel, and displays the start panel. The `resetgame()` function resets the game by calling the `restartGame()` function and then starting a new game with the same difficulty level.

#### Highscore.cs
This script is simply a scritable object that stores the highscores of the device and continues to store the data even when the game is not running.

#### SoundManager.cs
Basic script that is accessible by all objects to control 2 seperate audio sources and makes overall playing of music and sound effects convenient without having to reference it in every component. I used a singleton to allow global access to the SoundManager object.