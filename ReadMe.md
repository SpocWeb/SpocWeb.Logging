# SpocWeb.Logging

<!-- digest-map
local-classes:
  DestructureWrapper: mtime=2026-03-11T07:58:58Z digest=de2eed1a9afb2c8a854e1053da6599131b07349921066987ca2a94faabcf6369
  ExcludeFromLoggingAttribute: mtime=2025-05-02T17:50:18Z digest=a0abcb7a602f933d81f71ef7fcb71b7e0559850e350427f6839c82827a93ffe8
  Int: mtime=2026-03-06T12:14:31Z digest=e7a003742c58af98acdca4f9428a9f0bc6558f0323f808fa56cee8cc2ff4dbe9
  IsExternalInit: mtime=2024-10-07T13:17:24Z digest=1c92ff0e1cf64aef6c485bdffc89ea343bbd6bbe9cefd939e21ababdd082d938
  Log: mtime=2026-03-06T12:13:23Z digest=afae3f6b5902d3e5547e169262035596db690e64ea00858288dd6d86ca7b3a82
  LoggingLimitPolicy: mtime=2026-03-06T09:42:35Z digest=b74a830f176cf637f66813edf9dcea72b6d085643d39dda456cb3cfc43c8bf0f
  LogX: mtime=2026-03-11T07:58:58Z digest=de2eed1a9afb2c8a854e1053da6599131b07349921066987ca2a94faabcf6369
  PrefixedStringHandler: mtime=2026-03-11T07:58:58Z digest=de2eed1a9afb2c8a854e1053da6599131b07349921066987ca2a94faabcf6369
  Program: mtime=2025-05-02T17:50:18Z digest=e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855
  StringInterpolationWithValues: mtime=2025-05-02T17:50:18Z digest=ab10d5f9a9a2bee8442630c682e99e51eae2a70c7cbabda62b8197577a1dac78
folders:
folder_digest: aecf509e95598af9df6d0f3cf8206c279636bd967083533aa63f830c442122c4
folder_mtime: 2026-03-11T07:58:58Z
-->

This minimal Project provides extension Methods to combine Logging and Formatting.

It can optionally be combined with the SpocWeb.Proxies Project,
which defines a dynamic LoggingProxy Interceptor that can be plugged in via Dependency Injection,
to log all Calls with Parameters and Results.

