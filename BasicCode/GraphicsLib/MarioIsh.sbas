REM Uses mario.png not included in repo for copyright reasons
#include "GraphicsLib.sbas"

1 win = GfxNewWindow(640, 480)
2 clock = GfxNewClock()
3 marPng = GfxNewTexture("/Users/adam/Documents/Mac Projects/SuperBAS/BasicCode/GraphicsLib/mario.png")
4 mario = GfxNewSprite(marPng)
5 gravitySpeed = 500
6 GfxSetSpritePosition(mario, 0, 0)
7 GfxScaleSprite(mario, 0.1, 0.1)
8 floorHeight = 100
9 moveSpeed = 400
10 jumpSpeed = 700

REM Main loop
100 deltaTime = GfxRestartClock(clock)
101 GfxHandleEvents(win)
102 GfxClearWindow(win)

REM Gravity
103 jumping = GfxIsKeyPressed("Up")
104 IF GfxGetSpriteY(mario) < 390 THEN IF jumping < 1 THEN GfxMoveSprite(mario, 0, gravitySpeed * deltaTime)

REM Movement
105 IF GfxIsKeyPressed("Right") > 0 THEN GfxMoveSprite(mario, moveSpeed * deltaTime, 0)
106 IF GfxIsKeyPressed("Left") > 0 THEN GfxMoveSprite(mario, 0 - moveSpeed * deltaTime, 0)
107 IF jumping > 0 THEN GfxMoveSprite(mario, 0, 0 - jumpSpeed * deltaTime)

110 GfxDraw(win, mario)
111 GfxDisplayWindow(win)

200 IF GfxWindowIsOpen(win) > 0 THEN GOTO 100
201 PRINT "Game Exited"
