# FruityTech Studio - User Manual  

## 1. High-Level Description

**FruityTech Studio** is an Unity-based 2D educational music sandbox game designed to make music creation intuitive, interactive, beginner-friendly.

Instead of working with complex timelines, piano rolls, or professional production tools like Ableton or FL Studio, users create music by **placing sound objects in a visual sandbox**. Each object automatically loops and synchronizes to a shared tempo, allowing users to experiment freely without needing music theory knowledge.

### What the System Does

FruityTech Studio allows users to:

- Place preloaded sound objects (notes, beats, melodies, loops) into a 2D sandbox
- Play synchronized music loops in real time
- Adjust properties such as volume and mute
- Save and load compositions
- Experiment creatively in a game-like environment

### Why Use FruityTech Studio?

You would use FruityTech Studio if you:

- Want to learn music fundamentals in a fun, low-pressure way  
- Want to prototype beats quickly  
- Prefer visual and interactive music creation  
- Feel overwhelmed by traditional music production software  

The system lowers the barrier to entry while still offering room for creative depth.

---

## 2. Installation Instructions

FruityTech Studio is distributed as a **Unity-based desktop application**.

### 2.1 System Requirements

- Operating System:
  - Windows 10 or later (64-bit)
  - macOS 12+
- Audio output device (speakers or headphones)

### 2.2 If You Are Running a Prebuilt Executable **(Recommended)**

1. Go to the GitHub repository:
   https://github.com/nechypak/CS362---FruityTech-Studio-
2. Navigate to the **Releases** section.
3. Download the appropriate build for your operating system.
4. Extract the `.zip` file.
5. Run:
   - Windows: `FruityTechStudio.exe`
   - macOS: `FruityTechStudio.app`

No additional software is required.

---

### 2.3 If You Are Running from Source (Developers)

#### Prerequisites

1. **Unity Hub**
   - Download from: https://unity.com/download
   - Install Unity Hub.

2. **Unity Editor**
   - **Required Version: 6000.3.6f1**
   - Install via Unity Hub.
   - Make sure to include:
     - 2D Core
     - Windows / macOS Build Support

#### Steps to Open the Project

1. Clone the repository:
  ```bash
  git clone https://github.com/nechypak/CS362---FruityTech-Studio-
  ```
2. Open Unity Hub.
3. Click **Add Project**.
4. Select the cloned project folder.
5. Open the project in the required Unity version.

---

## 3. How to Run the Software

### If Using a Prebuilt Version

1. Simply run the executable file.

### If Running in Unity Editor

1. Open the project in Unity.
2. Open the main scene.
3. Press the **Play** button in the Unity Editor.
4. The sandbox environment will load.

---

## 4. How to Use FruityTech Studio

You can assume familiarity with basic desktop UI interactions (clicking, dragging, buttons).

### 4.1 Starting a New Composition

1. Launch the application.
2. Click **Start New Sandbox**.
3. The interactive 2D sandbox will appear.
4. A sound palette with available sound objects will be displayed.

### 4.2 Placing Sound Objects

1. Select a sound object from the sound palette.
2. Drag it into the sandbox.
3. Drop it in a valid location.
4. The system automatically:
    - Registers the sound
    - Synchronizes it to the global tempo
    - Prepares it for looping playback

If placement is invalid, the system will reject it and provide feedback.

### 4.3 Playback Controls

Use the transport controls:
  - **Play** – Starts synchronized looping playback
  - **Stop** – Stops all playback

All active sound objects will play in sync with the global tempo (BPM).

### 4.4 Editing Sound Properties

1. Click on a placed sound object.
2. A properties panel will appear.
3. Adjust available parameters:
    - Volume
    - Mute

Changes are applied in real time without breaking synchronization.

### 4.5 Saving a Composition

1. Place at least one sound object.
2. Click the **Save** button.
3. Enter a name for your composition.
4. The system saves the composition as a local JSON file.

### 4.6 Loading a Composition

1. Click the **Load** button.
2. Select a previously saved composition from file explorer.
3. The sandbox restores all sound objects and settings.
4. Press **Play** to hear the restored loop.

---

## 5. How to Report a Bug

We use **GitHub Issues** to track bugs and known limitations.

### Issue Tracker

Submit bugs here:

https://github.com/nechypak/CS362---FruityTech-Studio-/issues

### Before Reporting

Please:
  1. Check existing issues to avoid duplicates.
  2. Confirm the issue is not listed as a known bug.
  3. Try restarting the application to verify it is reproducible.

### What to Include in a Bug Report

A good bug report should be clear, reproducible, and detailed.

Please include:
**1. Title**
  - Short and specific summary  
  - Example:  
  `Sound object desynchronizes after loading composition`

**2. Environment**
  - OS (Windows 10, etc.)
  - App version or commit hash
  - Unity version (if running from source)

**3. Steps to Reproduce**
  - Numbered, precise steps
  - Example:
    1. Open application
    2. Start new sandbox
    3. Place kick and melody
    4. Save composition
    5. Load composition
    6. Press Play

**4. Expected Behavior**
  - What you thought should happen.

**5. Actual Behavior**
  - What actually happened.

**6. Screenshots or Logs (if applicable)**
  - Screenshots
  - Unity console logs
  - Error messages

### Writing Good Bug Reports

If you are unsure how detailed your report should be, refer to:

Bugzilla Bug Writing Guidelines:  
https://bugzilla.mozilla.org/page.cgi?id=bug-writing.html

These guidelines explain how to create clear, actionable bug reports.

---

## 6. Known Bugs and Limitations

Known issues are tracked in the GitHub issue tracker and labeled appropriately.
