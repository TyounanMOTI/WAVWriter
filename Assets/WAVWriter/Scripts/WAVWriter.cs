using System;
using System.IO;

public class WAVWriter {
    int numChannels;
    int samplingRate;
    int dataSize;
    BinaryWriter writer;
    const UInt16 WAVE_FORMAT_EXTENSIBLE = 0xFFFE;
    const int headerSize = 4    // "WAVE"
                            + 4    // "fmt "
                            + 40   // WAVEFORMATEXTENSIBLE
                            + 4    // "data"
                            + 4    // size of data chunk
                            ;

    public WAVWriter(string path, int numChannels, int samplingRate) {
        this.numChannels = numChannels;
        this.samplingRate = samplingRate;
        writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write), System.Text.Encoding.ASCII);

        WriteHeader();
    }

    enum ChannelMask : UInt32 {
        SPEAKER_FRONT_LEFT = 0x1,
        SPEAKER_FRONT_RIGHT = 0x2,
        SPEAKER_FRONT_CENTER = 0x4,
        SPEAKER_LOW_FREQUENCY = 0x8,
        SPEAKER_BACK_LEFT = 0x10,
        SPEAKER_BACK_RIGHT = 0x20,
        SPEAKER_FRONT_LEFT_OF_CENTER = 0x40,
        SPEAKER_FRONT_RIGHT_OF_CENTER = 0x80,
        SPEAKER_BACK_CENTER = 0x100,
        SPEAKER_SIDE_LEFT = 0x200,
        SPEAKER_SIDE_RIGHT = 0x400,
        SPEAKER_TOP_CENTER = 0x800,
        SPEAKER_TOP_FRONT_LEFT = 0x1000,
        SPEAKER_TOP_FRONT_CENTER = 0x2000,
        SPEAKER_TOP_FRONT_RIGHT = 0x4000,
        SPEAKER_TOP_BACK_LEFT = 0x8000,
        SPEAKER_TOP_BACK_CENTER = 0x10000,
        SPEAKER_TOP_BACK_RIGHT = 0x20000,
    }

    void WriteHeader() {
        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write((UInt32)headerSize);   // あとでdataSizeを足して書き込む
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
        writer.Write((UInt32)40);
        writer.Write(WAVE_FORMAT_EXTENSIBLE);
        writer.Write((UInt16)numChannels);
        writer.Write((UInt32)samplingRate);
        writer.Write((UInt32)(sizeof(float) * numChannels * samplingRate));
        writer.Write((UInt16)sizeof(float));
        writer.Write((UInt16)(sizeof(float) * 8));
        writer.Write((UInt16)22);
        writer.Write((UInt16)(sizeof(float) * 8));

        ChannelMask channelMask = 0;
        switch (numChannels) {
            case 1:
                channelMask = ChannelMask.SPEAKER_FRONT_CENTER;
                break;
            case 2:
                channelMask = ChannelMask.SPEAKER_FRONT_LEFT | ChannelMask.SPEAKER_FRONT_RIGHT;
                break;
        }

        writer.Write((UInt32)channelMask);

        // KSDATAFORMAT_SUBTYPE_IEEE_FLOAT
        writer.Write((UInt32)0x00000003);
        writer.Write((UInt16)0x0000);
        writer.Write((UInt16)0x0010);
        writer.Write(new byte[] {0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71});

        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write((UInt32)0);
    }

    public void Write(float[] data) {
        dataSize += data.Length * sizeof(float);

        writer.Seek(4, SeekOrigin.Begin);
        writer.Write((UInt32)(headerSize + dataSize));

        writer.Seek(64, SeekOrigin.Begin);
        writer.Write((UInt32)dataSize);

        writer.Seek(0, SeekOrigin.End);
        foreach (var sample in data) {
            writer.Write(sample);
        }
    }

    public void Close() {
        writer.Close();
    }
}
