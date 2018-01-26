[README.TXT]

This is a sample Visual C# GUI application which demonstrates 
how to successfully connect to and command a USB Ocean Optics 
spectrometer.

The program assumes the following:

- Visual Studio 2010
- .NET 4.0 (the graph isn't supported under 2.0)
- OmniDriver 2.x (hasn't been tested under 1.66, though may work)

At any given point in time, this solution may be configured for
32-bit or 64-bit modes, as well as different versions of .NET,
OmniDriver, etc, so you may need to tweak a few things to get it
working in your environment.  It generally works well, though.

Besides demonstrating OmniDriver, the program also shows the
following, each requested by one or more customers:

- how to acquire spectra from a background thread
- how to graph spectra
- how to support multiple spectrometers in parallel
- how to compute absolute irradiance
- how to create a standalone installer
