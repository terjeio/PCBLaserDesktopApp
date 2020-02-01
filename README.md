# PCBLaserDesktopApp

Desktop application for rendering PCB designs on the PCB Laser Exposer, written i C# for Windows.

Laser PCB Exposer is blu-ray laser "printer" for exposing photo-sensitive PCBs at 1200 dpi resolution by raster-rendering.

A typical workflow is to "print" the PCB artwork to a bitmap, BMP or PNG are supported, then drop the image onto the desktop application for exposing the PCB. I am using the [KiCad EDA](http://kicad-pcb.org/) for my designs, from that "plotting" to PDF and printing the resulting file to a PNG-file via the [PDFCreator](http://www.pdfforge.org/pdfcreator) printer-driver. I prefer to crop the PNG-files before using them for exposing the PCBs, this is not necesseary to do as the exposer software will automatically do that - but it will speed up the process.

It is possible to have a drill-ready PCB with solder mask in just two hours from outputting the artwork, this includes about one hour for "hardening" the solder mask.

Related projects are for [schematics, PCB](https://github.com/terjeio/PCBLaserSchematics) and [firmware](https://github.com/terjeio/PCBLaserFirmvare) for the controller. A [new design](https://github.com/terjeio/PCBLaserMkII) is in development which will make it simpler and cheaper to build.

---

1.06 - 2019-04-21:

Added motor off timeout (after idle for 1 minute motors will be disabled), configurable Y offset and optional command line parameter for ini-file to use.
Default file endings are changed to match KiCad v5.

---

Release notes:

1.04 - 2018-01-19:

Added configuration settings in PCBLaser.config for default emulsion types used \(Negative or Positive\) and file endings for autodetecting image inversion and mirroring. File endings defaults to those used by KiCad. Emulsion type can also be selected from the UI - note that the "Invert" checkbox will toggle on emulsion type change if a PCB image is loaded!

---

Further information and some images can be found over at [43oh.com](http://forum.43oh.com/topic/9645-pcb-laser-exposerprinter/#comment-72756)
