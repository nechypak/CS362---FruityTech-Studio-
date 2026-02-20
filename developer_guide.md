# FruityTech Studio - Developer Guide  

This document provides guidelines for developers who want to contribute to FruityTech Studio. It explains how to obtain the source code, understand the repository structure, build the software, run and add tests, and create release builds.

---

## 1. Obtaining the Source Code

#### Repository Location

```
https://github.com/nechypak/CS362---FruityTech-Studio-
```

#### Clone via HTTPS

```bash
git clone https://github.com/nechypak/CS362---FruityTech-Studio-.git
```

#### Clone via SSH

```bash
git clone git@github.com:nechypak/CS362---FruityTech-Studio-.git
```

After cloning:

1. Open Unity Hub
2. Click 'Add Project'
3. Select the cloned directory
4. Ensure the correct Unity version is installed (see below)
5. Open the project

### Development Environment Requirements

- **Unity Editor Version:** 6000.3.6f1
- **Git:** Required for version control  

> Using a different Unity version may cause compatibility issues.

---

## 2. Directory Structure

```
/Assets
  /Scenes
  /Scripts
    /Application
    /Audio
    /Persistence
    /UI
    /Tests
  /Prefabs
  /Resources
/Team Artifacts
/reports
README.md
developer_guide.md
```

### `/Assets`
Contains all assets including scenes, scripts, audio files, and prefabs.

### `/Assets/Scenes`
Unity scene files used to run the application.

### `/Assets/Scripts`
All C# code.

#### `/Assets/Scripts/Application`
Responsible for:
- User interaction logic
- Maintaining the composition of placed objects


#### `/Assets/Scripts/Audio`
Responsible for:
- Audio playback
- Loop timing and synchronization
- Playback state management

#### `/Assets/Scripts/Persistence`
Responsible for:
- Saving / Loading to and from JSON

#### `/Assets/Scripts/UI`
Responsible for:
- Drag-and-drop functionality
- Button handling (Play, Stop, Save, Load)
- UI updates 

#### `/Assets/Scripts/Tests`
Contains automated test cases using the Unity Test Framework.

Includes:
- Unit tests (Edit Mode)
- Integration tests (Play Mode)

### `/Team Artifacts`
Contains planning documents, diagrams, and other materials.

### `/reports`
Contains weekly status reports.

---

## 3. Building the Software

1. Open the project in Unity.
2. Navigate to:

   File → Build Settings

3. Select the target platform (Windows/macOS).
4. Ensure required scenes are added under 'Scenes In Build'.
5. Click 'Build'.
6. Choose an output directory.

Do not commit build artifacts to the repository.

---

## 4. Testing the Software

FruityTech uses Unity's built-in Test Framework.

#### Running Tests

1. Open Unity.
2. Navigate to:

   Window → General → Test Runner

3. Select:
   - Edit Mode (unit tests)
   - Play Mode (integration tests)
4. Click 'Run All'.

Test results will appear in the Test Runner window.

### Types of Tests

#### Unit Tests
Validate isolated logic components such as:
- Loop timing logic
- Save/load serialization
- Playback state transitions

#### Integration Tests
Validate interactions between systems such as:
- Drag-and-drop triggering audio playback
- Save functionality creating JSON files
- Load functionality restoring scene state

---

## 5. Adding New Tests

Place new test files in:

```
/Assets/Scripts/Tests
```

#### Naming Conventions

- Unit tests: `FeatureNameTests.cs`
- Integration tests: `FeatureNameIntegrationTests.cs`


---

## 6. Building a Release

#### Step 1: Update Version

Before building a release:

- Update version number in:
  - Project Settings → Player → Version

#### Step 2: Run All Tests

Ensure:
- All unit tests pass
- All integration tests pass
- CI pipeline is green

#### Step 3: Generate Build

Use:

File → Build Settings → Build

#### Step 4: Perform Sanity Checks

After building:

- Launch executable
- Verify audio playback
- Verify drag-and-drop functionality
- Verify save/load behavior
- Confirm no console errors

#### Step 5: Tag the Release

```bash
git tag v1.0.0
git push origin v1.0.0
```

---

## 7. Contribution Workflow

1. Create a feature branch:

```bash
git checkout -b feature/<feature-name>
```

2. Implement changes.
3. Run tests locally.
4. Commit with descriptive messages.
5. Submit a pull request.
6. Ensure CI passes before merging into `main`.
