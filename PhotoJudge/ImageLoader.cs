using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoJudge
{
    public class ImageInfo
    {
        public BitmapSource BitmapSource { get; private set; }
        public FileInfo FileInfo { get; private set; }

        public ImageInfo(BitmapSource bitmapSource, FileInfo fileInfo)
        {
            BitmapSource = bitmapSource;
            FileInfo = fileInfo;
        }
    }

    public class ImageLoader
    {
        private FileEnumerator Enumerator { get; set; }
        private Thread LoadWorker { get; set; }
        private AutoResetEvent ImageCacheEvent { get; set; }
        private Queue<ImageInfo> ImageCache { get; set; }
        private bool HasMoreFiles { get; set; }

        private const int MaxImageCacheCount = 5;

        public ImageLoader(FileEnumerator enumerator)
        {
            Enumerator = enumerator;

            ImageCacheEvent = new AutoResetEvent(false);
            ImageCache = new Queue<ImageInfo>();
        }

        private void EnsureLoaderThread()
        {
            if (LoadWorker == null)
            {
                ImageCacheEvent.Set();

                LoadWorker = new Thread(LoaderThread);
                LoadWorker.Start();

                // Allow some time for the queue to fill
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void LoaderThread()
        {
            HasMoreFiles = true;

            while (HasMoreFiles)
            {
                ImageCacheEvent.WaitOne();

                while (ImageCache.Count < MaxImageCacheCount)
                {
                    FileInfo fileInfo = Enumerator.GetNextFile();
                    if (fileInfo == null)
                    {
                        HasMoreFiles = false;
                        break;
                    }

                    // TODO: Does this have thread affinity?
                    BitmapDecoder decoder = BitmapDecoder.Create(new Uri(fileInfo.FullName), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    ImageInfo imageInfo = new ImageInfo(decoder.Frames[0], fileInfo);

                    lock (ImageCache)
                    {
                        ImageCache.Enqueue(imageInfo);
                    }
                }
            }
        }

        public ImageInfo GetNextImage()
        {
            EnsureLoaderThread();

            try
            {
                while (!ImageCache.Any())
                {
                    if (!HasMoreFiles)
                    {
                        return null;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                lock (ImageCache)
                {
                    ImageInfo imageInfo = ImageCache.Dequeue();
                    return imageInfo;
                }
            }
            finally
            {
                ImageCacheEvent.Set();
            }
        }
    }
}
