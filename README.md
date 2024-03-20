# XnbCli
A port to C# from the Javascript code of [the original implementation](https://github.com/LeonBlade/xnbcli) by LeonBlade and the [unofficial version support Stardew Valley 1.4 above](https://github.com/LeonBlade/xnbcli/issues/13#issuecomment-1138169672).

From the original README:

> A CLI tool for XNB packing/unpacking purpose built for Stardew Valley.
>
> This tool currently supports unpacking all LZX compressed XNB files for Stardew Valley.
> There is some basic groundwork for XACT as well.
>
> The end goal for this project is to serve as a CLI for a GUI wrapper to leverage so the average user can interface with XNB files a lot easier.


## Special features
- Support unpacking through source generator (which means it is NativeAOT-friendly, sort of).
- High performance extraction.
- Support extracting POCO (Plain Old C# Object) data from Stardew Valley 1.4 above.

## TODO
- Support packing, though a bit unnecessary because of SMAPI.
- Support packing and unpacking in parallel.
- Support XACT (seriously, why?)
- More refactoring because the code is extremely messy.
