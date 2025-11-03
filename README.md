<<<<<<< HEAD
=======
<<<<<<< HEAD
# VEX IQ Card Debugger

A Windows application for reading, decoding, and managing VEX IQ SD card `.txt` files. The debugger displays device actions, ports, and times in a human-readable format.

---

## The SDCard HAS To Be Letter V Or Else It Won't Work

## Features

- Load and display `.txt` files from the VEX IQ SD card.
- Decode tokens into readable actions for various devices:
  - **Controller** (EUp, EDw, FUp, FDw, LUp, LDw, RUp, RDw)
  - **Motor Group** (Frd, Rrs, Stp)
  - **Motor** (Frd, Rrs, Stp)
  - **Bumper** (Prs)
  - **TouchLED** (Tch)
  - **Pneumatic** (PON, POF, 1EX, 1RT, 2EX, 2RT)
- Display token, time, device, port, and decoded action.
- Right-click `.txt` files to delete them.
- Dark-themed UI with adjustable log font size.
- Self-contained `.exe` build for easy deployment.

---

## Installation

1. Clone the repository:


git clone https://github.com/user123467567657567/Vex-IQ-Card-Application.git
cd Vex-IQ-Card-Application/MyTestApp

dotnet publish -c Release -r win-x64 --self-contained true -o full folder name to build to

[13-bit binary time][Device Letter][Port][Action Code] 
13-bit is 60 seconds + 2 decimal

C = Controller, D = Drivetrain, G = Motor Group, M = Motor,
B = Bumper, X = Distance, T = TouchLED, Y = Color,
V = Vision, A = AI Vision, O = Optical, Z = Gyro, P = Pneumatic

Example
0001010101010P12EX1
Decoded too
(0001010101010P12EX1) 6.82s Pneumatic 12 Cylinder 1 Extend

Contributing

Fork the repository.

Create a new branch for your feature/bugfix.

Commit changes with clear messages.

Push the branch and open a Pull Request.
=======
****


>>>>>>> eadebf2 (Make MainForm start fullscreen and clean up fileList declaration warning)
*How to use*
[time][device][port][action]
first 13bits are binary to be 60 seconds + 2 decimal places
**TO BE ADDED**
Distance
Color
vision
AI Vision
Optical
Gyro
**DEVICES**
        { 'C', "Controller" },
        { 'D', "Drivetrain" },
        { 'G', "Motor Group" },
        { 'M', "Motor" },
        { 'B', "Bumper" },
        { 'X', "Distance" },
        { 'T', "TouchLED" },
        { 'Y', "Color" },
        { 'V', "Vision" },
        { 'A', "AI Vision" },
        { 'O', "Optical" },
        { 'Z', "Gyro" },
        { 'P', "Pneumatic" }
**PORTS** 
1-12 
00 For Controller
**ACTIONS**
        { "PON", "Pump On" },
        { "POF", "Pump Off" },
        { "EX1", "Cylinder 1 Extend" },
        { "RT1", "Cylinder 1 Retract" },
        { "EX2", "Cylinder 2 Extend" },
        { "RT2", "Cylinder 2 Retract" },
        { "EUP", "EUp Pressed" },
        { "EDW", "EDown Pressed" },
        { "FUP", "FUp Pressed" },
        { "FDW", "FDown Pressed" },
        { "LUP", "Left Up Pressed" },
        { "LDW", "Left Down Pressed" },
        { "RUP", "Right Up Pressed" },
        { "RDW", "Right Down Pressed" },
        { "FRD", "Forward" },
        { "RRS", "Reverse" },
        { "STP", "Stop" },
        { "PRS", "Pressed" },
        { "TCH", "TouchLED Pressed" }
<<<<<<< HEAD
=======
# Vex-IQ-Card-Application
A C# application that decodes a string and shows it to you from a VEX IQ program.
>>>>>>> 469ab35fdf76ae40f30c6c0b331dce1be077b1bf
=======
>>>>>>> 396b882 (Initial commit of VEX SD Card Debugger)
>>>>>>> eadebf2 (Make MainForm start fullscreen and clean up fileList declaration warning)
