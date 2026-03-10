# CS362 - FruityTech Studio (Group #15)

## Team Members
- Kanstansin Nechyparenka - Project Manager and Gameplay Engineer
- Logan Jordan - UI/UX Engineer
- Allam Syahriza - Audio Features Engineer

## Project Summary
**FruityTech Studio** is a Unity-based 2D educational music sandbox game designed to make music creation intuitive, interactive, beginner-friendly.

Instead of working with complex timelines, piano rolls, or professional production tools like Ableton or FL Studio, users create music by **placing sound objects in a visual sandbox**. Each object automatically loops and synchronizes to a shared tempo, allowing users to experiment freely without needing music theory knowledge.

### What Does FruityTech Studio Do?

FruityTech Studio allows users to:
- Place preloaded sound objects into a 2D sandbox
- Play synchronized music loops in real time
- Adjust properties such as volume and mute
- Save and load compositions
- Learn using contextual hints
- Experiment creatively in a game-like environment

### Why Use FruityTech Studio?

You would use FruityTech Studio if you:
- Want to learn music fundamentals in a fun, low-pressure way
- Want to prototype beats quickly
- Prefer visual and interactive music creation
- Feel overwhelmed by traditional music production software

---

## Installation Instructions

FruityTech Studio is distributed as a **Unity-based desktop application**.

### System Requirements

- Operating System:
    - Windows 10 or later (64-bit)
    - macOS 12+
- Audio output device (speakers or headphones)

### If You Are Running a Prebuilt Executable **(Recommended)**

1. Go to the GitHub repository:
   https://github.com/nechypak/CS362---FruityTech-Studio-
2. Navigate to the **Releases** page.
3. Download the latest release and appropriate build for your operating system.
4. Extract the `.zip` file.
5. Run:
    - Windows: `FruityTechStudio.exe`
    - macOS: `FruityTechStudio.app`

### If You Are Running from Source (Developers)

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

## Building & Testing
Use GitHub Actions as the default build/test workflow.

1. Push your branch to GitHub, or open a pull request to `main`.
2. GitHub Actions automatically runs the workflow at `.github/workflows/unity-ci.yaml`.
3. The pipeline executes:
   - Unity tests (Edit Mode + Play Mode via `game-ci/unity-test-runner@v4`)
   - Windows build (`StandaloneWindows64` target via `game-ci/unity-builder@v4`)
4. Download build artifacts from the workflow run:
   - Artifact name: `FruityTech-Build`

### How To Build Locally

#### Prerequisites
- Project added and opened from source (see **Running from Source (Developers)** above)

#### Steps
1. In Unity, open `File > Build Settings`. 
2. Select `PC, Mac & Linux Standalone` and target `Windows`. 
3. Confirm required scenes are listed in `Scenes In Build`. 
4. Click `Build` and choose an output folder.

## How to Test Locally
1. Run all `Play Mode` tests.
2. Verify all tests pass.

## Repository Layout
- `src/FruityTech-Studio/` - Unity project source code
- `reports/` - Weekly/milestone reports
- `Team Artifacts/` - Team deliverables and project artifacts
- `developer_guide.md` - Developer-focused workflow details
- `user_manual.md` - user-faced manual of set up the working environment 
