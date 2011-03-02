Got probs on OSX, should try Linux ,next.

Compile & execute from command line (Mono)
==========================================

* compile (in *.csproj directory)
  xbuild /property:Configuration=<release> e.g.
  
  xbuild /property:Configuration=Debug


* execute:
  mono <*.csproj-dir>/bin/<release>/OpenTK_GL_Outline_example.exe e.g.
  
  mono ./bin/Debug/OpenTK_GL_Outline_example.exe

