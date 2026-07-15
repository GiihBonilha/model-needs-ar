# ModelNeedsAR 🥽📚

An Augmented Reality educational Android application built with Unity, designed to support the teaching of **Model Needs** elicitation in Requirements Engineering for AI systems (RE4AI). Students interact with AR-based missions — placing 3D characters and objects on a real surface via the device camera — answering scenario-based questions whose outcomes feed into a final decision-tree feedback.

This repository is the artifact companion to the paper:

> **An Empirical Study on the Use of Augmented Reality to Support the Teaching of Model Needs in Requirements Engineering for AI Systems**
> Giovanna Bonilha, Bruna Santos, Ayumi Aoki, Fabiann Matthaus, Bianca Trinkenreich, Tayana Conte.
> 📄 Accepted at CBSoft 2026. The paper link will be added here once it is published.

## 📦 Repository Contents

- `Assets/Scripts/` — C# source code
  - `AR/` — AR placement and scene control (`ARPlacementManager`, `ARSceneController`, `ARImageController`)
  - `Mission/` — mission logic (`DialogueManager`, `QuestionManager`, `ScoreManager`, `ComboManager`, `MissionData`)
  - `UI/` — screen managers (`LoginManager`, `MissionsManager`, `BriefingManager`, `AdminManager`, `ProfessorLoginManager`, `RankingManager`, `RevisaoManager`)
  - `Feedback/` — final feedback logic (`FeedbackManager`)
  - `FirebaseManager.cs` — Firestore read/write layer for student data
- `Assets/Scenes/` — Unity scenes: `LoginScene`, `MissionScene`, `BriefingScene`, `ARScene`, `ResultScene`, `RankingScene`, `AdminScene`, `ProfessorLoginScene`, `RevisaoScene`
- `Assets/google-services.json` — Firebase project configuration (see **Notes for evaluators**)
- `ProjectSettings/`, `Packages/` — Unity project and package configuration
- `Materials/` — supplementary study materials (see **🧪 Study Materials** below)

## ✅ Requirements

