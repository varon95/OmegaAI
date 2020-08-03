# OmegaAI
## About Omega

Omega is a simple board game, developed by Néstor Romeral Andrés. The game is played on a board made of hexagonal fields, and the shape of the board is also a hexagon. The game can be played by two players, white (player 1) and black (player 2). The players take turns, in each turn, player one places two stones on the board, one black and one white. Then the second player does the same. This continues, until there is not enough empty space for both players to place stones. The score is then calculated by multiplying the sizes of the groups of the same colours.

## Demo

[![Omega AI demo](http://img.youtube.com/vi/YcAqGhwUx5o/0.jpg)](http://www.youtube.com/watch?v=YcAqGhwUx5o "")

## General approach

I wrote my program in C# using Microsoft Visual Studio. I generated the board from “Field” object, which is based on the button class to make it clickable. The Field shape are also modified to be hexagonal, then they are added to the user interface to create the board. On click events, they change their background color depending of the number of the previous steps (one step in this case is placing a stone).
The groups of each color are counted using the Union Find algorithm.

The AI uses NegaMax search with alpha-beta windows. The algorithm starts with alpha by choosing first a black then a white field from the empty fields. Then invites itself, decreasing the depth, negating and switching alpha and beta, and negating the color variable. Then, when depth reaches 0 or a terminal node is reached, it evaluates the node. The evaluation function is for simply the score, whith some added heuristics.

Upon initialization the program calculates the number of possible steps. When a stone is placed, this number is decreased by 1. When there are no more available steps left, the game ends, and a Message Box pops up.

## About the code files

**"Main.cs"** contains the important part of the code, other files are either design related, or class definitions.

