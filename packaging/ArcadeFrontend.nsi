;--------------------------------
; https://gist.github.com/mattiasghodsian/a30f50568792939e35e93e6bc2084c2a
; Includes

!include "MUI2.nsh"
!include "logiclib.nsh"

;--------------------------------
; Custom defines
!define NAME "Arcade Frontend"
!define APPFILE "ArcadeFrontend.exe"
!define VERSION "0.0.1"
!define SLUG "${NAME} v${VERSION}"

;--------------------------------
; General

Name "${NAME}"
OutFile "${NAME} Setup.exe"
InstallDir "$PROGRAMFILES64\${NAME}"
InstallDirRegKey HKCU "Software\${NAME}" ""
RequestExecutionLevel admin

;--------------------------------
; UI

!define MUI_ICON "assets\icon.ico"
;!define MUI_HEADERIMAGE
;!define MUI_WELCOMEFINISHPAGE_BITMAP "assets\welcome.bmp"
;!define MUI_HEADERIMAGE_BITMAP "assets\head.bmp"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TITLE "${SLUG} Setup"

;--------------------------------
; Pages

; Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "licence.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set UI language
!insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Section - Install App

Section "-hidden app"
  SectionIn RO

  SetOutPath "$INSTDIR"

  File /r "app\*.*"

  WriteRegStr HKCU "Software\${NAME}" "" $INSTDIR

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "DisplayName" "${NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "NoRepair" 1
  WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"
  CreateShortcut "$SMPROGRAMS\Arcade Frontend.lnk" "$INSTDIR\ArcadeFrontend.exe"
SectionEnd

;--------------------------------
; Section - Uninstaller
Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}"
  DeleteRegKey HKCU "Software\${NAME}"

  ; Remove files and uninstaller
  Delete $INSTDIR\*.*
  Delete $INSTDIR\Content\*.*
  Delete $INSTDIR\Content\backgrounds\*.*
  Delete $INSTDIR\Content\images\*.*
  Delete $INSTDIR\Content\shader\*.*
;  Delete $INSTDIR\runtimes\*.*  (Heaps of shit in here)

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${NAME}\*.lnk"

  ; Remove directories
  RMDir "$SMPROGRAMS\${NAME}"
  RMDir /r "$INSTDIR"

SectionEnd