- **Unity Editor 2022.3.62f3** (or a compatible 2022.3 LTS patch), installed via [Unity Hub](https://unity.com/download)
  - **Android Build Support** module, with **Android SDK & NDK Tools** and **OpenJDK** sub-modules (installable from the same Unity Hub screen)
- Key packages (already declared in `Packages/manifest.json`, auto-restored by Unity on first open):
  - AR Foundation 5.2.2
  - ARCore XR Plugin 5.2.2
  - TextMeshPro 3.0.7
  - Firebase Unity SDK (Firestore)
- **Hardware to run the built app:** an Android device with [ARCore support](https://developers.google.com/ar/devices), Android 7.0 (API 24) or higher, rear camera, USB cable for deployment (or Wi-Fi ADB)
- A **Google account**, to create your own Firebase project (see below)
- Git
- Internet connection (first package import, Firebase setup, and at runtime for Firestore read/write)
- ~10-15 GB free disk space (Unity Editor + Android SDK/NDK + project)

## 🗄️ Setting Up the Database (Firebase / Firestore)

The app persists student progress (login, scores, ranking) in a **Firebase Firestore** database. The repository ships with the authors' own `google-services.json`, but if you want to run your own independent instance (recommended for evaluators, to avoid writing test data into the authors' database), create your own Firebase project by following these steps:

1. Go to [firebase.google.com](https://firebase.google.com) and sign in with a Google account.
2. Click **"Add project"**, name it (e.g. `ModelNeedsAR`), and click **Continue**. You can leave Google Analytics **disabled** — it is not needed. Click **Create project**.
3. In the left sidebar, click **Firestore Database → Create database**.
4. Choose **Start in test mode** for a quick setup (open read/write for 30 days), select a region (e.g. `us-east1`), and click **Enable**.
   - ⚠️ Before using the app beyond local testing, replace the test-mode rules with production rules (see **Notes for evaluators** below) so the database isn't left publicly writable.
5. Back on the project's home page, click the **Android icon** to register a new app.
6. In **Android package name**, enter exactly: `com.GiihBonilha.ModelNeedsAR`
7. (Optional) Give it a nickname, e.g. `ModelNeedsAR`, then click **Register app**.
8. Download the generated **`google-services.json`** file and replace the one at `Assets/google-services.json` in the cloned project.
9. Back in Firebase, finish the wizard (you can skip the SDK-add steps, since the Firebase Unity SDK is already integrated in this project) and click **Continue to console**.
10. Re-open (or re-import) the project in Unity so it picks up the new configuration file.

## ⚙️ Installation (Building & Running the App)

1. Install **Unity Hub**, then install **Unity Editor 2022.3.62f3** through it, making sure to check the **Android Build Support** module (and its Android SDK & NDK / OpenJDK sub-items) during installation.
2. Clone this repository:
   ```
   git clone https://github.com/GiihBonilha/model-needs-ar.git
   ```
3. Open **Unity Hub -> Projects -> Add -> Add project from disk**, and select the cloned `model-needs-ar` folder.
4. Open the project. Unity will automatically resolve and import the packages listed in `Packages/manifest.json` (AR Foundation, ARCore XR Plugin, TextMeshPro, Firebase). This first import can take several minutes — wait for the progress bar at the bottom right to finish.
5. If you created your own Firebase project, replace `Assets/google-services.json` as described above.
6. Switch the build target to Android: **File -> Build Settings -> Android -> Switch Platform**.
7. Connect an ARCore-compatible Android device with **Developer Mode** and **USB Debugging** enabled.
8. In **File -> Build Settings**, click **Build And Run** (or **Build** to generate an installable `.apk`).
9. On first launch, grant the **camera permission** when prompted by Android.

## 🚀 How to Use the App (Step-by-Step)

> ⚠️ **First-time setup note:** if you are running the app against a fresh/empty database (e.g. your own newly created Firebase project), no classes ("turmas") exist yet. Students **cannot log in without selecting a turma**, and the turma dropdown will be empty until one is registered. **The first person to use the app must be a professor/admin**, who logs in through the professor flow and registers at least one turma — only after that can students log in and join it. See the "As a professor / admin" flow below.

### As a student
1. On **LoginScene**, enter your e-mail and select your class (turma) from the dropdown, then tap **Login**.
2. **MissionsScene** lists the available missions with your progress. Tap a mission to start it.
3. **BriefingScene** presents the mission's context before entering AR.
4. In **ARScene**, point the device camera at a flat surface (e.g. a table) until a plane is detected, then tap to place the 3D characters/objects.
5. Interact with the dialogue and answer the questions presented during the mission; correct/incorrect answers affect your combo/score.
6. **ResultScene** shows your final score and a feedback text generated from a decision tree based on the answers you gave. From here you can go to **RankingScene** (leaderboard) or **RevisaoScene** (review your answers).

### As a professor / admin
1. On **LoginScene**, tap **"Sou professor" (BotaoProfessor)** to go to **ProfessorLoginScene**.
2. Enter the professor password and tap **Entrar**.
3. **AdminScene** shows a summary dashboard with buttons for **Alunos** (student list), **Gráfico** (performance charts), and **Turmas** (classes), plus access to the global ranking.
4. On a fresh database, open **Turmas** first and register at least one turma — this is required before any student can log in, since the student login screen requires selecting an existing turma from the dropdown.

## 🧪 Study Materials

In addition to the application itself, this repository includes materials used in the empirical study reported in the paper, under `Materials/`:

- `Materials/TCLE - RA.pdf` — Informed consent form (Termo de Consentimento Livre e Esclarecido) presented to participants.
- `Materials/Questionário RA .pdf` — Questionnaire used to evaluate participants' experience with the app.
- `Materials/RE4HCAI Framework.pdf` — Reference framework on Requirements Engineering for AI used as the theoretical basis for the study.
- `Materials/marcador_ar (2).png` — The AR marker image used by the app: the device camera detects this marker to trigger the AR content.
- `Materials/paper_based_activity (2).pdf` — The paper-based activity used as the basis for the in-app mission content.

> The consent form and questionnaire were originally applied as Google Forms; the PDFs above are static exports of their content, included here so the artifact remains self-contained and doesn't rely on external, non-persistent links.

## Notes for evaluators

- The AR functionality requires a **physical ARCore-compatible Android device** — it does not fully run in the Unity Editor's Play Mode.
- `Assets/google-services.json` points to the authors' Firebase/Firestore project by default. If you don't create your own project (see **Setting Up the Database**), running the app will write test data into the authors' shared database.
- If the Firestore database is still in **test mode** (open read/write), consider tightening the rules before publishing the repository, for example:
  ```
  rules_version = '2';
  service cloud.firestore {
    match /databases/{database}/documents {
      match /{document=**} {
        allow read: if true;
        allow write: if false;
      }
    }
  }
  ```

## 📄 License

Distributed under the **MIT License** — see [`LICENSE`](./LICENSE).

## 👥 Authors

- Giovanna Bonilha — Federal Institute of Amazonas
- Bruna Santos — Federal University of Amazonas
- Ayumi Aoki — Federal University of Amazonas
- Fabiann Matthaus — Federal University of Amazonas
- Bianca Trinkenreich — Colorado State University
- Tayana Conte — Federal University of Amazonas

## 🤝 Contributing

Contributions are welcome! Feel free to open an issue or pull request with suggestions and improvements.
