#pragma once

// Corona.exe: GUI launcher for app\Corona.exe. See ..\Launcher\launcher.cpp.

#define LAUNCHER_TITLE L"Corona"
#define LAUNCHER_APP_RELATIVE_PATH L"app\\Corona.exe"

// Must match AppUserModelId.Id in ..\Corona\AppUserModelId.cs: both processes claim the same
// taskbar identity so a pin made from the running window and this launcher are one
// application to the shell, not two.
#define LAUNCHER_APP_USER_MODEL_ID L"SchweppeLab.Corona"
