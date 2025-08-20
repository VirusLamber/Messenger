@echo off
chcp 65001 > nul
echo Creating distribution package...

if exist "Messenger_Distribute.zip" del "Messenger_Distribute.zip"
if exist "Distribute" rmdir /s /q Distribute

mkdir Distribute
xcopy /E /I Build Distribute\

echo Creating README for distribution...
echo # Messenger Chat Application > Distribute/README.txt
echo. >> Distribute/README.txt
echo How to use: >> Distribute/README.txt
echo 1. Run Server.exe on one computer >> Distribute/README.txt
echo 2. Run Client.exe on other computer >> Distribute/README.txt
echo 3. Connect using IP address >> Distribute/README.txt
echo. >> Distribute/README.txt
echo For internet: use Hamachi >> Distribute/README.txt
echo For local network: use local IP >> Distribute/README.txt

"C:\Program Files\7-Zip\7z.exe" a -tzip Messenger_Distribute.zip Distribute\*

if exist "Distribute" rmdir /s /q Distribute

echo.
echo ✅ Distribution package created: Messenger_Distribute.zip
echo Send this file to your friend!
echo.
pause