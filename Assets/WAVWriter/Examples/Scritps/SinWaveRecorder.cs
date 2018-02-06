using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SinWaveRecorder : MonoBehaviour {
    [SerializeField]
    float frequency = 440;

    [SerializeField]
    float amplitude = 0.5f;

    float phase = 0;
    const int samplingRate = 48000;
    WAVWriter writer;
    AudioSource source;

    float[] recordingBuffer;
    int recordingHead = 0;

    void Start() {
        writer = new WAVWriter("result.wav", 2, samplingRate);
        recordingBuffer = new float[1024 * 20];

        source = GetComponent<AudioSource>();
        source.clip = AudioClip.Create("SinWave", 1, 2, samplingRate, false);
        source.loop = true;
        source.Play();
    }

    void Update() {
        lock (recordingBuffer) {
            float[] result = new float[recordingHead];
            Array.Copy(recordingBuffer, result, recordingHead);
            writer.Write(result);
            recordingHead = 0;
        }
    }

    void OnAudioFilterRead(float[] data, int numChannels) {
        for (int i = 0; i < data.Length; i += numChannels) {
            phase += 2.0f * Mathf.PI / samplingRate * frequency;
            phase %= 2.0f * Mathf.PI;
            float sample = amplitude * Mathf.Sin(phase);

            for (int channel = 0; channel < numChannels; channel++) {
                data[i + channel] = sample;
            }
        }
        lock(recordingBuffer) {
            for (int i = 0; i < data.Length; i += numChannels) {
                recordingBuffer[recordingHead + i / numChannels] = data[i];
            }
            recordingHead += data.Length / numChannels;
        }
    }

    void OnApplicationQuit() {
        if (writer != null) {
            writer.Close();
        }
    }
}
