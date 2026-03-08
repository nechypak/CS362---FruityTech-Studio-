# FruityTech Studio Installation and Usage Guide

This document explains how to install and run FruityTech Studio if you are:
- A normal user who wants to run the app
- A developer who wants to open and run the source code in Unity

## 1. What This Software Is

FruityTech Studio is a Unity desktop application (not a website, and not a mobile app).  
You run it either:
- As a prebuilt desktop executable from GitHub Releases, or
- From source using Unity Editor.

Project repository:
- https://github.com/nechypak/CS362---FruityTech-Studio-

Releases page:
- https://github.com/nechypak/CS362---FruityTech-Studio-/releases

---

## 2. System Requirements

Minimum recommended:
- 64-bit Windows 10+ or macOS 12+
- Speakers or headphones
- Keyboard and mouse/trackpad

For source/developer setup:
- Unity Hub (latest stable)
- Unity Editor `6000.3.6f1` (required for this project)
- Git (to clone/download the repository)

---

## 3. Option A (Recommended): Install a Prebuilt Desktop Version

Use this if you just want to run the application.

1. Open the Releases page:  
   https://github.com/nechypak/CS362---FruityTech-Studio-/releases
2. Download the latest release archive for your OS.
3. Extract the archive.
4. Run the app:
   - Windows: double-click `FruityTechStudio.exe`
   - macOS: open `FruityTechStudio.app`
5. If your OS shows a security warning:
   - Windows: click `More info` -> `Run anyway` (only if downloaded from the official repo).
   - macOS: right-click app -> `Open` -> confirm.

Notes:
- You do not need Unity installed for a prebuilt release.
- If no release is available for your OS, use Option B below.

---

## 4. Option B: Install and Run from Source (Unity)

Use this if you are building or developing the project.

### 4.1 Install Prerequisites

1. Install Git  
   - https://git-scm.com/downloads
2. Install Unity Hub  
   - https://unity.com/download
3. In Unity Hub, install Unity Editor version `6000.3.6f1`  
   Include modules for your platform:
   - Windows Build Support (on Windows)
   - Mac Build Support (on macOS)

### 4.2 Download the Source Code

Option 1 (Git clone):

```bash
git clone https://github.com/nechypak/CS362---FruityTech-Studio-.git
```

Option 2 (ZIP):
1. Open repository page.
2. Click `Code` -> `Download ZIP`.
3. Extract the ZIP.

### 4.3 Open the Project in Unity Hub

1. Launch Unity Hub.
2. Click `Add` (or `Add project from disk`).
3. Select this folder inside the repository:
   - `src/FruityTech-Studio`
4. Ensure Unity Hub shows editor version `6000.3.6f1`.
5. Click `Open`.

First open may take several minutes while Unity imports packages.

### 4.4 Run the Project in Unity

1. In Unity, open scene:
   - `Assets/Scenes/SampleScene.unity`
2. Press the Play button at the top of the Unity Editor.
3. Verify audio output device is enabled on your computer.

---

## 5. Build a Desktop Executable Yourself (Optional)

1. In Unity, go to `File -> Build Settings`.
2. Confirm scene list includes:
   - `Assets/Scenes/SampleScene.unity`
3. Select target platform under `PC, Mac & Linux Standalone`.
4. Click `Build`.
5. Choose an output folder and wait for build to finish.

---

## 6. Dependencies Summary

Runtime dependencies (for prebuilt app):
- OS: Windows 10+ or macOS 12+
- Audio output device

Development dependencies (for source use):
- Unity Hub
- Unity Editor `6000.3.6f1`
- Unity packages from `src/FruityTech-Studio/Packages/manifest.json` (auto-restored by Unity)
- Git

---

## 7. Troubleshooting

If the project does not open in Unity:
- Verify you opened `src/FruityTech-Studio` (not the repository root).
- Verify Unity version is exactly `6000.3.6f1`.
- Close Unity and reopen the project from Unity Hub.

If audio is silent:
- Check system volume and output device.
- Check Unity Game view is focused while testing in editor.

If prebuilt app does not launch:
- Re-download the release archive from the official Releases page.
- Extract fully before launching (do not run from inside compressed archive).

---

## 8. Where to Get Help

- Issues / bug reports:  
  https://github.com/nechypak/CS362---FruityTech-Studio-/issues
- Repository home:  
  https://github.com/nechypak/CS362---FruityTech-Studio-
