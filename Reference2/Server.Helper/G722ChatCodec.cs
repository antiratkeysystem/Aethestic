using System;
using NAudio.Codecs;
using NAudio.Wave;

namespace Server.Helper;

public class G722ChatCodec
{
	private readonly int bitrate;

	private readonly G722CodecState encoderState;

	private readonly G722CodecState decoderState;

	private readonly G722Codec codec;

	public string Name => "G.722 16kHz";

	public int BitsPerSecond => bitrate;

	public WaveFormat RecordFormat { get; }

	public bool IsAvailable => true;

	public G722ChatCodec()
	{
		bitrate = 64000;
		encoderState = new G722CodecState(bitrate, G722Flags.None);
		decoderState = new G722CodecState(bitrate, G722Flags.None);
		codec = new G722Codec();
		RecordFormat = new WaveFormat(16000, 1);
	}

	public byte[] Encode(byte[] data, int offset, int length)
	{
		if (offset != 0)
		{
			throw new ArgumentException("G722 does not yet support non-zero offsets");
		}
		WaveBuffer waveBuffer = new WaveBuffer(data);
		byte[] array = new byte[length / 4];
		codec.Encode(encoderState, array, waveBuffer.ShortBuffer, length / 2);
		return array;
	}

	public byte[] Decode(byte[] data, int offset, int length)
	{
		if (offset != 0)
		{
			throw new ArgumentException("G722 does not yet support non-zero offsets");
		}
		byte[] array = new byte[length * 4];
		WaveBuffer waveBuffer = new WaveBuffer(array);
		codec.Decode(decoderState, waveBuffer.ShortBuffer, data, length);
		return array;
	}

	public void Dispose()
	{
	}
}
