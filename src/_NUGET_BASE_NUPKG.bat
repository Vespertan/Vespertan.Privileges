@rem Oczekiwane jest ze przed wykonanie tego pliku 
@rem zostana ustawione wczesniej zmienne: projekt, katalog, build
@rem projekt to nazwa projektu. Np. ProjektA
@rem katalog to miejsce polozenia projektu wzgledem tego pliku. Np. ..\projekt\
@rem Najlepszym sposobem jest w innym pliku bat zdefioniowac te zmienne,
@rem a nastepnie wywoˆac ten plik.

@rem Zawarto˜c takiego pliku moze wygladac nastepujaco:
@rem @set projekt=Vespertan.DataBase
@rem @set katalog=..\Vespertan.DataBase\
@rem @set apiKey=%VespertanApiKey%
@rem @set src=https://api.nuget.org/v3/index.json
@rem @set localSrc=C:\NugetLocalFeed
@rem @set build=-Build
@rem @_NUGET_BASE.bat

@set build=-Build
@set src=https://api.nuget.org/v3/index.json
@set localSrc=C:\NugetLocalFeed
@set apiKey=%VespertanApiKey%

@echo off
set wersja=
set push=T
set localPush=T
set del=T

if exist bin\%projekt%*.nupkg del bin\%projekt%*.nupkg
dotnet pack %katalog%%projekt%.csproj --configuration Release -o bin
if %errorlevel% NEQ 0 goto  err 

move bin\%projekt%*.nupkg bin\%projekt%.nupkg

echo.
if NOT "%localSrc%" == "" (set /P push="Czy wypchnac na lokalny serwer plikow? (T/N)[%localPush%]")
if NOT "%localSrc%" == "" (if /I [%localPush%] == [t] (nuget add bin\%projekt%.nupkg -Source %localSrc%))

echo.
if NOT "%src%" == "" (set /P push="Czy wypchnac na serwer? (T/N)[%push%]")
if NOT "%src%" == "" (if /I [%push%] == [t] (nuget push bin\%projekt%.nupkg %apiKey% -Source %src%))

echo.
set /P del=Usun plik paczki %projekt%.nupkg? (T/N)[%del%]
if /I [%del%] == [t] (del bin\%projekt%.nupkg)
:err
pause