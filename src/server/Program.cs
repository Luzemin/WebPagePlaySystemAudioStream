using NAudio.Utils;
using NAudio.Wave;
using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ConsoleApp
{
    //https://github.com/sta/websocket-sharp
    public class Program
    {
        public static void Main(string[] args)
        {
            var wssv = new WebSocketServer("ws://localhost:2025");
            wssv.AddWebSocketService<Audio>("/Audio");
            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }
    }

    public class Audio : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "start")
            {
                WasapiLoopbackCapture waveSource = new WasapiLoopbackCapture();
                var outputStream = new MemoryStream();
                WaveFileWriter waveFileWriter = new WaveFileWriter(new IgnoreDisposeStream(outputStream), waveSource.WaveFormat);
                waveSource.DataAvailable += (object sender, WaveInEventArgs args) =>
                {
                    Console.WriteLine("record...");
                    waveFileWriter.Write(args.Buffer, 0, args.BytesRecorded);
                    waveFileWriter.Flush();
                    Send(outputStream.GetBuffer());
                };

                waveSource.StartRecording();
            }
        }
    }
}

