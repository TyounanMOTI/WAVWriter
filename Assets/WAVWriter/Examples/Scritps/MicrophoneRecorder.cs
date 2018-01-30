using System;
using UnityEngine;

// 1. WAVWriter: https://github.com/TyounanMOTI/WAVWriter
//    Releasesから.unitypackageをダウンロードしてインポートしてください
// 2. 空のGameObjectにアタッチして開始すると、プロジェクトのルートフォルダに record.wav を書き出します。
//    録音するマイクは、Windowsであれば既定のマイクです。
public class MicrophoneRecorder : MonoBehaviour {
    AudioClip clip;
    int head = 0;
    const int samplingFrequency = 16000;
    const int lengthSeconds = 1;
    float[] processBuffer = new float[256];
    float[] microphoneBuffer = new float[lengthSeconds * samplingFrequency];

    WAVWriter writer;

    void Start() {
        writer = new WAVWriter("record.wav", 1, 16000);
        clip = Microphone.Start(null, true, lengthSeconds, samplingFrequency);
    }

    void Update() {
        var position = Microphone.GetPosition(null);
        if (position < 0 || head == position) {
            return;
        }

        clip.GetData(microphoneBuffer, 0);
        while (GetDataLength(microphoneBuffer.Length, head, position) > processBuffer.Length) {
            var remain = microphoneBuffer.Length - head;
            if (remain < processBuffer.Length) {
                Array.Copy(microphoneBuffer, head, processBuffer, 0, remain);
                Array.Copy(microphoneBuffer, 0, processBuffer, remain, processBuffer.Length - remain);
            } else {
                Array.Copy(microphoneBuffer, head, processBuffer, 0, processBuffer.Length);
            }

            // processBufferを何か処理する部分。今回は.wavへ書き出し
            writer.Write(processBuffer);

            head += processBuffer.Length;
            if (head > microphoneBuffer.Length) {
                head -= microphoneBuffer.Length;
            }
        }
    }

    static int GetDataLength(int bufferLength, int head, int tail) {
        if (head < tail) {
            return tail - head;
        } else {
            return bufferLength - head + tail;
        }
    }
}

// The MIT License
// Copyright 2018 Hirotoshi Yoshitaka
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
