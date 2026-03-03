# CS362 - FruityTech Studio (Group #15)

## Team Members
- Logan Jordan - UI/UX Engineer
- Kanstansin Nechyparenka - Project Manager and Gameplay Engineer
- Allam Syahriza - Audio Features Engineer

## Project Summary
FruityTech Studio is a game-like educational music tool designed to help users prototype beats and loops without requiring prior music theory knowledge.

## How to Run
1. Go to the repository **Releases** page.
2. Download the **latest release**.
3. Extract the downloaded archive.
4. Open the extracted folder and run `FruityTech-Studio.exe`.

## Building & Testing
Use GitHub Actions as the default build/test workflow.

1. Push your branch to GitHub, or open a pull request to `main`.
2. GitHub Actions automatically runs the workflow at `.github/workflows/unity-ci.yaml`.
3. The pipeline executes:
   - Unity tests (Edit Mode + Play Mode via `game-ci/unity-test-runner@v4`)
   - Windows build (`StandaloneWindows64` target via `game-ci/unity-builder@v4`)
4. Download build artifacts from the workflow run:
   - Artifact name: `FruityTech-Build`

## How to Build Locally
### Prerequisites
- Unity Editor `6000.3.6f1`
- Git

### Steps
1. Clone the repository:
    -`git clone https://github.com/nechypak/CS362---FruityTech-Studio-.git`
2. Open Unity Hub.
3. Add and open `src/FruityTech-Studio`.
4. In Unity, open `File > Build Settings`.
5. Select `PC, Mac & Linux Standalone` and target `Windows`.
6. Confirm required scenes are listed in `Scenes In Build`.
7. Click `Build` and choose an output folder.

## How to Test Locally
1. Download appropriate build for your system in '/Releases'
2. Open file
4. Run all `Play Mode` tests.
5. Verify all tests pass.

## Use Case Covered in Beta
[UC01 - Create a music loop using sound objects](https://github.com/nechypak/CS362---FruityTech-Studio-/issues/3)


## Repository Layout
- `src/FruityTech-Studio/` - Unity project source code
- `Releases/` - Builds of every version
- `reports/` - Weekly/milestone reports
- `Team Artifacts/` - Team deliverables and project artifacts
- `developer_guide.md` - Developer-focused workflow details
- `user_manual.md` - user-faced manual of set up the working environment 
