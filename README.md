# celeste-game-feel
This project was made to study how to implement a platflormer controller like in [Celeste](http://www.celestegame.com/ "Celeste Game") game. This project was build with Unity version [2019.3.0f6](https://unity3d.com/pt/unity/whats-new/2019.3.0 "Release Notes").

## Package Installed
[2D Pixel Perfect - 2.0.4](https://docs.unity3d.com/Packages/com.unity.2d.pixel-perfect@2.0/manual/index.html "Documentation")

## Game Fell
In addition to the basic mechanics, this project implement some little ajust as explained by [Matt Thorson](https://twitter.com/MattThorson "Twitter Profile") in this [thread](https://twitter.com/MattThorson/status/1238338574220546049 "Game Fell in Celeste"). Some of the game fell techniques implemented in this project are listed below.

### Coyote Time
You can still jump for a short time after leaving a ledge.

### Jump Buffering
If you press and hold the jump button a short time before landing, you will jump on the exact frame that you land.

### Jump Corner Correction
If you bonk your head on a corner, the game tries to wiggle you to the side around it.

## Preview

<table cellspacing="0" cellpadding="0">
  <tr>
    <td><img alt="Coyote Time" src="Preview/coyote-time.GIF" /></td>
    <td><img alt="Corner Correction" src="Preview/corner-correction.gif" /></td>
    <td><img alt="Ledge Climb" src="Preview/climb-ledge.GIF" /></td>
  </tr>
  
