# WAVWriter
WAV file writer for Unity

# Usage
```C#
  var writer = new WAVWriter("result.wav", numChannels, samplingRate);
  var data = new float[1024];
  writer.Write(data);
```
# License
MIT License (c) Hirotoshi Yoshitaka
