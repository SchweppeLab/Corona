// The one file in a release folder's root. Its entire job is to start the real .NET
// application, which lives with all of its DLLs in the app\ folder in the flat layout the
// .NET host expects, and either exit (GUI launcher) or wait and return the application's
// exit code (console launcher). Keeping the launcher separate means nothing about how the
// application loads assemblies had to change to get a tidy root; the runtime still sees a
// perfectly ordinary framework-dependent app.
//
// Pure Win32, statically linked CRT, no CLR: this must run on a machine that has nothing
// installed yet. The .NET 8 runtime remains a prerequisite by design. When it is missing,
// the application's own apphost shows the standard "install .NET" message with the download
// link, so this launcher never has to know about the runtime at all.
//
// One source for both launchers. Each *.Launcher project compiles it with its own
// launcher_config.h next to the vcxproj, which defines:
//   LAUNCHER_TITLE               the product name, used in error messages
//   LAUNCHER_APP_RELATIVE_PATH   the application exe, relative to this launcher
//   LAUNCHER_CONSOLE             defined for a console application: inherit the console and
//                                standard handles, wait, and return the child's exit code
//   LAUNCHER_APP_USER_MODEL_ID   optional; a GUI application's taskbar identity, which the
//                                application must also set on itself

#include "launcher_config.h"

#include <windows.h>
#include <shobjidl_core.h>

#include <string>

#if defined(LAUNCHER_CONSOLE)
#include <cstdio>
#endif

namespace {

// The directory this launcher was started from, with a trailing backslash: the release root.
std::wstring launcherDirectory() {
  wchar_t path[MAX_PATH];
  DWORD length = GetModuleFileNameW(nullptr, path, MAX_PATH);
  std::wstring full(path, length);
  size_t lastSlash = full.find_last_of(L'\\');
  return lastSlash == std::wstring::npos ? std::wstring() : full.substr(0, lastSlash + 1);
}

// Everything after this launcher's own executable token in the raw command line, forwarded
// to the application byte-for-byte so quoting and spacing survive.
std::wstring forwardedArguments() {
  const wchar_t* cursor = GetCommandLineW();
  if (*cursor == L'"') {
    cursor++;
    while (*cursor != L'\0' && *cursor != L'"') cursor++;
    if (*cursor == L'"') cursor++;
  } else {
    while (*cursor != L'\0' && *cursor != L' ' && *cursor != L'\t') cursor++;
  }
  while (*cursor == L' ' || *cursor == L'\t') cursor++;
  return cursor;
}

void showError(const std::wstring& message) {
#if defined(LAUNCHER_CONSOLE)
  fwprintf(stderr, L"%s\n", message.c_str());
#else
  MessageBoxW(nullptr, message.c_str(), LAUNCHER_TITLE, MB_OK | MB_ICONERROR);
#endif
}

#if defined(LAUNCHER_CONSOLE)
// Ctrl+C and Ctrl+Break reach every process attached to the console. The application
// decides what they mean; this launcher only has to stay alive long enough to report the
// application's exit code. A real handler (not the NULL "ignore" flag) so the application
// does not inherit the ignore.
BOOL WINAPI ignoreConsoleSignals(DWORD) {
  return TRUE;
}
#endif

} // namespace

#if defined(LAUNCHER_CONSOLE)
int wmain()
#else
int WINAPI wWinMain(HINSTANCE, HINSTANCE, PWSTR, int)
#endif
{
  const std::wstring root = launcherDirectory();
  const std::wstring app = root + LAUNCHER_APP_RELATIVE_PATH;

  if (GetFileAttributesW(app.c_str()) == INVALID_FILE_ATTRIBUTES) {
    showError(std::wstring(LAUNCHER_TITLE) + L" could not find its application files:\n\n" + app +
              L"\n\nThe app folder must sit next to " + LAUNCHER_TITLE +
              L".exe. Re-extract the release and try again.");
    return 1;
  }

#if defined(LAUNCHER_APP_USER_MODEL_ID)
  SetCurrentProcessExplicitAppUserModelID(LAUNCHER_APP_USER_MODEL_ID);
#endif

  // CreateProcessW may write into the command-line buffer, hence a mutable copy.
  std::wstring commandLine = L"\"" + app + L"\"";
  const std::wstring arguments = forwardedArguments();
  if (!arguments.empty()) {
    commandLine += L" " + arguments;
  }

#if defined(LAUNCHER_CONSOLE)
  // Share this console and its standard handles; keep the caller's working directory so
  // relative file paths on the command line mean what the user expects.
  SetConsoleCtrlHandler(ignoreConsoleSignals, TRUE);
  const BOOL inheritHandles = TRUE;
  const wchar_t* workingDirectory = nullptr;
#else
  const BOOL inheritHandles = FALSE;
  const wchar_t* workingDirectory = root.c_str();
#endif

  STARTUPINFOW startup{};
  startup.cb = sizeof(startup);
  PROCESS_INFORMATION process{};
  if (!CreateProcessW(app.c_str(), commandLine.data(), nullptr, nullptr, inheritHandles, 0, nullptr,
                      workingDirectory, &startup, &process)) {
    showError(std::wstring(LAUNCHER_TITLE) + L" could not start its application:\n\n" + app +
              L"\n\nWindows error " + std::to_wstring(GetLastError()) + L".");
    return 1;
  }
  CloseHandle(process.hThread);

#if defined(LAUNCHER_CONSOLE)
  WaitForSingleObject(process.hProcess, INFINITE);
  DWORD exitCode = 1;
  GetExitCodeProcess(process.hProcess, &exitCode);
  CloseHandle(process.hProcess);
  return static_cast<int>(exitCode);
#else
  // Fire and forget: the application owns its own lifetime from here.
  CloseHandle(process.hProcess);
  return 0;
#endif
}
