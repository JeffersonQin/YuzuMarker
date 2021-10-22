if exist %1YuzuIPS (
del /S /Q %1YuzuIPS
rd /S /Q %1YuzuIPS
)
echo d|xcopy /E %2\build\YuzuIPS %1YuzuIPS
